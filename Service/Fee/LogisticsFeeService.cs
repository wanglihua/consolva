using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;

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
    private ISession session = null;
    private LogisticsFeeInsuranceService logisticsFeeInsuranceService = null;
    private LogisticsFeeGstService logisticsFeeGstService = null;
    private LogisticsFeeCnAuExpressService logisticsFeeCnAuExpressService = null;
    private LogisticsFeeAuExpressService logisticsFeeAuExpressService = null;
    private FeeCalculateLogService feeCalculateLogService = null;

    public LogisticsFeeService(
        ISession session,
        LogisticsFeeInsuranceService logisticsFeeInsuranceService,
        LogisticsFeeGstService logisticsFeeGstService,
        LogisticsFeeCnAuExpressService logisticsFeeCnAuExpressService,
        LogisticsFeeAuExpressService logisticsFeeAuExpressService,
        FeeCalculateLogService feeCalculateLogService) {
        this.session = session;

        this.logisticsFeeInsuranceService = logisticsFeeInsuranceService;
        this.logisticsFeeGstService = logisticsFeeGstService;

        this.logisticsFeeCnAuExpressService = logisticsFeeCnAuExpressService;
        this.logisticsFeeAuExpressService = logisticsFeeAuExpressService;
        this.feeCalculateLogService = feeCalculateLogService;
    }

    /// <summary>
    /// 根据物流数据计算物流费用
    /// 这是只计算一程的费用， 所以参数里面需要指定 LogisticsMode
    /// </summary>
    /// <param name="logisticsFeeData"></param>
    /// <param name="logisticsMode"></param>
    /// <returns></returns>
    public IList<LogisticsFeeResultEntry> GetLogisticsFeeForCustomer(LogisticsFeeData logisticsFeeData, LogisticsModeInfo logisticsMode) {
        return GetLogisticsFee(logisticsFeeData, logisticsMode, LogisticsFeeRateType.Customer, FeeMessageType.Customer);
    }

    /// <summary>
    /// 生成一个假定费用都是零的取货的计费结果
    /// </summary>
    /// <returns></returns>
    public IList<LogisticsFeeResultEntry> MockPickupStationFee(LogisticsGoodsType goodsType) {
        IList<LogisticsFeeResultEntry> feeResultEntryList = new List<LogisticsFeeResultEntry>();

        feeResultEntryList.Add(new LogisticsFeeResultEntry() {
            EntryType = LogisticsFeeResultEntryType.Single,
            GoodsType = goodsType,
            FeeType = LogisticsOrderFeeType.SecondLogisticsFee,
            Title = "澳洲境内初始费用",
            Fee = 0m
        });

        feeResultEntryList.Add(new LogisticsFeeResultEntry() {
            EntryType = LogisticsFeeResultEntryType.Single,
            GoodsType = goodsType,
            FeeType = LogisticsOrderFeeType.SecondLogisticsFeeExtraLength,
            Title = "超长费用",
            Fee = 0m
        });

        feeResultEntryList.Add(new LogisticsFeeResultEntry() {
            EntryType = LogisticsFeeResultEntryType.Single,
            GoodsType = goodsType,
            FeeType = LogisticsOrderFeeType.SecondLogisticsFeeSingleExtraWeight,
            Title = "单箱超重费用",
            Fee = 0m
        });

        feeResultEntryList.Add(new LogisticsFeeResultEntry() {
            EntryType = LogisticsFeeResultEntryType.Single,
            GoodsType = goodsType,
            FeeType = LogisticsOrderFeeType.SecondLogisticsFeeExtraWeight,
            Title = "超重费用",
            Fee = 0m
        });

        feeResultEntryList.Add(new LogisticsFeeResultEntry() {
            EntryType = LogisticsFeeResultEntryType.SubSum,
            GoodsType = goodsType,
            Title = "澳洲境内运费合计",
            Fee = 0m
        });

        return feeResultEntryList;
    }

    /// <summary>
    /// 根据物流数据计算物流费用
    /// 这是只计算一程的费用， 所以参数里面需要指定 LogisticsMode
    /// </summary>
    /// <param name="logisticsFeeData"></param>
    /// <param name="logisticsMode"></param>
    /// <param name="feeRateType"></param>
    /// <returns></returns>
    public IList<LogisticsFeeResultEntry> GetLogisticsFee(LogisticsFeeData logisticsFeeData, LogisticsModeInfo logisticsMode, LogisticsFeeRateType feeRateType) // 原来的代码都调用了这个版本
    {
        return GetLogisticsFee(logisticsFeeData, logisticsMode, feeRateType, FeeMessageType.User);
    }

    /// <summary>
    /// 根据物流数据计算物流费用
    /// 这是只计算一程的费用， 所以参数里面需要指定 LogisticsMode
    /// </summary>
    /// <param name="logisticsFeeData"></param>
    /// <param name="logisticsMode"></param>
    /// <param name="feeRateType"></param>
    /// <param name="feeMessageType"></param>
    /// <returns></returns>
    public IList<LogisticsFeeResultEntry> GetLogisticsFee(LogisticsFeeData logisticsFeeData, LogisticsModeInfo logisticsMode, LogisticsFeeRateType feeRateType, FeeMessageType feeMessageType) {
        if (logisticsMode == null) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！AP0121");
            } else {
                throw new Exception("物流方式不能为空！");
            }
        }

        LogisticsGoodsType goodsType = logisticsFeeData.GoodsType;

        if (goodsType == LogisticsGoodsType.Forbidden) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("禁止类物品不允许运输！AP0130");
            } else {
                throw new Exception("禁止类物品不进行计费！");
            }
        }

        bool isFirstLogisticsModeType = LogisticsModeType.IsCnAuFirstType(logisticsMode.Type.Id);
        bool isSecondLogisticsModeType = LogisticsModeType.IsCnAuSecondLogisticsModeType(logisticsMode.Type.Id);
        int goodsTypeId = logisticsFeeData.GoodsType.Id;

        #region 数据检查

        if (isFirstLogisticsModeType == false && isSecondLogisticsModeType == false) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！AP0131");
            } else {
                throw new Exception($"{logisticsMode.Name} 当前不支持的物流方式类型！");
            }
        }

        IList<LogisticsFeePack> feePackList = logisticsFeeData.FeePackList;
        if (feePackList == null || feePackList.Count == 0) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("未能获取物品尺寸数据！AP0140");
            } else {
                throw new Exception("物品打包数据不能为空！");
            }
        }

        IList<LogisticsFeeRateInfo> feeRateList = new List<LogisticsFeeRateInfo>();

        if (isFirstLogisticsModeType) {
            if (logisticsFeeData.GoodsType == LogisticsGoodsType.Common) {
                if (logisticsMode.CommonFeeRate == null) {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物流方式不支持！AP0150");
                    } else {
                        throw new LogisticsFeeException($"物流方式 {logisticsMode.Name} 普通类通用费率不能为空！");
                    }
                }

                feeRateList.Add(logisticsMode.CommonFeeRate);
            }

            if (logisticsFeeData.GoodsType == LogisticsGoodsType.Restrict || logisticsFeeData.GoodsType == LogisticsGoodsType.RestrictBamboo) {
                if (logisticsMode.RestrictFeeRate == null) {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物流方式不支持！AP0160");
                    } else {
                        throw new LogisticsFeeException($"物流方式 {logisticsMode.Name} 限制类通用费率不能为空！");
                    }
                }

                feeRateList.Add(logisticsMode.RestrictFeeRate);
            }

            if (logisticsFeeData.GoodsType == LogisticsGoodsType.Bamboo || logisticsFeeData.GoodsType == LogisticsGoodsType.RestrictBamboo) {
                if (logisticsMode.BambooFeeRate == null) {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物流方式不支持！AP0170");
                    } else {
                        throw new LogisticsFeeException($"物流方式 {logisticsMode.Name} 草木竹类通用费率不能为空！");
                    }
                }

                feeRateList.Add(logisticsMode.BambooFeeRate);
            }

            if (logisticsFeeData.GoodsType == LogisticsGoodsType.Special) {
                if (logisticsMode.SpecialFeeRate == null) {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物流方式不支持！AP0180");
                    } else {
                        throw new LogisticsFeeException($"物流方式 {logisticsMode.Name} 特货类通用费率不能为空！");
                    }
                }

                feeRateList.Add(logisticsMode.SpecialFeeRate);
            }
        }

        if (isSecondLogisticsModeType) {
            if (logisticsMode.Type == LogisticsModeType.TerminalPickup) {
                if (feeRateType == LogisticsFeeRateType.Customer) {
                    if (logisticsMode.CustomerFeeRate == null) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物流方式不支持！AP0190");
                        } else {
                            throw new LogisticsFeeException($"自提点物流方式 {logisticsMode.Name} 客户费率不能为空！");
                        }
                    }

                    feeRateList.Add(logisticsMode.CustomerFeeRate);
                }

                if (feeRateType == LogisticsFeeRateType.Inner) {
                    if (logisticsMode.InnerFeeRate == null) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物流方式不支持！AP0200");
                        } else {
                            throw new LogisticsFeeException($"自提点物流方式 {logisticsMode.Name} 内部费率不能为空！");
                        }
                    }

                    feeRateList.Add(logisticsMode.InnerFeeRate);
                }
            } else if (logisticsMode.Type == LogisticsModeType.HomeDelivery) {
                if (feeRateType == LogisticsFeeRateType.Customer) {
                    if (logisticsMode.CustomerFeeRate == null) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物流方式不支持！AP0210");
                        } else {
                            throw new LogisticsFeeException($"送货上门物流方式 {logisticsMode.Name} 客户费率不能为空！");
                        }
                    }

                    feeRateList.Add(logisticsMode.CustomerFeeRate);
                }

                if (feeRateType == LogisticsFeeRateType.Inner) {
                    if (logisticsMode.InnerFeeRate == null) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物流方式不支持！AP0220");
                        } else {
                            throw new LogisticsFeeException($"送货上门物流方式 {logisticsMode.Name} 内部费率不能为空！");
                        }
                    }

                    feeRateList.Add(logisticsMode.InnerFeeRate);
                }
            } else {
                if (logisticsMode.FeeRate == null) {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物流方式不支持！AP0221");
                    } else {
                        throw new LogisticsFeeException($"物流方式 {logisticsMode.Name} 通用费率不能为空！");
                    }
                }

                feeRateList.Add(logisticsMode.FeeRate);
            }
        }

        if (feeRateList.Count != 0) {
            foreach (var feeRate in feeRateList) {
                if (feeRate.CareVolumeWeight) {
                    if (feeRate.VolumeDenominator <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物流方式不支持！AP0230");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率考虑体积重时，体积重分母应该大于0！");
                        }
                    }

                    if (feeRate.CareVolumeActualWeightRatio && feeRate.MinVolumeActualWeightRatio <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物流方式不支持！AP0240");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率考虑体积重实际重倍率时，体积重实际重最小倍率应该大于0！");
                        }
                    }

                    if (feeRate.VolumeWeightCareMaxSideLength && feeRate.VolumeWeightMaxSideLengthBegin <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物流方式不支持！AP0243");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率体积重考虑最长边时，体积重最长边起始应该大于0！");
                        }
                    }
                }
            }
        }

        if (feeRateList.Count == 0) {
            feeRateList.Add(logisticsMode.FeeRate);
        }

        #endregion

        #region 检查起步长度和重量

        foreach (LogisticsFeeRateInfo feeRate in feeRateList) {
            CheckLengthStart(feePackList, feeRate, feeMessageType);
            CheckSingleWeightStart(feePackList, feeRate, feeMessageType);
            CheckAverageWeightStart(feePackList, feeRate, feeMessageType);
            CheckWeightStart(feePackList, feeRate, feeMessageType);
        }

        #endregion

        #region 计算最终体积重量和最长边

        // kg，订单总重量，考虑了体积重后的重量
        decimal finalTotalWeightKg = feeRateList.Max(feeRate => GetWholePackTotalWeight(feePackList, feeRate, feeMessageType));

        feeCalculateLogService.AddLog($"考虑体积重后的订单总重量：{finalTotalWeightKg}");

        // 立方厘米
        // decimal finalTotalVolumeCm3 = feeRateList.Max(feeRate => GetAllPackTotalVolume(feePackList, feeRate));

        #endregion

        IList<LogisticsFeeResultEntry> resultEntryList = new List<LogisticsFeeResultEntry>();

        #region 计算初步费用

        feeCalculateLogService.AddLog($"计算初步费用 开始");

        decimal initialTotalFee = 0m;

        var logisticsModeId = logisticsMode.Id;
        var logisticsModeTypeId = logisticsMode.Type;

        // 跨国快递
        if (logisticsModeTypeId == LogisticsModeType.Express)
        {
            initialTotalFee = logisticsFeeCnAuExpressService.GetFee(finalTotalWeightKg, logisticsModeId, feeMessageType);
        } else {
            throw new ValidationException("未知物流方式类型，无法计费！");
        }

        string initialTotalFeeTitle = "初始费用";

        LogisticsOrderFeeType feeType = null;

        if (isFirstLogisticsModeType) {
            initialTotalFeeTitle = "跨国段初始费用";
            feeType = LogisticsOrderFeeType.FirstLogisticsFee;
        } else if (isSecondLogisticsModeType) {
            initialTotalFeeTitle = "澳洲境内初始费用";
            feeType = LogisticsOrderFeeType.SecondLogisticsFee;
        }

        resultEntryList.Add(new LogisticsFeeResultEntry() {
            Title = initialTotalFeeTitle,
            Fee = initialTotalFee,
            GoodsType = logisticsFeeData.GoodsType,
            FeeType = feeType,
            EntryType = LogisticsFeeResultEntryType.Single
        });

        feeCalculateLogService.AddLog($"{initialTotalFeeTitle}: {initialTotalFee}");
        feeCalculateLogService.AddLog($"计算初步费用 结束");

        #endregion

        #region 计算加收费用

        decimal extraTotalFee = 0m;

        // 这里的 超长 和 超重 都是针对的是 "原来的没打折的数据" 还是 "打折后的数据".
        // 这里实际使用的是 "打折后的数据"

        // 超长费用
        decimal overLengthFee = feeRateList.Max(feeRate => GetOverLengthFee(feePackList, feeRate, feeMessageType));

        //if (overLengthFee > 0)
        {
            extraTotalFee += overLengthFee;

            LogisticsOrderFeeType feeTypeExtraLength = null;

            if (isFirstLogisticsModeType) {
                feeTypeExtraLength = LogisticsOrderFeeType.FirstLogisticsFeeExtraLength;
            }

            if (isSecondLogisticsModeType) {
                feeTypeExtraLength = LogisticsOrderFeeType.SecondLogisticsFeeExtraLength;
            }

            resultEntryList.Add(new LogisticsFeeResultEntry() {
                Title = "超长费用",
                Fee = overLengthFee,
                GoodsType = logisticsFeeData.GoodsType,
                FeeType = feeTypeExtraLength,
                EntryType = LogisticsFeeResultEntryType.Single
            });

            feeCalculateLogService.AddLog($"超长费用 {overLengthFee}");
        }

        // 单箱超重费用
        decimal singleOverWeightFee = feeRateList.Max(feeRate => GetSingleOverWeightFee(feePackList, feeRate, feeMessageType));

        // if (singleOverWeightFee > 0)
        {
            extraTotalFee += singleOverWeightFee;

            LogisticsOrderFeeType feeTypeSingleExtraWeight = null;

            if (isFirstLogisticsModeType) {
                feeTypeSingleExtraWeight = LogisticsOrderFeeType.FirstLogisticsFeeSingleExtraWeight;
            }

            if (isSecondLogisticsModeType) {
                feeTypeSingleExtraWeight = LogisticsOrderFeeType.SecondLogisticsFeeSingleExtraWeight;
            }

            resultEntryList.Add(new LogisticsFeeResultEntry() {
                Title = "单箱超重费用",
                Fee = singleOverWeightFee,
                GoodsType = logisticsFeeData.GoodsType,
                FeeType = feeTypeSingleExtraWeight,
                EntryType = LogisticsFeeResultEntryType.Single
            });

            feeCalculateLogService.AddLog($"单箱超重费用 {singleOverWeightFee}");
        }

        // 平均单箱超重费用
        decimal averageOverWeightFee = feeRateList.Max(feeRate => GetAverageOverWeightFee(feePackList, feeRate, feeMessageType));

        // if (averageOverWeightFee > 0)
        {
            extraTotalFee += averageOverWeightFee;

            LogisticsOrderFeeType feeTypeExtraAverageWeight = null;

            if (isFirstLogisticsModeType) {
                feeTypeExtraAverageWeight = LogisticsOrderFeeType.FirstLogisticsFeeAverageExtraWeight;
            }

            if (isSecondLogisticsModeType) {
                feeTypeExtraAverageWeight = LogisticsOrderFeeType.SecondLogisticsFeeAverageExtraWeight;
            }

            resultEntryList.Add(new LogisticsFeeResultEntry() {
                Title = "平均单箱超重费用",
                Fee = averageOverWeightFee,
                GoodsType = logisticsFeeData.GoodsType,
                FeeType = feeTypeExtraAverageWeight,
                EntryType = LogisticsFeeResultEntryType.Single
            });

            feeCalculateLogService.AddLog($"平均单箱超重费用 {averageOverWeightFee}");
        }

        // 超重费用
        decimal overWeightFee = feeRateList.Max(feeRate => GetOverWeightFee(feePackList, feeRate, feeMessageType));

        // if (overWeightFee > 0)
        {
            extraTotalFee += overWeightFee;

            LogisticsOrderFeeType feeTypeExtraWeight = null;

            if (isFirstLogisticsModeType) {
                feeTypeExtraWeight = LogisticsOrderFeeType.FirstLogisticsFeeExtraWeight;
            }

            if (isSecondLogisticsModeType) {
                feeTypeExtraWeight = LogisticsOrderFeeType.SecondLogisticsFeeExtraWeight;
            }

            resultEntryList.Add(new LogisticsFeeResultEntry() {
                Title = "超重费用",
                Fee = overWeightFee,
                GoodsType = logisticsFeeData.GoodsType,
                FeeType = feeTypeExtraWeight,
                EntryType = LogisticsFeeResultEntryType.Single
            });

            feeCalculateLogService.AddLog($"超重费用 {overWeightFee}");
        }

        // 额外包裹数量，调用 GetExtraPackageCountFee()，这里的额外包裹数量是快递单号的数量

        if (isFirstLogisticsModeType && logisticsFeeData.PackageCount > 0) {
            decimal extraPackageCountFee = feeRateList.Max(feeRate => GetExtraPackageCountFee(logisticsFeeData.PackageCount, feeRate, feeMessageType));

            extraTotalFee += extraPackageCountFee;

            resultEntryList.Add(new LogisticsFeeResultEntry() {
                Title = "额外包裹费",
                Fee = extraPackageCountFee,
                GoodsType = logisticsFeeData.GoodsType,
                FeeType = LogisticsOrderFeeType.FirstLogisticsFeeExtraPackage,
                EntryType = LogisticsFeeResultEntryType.Single
            });

            feeCalculateLogService.AddLog($"额外包裹费 {extraPackageCountFee}");
        }

        // todo 仓储, 调用 GetExtraStorageDayFee() ，这个是不是可以用订单入库的时间 和 提交打包的时间差 来做这个计算

        #endregion

        #region 限制类、草木竹类、特货类

        // todo 普通类默认所有物流方式都允许，直接忽略通用费率中的相关设置
        // 普通类一般都允许, 这里基本也不会去用到和去判断那个普通类是不是允许的设置

        decimal typeTotalFee = 0m;

        if (isFirstLogisticsModeType) // 头程才需要计算这个 限制类和草木竹类
        {
            // 限制类
            decimal restrictGoodsFee = feeRateList.Max(feeRate => GetRestrictTypeFee(feePackList, logisticsFeeData.TotalPrice, logisticsFeeData.GoodsType, feeRate, feeMessageType));

            //if (restrictGoodsFee > 0)
            {
                typeTotalFee += restrictGoodsFee;

                resultEntryList.Add(new LogisticsFeeResultEntry() {
                    Title = "限制类物品加收",
                    Fee = restrictGoodsFee,
                    GoodsType = logisticsFeeData.GoodsType,
                    FeeType = LogisticsOrderFeeType.FirstLogisticsFeeRestrictType,
                    EntryType = LogisticsFeeResultEntryType.Single
                });

                feeCalculateLogService.AddLog($"限制类物品加收 {restrictGoodsFee}");
            }

            // 草木竹类
            decimal bambooGoodsFee = feeRateList.Max(feeRate => GetBambooTypeFee(feePackList, logisticsFeeData.TotalPrice, logisticsFeeData.GoodsType, feeRate, feeMessageType));

            // if (bambooGoodsFee > 0)
            {
                typeTotalFee += bambooGoodsFee;

                resultEntryList.Add(new LogisticsFeeResultEntry() {
                    Title = "草木竹类物品加收",
                    Fee = bambooGoodsFee,
                    GoodsType = logisticsFeeData.GoodsType,
                    FeeType = LogisticsOrderFeeType.FirstLogisticsFeeBambooType,
                    EntryType = LogisticsFeeResultEntryType.Single
                });

                feeCalculateLogService.AddLog($"草木竹类物品加收 {bambooGoodsFee}");
            }

            // 特货类
            decimal specialGoodsFee = feeRateList.Max(feeRate => GetSpecialTypeFee(feePackList, logisticsFeeData.TotalPrice, logisticsFeeData.GoodsType, feeRate, feeMessageType));

            // if (specialGoodsFee > 0)
            {
                typeTotalFee += specialGoodsFee;

                resultEntryList.Add(new LogisticsFeeResultEntry() {
                    Title = "特货类物品加收",
                    Fee = specialGoodsFee,
                    GoodsType = logisticsFeeData.GoodsType,
                    FeeType = LogisticsOrderFeeType.FirstLogisticsFeeSpecialType,
                    EntryType = LogisticsFeeResultEntryType.Single
                });

                feeCalculateLogService.AddLog($"特货类物品加收 {specialGoodsFee}");
            }
        }

        #endregion

        // 公布系数

        decimal publishRate = feeRateList.Max(feeRate => feeRate.PublishRate.HasValue ? feeRate.PublishRate.Value : 1.0m);

        feeCalculateLogService.AddLog($"公布系数 {publishRate}");

        decimal totalFee = (initialTotalFee + extraTotalFee + typeTotalFee) * publishRate;

        #region 记录合计费用

        string totalFeeTitle = "";

        if (isFirstLogisticsModeType) {
            totalFeeTitle = "跨国段运费合计";
        }

        if (isSecondLogisticsModeType) {
            totalFeeTitle = "澳洲境内运费合计";
        }

        resultEntryList.Add(new LogisticsFeeResultEntry() {
            Title = totalFeeTitle,
            Fee = totalFee,
            GoodsType = logisticsFeeData.GoodsType,
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
                if (feeRate.VolumeDenominator.HasValue == false || feeRate.VolumeDenominator.Value <= 0) {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物流方式不支持！AP0250");
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
        if (feePack.Quantity <= 0) {
            feePack.Quantity = 1;
        }

        return GetSinglePackActualWeight(feePack, feeRate) * feePack.Quantity;
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
                if (feeRate.VolumeDenominator.HasValue == false || feeRate.VolumeDenominator.Value <= 0) {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物流方式不支持！AP0260");
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
        return wholePackTotalWeight / packList.Sum(e => e.Quantity);
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
        if (feePack.Quantity <= 0) {
            feePack.Quantity = 1;
        }

        return finalVolumeCm3 * feePack.Quantity;
    }

    private decimal GetSinglePackVolumeWeight(LogisticsFeePack feePack, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (feeRate.CareVolumeWeight == false) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！AP0270");
            } else {
                throw new LogisticsFeeException($"通用费率{feeRate.Name}，不考虑体积重，不能计算体积重！");
            }
        }

        if (feeRate.VolumeDenominator.HasValue == false || feeRate.VolumeDenominator.Value <= 0) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！AP0280");
            } else {
                throw new LogisticsFeeException($"通用费率 {feeRate.Name} 考虑体积重，但体积重分母 未配置 或 为0 ！");
            }
        }

        return GetSinglePackVolume(feePack, feeRate) / feeRate.VolumeDenominator.Value;
    }

    private decimal GetMultiPackVolumeWeight(LogisticsFeePack feePack, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        // 如果没有 "数量" 数据，就假设为 1
        if (feePack.Quantity <= 0) {
            feePack.Quantity = 1;
        }

        return GetSinglePackVolumeWeight(feePack, feeRate, feeMessageType) * feePack.Quantity;
    }

    private decimal GetSinglePackSideAvgVolumeWeight(LogisticsFeePack feePack, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (feeRate.CareVolumeWeight) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！AP0290");
            } else {
                throw new LogisticsFeeException($"通用费率{feeRate.Name}，不考虑体积重，不能计算三边之和体积重！");
            }
        }

        if (feeRate.VolumeDenominator.HasValue == false || feeRate.VolumeDenominator.Value <= 0) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！AP0300");
            } else {
                throw new LogisticsFeeException($"通用费率 {feeRate.Name} 考虑体积重，但体积重分母 未配置 或 为0 ！");
            }
        }

        if (feeRate.CareSideSum) {
            if (feeMessageType == FeeMessageType.Customer) {
                throw new LogisticsFeeException("物流方式不支持！AP0310");
            } else {
                throw new LogisticsFeeException($"通用费率{feeRate.Name}，不考虑三边之和，不能计算三边之和体积重！");
            }
        }

        // 计算客户长度打折
        decimal finalLength = feePack.Length * feeRate.CustomerLengthDiscount;
        decimal finalWidth = feePack.Width * feeRate.CustomerWidthDiscount;
        decimal finalHeight = feePack.Height * feeRate.CustomerHeightDiscount;

        decimal averageLength = (finalLength + finalWidth + finalHeight) / 3.0m;
        return averageLength * averageLength * averageLength / feeRate.VolumeDenominator.Value;
    }

    private decimal GetMultiPackSideAvgVolumeWeight(LogisticsFeePack feePack, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        // 如果没有 "数量" 数据，就假设为 1
        if (feePack.Quantity <= 0) {
            feePack.Quantity = 1;
        }

        return GetSinglePackSideAvgVolumeWeight(feePack, feeRate, feeMessageType) * feePack.Quantity;
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
        if (feeRate.HasLengthStart) {
            if (feeRate.LengthStart <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("物品起步长度物流方式不支持！AP0810");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 有长度起步，但起步长度 未配置 或 为0 ！");
                }
            }

            decimal maxLength = GetAllPackMaxLength(packList, feeRate); // 最大长度，就是这个订单的长度  
            decimal lengthStart = feeRate.LengthStart;
            if (maxLength < lengthStart) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("物品长度未到物流方式起步长度！AP0820");
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
        if (feeRate.HasSingleWeightStart) {
            if (feeRate.SingleWeightStart <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("物品单箱起步重量物流方式不支持！AP0830");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 有单箱重量起步，但单箱起步重量 未配置 或 为0 ！");
                }
            }

            decimal singleWeightStart = feeRate.SingleWeightStart;
            foreach (var orderPack in packList) {
                decimal singleFinalWeight = GetSinglePackWeight(orderPack, feeRate, feeMessageType);
                if (singleFinalWeight < singleWeightStart) {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物品单箱重量未到物流方式单箱起步重量！AP0840");
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
        if (feeRate.HasAverageWeightStart) {
            if (feeRate.AverageWeightStart <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("物品平均单箱起步重量物流方式不支持！AP0831");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 有平均单箱重量起步，但平均单箱起步重量 未配置 或 为0 ！");
                }
            }

            decimal averageWeightStart = feeRate.AverageWeightStart;
            decimal averageFinalWeight = GetAveragePackWeight(packList, feeRate, feeMessageType);
            if (averageFinalWeight < averageWeightStart) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("物品平均单箱重量未到物流方式平均单箱起步重量！AP0841");
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
        if (feeRate.HasWeightStart) {
            if (feeRate.WeightStart <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("物品起步重量物流方式不支持！AP0850");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 有重量起步，但起步重量 未配置 或 为0 ！");
                }
            }

            decimal totalWeight = GetWholePackTotalWeight(packList, feeRate, feeMessageType);
            decimal weightStart = feeRate.WeightStart;
            if (totalWeight < weightStart) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("物品重量未到物流方式起步重量！AP0860");
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
                    throw new LogisticsFeeException("物品超长物流方式不支持！AP0320");
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
                            throw new LogisticsFeeException("物品超长物流方式不支持！AP0330");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 可以超长，但超长阶梯 未配置 或 为0 ！");
                        }
                    }

                    if (feeRate.OverLengthStepFee.HasValue == false || feeRate.OverLengthStepFee.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物品超长物流方式不支持！AP0340");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 可以超长，但超长阶梯收费 未配置 或 为0 ！");
                        }
                    }

                    decimal overLengthStep = feeRate.OverLengthStep.Value;
                    if (overLengthStep <= 0m) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物品超长物流方式不支持！AP0350");
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
                            overLengthFee = overLengthFee + (overStepCount * overLengthStepFee) * feePack.Quantity; // Quantity 也不能忘了 
                        }
                    }

                    // 是否有封顶金额
                    if (feeRate.HasOverLengthCapAmount) {
                        if (feeRate.OverLengthCapAmount.HasValue == false || feeRate.OverLengthCapAmount.Value <= 0) {
                            if (feeMessageType == FeeMessageType.Customer) {
                                throw new LogisticsFeeException("物品超长物流方式不支持！AP0360");
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
                        throw new LogisticsFeeException("物品长度超出物流方式长度限制！AP0370");
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
                    throw new LogisticsFeeException("重量超出物流方式单箱重量限制！AP0380");
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
                                throw new LogisticsFeeException("物品重量超出物流方式单箱重量限制！AP0390");
                            } else {
                                throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率设置允许单箱超重，但单箱超重阶梯 未设置 或 为0 ！");
                            }
                        }

                        if (feeRate.SingleOverWeightStepFee.HasValue == false || feeRate.SingleOverWeightStepFee.Value <= 0) {
                            if (feeMessageType == FeeMessageType.Customer) {
                                throw new LogisticsFeeException("物品重量超出物流方式单箱重量限制！AP0400");
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

                        decimal singleOverWeightFee = singleOverStepCount * singleOverWeightStepFee * orderPack.Quantity;

                        totalSingleOverWeightFee += singleOverWeightFee;
                    } else {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物品重量超出物流方式单箱重量限制！AP0410");
                        } else {
                            throw new LogisticsFeeException($"单箱重量(或体积重)超出重量限制，但通用费率 {feeRate.Name} 费率设置不允许单箱超重！");
                        }
                    }
                }
            }

            if (feeRate.HasSingleOverWeightCapAmount) {
                if (feeRate.SingleOverWeightCapAmount.HasValue == false || feeRate.SingleOverWeightCapAmount.Value <= 0) {
                    if (feeMessageType == FeeMessageType.Customer) {
                        throw new LogisticsFeeException("物品重量超出物流方式单箱重量限制！AP0420");
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
                            throw new LogisticsFeeException("物品平均单箱重量超出物流方式限制！AP0431");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率设置允许平均单箱超重，但超重阶梯 未设置 或 为0 ！");
                        }
                    }

                    if (feeRate.AverageOverWeightStepFee.HasValue == false || feeRate.AverageOverWeightStepFee.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物品总重量超出物流方式限制！AP0441");
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
                                throw new LogisticsFeeException("物品平均单箱重量超出物流方式限制！AP0451");
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
                        throw new LogisticsFeeException("物品平均单箱重量超出物流方式限制！AP0461");
                    } else {
                        throw new LogisticsFeeException($"平均单箱重量(或体积重)超出重量限制，但通用费率 {feeRate.Name} 费率设置不允许超重！");
                    }
                }
            }
        }

        // 如果没有重量限制就没有超重费用
        return 0m;
    }

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
                            throw new LogisticsFeeException("物品总重量超出物流方式限制！AP0430");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name} 费率设置允许超重，但超重阶梯 未设置 或 为0 ！");
                        }
                    }

                    if (feeRate.OverWeightStepFee.HasValue == false || feeRate.OverWeightStepFee.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("物品总重量超出物流方式限制！AP0440");
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
                                throw new LogisticsFeeException("物品总重量超出物流方式限制！AP0450");
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
                        throw new LogisticsFeeException("物品总重量超出物流方式限制！AP0460");
                    } else {
                        throw new LogisticsFeeException($"总重量(或体积重)超出重量限制，但通用费率 {feeRate.Name} 费率设置不允许超重！");
                    }
                }
            }
        }

        // 如果没有重量限制就没有超重费用
        return 0m;
    }

    /// <summary>
    /// 计算额外包裹数量费用
    /// 现在应该是按入库快递单号数量计算这个费用的
    /// </summary>
    /// <param name="packageCount"></param>
    /// <param name="feeRate"></param>
    /// <returns></returns>
    private decimal GetExtraPackageCountFee(int packageCount, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (feeRate.HasFreePackageLimit) {
            if (feeRate.FreePackageLimit.HasValue == false || feeRate.FreePackageLimit.Value < 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("包裹数量超出物流方式限制！AP0470");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 包裹有免费数量限制，但免费包裹数量 未配置 或 小于0 ！");
                }
            }

            if (feeRate.ExtraPackageFee.HasValue == false || feeRate.ExtraPackageFee.Value <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("包裹数量超出物流方式限制！AP0480");
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
                            throw new LogisticsFeeException("包裹数量超出物流方式限制！AP0490");
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

    /// <summary>
    /// 计算额外仓储天数费用
    /// </summary>
    /// <param name="storageDaysCount"></param>
    /// <param name="feeRate"></param>
    /// <returns></returns>
    private decimal GetExtraStorageDayFee(int storageDaysCount, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (feeRate.HasFreeStorageDayLimit) {
            if (feeRate.FreeStorageDayLimit.HasValue == false || feeRate.FreeStorageDayLimit.Value <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException($"超出物流方式仓储天数限制 ！AP0500");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 仓储有免费天数限制，但免费仓储天数 未配置 或 为0 ！");
                }
            }

            if (feeRate.ExtraStorageDayFee.HasValue == false || feeRate.ExtraStorageDayFee.Value <= 0) {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException($"超出物流方式仓储天数限制 ！AP0510");
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
                            throw new LogisticsFeeException($"超出物流方式仓储天数限制 ！AP0520");
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

    #region 限制类、草木竹类、特货类 费用

    /// <summary>
    /// 计算限制类加收费用
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="totalPrice"></param>
    /// <param name="goodsType"></param>
    /// <param name="feeRate"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private decimal GetRestrictTypeFee(IList<LogisticsFeePack> packList, decimal totalPrice, LogisticsGoodsType goodsType, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (goodsType == LogisticsGoodsType.Restrict || goodsType == LogisticsGoodsType.RestrictBamboo) {
            decimal restrictGoodsFee = 0m;

            if (feeRate.RestrictGoodsPermit) {
                // 按件加收费用
                if (feeRate.RestrictGoodsByCount) {
                    if (feeRate.RestrictGoodsByCountFee.HasValue == false || feeRate.RestrictGoodsByCountFee.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("限制类物品物流方式不支持！AP0530");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name}，限制类已启用按件加收，但 按件加收费用 未设置 或为0！");
                        }
                    }

                    int goodsCount = packList.Count;
                    restrictGoodsFee = goodsCount * feeRate.RestrictGoodsByCountFee.Value;
                }

                // 按体积加收费用
                if (feeRate.RestrictGoodsByVolume) {
                    if (feeRate.RestrictGoodsByVolumeFee.HasValue == false || feeRate.RestrictGoodsByVolumeFee.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("限制类物品物流方式不支持！AP0540");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name}，限制类已启用按体积加收，但 按体积加收费用 未设置 或为0！");
                        }
                    }

                    decimal totalVolumeCm3 = GetAllPackTotalVolume(packList, feeRate);
                    restrictGoodsFee = (totalVolumeCm3 / 1_00_00_00m) * feeRate.RestrictGoodsByVolumeFee.Value;
                }

                // 按申报金额加收费用
                if (feeRate.RestrictGoodsByAmount) {
                    if (feeRate.RestrictGoodsByAmountFeePercent.HasValue == false || feeRate.RestrictGoodsByAmountFeePercent.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("限制类物品物流方式不支持！AP0550");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name}，限制类已启用按金额加收，但 按金额加收比率 未设置 或为0！");
                        }
                    }

                    restrictGoodsFee = totalPrice * feeRate.RestrictGoodsByAmountFeePercent.Value;
                }

                if (feeRate.HasRestrictGoodsCapAmount) {
                    if (feeRate.RestrictGoodsCapAmount.HasValue == false || feeRate.RestrictGoodsCapAmount.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("限制类物品物流方式不支持！AP0560");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name}，限制类有费用封顶，但 封顶金额 未设置 或为0！");
                        }
                    }

                    if (restrictGoodsFee > feeRate.RestrictGoodsCapAmount.Value) {
                        restrictGoodsFee = feeRate.RestrictGoodsCapAmount.Value;
                    }
                }

                return restrictGoodsFee;
            } else {
                if (goodsType == LogisticsGoodsType.RestrictBamboo) {
                    return 0m;
                }

                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("限制类物品物流方式不支持！AP0570");
                } else {
                    throw new LogisticsFeeException($"通用费率{feeRate.Name} 当前费率不允许限制类的物品！");
                }
            }
        }

        // 如果不是限制类型的，限制类的加收费用就为0
        return 0m;
    }

    /// <summary>
    /// 计算草木竹类加收费用
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="totalPrice"></param>
    /// <param name="goodsType"></param>
    /// <param name="feeRate"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private decimal GetBambooTypeFee(IList<LogisticsFeePack> packList, decimal totalPrice, LogisticsGoodsType goodsType, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (goodsType == LogisticsGoodsType.Bamboo || goodsType == LogisticsGoodsType.RestrictBamboo) {
            if (feeRate.BambooGoodsPermit) {
                decimal bambooGoodsFee = 0m;

                // 按件加收费用
                if (feeRate.BambooGoodsByCount) {
                    if (feeRate.BambooGoodsByCountFee.HasValue == false || feeRate.BambooGoodsByCountFee.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("草木竹类物品物流方式不支持！AP0580");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name}，草木竹类已启用按件加收，但 按件加收费用 未设置 或为0！");
                        }
                    }

                    int goodsCount = packList.Count;
                    bambooGoodsFee = goodsCount * feeRate.BambooGoodsByCountFee.Value;
                }

                // 按体积加收费用
                if (feeRate.BambooGoodsByVolume) {
                    if (feeRate.BambooGoodsByVolumeFee.HasValue == false || feeRate.BambooGoodsByVolumeFee.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("草木竹类物品物流方式不支持！AP0590");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name}，草木竹类已启用按体积加收，但 按体积加收费用 未设置 或为0！");
                        }
                    }

                    decimal totalVolumeCm3 = GetAllPackTotalVolume(packList, feeRate);
                    bambooGoodsFee = (totalVolumeCm3 / 1_00_00_00) * feeRate.BambooGoodsByVolumeFee.Value;
                }

                // 按申报金额加收费用
                if (feeRate.BambooGoodsByAmount) {
                    if (feeRate.BambooGoodsByAmountFeePercent.HasValue == false || feeRate.BambooGoodsByAmountFeePercent.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("草木竹类物品物流方式不支持！AP0600");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name}，草木竹类已启用按金额加收，但 按金额加收比率 未设置 或为0！");
                        }
                    }

                    bambooGoodsFee = totalPrice * feeRate.BambooGoodsByAmountFeePercent.Value;
                }

                if (feeRate.HasBambooGoodsCapAmount) {
                    if (feeRate.BambooGoodsCapAmount.HasValue == false || feeRate.BambooGoodsCapAmount.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("草木竹类物品物流方式不支持！AP0610");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name}，草木竹类有费用封顶，但 封顶金额 未设置 或为0！");
                        }
                    }

                    if (bambooGoodsFee > feeRate.BambooGoodsCapAmount.Value) {
                        bambooGoodsFee = feeRate.BambooGoodsCapAmount.Value;
                    }
                }

                return bambooGoodsFee;
            } else {
                if (goodsType == LogisticsGoodsType.RestrictBamboo) {
                    return 0m;
                }

                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("草木竹类物品物流方式不支持！AP0620");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 当前费率不允许草木竹类的物品！");
                }
            }
        }

        // 如果不是草木竹类型的，草木竹类的加收费用就为0
        return 0m;
    }

    /// <summary>
    /// 计算特货类加收费用
    /// </summary>
    /// <param name="packList"></param>
    /// <param name="totalPrice"></param>
    /// <param name="goodsType"></param>
    /// <param name="feeRate"></param>
    /// <param name="feeMessageType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private decimal GetSpecialTypeFee(IList<LogisticsFeePack> packList, decimal totalPrice, LogisticsGoodsType goodsType, LogisticsFeeRateInfo feeRate, FeeMessageType feeMessageType) {
        if (goodsType == LogisticsGoodsType.Special) {
            if (feeRate.SpecialGoodsPermit) {
                decimal specialGoodsFee = 0m;

                // 按件加收费用
                if (feeRate.SpecialGoodsByCount) {
                    if (feeRate.SpecialGoodsByCountFee.HasValue == false || feeRate.SpecialGoodsByCountFee.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("特货类物品物流方式不支持！AP0630");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name}，特货类已启用按件加收，但 按件加收费用 未设置 或为0！");
                        }
                    }

                    int goodsCount = packList.Count;
                    specialGoodsFee = goodsCount * feeRate.SpecialGoodsByCountFee.Value;
                }

                // 按体积加收费用
                if (feeRate.SpecialGoodsByVolume) {
                    if (feeRate.SpecialGoodsByVolumeFee.HasValue == false || feeRate.SpecialGoodsByVolumeFee.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("特货类物品物流方式不支持！AP0640");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name}，特货类已启用按体积加收，但 按体积加收费用 未设置 或为0！");
                        }
                    }

                    decimal totalVolumeCm3 = GetAllPackTotalVolume(packList, feeRate);
                    specialGoodsFee = (totalVolumeCm3 / 1_00_00_00) * feeRate.SpecialGoodsByVolumeFee.Value;
                }

                // 按申报金额加收费用
                if (feeRate.SpecialGoodsByAmount) {
                    if (feeRate.SpecialGoodsByAmountFeePercent.HasValue == false || feeRate.SpecialGoodsByAmountFeePercent.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("特货类物品物流方式不支持！AP0650");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name}，特货类已启用按金额加收，但 按金额加收比率 未设置 或为0！");
                        }
                    }

                    specialGoodsFee = totalPrice * feeRate.SpecialGoodsByAmountFeePercent.Value;
                }

                if (feeRate.HasSpecialGoodsCapAmount) {
                    if (feeRate.SpecialGoodsCapAmount.HasValue == false || feeRate.SpecialGoodsCapAmount.Value <= 0) {
                        if (feeMessageType == FeeMessageType.Customer) {
                            throw new LogisticsFeeException("特货类物品物流方式不支持！AP0660");
                        } else {
                            throw new LogisticsFeeException($"通用费率 {feeRate.Name}，特货类有费用封顶，但 封顶金额 未设置 或为0！");
                        }
                    }

                    if (specialGoodsFee > feeRate.SpecialGoodsCapAmount.Value) {
                        specialGoodsFee = feeRate.SpecialGoodsCapAmount.Value;
                    }
                }

                return specialGoodsFee;
            } else {
                if (feeMessageType == FeeMessageType.Customer) {
                    throw new LogisticsFeeException("特货类物品物流方式不支持！AP0670");
                } else {
                    throw new LogisticsFeeException($"通用费率 {feeRate.Name} 当前费率不允许特货类的物品！");
                }
            }
        }

        // 如果不是特货类型的，特货类的加收费用就为0
        return 0m;
    }

    #endregion

    #region 计算保险和GST费用

    /// <summary>
    /// 计算保险费用， 一般只用于订单费用的计算
    /// </summary>
    /// <param name="amount">物品总价</param>
    /// <param name="fee">运费</param>
    /// <param name="logisticsInsuranceId">GST费用的设置记录的 Id</param>
    /// <returns></returns>
    public decimal GetInsuranceFee(decimal amount, decimal fee, int logisticsInsuranceId) {
        return logisticsFeeInsuranceService.GetInsuranceFee(amount, fee, logisticsInsuranceId);
    }

    /// <summary>
    /// 计算GST费用，只用于费用估算
    /// </summary>
    /// <param name="amount">物品总价</param>
    /// <param name="fee">运费</param>
    /// <param name="totalVolume"></param>
    /// <param name="totalWeight"></param>
    /// <param name="logisticsGstId">GST费用的设置记录的 Id</param>
    /// <param name="firstLogisticsModeId"></param>
    /// <param name="orderGoodsType"></param>
    /// <returns></returns>
    public decimal GetGstFeeWhenEstimate(decimal amount, decimal fee, decimal totalVolume, decimal totalWeight, int logisticsGstId, int firstLogisticsModeId, LogisticsGoodsType orderGoodsType) {
        return logisticsFeeGstService.GetGstFeeWhenEstimate(amount, fee, totalVolume, totalWeight, logisticsGstId, firstLogisticsModeId, orderGoodsType);
    }

    /// <summary>
    /// 计算GST费用，用于订单费用计算
    /// </summary>
    /// <param name="fee">运费，不包含保险费和GST费用的运费</param>
    /// <param name="logisticsOrder"></param>
    /// <returns></returns>
    public decimal GetGstFeeAfterPacked(decimal fee, LogisticsOrderInfo logisticsOrder) {
        return logisticsFeeGstService.GetGstFeeAfterPacked(fee, logisticsOrder);
    }

    public decimal GetGstFeeByWeightVolume(decimal fee, decimal goodsPrice, LogisticsGoodsType goodsType, decimal totalActualWeight, decimal totalVolume, LogisticsModeInfo firstMode, LogisticsGstInfo logisticsGst) {
        return logisticsFeeGstService.GetGstFeeByWeightVolume(fee, goodsPrice, goodsType, totalActualWeight, totalVolume, firstMode, logisticsGst);
    }

    #endregion

    #region 计算超大超重附加费用

    public decimal GetExtraSizeWeightFee(IList<int> claimIdList) {
        if (claimIdList == null || claimIdList.Count == 0) {
            return 0;
        }

        var holdExpressList = session.Query<SizeWeightHoldExpressInfo>()
            .Where(e => claimIdList.Contains(e.Claim.Id)
                        && e.Status == SizeWeightHoldExpressStatus.Release)
            .ToList();

        if (holdExpressList.Count == 0) {
            return 0;
        }

        return holdExpressList.Sum(e => e.Fee);
    }

    public decimal GetExtraSizeWeightFee(IList<string> trackingNoList) {
        if (trackingNoList == null || trackingNoList.Count == 0) {
            return 0;
        }

        var holdExpressList = session.Query<SizeWeightHoldExpressInfo>()
            .Where(e => trackingNoList.Contains(e.Claim.TrackingNo)
                        && e.Status == SizeWeightHoldExpressStatus.Release)
            .ToList();

        if (holdExpressList.Count == 0) {
            return 0;
        }

        return holdExpressList.Sum(e => e.Fee);
    }

    public decimal GetExtraSizeWeightFee(int orderId) {
        IList<string> trackingNoList = session.Query<LogisticsOrderGoodsExpressInfo>()
            .Where(e => e.Order.Id == orderId)
            .Select(e => e.ExpressTrackingNo)
            .ToList();

        return GetExtraSizeWeightFee(trackingNoList);
    }

    #endregion
}