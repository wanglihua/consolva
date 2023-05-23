namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 物品管理
    /// </summary>
    public class Goods {
        public const int CateId = PassResCate.GoodsCateId;
        public static PassResCateInfo Cate => PassResCate.Goods;

        public static IList<PassResInfo> AllData => new List<PassResInfo>() {
            new PassResInfo { Id = GoodsWarehouse, Name = "扩展名添加", Cate = Cate },
            new PassResInfo { Id = GoodsTrade, Name = "扩展名确认", Cate = Cate },
            new PassResInfo { Id = GoodsListByType, Name = "类型物品清单", Cate = Cate },
            new PassResInfo { Id = HsGoods, Name = "海关物品名", Cate = Cate },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static IList<int> All => AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 扩展名添加
        /// </summary>
        public const int GoodsWarehouse = CateId + 0200;

        /// <summary>
        /// 扩展名确认
        /// </summary>
        public const int GoodsTrade = CateId + 0400;

        /// <summary>
        /// 类型物品清单
        /// </summary>
        public const int GoodsListByType = CateId + 0600;

        /// <summary>
        /// 海关物品名
        /// </summary>
        public const int HsGoods = CateId + 0800;
    }
}