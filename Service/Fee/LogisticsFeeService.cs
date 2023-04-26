using NHibernate;
using NHibernate.Linq;
using shunda.Core;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;

namespace shunda.Service.Fee;

// todo
/*
 * 这些是原系统遗留的
 * 
 * 1、包裹数量是针对整个订单的快递单号数量计算的, 不是打包后的包裹数量, 因为打包后的数量，不是客户能控制的
 *
 * 2、长度超长是针对整个订单计算的，如果一个订单里面有10个超长的。实际上只按照最长的货物，收取一次费用
 *
 */

/// <summary>
/// 各个具体物流费用的计算
/// </summary>
[Service(ServiceType.Scoped)]
public class LogisticsFeeService {
    private readonly ISession session;
    private readonly LogisticsFeeExpressService logisticsFeeExpressService;
    private readonly FeeCalculateLogService feeCalculateLogService;

    public LogisticsFeeService(
        ISession session,
        LogisticsFeeExpressService logisticsFeeExpressService,
        FeeCalculateLogService feeCalculateLogService) {
        this.session = session;

        this.logisticsFeeExpressService = logisticsFeeExpressService;
        this.feeCalculateLogService = feeCalculateLogService;
    }

    /// <summary>
    /// 根据物流数据计算物流费用
    /// 这是只计算一程的费用， 所以参数里面需要指定 LogisticsMode
    /// </summary>
    /// <param name="feeDataItem"></param>
    /// <param name="feeMessageType"></param>
    /// <returns></returns>
    public IList<LogisticsFeeResultEntry> GetLogisticsFee(LogisticsFeeDataItem feeDataItem, FeeMessageType feeMessageType) {
        ValidationUtil.Validate<LogisticsFeeDataValidator, LogisticsFeeDataItem>(feeDataItem);

        var logisticsMode = feeDataItem.LogisticsMode;

        if (logisticsMode == null) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！SD0121");
            } else {
                throw new Exception("物流方式不能为空！");
            }
        }

        int logisticsModeId = logisticsMode.Id;

        LogisticsGoodsType goodsType = feeDataItem.GoodsType;

        if (goodsType == LogisticsGoodsType.Forbidden) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("禁止类物品不允许运输！SD0130");
            } else {
                throw new Exception("禁止类物品不进行计费！");
            }
        }

        #region 数据检查

        IList<LogisticsFeePack> feePackList = feeDataItem.FeePackList;
        if (feePackList == null || feePackList.Count == 0) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("未能获取物品尺寸数据！SD0140");
            } else {
                throw new Exception("物品打包数据不能为空！");
            }
        }

        // 检查物流类型和物流方式的匹配 
        var logisticsModeGoodsType = session.Query<LogisticsModeGoodsTypeInfo>()
            .Where(e => e.LogisticsMode.Id == logisticsModeId && e.GoodsType == goodsType)
            .Take(1)
            .SingleOrDefault();
        if (logisticsModeGoodsType == null) {
            throw new ValidationException($"物流方式{logisticsMode.Name}不支持物品类型{goodsType.Name}！");
        }

        // 检查物流方式对应的费率设置
        var logisticsModeFeeRate = session.Query<LogisticsModeFeeRateInfo>()
            .Fetch(e => e.FeeRate)
            .Where(e => e.Mode.Id == logisticsModeId
                        && e.GoodsType == goodsType)
            .Take(1)
            .SingleOrDefault();
        if (logisticsModeFeeRate == null) {
            throw new ValidationException($"物流方式{logisticsMode.Name}中物品类型{goodsType.Name}对应的通用费率未设置！");
        }

        // 通用费率
        LogisticsFeeRateInfo feeRate = logisticsModeFeeRate.FeeRate;

        if (feeRate.VolumeWeightDenominator <= 0) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！SD0230");
            } else {
                throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率考虑体积重时，体积重分母应该大于0！");
            }
        }

        if (feeRate.CareVolumeActualWeightRatio && feeRate.MinVolumeActualWeightRatio <= 0) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！SD0240");
            } else {
                throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率考虑体积重实际重倍率时，体积重实际重最小倍率应该大于0！");
            }
        }

        if (feeRate.VolumeWeightCareMaxSideLength && feeRate.VolumeWeightMaxSideLengthBegin <= 0) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！SD0243");
            } else {
                throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率体积重考虑最长边起始时，体积重最长边起始应该大于0！");
            }
        }

        if (feeRate.VolumeWeightCareSideSumBegin && feeRate.VolumeWeightSideSumBegin <= 0) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！SD0244");
            } else {
                throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率体积重考虑三边之和起始时，体积重三边之和起始应该大于0！");
            }
        }

        #endregion

        #region 检查起步长度和重量

        CheckLengthStart(feePackList, feeRate, feeMessageType);
        CheckSingleWeightStart(feePackList, feeRate, feeMessageType);
        CheckAverageWeightStart(feePackList, feeRate, feeMessageType);
        CheckWeightStart(feePackList, feeRate, feeMessageType);

        #endregion

        #region 计算最终体积重量和最长边

        // kg，订单总重量，考虑了体积重后的重量
        decimal finalTotalWeightKg = GetWholePackTotalWeight(feePackList, feeRate, feeMessageType);

        feeCalculateLogService.AddLog($"考虑体积重后的订单总重量：{finalTotalWeightKg}");

        // 立方厘米
        // decimal finalTotalVolumeCm3 = GetAllPackTotalVolume(feePackList, feeRate);

        #endregion

        IList<LogisticsFeeResultEntry> resultEntryList = new List<LogisticsFeeResultEntry>();

        #region 计算初步费用

        feeCalculateLogService.AddLog("计算初步费用 开始");

        decimal initialTotalFee = 0m;

        var logisticsModeType = logisticsMode.Type;


        if (logisticsMode.FeeType != LogisticsModeFeeType.SinglePackBox) {
            // 这种情况是汇总计算

            // 跨国快递
            if (logisticsModeType == LogisticsModeType.Express) {
                initialTotalFee = logisticsFeeExpressService.GetFee(finalTotalWeightKg, logisticsModeId, feeMessageType);

                resultEntryList.Add(new LogisticsFeeResultEntry {
                    OrderFirstMode = feeDataItem.OrderFirstMode,
                    LogisticsMode = feeDataItem.LogisticsMode,
                    Title = LogisticsOrderFeeType.InitFee.Name,
                    FeeType = LogisticsOrderFeeType.InitFee,
                    Fee = initialTotalFee,
                    GoodsType = feeDataItem.GoodsType,
                    EntryType = LogisticsFeeResultEntryType.Single
                });
            } else {
                throw new ValidationException("计费不支持的物流方式类型，无法计费！");
            }
        } else {
            // 这种情况是分箱计算

            int boxNum = 0;
            feePackList = feePackList.OrderBy(e => e.PackId).ToList(); // 让箱号推断正确
            foreach (var feePack in feePackList) {
                for (int boxIndx = 0; boxIndx < feePack.PackageCount; boxIndx++) {
                    boxNum = boxNum + 1;

                    // 跨国快递
                    if (logisticsModeType == LogisticsModeType.Express) {
                        decimal initFeeForPackBox = logisticsFeeExpressService.GetFee(feePack.Weight, logisticsModeId, feeMessageType);
                        initialTotalFee = initialTotalFee + initFeeForPackBox;

                        resultEntryList.Add(new LogisticsFeeResultEntry {
                            OrderFirstMode = feeDataItem.OrderFirstMode,
                            LogisticsMode = feeDataItem.LogisticsMode,
                            Title = LogisticsOrderFeeType.InitFee.Name + $"箱号{boxNum}",
                            FeeType = LogisticsOrderFeeType.InitFee,
                            Fee = initFeeForPackBox,
                            GoodsType = feeDataItem.GoodsType,
                            EntryType = LogisticsFeeResultEntryType.Single
                        });
                    } else {
                        throw new ValidationException("计费不支持的物流方式类型，无法计费！");
                    }
                }
            }
        }

        feeCalculateLogService.AddLog($"{LogisticsOrderFeeType.InitFee.Name}: {initialTotalFee}");
        feeCalculateLogService.AddLog("计算初步费用 结束");

        #endregion

        #region 计算加收费用

        decimal extraTotalFee = 0m;

        // 这里的 超长 和 超重 都是针对的是 "原来的没打折的数据" 还是 "打折后的数据".
        // 这里实际使用的是 "打折后的数据"

        // 超长费用
        decimal overLengthFee = GetOverLengthFee(feePackList, feeRate, feeMessageType);

        //if (overLengthFee > 0)
        {
            extraTotalFee += overLengthFee;

            resultEntryList.Add(new LogisticsFeeResultEntry {
                OrderFirstMode = feeDataItem.OrderFirstMode,
                LogisticsMode = feeDataItem.LogisticsMode,
                Title = LogisticsOrderFeeType.ExtraLengthFee.Name,
                FeeType = LogisticsOrderFeeType.ExtraLengthFee,
                Fee = overLengthFee,
                GoodsType = feeDataItem.GoodsType,
                EntryType = LogisticsFeeResultEntryType.Single
            });

            feeCalculateLogService.AddLog($"超长费用 {overLengthFee}");
        }

        // 单箱超重费用
        decimal singleOverWeightFee = GetSingleOverWeightFee(feePackList, feeRate, feeMessageType);

        // if (singleOverWeightFee > 0)
        {
            extraTotalFee += singleOverWeightFee;

            resultEntryList.Add(new LogisticsFeeResultEntry {
                OrderFirstMode = feeDataItem.OrderFirstMode,
                LogisticsMode = feeDataItem.LogisticsMode,
                Title = LogisticsOrderFeeType.SingleExtraWeightFee.Name,
                FeeType = LogisticsOrderFeeType.SingleExtraWeightFee,
                Fee = singleOverWeightFee,
                GoodsType = feeDataItem.GoodsType,
                EntryType = LogisticsFeeResultEntryType.Single
            });

            feeCalculateLogService.AddLog($"单箱超重费用 {singleOverWeightFee}");
        }

        /*
         这个费用不用计算了
        // 平均单箱超重费用
        decimal averageOverWeightFee = GetAverageOverWeightFee(feePackList, feeRate, feeMessageType);

        // if (averageOverWeightFee > 0)
        {
            extraTotalFee += averageOverWeightFee;

            resultEntryList.Add(new LogisticsFeeResultEntry {
                OrderFirstMode = logisticsFeeData.OrderFirstMode,
                LogisticsMode = logisticsFeeData.LogisticsMode,
                Title = LogisticsOrderFeeType.AverageExtraWeightFee.Name,
                FeeType = LogisticsOrderFeeType.AverageExtraWeightFee,
                Fee = averageOverWeightFee,
                GoodsType = logisticsFeeData.GoodsType,
                EntryType = LogisticsFeeResultEntryType.Single
            });

            feeCalculateLogService.AddLog($"平均单箱超重费用 {averageOverWeightFee}");
        }
        */

        /*
         这个费用不用计算了
        // 超重费用
        decimal overWeightFee = GetOverWeightFee(feePackList, feeRate, feeMessageType);

        // if (overWeightFee > 0)
        {
            extraTotalFee += overWeightFee;

            resultEntryList.Add(new LogisticsFeeResultEntry {
                OrderFirstMode = logisticsFeeData.OrderFirstMode,
                LogisticsMode = logisticsFeeData.LogisticsMode,
                Title = LogisticsOrderFeeType.ExtraWeightFee.Name,
                FeeType = LogisticsOrderFeeType.ExtraWeightFee,
                Fee = overWeightFee,
                GoodsType = logisticsFeeData.GoodsType,
                EntryType = LogisticsFeeResultEntryType.Single
            });

            feeCalculateLogService.AddLog($"超重费用 {overWeightFee}");
        }
        */

        // 额外包裹数量，调用 GetExtraPackageCountFee()，这里的额外包裹数量是快递单号的数量

        /*
         这个放到后面总的计算了，一个订单只计算一次 合箱打包费
        // if (logisticsFeeData.PackageCount > 0) // 这个肯定成立
        {
            int totalPackageCount = logisticsFeeData.FeePackList.Sum(e => e.PackageCount); 这里的计算也不对，应该是按照入库包裹数量计算的
            decimal extraPackageCountFee = GetExtraPackageCountFee(totalPackageCount, feeRate, feeMessageType);

            extraTotalFee += extraPackageCountFee;

            resultEntryList.Add(new LogisticsFeeResultEntry {
                OrderFirstMode = logisticsFeeData.OrderFirstMode,
                LogisticsMode = logisticsFeeData.LogisticsMode,
                Title = LogisticsOrderFeeType.ExtraPackageFee.Name,
                FeeType = LogisticsOrderFeeType.ExtraPackageFee,
                Fee = extraPackageCountFee,
                GoodsType = logisticsFeeData.GoodsType,
                EntryType = LogisticsFeeResultEntryType.Single
            });

            feeCalculateLogService.AddLog($"额外包裹费 {extraPackageCountFee}");
        }
        */

        // todo 仓储, 调用 GetExtraStorageDayFee() ，这个是不是可以用订单入库的时间 和 提交打包的时间差 来做这个计算

        #endregion

        // 公布系数

        decimal publishRate = feeRate.PublishRate;

        feeCalculateLogService.AddLog($"公布系数 {publishRate}");

        decimal totalFee = (initialTotalFee + extraTotalFee) * publishRate;

        #region 记录合计费用

        string totalFeeTitle = "物流运费小计";
        resultEntryList.Add(new LogisticsFeeResultEntry {
            OrderFirstMode = feeDataItem.OrderFirstMode,
            LogisticsMode = feeDataItem.LogisticsMode,
            Title = totalFeeTitle,
            FeeType = LogisticsOrderFeeType.None,
            Fee = totalFee,
            GoodsType = feeDataItem.GoodsType,
            EntryType = LogisticsFeeResultEntryType.SubSum
        });

        feeCalculateLogService.AddLog($"{totalFeeTitle} {totalFee}");

        #endregion

        return resultEntryList;
    }

    #region 计算重量体积和最大长度

    /*
     
    /// <summary>
    /// 计算所有单个的体积重和实际重量，再计算总重量，Kg
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="feeRate"></param>
    /// <returns></returns>
    private decimal GetAllPackTotalWeight(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate)
    {
        if (packList == null || packList.Count == 0)
        {
            return 0m;
        }

        return packList.Sum(feePack => GetMultiPackWeight(feePack, feeRate));
    }
    
    */

    /// <summary>
    /// 先计算总的体积重和实际重量，再计算总重量, Kg
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="feeRate"></param>
    /// <returns></returns>
    private decimal GetWholePackTotalWeight(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        // 这里的总体逻辑和 GetSinglePackWeight 保持一致

        decimal finalWeightKg = 0m; // kg

        decimal maxSideLength = GetAllPackMaxLength(packList, feeRate);

        decimal finalActualWeightKg = GetAllPackTotalActualWeight(packList, feeRate);
        decimal finalVolumeWeightKg = 0m;

        bool careVolumeWeight = feeRate.CareVolumeWeight;
        if (careVolumeWeight == true) // 如果没有这个判断，如果费率不考虑体积重，计算体积重的时候会报错
        {
            finalVolumeWeightKg = GetAllPackTotalVolumeWeight(packList, feeRate, feeMessageType);
        }

        bool finalCareVolumeWeight = careVolumeWeight;

        if (careVolumeWeight) {
            if (feeRate.CareVolumeActualWeightRatio) {
                // 体积重实际重最小倍率，低于这个倍率，不考虑体积重；大于这个倍率，考虑体积重
                if (finalVolumeWeightKg <= finalActualWeightKg * feeRate.MinVolumeActualWeightRatio) {
                    finalCareVolumeWeight = false;
                }
            }

            if (feeRate.VolumeWeightCareMaxSideLength) {
                // 体积重最长边起始，低于这个长度，不考虑体积重；大于这个长度，考虑体积重
                if (maxSideLength <= feeRate.VolumeWeightMaxSideLengthBegin) {
                    finalCareVolumeWeight = false;
                }
            }

            if (feeRate.VolumeWeightCareSideSumBegin) {
                // 体积重三边之和起始，低于这个长度，不考虑体积重；大于这个长度，考虑体积重
                if (maxSideLength <= feeRate.VolumeWeightSideSumBegin) {
                    finalCareVolumeWeight = false;
                }
            }
        }

        decimal finalVolume = GetAllPackTotalVolume(packList, feeRate);
        decimal finalVolumeM3 = finalVolume / 100_00_00;

        /*

            启用判断逻辑：1.是否为实际重大于体积重，如果不是，则略过；2. 如果是，则执行：
            A.体积≥1个立方，＜3个立方的，采取按照实际重70%，体积重30%，折算最终计费重量 ；
            B.体积≥3个立方，＜5个立方的，采取按照实际重50%，体积重50%，折算最终计费重量 ；
            C.体积≥5个立方的，采取按照实际重30%，体积重70%，折算最终计费重量 。
         
         */

        if (finalCareVolumeWeight == true && feeRate.VolumeActualWeightMiddle == true && finalActualWeightKg > finalVolumeWeightKg && finalVolumeM3 >= 1) {
            if (1 <= finalVolumeM3 && finalVolumeM3 < 3) {
                finalWeightKg = 0.7m * finalActualWeightKg + 0.3m * finalVolumeWeightKg;
            } else if (3 <= finalVolumeM3 && finalVolumeM3 < 5) {
                finalWeightKg = 0.5m * finalActualWeightKg + 0.5m * finalVolumeWeightKg;
            } else // 5 <= finalVolumeM3 
            {
                finalWeightKg = 0.3m * finalActualWeightKg + 0.7m * finalVolumeWeightKg;
            }
        } else {
            bool careSideSum = feeRate.CareSideSum;
            if (finalCareVolumeWeight) // 考虑体积重
            {
                if (feeRate.VolumeWeightDenominator <= 0) {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物流方式不支持！SD0250");
                    } else {
                        throw new LogisticsFeeException($"通用费率 {feeRate.Name} 考虑体积重，但体积重分母 未配置 或 为0 ！");
                    }
                }

                IList<decimal> weightList = new List<decimal>();
                weightList.Add(finalActualWeightKg);
                weightList.Add(finalVolumeWeightKg); // 不考虑三边之和时 的体积重

                if (careSideSum) // 考虑三边之和时的体积重
                {
                    weightList.Add(GetAllPackTotalSideAvgVolumeWeight(packList, feeRate, feeMessageType));
                }

                // 取大 取中 取小 的问题
                // 先把重量排序
                weightList = weightList.OrderBy(e => e).ToList();

                if (feeRate.VolumeWeightResultType == VolumeWeightResultType.Big) // 取大
                {
                    finalWeightKg = weightList[weightList.Count - 1];
                } else if (feeRate.VolumeWeightResultType == VolumeWeightResultType.Small) // 取小
                {
                    finalWeightKg = weightList[0];
                } else if (feeRate.VolumeWeightResultType == VolumeWeightResultType.Middle) // 取中
                {
                    finalWeightKg = weightList[1]; // 三个元素的时候是取中，两个元素的时候就是取小了
                } else // 其他都取大
                {
                    finalWeightKg = weightList[weightList.Count - 1];
                }
            } else // 不考虑体积重
            {
                finalWeightKg = finalActualWeightKg;
            }
        }

        return finalWeightKg;
    }

    private decimal GetAllPackTotalActualWeight(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate) {
        if (packList == null || packList.Count == 0) {
            return 0m;
        }

        return packList.Sum(feePack => GetMultiPackActualWeight(feePack, feeRate));
    }

    /// <summary>
    /// 获取所以物品的总体积, 立方厘米
    /// 这里是打折后的数据
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="feeRate"></param>
    /// <returns></returns>
    private decimal GetAllPackTotalVolume(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate) {
        if (packList == null || packList.Count == 0) {
            return 0m;
        }

        return packList.Sum(feePack => GetMultiPackVolume(feePack, feeRate));
    }

    private decimal GetAllPackTotalVolumeWeight(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (packList == null || packList.Count == 0) {
            return 0m;
        }

        return packList.Sum(feePack => GetMultiPackVolumeWeight(feePack, feeRate, feeMessageType));
    }

    private decimal GetAllPackTotalSideAvgVolumeWeight(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (packList == null || packList.Count == 0) {
            return 0m;
        }

        return packList.Sum(feePack => GetMultiPackSideAvgVolumeWeight(feePack, feeRate, feeMessageType));
    }

    /// <summary>
    /// 获取所以物品的最大长度, cm
    /// 整个订单的长度，一个订单里面最长的物品的长度
    /// 这里是打折后的数据
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="feeRate"></param>
    /// <returns></returns>
    private decimal GetAllPackMaxLength(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate) {
        if (packList == null) {
            return 0m;
        }

        decimal allGoodsMaxLength = 0m; // cm， 整个订单的长度，一个订单里面最长的物品的长度

        foreach (LogisticsFeePack feePack in packList) {
            // cm
            decimal goodsMaxLength = GetPackMaxLength(feePack, feeRate);
            if (goodsMaxLength > allGoodsMaxLength) {
                allGoodsMaxLength = goodsMaxLength;
            }
        }

        return allGoodsMaxLength;
    }

    private decimal GetSinglePackActualWeight(LogisticsFeePack feePack, LogisticsFeeRateInfo feeRate) {
        if (feePack == null) {
            return 0m;
        }

        decimal finalActualWeightKg = feePack.Weight * feeRate.CustomerWeightDiscount;
        return finalActualWeightKg;
    }

    private decimal GetMultiPackActualWeight(LogisticsFeePack feePack, LogisticsFeeRateInfo feeRate) {
        // 如果没有 "数量" 数据，就假设为 1
        if (feePack.PackageCount <= 0) {
            feePack.PackageCount = 1;
        }

        return GetSinglePackActualWeight(feePack, feeRate) * feePack.PackageCount;
    }

    /// <summary>
    /// 计算单箱重量
    /// </summary>
    /// <param name="feePack"></param>
    /// <param name="feeRate"></param>
    /// <param name="feeMessageType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private decimal GetSinglePackWeight(LogisticsFeePack feePack, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        // 这里的总体逻辑和 GetWholePackTotalWeight 保持一致

        decimal finalWeightKg = 0m; // kg

        decimal maxSideLength = GetPackMaxLength(feePack, feeRate);

        decimal finalActualWeightKg = feePack.Weight * feeRate.CustomerWeightDiscount;
        decimal finalVolumeWeightKg = 0m;

        bool careVolumeWeight = feeRate.CareVolumeWeight;
        if (careVolumeWeight == true) // 如果没有这个判断，如果费率不考虑体积重，计算体积重的时候会报错
        {
            finalVolumeWeightKg = GetSinglePackVolumeWeight(feePack, feeRate, feeMessageType);
        }

        bool finalCareVolumeWeight = careVolumeWeight;

        if (careVolumeWeight) {
            if (feeRate.CareVolumeActualWeightRatio) {
                // 体积重实际重最小倍率，低于这个倍率，不考虑体积重；大于这个倍率，考虑体积重
                if (finalVolumeWeightKg <= finalActualWeightKg * feeRate.MinVolumeActualWeightRatio) {
                    finalCareVolumeWeight = false;
                }
            }

            if (feeRate.VolumeWeightCareMaxSideLength) {
                // 体积重最长边起始，低于这个长度，不考虑体积重；大于这个长度，考虑体积重
                if (maxSideLength <= feeRate.VolumeWeightMaxSideLengthBegin) {
                    finalCareVolumeWeight = false;
                }
            }

            if (feeRate.VolumeWeightCareSideSumBegin) {
                // 体积重三边之和起始，低于这个长度，不考虑体积重；大于这个长度，考虑体积重
                if (maxSideLength <= feeRate.VolumeWeightSideSumBegin) {
                    finalCareVolumeWeight = false;
                }
            }
        }

        decimal finalVolume = GetSinglePackVolume(feePack, feeRate);
        decimal finalVolumeM3 = finalVolume / 100_00_00;

        /*

            启用判断逻辑：1.是否为实际重大于体积重，如果不是，则略过；2. 如果是，则执行：
            A.体积≥1个立方，＜3个立方的，采取按照实际重70%，体积重30%，折算最终计费重量 ；
            B.体积≥3个立方，＜5个立方的，采取按照实际重50%，体积重50%，折算最终计费重量 ；
            C.体积≥5个立方的，采取按照实际重30%，体积重70%，折算最终计费重量 。
         
         */

        if (finalCareVolumeWeight == true && feeRate.VolumeActualWeightMiddle == true && finalActualWeightKg > finalVolumeWeightKg && finalVolumeM3 >= 1) {
            if (1 <= finalVolumeM3 && finalVolumeM3 < 3) {
                finalWeightKg = 0.7m * finalActualWeightKg + 0.3m * finalVolumeWeightKg;
            } else if (3 <= finalVolumeM3 && finalVolumeM3 < 5) {
                finalWeightKg = 0.5m * finalActualWeightKg + 0.5m * finalVolumeWeightKg;
            } else // 5 <= finalVolumeM3 
            {
                finalWeightKg = 0.3m * finalActualWeightKg + 0.7m * finalVolumeWeightKg;
            }
        } else {
            bool careSideSum = feeRate.CareSideSum;
            if (finalCareVolumeWeight) // 考虑体积重
            {
                if (feeRate.VolumeWeightDenominator <= 0) {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物流方式不支持！SD0260");
                    } else {
                        throw new LogisticsFeeException($"通用费率 {feeRate.Name} 考虑体积重，但体积重分母 未配置 或 为0 ！");
                    }
                }

                IList<decimal> weightList = new List<decimal>();
                weightList.Add(finalActualWeightKg);
                weightList.Add(finalVolumeWeightKg); // 不考虑三边之和时 的体积重

                if (careSideSum) // 考虑三边之和时的体积重
                {
                    weightList.Add(GetSinglePackSideAvgVolumeWeight(feePack, feeRate, feeMessageType));
                }

                // 取大 取中 取小 的问题
                // 先把重量排序
                weightList = weightList.OrderBy(e => e).ToList();

                if (feeRate.VolumeWeightResultType == VolumeWeightResultType.Big) // 取大
                {
                    finalWeightKg = weightList[weightList.Count - 1];
                } else if (feeRate.VolumeWeightResultType == VolumeWeightResultType.Small) // 取小
                {
                    finalWeightKg = weightList[0];
                } else if (feeRate.VolumeWeightResultType == VolumeWeightResultType.Middle) // 取中
                {
                    finalWeightKg = weightList[1]; // 三个元素的时候是取中，两个元素的时候就是取小了
                } else // 其他都取大
                {
                    finalWeightKg = weightList[weightList.Count - 1];
                }
            } else // 不考虑体积重
            {
                finalWeightKg = finalActualWeightKg;
            }
        }

        return finalWeightKg;
    }

    /// <summary>
    /// 计算平均单箱重量
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="feeRate"></param>
    /// <param name="feeMessageType"></param>
    /// <returns></returns>
    private decimal GetAveragePackWeight(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (packList == null || packList.Count == 0) {
            return 0m;
        }

        decimal wholePackTotalWeight = GetWholePackTotalWeight(packList, feeRate, feeMessageType);
        return wholePackTotalWeight / packList.Sum(e => e.PackageCount);
    }

    /*
     
    /// <summary>
    /// 计算一个物品或多个相同物品的“重量”， 这个“重量”是考虑体积重的重量
    /// 这里是打折后的数据
    /// </summary>
    /// <param name="feePack"></param>
    /// <param name="feeRate"></param>
    /// <returns></returns>
    private decimal GetMultiPackWeight(LogisticsFeePack feePack, LogisticsFeeRateInfo feeRate)
    {
        // 如果没有 "数量" 数据，就假设为 1
        if (feePack.Quantity <= 0)
        {
            feePack.Quantity = 1;
        }

        return GetSinglePackWeight(feePack, feeRate) * feePack.Quantity;
    }
    
    */

    private decimal GetSinglePackVolume(LogisticsFeePack feePack, LogisticsFeeRateInfo feeRate) {
        decimal finalVolumeCm3 = 0m; // 立方厘米

        // 计算客户长度打折
        decimal finalLength = feePack.Length * feeRate.CustomerLengthDiscount;
        decimal finalWidth = feePack.Width * feeRate.CustomerWidthDiscount;
        decimal finalHeight = feePack.Height * feeRate.CustomerHeightDiscount;

        finalVolumeCm3 = finalLength * finalWidth * finalHeight;

        return finalVolumeCm3;
    }

    /// <summary>
    /// 计算单个物品或多个相同物品的体积
    /// 这里是打折后的数据
    /// </summary>
    /// <param name="feePack"></param>
    /// <param name="feeRate"></param>
    /// <returns></returns>
    private decimal GetMultiPackVolume(LogisticsFeePack feePack, LogisticsFeeRateInfo feeRate) {
        decimal finalVolumeCm3 = GetSinglePackVolume(feePack, feeRate); // 立方厘米

        // 如果没有 "数量" 数据，就假设为 1
        if (feePack.PackageCount <= 0) {
            feePack.PackageCount = 1;
        }

        return finalVolumeCm3 * feePack.PackageCount;
    }

    private decimal GetSinglePackVolumeWeight(LogisticsFeePack feePack, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (feeRate.CareVolumeWeight == false) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！SD0270");
            } else {
                throw new LogisticsFeeException($"通用费率{feeRate.Name}，不考虑体积重，不能计算体积重！");
            }
        }

        if (feeRate.VolumeWeightDenominator <= 0) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！SD0280");
            } else {
                throw new LogisticsFeeException($"通用费率 {feeRate.Name} 考虑体积重，但体积重分母 未配置 或 为0 ！");
            }
        }

        return GetSinglePackVolume(feePack, feeRate) / feeRate.VolumeWeightDenominator;
    }

    private decimal GetMultiPackVolumeWeight(LogisticsFeePack feePack, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        // 如果没有 "数量" 数据，就假设为 1
        if (feePack.PackageCount <= 0) {
            feePack.PackageCount = 1;
        }

        return GetSinglePackVolumeWeight(feePack, feeRate, feeMessageType) * feePack.PackageCount;
    }

    private decimal GetSinglePackSideAvgVolumeWeight(LogisticsFeePack feePack, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (feeRate.CareVolumeWeight) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！SD0290");
            } else {
                throw new LogisticsFeeException($"通用费率{feeRate.Name}，不考虑体积重，不能计算三边之和体积重！");
            }
        }

        if (feeRate.VolumeWeightDenominator <= 0) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！SD0300");
            } else {
                throw new LogisticsFeeException($"通用费率 {feeRate.Name} 考虑体积重，但体积重分母 未配置 或 为0 ！");
            }
        }

        if (feeRate.CareSideSum) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！SD0310");
            } else {
                throw new LogisticsFeeException($"通用费率{feeRate.Name}，不考虑三边之和，不能计算三边之和体积重！");
            }
        }

        // 计算客户长度打折
        decimal finalLength = feePack.Length * feeRate.CustomerLengthDiscount;
        decimal finalWidth = feePack.Width * feeRate.CustomerWidthDiscount;
        decimal finalHeight = feePack.Height * feeRate.CustomerHeightDiscount;

        decimal averageLength = (finalLength + finalWidth + finalHeight) / 3.0m;
        return averageLength * averageLength * averageLength / feeRate.VolumeWeightDenominator;
    }

    private decimal GetMultiPackSideAvgVolumeWeight(LogisticsFeePack feePack, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        // 如果没有 "数量" 数据，就假设为 1
        if (feePack.PackageCount <= 0) {
            feePack.PackageCount = 1;
        }

        return GetSinglePackSideAvgVolumeWeight(feePack, feeRate, feeMessageType) * feePack.PackageCount;
    }

    /// <summary>
    /// 计算物品的最长边，这个跟数量没关系
    /// 这里是打折后的数据
    /// </summary>
    /// <param name="feePack"></param>
    /// <param name="feeRate"></param>
    /// <returns></returns>
    private decimal GetPackMaxLength(LogisticsFeePack feePack, LogisticsFeeRateInfo feeRate) {
        // 长宽高排下序
        IList<decimal> lengthList = new List<decimal>();
        lengthList.Add(feePack.Length);
        lengthList.Add(feePack.Width);
        lengthList.Add(feePack.Height);

        lengthList = lengthList.OrderBy(e => e).ToList();

        //  取最长的一边，并计算客户长度打折
        decimal finalLength = lengthList[2] * feeRate.CustomerLengthDiscount;

        return finalLength;
    }

    #endregion

    #region 检查起步长度和重量

    /// <summary>
    /// 检查起步长度
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="feeRate"></param>
    /// <param name="feeMessageType"></param>
    private void CheckLengthStart(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (feeRate.HasLengthStart && feeRate.LengthStart != null) {
            if (feeRate.LengthStart <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("物品起步长度物流方式不支持！SD0810");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 有长度起步，但起步长度 未配置 或 为0 ！");
                }
            }

            decimal maxLength = GetAllPackMaxLength(packList, feeRate); // 最大长度，就是这个订单的长度  
            decimal lengthStart = feeRate.LengthStart.Value;
            if (maxLength < lengthStart) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("物品长度未到物流方式起步长度！SD0820");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 长度未到起步长度！");
                }
            }
        }
    }

    /// <summary>
    /// 检查单箱起步重量
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="feeRate"></param>
    /// <param name="feeMessageType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private void CheckSingleWeightStart(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (feeRate.HasSingleWeightStart && feeRate.SingleWeightStart != null) {
            if (feeRate.SingleWeightStart <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("物品单箱起步重量物流方式不支持！SD0830");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 有单箱重量起步，但单箱起步重量 未配置 或 为0 ！");
                }
            }

            decimal singleWeightStart = feeRate.SingleWeightStart.Value;
            foreach (var orderPack in packList) {
                decimal singleFinalWeight = GetSinglePackWeight(orderPack, feeRate, feeMessageType);
                if (singleFinalWeight < singleWeightStart) {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物品单箱重量未到物流方式单箱起步重量！SD0840");
                    } else {
                        throw new LogisticsFeeException($"通用费率 {feeRate.Name} 单箱重量(或体积重)未到单箱起步重量！");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 检查平均单箱起步重量
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="feeRate"></param>
    /// <param name="feeMessageType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private void CheckAverageWeightStart(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (feeRate.HasAverageWeightStart && feeRate.AverageWeightStart != null) {
            if (feeRate.AverageWeightStart <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("物品平均单箱起步重量物流方式不支持！SD0831");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 有平均单箱重量起步，但平均单箱起步重量 未配置 或 为0 ！");
                }
            }

            decimal averageWeightStart = feeRate.AverageWeightStart.Value;
            decimal averageFinalWeight = GetAveragePackWeight(packList, feeRate, feeMessageType);
            if (averageFinalWeight < averageWeightStart) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("物品平均单箱重量未到物流方式平均单箱起步重量！SD0841");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 平均单箱重量(或体积重)未到平均单箱起步重量！");
                }
            }
        }
    }

    /// <summary>
    /// 检查起步重量
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="feeRate"></param>
    /// <param name="feeMessageType"></param>
    private void CheckWeightStart(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (feeRate.HasWeightStart && feeRate.WeightStart != null) {
            if (feeRate.WeightStart <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("物品起步重量物流方式不支持！SD0850");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 有重量起步，但起步重量 未配置 或 为0 ！");
                }
            }

            decimal totalWeight = GetWholePackTotalWeight(packList, feeRate, feeMessageType);
            decimal weightStart = feeRate.WeightStart.Value;
            if (totalWeight < weightStart) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("物品重量未到物流方式起步重量！SD0860");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 重量未到起步重量！");
                }
            }
        }
    }

    #endregion

    #region 计算加收费用

    /// <summary>
    /// 计算超长费用
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="feeRate"></param>
    /// <param name="feeMessageType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private decimal GetOverLengthFee(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        // 如果有长度限制
        if (feeRate.HasLengthLimit) {
            if (feeRate.LengthLimit.HasValue == false || feeRate.LengthLimit.Value <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("物品超长物流方式不支持！SD0320");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 有长度限制，但限制长度 未配置 或 为0 ！");
                }
            }

            decimal allPackMaxLength = GetAllPackMaxLength(packList, feeRate);

            decimal lengthLimit = feeRate.LengthLimit.Value;

            if (allPackMaxLength > lengthLimit) {
                if (feeRate.CanOverLength) {
                    if (feeRate.OverLengthStep.HasValue == false || feeRate.OverLengthStep.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物品超长物流方式不支持！SD0330");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 可以超长，但超长阶梯 未配置 或 为0 ！");
                        }
                    }

                    if (feeRate.OverLengthStepFee.HasValue == false || feeRate.OverLengthStepFee.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物品超长物流方式不支持！SD0340");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 可以超长，但超长阶梯收费 未配置 或 为0 ！");
                        }
                    }

                    decimal overLengthStep = feeRate.OverLengthStep.Value;
                    if (overLengthStep <= 0m) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物品超长物流方式不支持！SD0350");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率设置允许超长，但超长阶梯为0！");
                        }
                    }

                    decimal overLengthStepFee = feeRate.OverLengthStepFee.Value;

                    // 初始化超长费用为0
                    decimal overLengthFee = 0m;

                    if (feeRate.OverLengthFeeEveryPack == false) // 只收超长最长的一件的费用
                    {
                        decimal overStepCount = 0m;
                        if (allPackMaxLength <= lengthLimit) // 其实这个判断肯定不成立
                        {
                            overStepCount = 0m;
                        } else {
                            overStepCount = Math.Ceiling(Math.Abs(allPackMaxLength - lengthLimit) / overLengthStep);
                        }

                        overLengthFee = overStepCount * overLengthStepFee;
                    } else // 每件超长的都收超长费用
                    {
                        foreach (LogisticsFeePack feePack in packList) {
                            // decimal singlePackMaxLength = Math.Max(Math.Max(feePack.Length, feePack.Width), feePack.Height); 用下面的方式
                            decimal singlePackMaxLength = GetPackMaxLength(feePack, feeRate);

                            if (singlePackMaxLength <= lengthLimit) {
                                continue;
                            }

                            decimal overStepCount = Math.Ceiling(Math.Abs(singlePackMaxLength - lengthLimit) / overLengthStep);

                            // 累加
                            overLengthFee = overLengthFee + (overStepCount * overLengthStepFee) * feePack.PackageCount; // Quantity 也不能忘了 
                        }
                    }

                    // 是否有封顶金额
                    if (feeRate.HasOverLengthCapAmount) {
                        if (feeRate.OverLengthCapAmount.HasValue == false || feeRate.OverLengthCapAmount.Value <= 0) {
                            if (feeMessageType == FeeMessageType.Customer) {
                                throw new LogisticsFeeException("物品超长物流方式不支持！SD0360");
                            } else {
                                throw new LogisticsFeeException($"通用费率 {feeRate.Name}，长度有费用封顶，但封顶金额 未设置 或 为0 ！");
                            }
                        }

                        if (overLengthFee > feeRate.OverLengthCapAmount.Value) {
                            overLengthFee = feeRate.OverLengthCapAmount.Value;
                        }
                    }

                    return overLengthFee;
                } else {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物品长度超出物流方式长度限制！SD0370");
                    } else {
                        throw new LogisticsFeeException($"通用费率 {feeRate.Name} 长度超出长度限制，但费率设置不允许超长！");
                    }
                }
            }
        }

        // 如果没用长度限制就没有超长费用
        return 0m;
    }

    /// <summary>
    /// 计算各个单箱超重的总费用
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="feeRate"></param>
    /// <param name="feeMessageType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private decimal GetSingleOverWeightFee(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        // 如果有单箱重量限制
        if (feeRate.HasSingleWeightLimit) {
            if (feeRate.SingleWeightLimit.HasValue == false || feeRate.SingleWeightLimit.Value <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("重量超出物流方式单箱重量限制！SD0380");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 有单箱重量限制，但单箱限制重量 未设置 或 为0 ！");
                }
            }

            decimal singleWeightLimit = feeRate.SingleWeightLimit.Value;

            decimal totalSingleOverWeightFee = 0m;

            foreach (var orderPack in packList) {
                decimal singleFinalWeight = GetSinglePackWeight(orderPack, feeRate, feeMessageType);
                if (singleFinalWeight > singleWeightLimit) {
                    if (feeRate.CanSingleOverWeight) {
                        if (feeRate.SingleOverWeightStep.HasValue == false || feeRate.SingleOverWeightStep.Value <= 0) {
                            if (feeMessageType == FeeMessageType.Customer) {
                                throw new LogisticsFeeException("物品重量超出物流方式单箱重量限制！SD0390");
                            } else {
                                throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率设置允许单箱超重，但单箱超重阶梯 未设置 或 为0 ！");
                            }
                        }

                        if (feeRate.SingleOverWeightStepFee.HasValue == false || feeRate.SingleOverWeightStepFee.Value <= 0) {
                            if (feeMessageType == FeeMessageType.Customer) {
                                throw new LogisticsFeeException("物品重量超出物流方式单箱重量限制！SD0400");
                            } else {
                                throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率设置允许单箱超重，但单箱超重阶梯收费 未设置 或 为0 ！");
                            }
                        }

                        decimal singleOverWeightStep = feeRate.SingleOverWeightStep.Value;
                        decimal singleOverWeightStepFee = feeRate.SingleOverWeightStepFee.Value;

                        decimal singleOverStepCount = 0m;
                        if (singleFinalWeight < singleWeightLimit) // 其实这个判断肯定不成立
                        {
                            singleOverStepCount = 0m;
                        } else {
                            singleOverStepCount = Math.Ceiling(Math.Abs(singleFinalWeight - singleWeightLimit) / singleOverWeightStep);
                        }

                        decimal singleOverWeightFee = singleOverStepCount * singleOverWeightStepFee * orderPack.PackageCount;

                        totalSingleOverWeightFee += singleOverWeightFee;
                    } else {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物品重量超出物流方式单箱重量限制！SD0410");
                        } else {
                            throw new LogisticsFeeException($"单箱重量(或体积重)超出重量限制，但通用费率 {feeRate.Name} 费率设置不允许单箱超重！");
                        }
                    }
                }
            }

            if (feeRate.HasSingleOverWeightCapAmount) {
                if (feeRate.SingleOverWeightCapAmount.HasValue == false || feeRate.SingleOverWeightCapAmount.Value <= 0) {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物品重量超出物流方式单箱重量限制！SD0420");
                    } else {
                        throw new LogisticsFeeException($"通用费率 {feeRate.Name} 单箱重量有费用封顶，但封顶金额 未设置 或 为0 ！");
                    }
                }

                if (totalSingleOverWeightFee > feeRate.SingleOverWeightCapAmount.Value) {
                    totalSingleOverWeightFee = feeRate.SingleOverWeightCapAmount.Value;
                }
            }

            return totalSingleOverWeightFee;
        }

        // 如果没有单箱重量限制就没有单箱超重费用
        return 0m;
    }

    /// <summary>
    /// 计算平均单箱重量超重费用
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="feeRate"></param>
    /// <param name="feeMessageType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private decimal GetAverageOverWeightFee(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        // 如果有重量限制
        if (feeRate.HasAverageWeightLimit) {
            decimal finalAverageWeightKg = GetAveragePackWeight(packList, feeRate, feeMessageType);

            decimal averageWeightLimit = 0m;
            if (feeRate.AverageWeightLimit.HasValue) {
                averageWeightLimit = feeRate.AverageWeightLimit.Value;
            }

            if (finalAverageWeightKg > averageWeightLimit) {
                if (feeRate.CanAverageOverWeight) {
                    if (feeRate.AverageOverWeightStep.HasValue == false || feeRate.AverageOverWeightStep.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物品平均单箱重量超出物流方式限制！SD0431");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率设置允许平均单箱超重，但超重阶梯 未设置 或 为0 ！");
                        }
                    }

                    if (feeRate.AverageOverWeightStepFee.HasValue == false || feeRate.AverageOverWeightStepFee.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物品总重量超出物流方式限制！SD0441");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率设置允许平均单箱超重，但超重阶梯收费 未设置 或 为0 ！");
                        }
                    }

                    decimal averageOverWeightStep = feeRate.AverageOverWeightStep.Value;
                    decimal averageOverWeightStepFee = feeRate.AverageOverWeightStepFee.Value;

                    decimal averageOverStepCount = 0m;
                    if (finalAverageWeightKg < averageWeightLimit) // 其实这个判断肯定不成立
                    {
                        averageOverStepCount = 0m;
                    } else {
                        averageOverStepCount = Math.Ceiling(Math.Abs(finalAverageWeightKg - averageWeightLimit) / averageOverWeightStep);
                    }

                    decimal averageOverWeightFee = averageOverStepCount * averageOverWeightStepFee;

                    if (feeRate.HasAverageOverWeightCapAmount) {
                        if (feeRate.AverageOverWeightCapAmount.HasValue == false || feeRate.AverageOverWeightCapAmount.Value <= 0) {
                            if (feeMessageType == FeeMessageType.Customer) {
                                throw new LogisticsFeeException("物品平均单箱重量超出物流方式限制！SD0451");
                            } else {
                                throw new LogisticsFeeException($"通用费率 {feeRate.Name} 重量有费用封顶，但封顶金额 未设置 或 为0 ！");
                            }
                        }

                        if (averageOverWeightFee > feeRate.AverageOverWeightCapAmount.Value) {
                            averageOverWeightFee = feeRate.AverageOverWeightCapAmount.Value;
                        }
                    }

                    return averageOverWeightFee;
                } else {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物品平均单箱重量超出物流方式限制！SD0461");
                    } else {
                        throw new LogisticsFeeException($"平均单箱重量(或体积重)超出重量限制，但通用费率 {feeRate.Name} 费率设置不允许超重！");
                    }
                }
            }
        }

        // 如果没有重量限制就没有超重费用
        return 0m;
    }

    /*
     
     这个费用不用计算了

    /// <summary>
    /// 计算总重量超重费用
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="feeRate"></param>
    /// <param name="feeMessageType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private decimal GetOverWeightFee(IList<LogisticsFeePack> packList, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        // 如果有重量限制
        if (feeRate.HasWeightLimit) {
            decimal finalTotalWeightKg = GetWholePackTotalWeight(packList, feeRate, feeMessageType);


            decimal weightLimit = 0m;
            if (feeRate.WeightLimit.HasValue) {
                weightLimit = feeRate.WeightLimit.Value;
            }

            if (finalTotalWeightKg > weightLimit) {
                if (feeRate.CanOverWeight) {
                    if (feeRate.OverWeightStep.HasValue == false || feeRate.OverWeightStep.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物品总重量超出物流方式限制！SD0430");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率设置允许超重，但超重阶梯 未设置 或 为0 ！");
                        }
                    }

                    if (feeRate.OverWeightStepFee.HasValue == false || feeRate.OverWeightStepFee.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物品总重量超出物流方式限制！SD0440");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率设置允许超重，但超重阶梯收费 未设置 或 为0 ！");
                        }
                    }

                    decimal overWeightStep = feeRate.OverWeightStep.Value;
                    decimal overWeightStepFee = feeRate.OverWeightStepFee.Value;

                    decimal overStepCount = 0m;
                    if (finalTotalWeightKg < weightLimit) // 其实这个判断肯定不成立
                    {
                        overStepCount = 0m;
                    } else {
                        overStepCount = Math.Ceiling(Math.Abs(finalTotalWeightKg - weightLimit) / overWeightStep);
                    }

                    decimal overWeightFee = overStepCount * overWeightStepFee;

                    if (feeRate.HasOverWeightCapAmount) {
                        if (feeRate.OverWeightCapAmount.HasValue == false || feeRate.OverWeightCapAmount.Value <= 0) {
                            if (feeMessageType == FeeMessageType.Customer) {
                                throw new LogisticsFeeException("物品总重量超出物流方式限制！SD0450");
                            } else {
                                throw new LogisticsFeeException($"通用费率 {feeRate.Name} 重量有费用封顶，但封顶金额 未设置 或 为0 ！");
                            }
                        }

                        if (overWeightFee > feeRate.OverWeightCapAmount.Value) {
                            overWeightFee = feeRate.OverWeightCapAmount.Value;
                        }
                    }

                    return overWeightFee;
                } else {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物品总重量超出物流方式限制！SD0460");
                    } else {
                        throw new LogisticsFeeException($"总重量(或体积重)超出重量限制，但通用费率 {feeRate.Name} 费率设置不允许超重！");
                    }
                }
            }
        }

        // 如果没有重量限制就没有超重费用
        return 0m;
    }
    
    */

    /// <summary>
    /// 计算额外包裹数量费用
    /// 现在应该是按入库快递单号数量计算这个费用的
    /// </summary>
    /// <param name="packageCount"></param>
    /// <param name="feeRate"></param>
    /// <param name="feeMessageType"></param>
    /// <returns></returns>
    [Obsolete]
    private decimal GetExtraPackageCountFee(int packageCount, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (feeRate.HasFreePackageLimit) {
            if (feeRate.FreePackageLimit.HasValue == false || feeRate.FreePackageLimit.Value < 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("包裹数量超出物流方式限制！SD0470");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 包裹有免费数量限制，但免费包裹数量 未配置 或 小于0 ！");
                }
            }

            if (feeRate.ExtraPackageFee.HasValue == false || feeRate.ExtraPackageFee.Value <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("包裹数量超出物流方式限制！SD0480");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 包裹有免费数量限制，但额外包裹收费 未配置 或 值不大于0 ！");
                }
            }

            decimal extraPackageTotalFee = 0m;
            if (packageCount > feeRate.FreePackageLimit.Value) {
                extraPackageTotalFee = (packageCount - feeRate.FreePackageLimit.Value) * feeRate.ExtraPackageFee.Value;

                if (feeRate.HasExtraPackageFeeCapAmount) {
                    if (feeRate.ExtraPackageFeeCapAmount.HasValue == false || feeRate.ExtraPackageFeeCapAmount.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("包裹数量超出物流方式限制！SD0490");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 有额外包裹数量费用封顶，但封顶金额 未配置 或 值不大于0 ！");
                        }
                    }

                    if (extraPackageTotalFee > feeRate.ExtraPackageFeeCapAmount.Value) {
                        extraPackageTotalFee = feeRate.ExtraPackageFeeCapAmount.Value;
                    }
                }
            }

            return extraPackageTotalFee;
        }

        return 0m;
    }

    public decimal GetExtraPackageCountFee(int packageCount, LogisticsModeSettingInfo logisticsModeSetting, FeeMessageType feeMessageType) {
        if (logisticsModeSetting.HasFreePackageLimit) {
            if (logisticsModeSetting.FreePackageLimit.HasValue == false || logisticsModeSetting.FreePackageLimit.Value < 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("包裹数量超出物流方式限制！SD0540");
                } else {
                    throw new LogisticsFeeException("物流设置中包裹有免费数量限制，但免费包裹数量 未配置 或 小于0 ！");
                }
            }

            if (logisticsModeSetting.ExtraPackageFee.HasValue == false || logisticsModeSetting.ExtraPackageFee.Value <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("包裹数量超出物流方式限制！SD0560");
                } else {
                    throw new LogisticsFeeException($"物流设置中包裹有免费数量限制，但额外包裹收费 未配置 或 值不大于0 ！");
                }
            }

            decimal extraPackageTotalFee = 0m;
            if (packageCount > logisticsModeSetting.FreePackageLimit.Value) {
                extraPackageTotalFee = (packageCount - logisticsModeSetting.FreePackageLimit.Value) * logisticsModeSetting.ExtraPackageFee.Value;

                if (logisticsModeSetting.HasExtraPackageFeeCapAmount) {
                    if (logisticsModeSetting.ExtraPackageFeeCapAmount.HasValue == false || logisticsModeSetting.ExtraPackageFeeCapAmount.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("包裹数量超出物流方式限制！SD0580");
                        } else {
                            throw new LogisticsFeeException($"物流设置中有额外包裹数量费用封顶，但封顶金额 未配置 或 值不大于0 ！");
                        }
                    }

                    if (extraPackageTotalFee > logisticsModeSetting.ExtraPackageFeeCapAmount.Value) {
                        extraPackageTotalFee = logisticsModeSetting.ExtraPackageFeeCapAmount.Value;
                    }
                }
            }

            return extraPackageTotalFee;
        }

        return 0m;
    }

    /// <summary>
    /// 计算额外仓储天数费用
    /// </summary>
    /// <param name="storageDaysCount"></param>
    /// <param name="feeRate"></param>
    /// <param name="feeMessageType"></param>
    /// <returns></returns>
    private decimal GetExtraStorageDayFee(int storageDaysCount, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (feeRate.HasFreeStorageDayLimit) {
            if (feeRate.FreeStorageDayLimit.HasValue == false || feeRate.FreeStorageDayLimit.Value <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException($"超出物流方式仓储天数限制 ！SD0500");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 仓储有免费天数限制，但免费仓储天数 未配置 或 为0 ！");
                }
            }

            if (feeRate.ExtraStorageDayFee.HasValue == false || feeRate.ExtraStorageDayFee.Value <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException($"超出物流方式仓储天数限制 ！SD0510");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 仓储有免费天数限制，但额外仓储收费 未配置 或 为0 ！");
                }
            }

            decimal extraStorageDayTotalFee = 0m;
            if (storageDaysCount > feeRate.FreeStorageDayLimit.Value) {
                extraStorageDayTotalFee = (storageDaysCount - feeRate.FreeStorageDayLimit.Value) * feeRate.ExtraStorageDayFee.Value;

                if (feeRate.HasExtraStorageDayFeeCapAmount) {
                    if (feeRate.ExtraStorageDayFeeCapAmount.HasValue == false || feeRate.ExtraStorageDayFeeCapAmount.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException($"超出物流方式仓储天数限制 ！SD0520");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 有额外仓储天数费用封顶，但封顶金额 未配置 或 为0 ！");
                        }
                    }

                    if (extraStorageDayTotalFee > feeRate.ExtraStorageDayFeeCapAmount.Value) {
                        extraStorageDayTotalFee = feeRate.ExtraStorageDayFeeCapAmount.Value;
                    }
                }
            }

            return extraStorageDayTotalFee;
        }

        return 0m;
    }

    #endregion
}