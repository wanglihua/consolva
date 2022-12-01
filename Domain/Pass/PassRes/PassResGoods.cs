namespace shunda.Domain.Pass.PassRes;

public partial class PassRes
{
    /// <summary>
    /// 物品管理
    /// </summary>
    public class Goods
    {
        /// <summary>
        /// 类别
        /// </summary>
        public const int CateId = 09000_00000;

        public static readonly PassResCateInfo Cate = new PassResCateInfo() { Id = CateId, Name = "物品管理" };

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo() { Id = GoodsWarehouse, Name = "扩展名添加", CateIdForCache = CateId },
            new PassResInfo() { Id = GoodsTrade, Name = "扩展名确认", CateIdForCache = CateId },
            new PassResInfo() { Id = GoodsListByType, Name = "类型物品清单", CateIdForCache = CateId },
            new PassResInfo() { Id = HsGoods, Name = "海关物品名", CateIdForCache = CateId },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 扩展名添加
        /// </summary>
        public const int GoodsWarehouse = CateId + 0100;

        /// <summary>
        /// 扩展名确认
        /// </summary>
        public const int GoodsTrade = CateId + 0200;

        /// <summary>
        /// 类型物品清单
        /// </summary>
        public const int GoodsListByType = CateId + 0300;

        /// <summary>
        /// 海关物品名
        /// </summary>
        public const int HsGoods = CateId + 0400;
    }
}