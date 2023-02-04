using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.LogisticsOrder;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderPackBoxService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public LogisticsOrderPackBoxService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 获取下一个打包箱子的编号
    /// </summary>
    public int GetNextBoxNum(int orderId) {
        IList<int> boxNumList = session.Query<LogisticsOrderPackBoxInfo>()
            .Where(e => e.Order.Id == orderId)
            .OrderBy(e => e.BoxNum)
            .Select(e => e.BoxNum)
            .ToList();

        if (boxNumList.Count == 0) {
            return 1;
        }

        int maxBoxNum = boxNumList.Max(e => e);
        for (int num = 1; num <= maxBoxNum; num++) {
            if (boxNumList.Contains(num) == false) {
                return num;
            }
        }

        return maxBoxNum + 1;
    }

    /// <summary>
    /// 获取订单打包箱子唯一编号
    /// 生成箱号
    /// 箱号格式 参考这里，之后查找箱号格式，就可以找到这里
    /// </summary>
    public string GetBoxCode(int packBoxId) {
        var orderPackBox = session.Get<LogisticsOrderPackBoxInfo>(packBoxId);
        if (orderPackBox == null) {
            throw new ValidationException($"指定的箱号#{packBoxId}不存在！");
        }

        return $"{orderPackBox.Order.Code}{orderPackBox.BoxNum:#0000}"; // 最后是 4 位数字
    }

    /// <summary>
    /// 获取箱号
    /// </summary>
    public string GetBoxCode(string orderCode, int packBoxNum) {
        if (string.IsNullOrWhiteSpace(orderCode)) {
            return string.Empty;
        }

        if (packBoxNum <= 0) {
            return string.Empty;
        }

        return $"{orderCode}{packBoxNum:#0000}"; // 最后是 4 位数字
    }

    /// <summary>
    /// 短箱号
    /// </summary>
    public string GetBoxShortCode(int packBoxId) {
        var orderPackBox = session.Get<LogisticsOrderPackBoxInfo>(packBoxId);
        if (orderPackBox == null) {
            throw new ValidationException($"指定的箱号#{packBoxId}不存在！");
        }

        return $"{orderPackBox.OrderPack.Id}#{orderPackBox.Order.Id}@{orderPackBox.BoxNum}";
    }

    /// <summary>
    /// 获取非常短箱号
    /// </summary>
    public string GetBoxExtraShortCode(int packBoxId) {
        var orderPackBox = session.Get<LogisticsOrderPackBoxInfo>(packBoxId);
        if (orderPackBox == null) {
            throw new ValidationException($"指定的箱号#{packBoxId}不存在！");
        }

        return $"{orderPackBox.Order.Id}@{orderPackBox.BoxNum}";
    }

    /// <summary>
    /// 获取 基于订单ID和箱序号的，订单箱号别名
    /// </summary>
    public string GetBoxCodeOrderIdAlias(int orderId, int orderPackBoxNum) {
        // orderId 占 9位，不足左边补0，orderPackBoxNum 占 3位，不足左边补0，总的编号是 12 位
        return orderId.ToString().PadLeft(9, '0') + orderPackBoxNum.ToString().PadLeft(3, '0');
    }

    /// <summary>
    /// 解析订单打包箱子唯一编号
    /// </summary>
    public LogisticsOrderPackBoxInfo? ParseBoxCode(string boxCode, bool throwException = true) {
        if (string.IsNullOrWhiteSpace(boxCode)) {
            if (throwException) {
                throw new ValidationException("箱号不能为空！");
            }

            return null;
        }

        boxCode = boxCode.Trim();
        boxCode = boxCode.Trim((char)65279); // 移除 UTF BOM，ZERO WIDTH NO-BREAK SPACE

        int indexOfHashTag = boxCode.IndexOf("#", StringComparison.InvariantCulture);
        int indexOfAtTag = boxCode.IndexOf("@", StringComparison.InvariantCulture);

        if (indexOfAtTag >= 0) // 有 @ 号的，就是原来的箱号格式
        {
            if (indexOfAtTag == 0) // @ 号，不可能在首位
            {
                if (throwException) {
                    throw new ValidationException("箱号格式不准确！");
                }

                return null;
            }

            if (indexOfHashTag < 0 || indexOfHashTag == 0) // 没有 # 号，或者 # 号在首位
            {
                // 超短箱号 逻辑

                boxCode = boxCode.TrimStart('#'); // 去掉 可能的起始的 # 号， 这样就可以都按照没有 # 起始的来做 

                string[] split = boxCode.Split("@");
                if (split.Length != 2) {
                    if (throwException) {
                        throw new ValidationException("箱号格式不准确！");
                    }

                    return null;
                }

                if (int.TryParse(split[0]?.Trim(), out int orderId) == false) {
                    if (throwException) {
                        throw new ValidationException("箱号格式不准确！");
                    }

                    return null;
                }

                if (int.TryParse(split[1]?.Trim(), out int boxNum) == false) {
                    if (throwException) {
                        throw new ValidationException("箱号格式不准确！");
                    }

                    return null;
                }

                var orderPackBox = session.Query<LogisticsOrderPackBoxInfo>()
                    .Where(e => e.OrderPack.Order.Id == orderId && e.BoxNum == boxNum)
                    .Take(1)
                    .SingleOrDefault();

                if (orderPackBox == null) {
                    if (throwException) {
                        throw new ValidationException("箱号不存在！");
                    }

                    return null;
                }

                return orderPackBox;
            } else {
                // 这里的 shunda系统基本已经不用
                // 原来的 长箱号，短箱号 逻辑

                string[] split1 = boxCode.Split("#");
                if (split1.Length != 2) {
                    if (throwException) {
                        throw new ValidationException("箱号格式不准确！");
                    }

                    return null;
                }

                if (int.TryParse(split1[0]?.Trim(), out int orderPackId) == false) {
                    if (throwException) {
                        throw new ValidationException("箱号格式不准确！");
                    }

                    return null;
                }

                string[] split2 = split1[1].Split("@");
                if (split2.Length != 2) {
                    if (throwException) {
                        throw new ValidationException("箱号格式不准确！");
                    }

                    return null;
                }

                if (int.TryParse(split2[1]?.Trim(), out int boxNum) == false) {
                    if (throwException) {
                        throw new ValidationException("箱号格式不准确！");
                    }

                    return null;
                }


                // 检查 orderPack Id 和 订单编号配对
                LogisticsOrderPackInfo orderPack = session.Get<LogisticsOrderPackInfo>(orderPackId);
                if (orderPack == null) {
                    if (throwException) {
                        throw new ValidationException("箱号不存在！");
                    }

                    return null;
                }

                string orderInfoInBoxCode = split2[0];

                if (orderPack.Order.Code != orderInfoInBoxCode
                    && orderPack.Order.Id.ToString() != orderInfoInBoxCode // 兼容新的短箱号，一般用在条形码上，只取订单的ID
                    && orderPack.Order.Code?.Substring(0, 8) != orderInfoInBoxCode) // 兼容短的箱号，一般用在条形码上。只取订单编号的前8位
                {
                    if (throwException) {
                        throw new ValidationException("箱号不存在！");
                    }

                    return null;
                }

                // 检查 orderPack Id 和 箱序号配对
                var orderPackBox = session.Query<LogisticsOrderPackBoxInfo>()
                    .Where(e => e.OrderPack.Id == orderPackId && e.BoxNum == boxNum)
                    .Take(1)
                    .SingleOrDefault();

                if (orderPackBox == null) {
                    if (throwException) {
                        throw new ValidationException("箱号不存在！");
                    }

                    return null;
                }

                return orderPackBox;
            }
        } else {
            // 2 检查新的格式

            /*
             
                订单编号，可以改成：
    
                CN+客户ID数字部分（ID数字部分5位，不到5位的，前面就加0）+订单次数（4位，不足4位，前面加0）
    
                箱序订单编号+4位，不足部分前面加0。
    
                目前的箱序前面的箱号，系统内部显示，但标签上不再显示
    
    
                以客户ID：5001,第15个订单，举例：
    
                订单编号为：CN050010015；
    
                共3箱的话：CN0500100150001，CN0500100150002，CN0500100150003    
                     
             */

            if (boxCode.Length <= 4) {
                if (throwException) {
                    throw new ValidationException("箱号格式不准确！");
                }

                return null;
            }

            /* 这里有个过渡期，过渡期内的箱号格式可能是 2021032710010001
            if (boxCode.StartsWith("CN") == false)
            {
                throw new ValidationException("箱号格式不准确！");
            }
            */

            // 一般来讲，一个订单的箱号数量不可能超过4位，这里是基于这个假设来做的
            if (int.TryParse(boxCode.Substring(boxCode.Length - 4), out int boxNum) == false) // 这个放在前面箱号格式不对的错误提示就会放到前面一点
            {
                if (throwException) {
                    throw new ValidationException("箱号格式不准确！");
                }

                return null;
            }

            string orderCode = boxCode.Substring(0, boxCode.Length - 4);

            // 检查 orderCode 最后两位是不是 字母，如果是就认定为打包分拆子单号，忽略
            string subCode = orderCode.Substring(orderCode.Length - 2);
            if (int.TryParse(subCode, out int subCodeInt) == false) // 如果后两位不是数字，就假设是分拆子单号
            {
                orderCode = orderCode.Substring(0, orderCode.Length - 2);
            }

            var order = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCode)
                .Take(1)
                .SingleOrDefault();

            if (order == null) {
                if (throwException) {
                    throw new ValidationException("箱号不存在！");
                }

                return null;
            }

            int orderId = order.Id;
            var packBox = session.Query<LogisticsOrderPackBoxInfo>()
                .Where(e => e.Order.Id == orderId && e.BoxNum == boxNum)
                .Take(1)
                .SingleOrDefault();

            if (packBox == null) {
                if (throwException) {
                    throw new ValidationException("箱号不存在！");
                }

                return null;
            }

            return packBox;
        }
    }
}