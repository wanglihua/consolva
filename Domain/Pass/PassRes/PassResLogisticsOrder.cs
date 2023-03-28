namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 物流订单
    /// </summary>
    public class LogisticsOrder {
        public const int CateId = PassResCate.LogisticsOrderCateId;
        public static readonly PassResCateInfo Cate = PassResCate.LogisticsOrder;

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo { Id = OrderListActive, Name = "活动订单", Cate = Cate },
            new PassResInfo { Id = OrderListComplete, Name = "正常结束订单", Cate = Cate },
            new PassResInfo { Id = ImportOrder, Name = "导入订单", Cate = Cate },
            new PassResInfo { Id = ImportOrderCheckinExpress, Name = "导入订单时登记快递单", Cate = Cate },
            new PassResInfo { Id = OrderListCancel, Name = "作废订单", Cate = Cate },
            new PassResInfo { Id = OrderPackBox, Name = "箱号跟踪", Cate = Cate },
            new PassResInfo { Id = OrderAirExpress, Name = "国际单号", Cate = Cate },
            new PassResInfo { Id = OrderModifyReceiverAddress, Name = "修改订单收货地址", Cate = Cate },
            new PassResInfo { Id = OrderSetCancel, Name = "设置订单作废", Cate = Cate },
            new PassResInfo { Id = OrderLogisticsModeChange, Name = "修改订单物流方式", Cate = Cate },
            new PassResInfo { Id = OrderRollbackSubmitPack, Name = "订单取消打包提交", Cate = Cate },
            new PassResInfo { Id = OrderExpressEdit, Name = "订单快递单号维护", Cate = Cate },
            new PassResInfo { Id = OrderGoodsEdit, Name = "订单物品维护", Cate = Cate },
            new PassResInfo { Id = OrderPackDataModify, Name = "订单打包信息修改", Cate = Cate },
            new PassResInfo { Id = OrderRemarkEdit, Name = "订单备注维护", Cate = Cate },
            new PassResInfo { Id = CustomerOrderCount, Name = "会员订单统计", Cate = Cate },
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
        /// 订单快递单号维护
        /// 订单快递单号添加修改删除
        /// </summary>
        public const int OrderExpressEdit = CateId + 24000;

        /// <summary>
        /// 订单物品维护
        /// 订单物品添加修改删除
        /// </summary>
        public const int OrderGoodsEdit = CateId + 26000;

        /// <summary>
        /// 订单打包信息修改
        /// </summary>
        public const int OrderPackDataModify = CateId + 28000;

        /// <summary>
        /// 订单备注维护
        /// </summary>
        public const int OrderRemarkEdit = CateId + 30000;

        /// <summary>
        /// 会员订单统计
        /// </summary>
        public const int CustomerOrderCount = CateId + 32000;
    }
}