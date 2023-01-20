namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 物流订单
    /// </summary>
    public class LogisticsOrder {
        /// <summary>
        /// 物流订单类别
        /// </summary>
        public const int CateId = 00001_00000;

        public static readonly PassResCateInfo Cate = new PassResCateInfo() { Id = CateId, Name = "物流订单" };

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo { Id = OrderListActive, Name = "活动订单", CateIdForCache = CateId },
            new PassResInfo { Id = OrderListComplete, Name = "正常结束订单", CateIdForCache = CateId },
            new PassResInfo { Id = ImportOrder, Name = "导入订单", CateIdForCache = CateId }, 
            new PassResInfo { Id = ImportOrderCheckinExpress, Name = "导入订单时登记快递单", CateIdForCache = CateId },
            new PassResInfo { Id = OrderListCancel, Name = "作废订单", CateIdForCache = CateId },
            new PassResInfo { Id = OrderPackBox, Name = "箱号跟踪", CateIdForCache = CateId },
            new PassResInfo { Id = OrderAirExpress, Name = "国际单号", CateIdForCache = CateId },
            new PassResInfo { Id = OrderModifyReceiverAddress, Name = "修改订单收货地址", CateIdForCache = CateId },
            new PassResInfo { Id = OrderSetCancel, Name = "设置订单作废", CateIdForCache = CateId },
            new PassResInfo { Id = OrderLogisticsModeChange, Name = "修改订单物流方式", CateIdForCache = CateId },
            new PassResInfo { Id = OrderRollbackSubmitPack, Name = "订单取消打包提交", CateIdForCache = CateId },
            new PassResInfo { Id = OrderExpressAddModifyDelete, Name = "订单快递单号添加修改删除", CateIdForCache = CateId },
            new PassResInfo { Id = OrderGoodsAddModifyDelete, Name = "订单物品添加修改删除", CateIdForCache = CateId },
            new PassResInfo { Id = OrderPackDataModify, Name = "订单打包信息修改", CateIdForCache = CateId },
            new PassResInfo { Id = CustomerOrderCount, Name = "会员订单统计", CateIdForCache = CateId },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 订单列表-活动订单
        /// </summary>
        public const int OrderListActive = CateId + 02000;

        /// <summary>
        /// 订单列表-正常结束订单
        /// </summary>
        public const int OrderListComplete = CateId + 04000;

        /// <summary>
        /// 导入订单
        /// </summary>
        public const int ImportOrder = CateId + 06000;

        /// <summary>
        /// 导入订单时登记快递单
        /// </summary>
        public const int ImportOrderCheckinExpress = CateId + 08000;

        /// <summary>
        /// 作废订单
        /// </summary>
        public const int OrderListCancel = CateId + 10000;

        /// <summary>
        /// 箱号跟踪
        /// </summary>
        public const int OrderPackBox = CateId + 12000;

        /// <summary>
        /// 国际单号
        /// </summary>
        public const int OrderAirExpress = CateId + 14000;

        /// <summary>
        /// 修改订单收货地址
        /// </summary>
        public const int OrderModifyReceiverAddress = CateId + 16000;

        /// <summary>
        /// 设置订单作废
        /// </summary>
        public const int OrderSetCancel = CateId + 18000;

        /// <summary>
        /// 修改订单物流方式
        /// </summary>
        public const int OrderLogisticsModeChange = CateId + 20000;

        /// <summary>
        /// 订单取消打包提交
        /// </summary>
        public const int OrderRollbackSubmitPack = CateId + 22000;

        /// <summary>
        /// 订单快递单号添加修改删除
        /// </summary>
        public const int OrderExpressAddModifyDelete = CateId + 24000;

        /// <summary>
        /// 订单物品添加修改删除
        /// </summary>
        public const int OrderGoodsAddModifyDelete = CateId + 26000;

        /// <summary>
        /// 订单打包信息修改
        /// </summary>
        public const int OrderPackDataModify = CateId + 28000;

        /// <summary>
        /// 会员订单统计
        /// </summary>
        public const int CustomerOrderCount = CateId + 30000;
    }
}