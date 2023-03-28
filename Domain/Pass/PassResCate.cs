namespace shunda.Domain.Pass;

public class PassResCate {
    public const int ExpressCateId = 01100_00000;
    public static readonly PassResCateInfo Express = new PassResCateInfo() { Id = ExpressCateId, Name = "快递管理" };

    public const int CustomerCateId = 07000_00000;
    public static readonly PassResCateInfo Customer = new PassResCateInfo() { Id = CustomerCateId, Name = "会员管理" };

    public const int FeeCateId = 06000_00000;
    public static readonly PassResCateInfo Fee = new PassResCateInfo() { Id = FeeCateId, Name = "费用管理" };
    
    public const int GoodsCateId = 09000_00000;
    public static readonly PassResCateInfo Goods = new PassResCateInfo() { Id = GoodsCateId, Name = "物品管理" };    

    public const int CustomerMessageCateId = 11000_00000;
    public static readonly PassResCateInfo CustomerMessage = new PassResCateInfo() { Id = CustomerMessageCateId, Name = "消息推送" };



    public static IList<PassResCateInfo> GetLst() {
        return new List<PassResCateInfo>() {
            Express,
            Customer,
            CustomerMessage,
        };
    }
}

/*

        IList<PassResCateInfo> cateList = new List<PassResCateInfo>() {
            PassRes.LogisticsOrder.Cate,
            PassRes.Express.Cate,
            PassRes.ReceivePack.Cate,


            PassRes.Fee.Cate,
            PassRes.Goods.Cate,

            PassRes.Storage.Cate,
            PassRes.Setting.Cate,
            PassRes.Logistics.Cate,
            PassRes.CustomerMessage.Cate,
        };
        */