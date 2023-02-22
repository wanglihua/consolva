using shunda.Core.Service;
using shunda.Domain.Logistics;

namespace shunda.Service.Logistics;

[Service(ServiceType.Scoped)]
public class LogisticsGoodsTypeService {
    /// <summary>
    /// 从一个物品类型列表中，获得一个综合的物品类型
    /// </summary>
    /// <param name="goodsTypeList"></param>
    /// <returns></returns>
    public LogisticsGoodsType GetGoodsTypeFromGoodsTypeList(IList<LogisticsGoodsType> goodsTypeList) {
        if (goodsTypeList == null || goodsTypeList.Count == 0) {
            return LogisticsGoodsType.None;
        }

        LogisticsGoodsType goodsType = LogisticsGoodsType.None;

        foreach (LogisticsGoodsType gt in goodsTypeList) {
            if (gt.Rank > goodsType.Rank) {
                goodsType = gt;
            }
        }

        return goodsType;
    }
}