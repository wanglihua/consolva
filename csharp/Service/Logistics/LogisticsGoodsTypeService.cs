using shunda.Core.Service;
using shunda.Domain.Logistics;

namespace shunda.Service.Logistics;

[Service(ServiceType.Scoped)]
public class LogisticsGoodsTypeService
{
    /// <summary>
    /// 从一个物品类型列表中，获得一个综合的物品类型
    /// 这里的实现很简单，只是id排序靠后的优先，取列表中id最靠后的一个类型
    /// </summary>
    /// <param name="goodsTypeList"></param>
    /// <returns></returns>
    public LogisticsGoodsType GetGoodsTypeFromGoodsTypeList(IList<LogisticsGoodsType> goodsTypeList)
    {
        if (goodsTypeList == null || goodsTypeList.Count == 0)
        {
            return LogisticsGoodsType.None;
        }

        LogisticsGoodsType goodsType = LogisticsGoodsType.None;

        foreach (LogisticsGoodsType gt in goodsTypeList)
        {
            if (gt.Id > goodsType.Id)
            {
                goodsType = gt;
            }
        }

        return goodsType;
    }
}