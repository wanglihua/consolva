const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        lastId: 0,
        orderList: [],
        orderCount: 0,
        currentOrder: {},
        showActionSheet: false,
        actionSheetActions: [],
    },

    onLoad(options) {
        this.loadOrderList();
    },

    onReachBottom() {
        this.loadOrderList();
    },

    loadOrderList() {
        const that = this;

        app.post("/client/order/list/waiting/transport", {
            lastId: that.data.lastId
        }, function (res) {
            const orderCount = res.data.orderCount;
            const orderList = res.data.orderList;
            const lastId = res.data.lastId;

            for (const expressClaim of orderList) {
                expressClaim.actions = "查看详细";
            }

            that.setData({
                orderList: util.distinctConcat(that.data.orderList, orderList),
                orderCount: orderCount,
                lastId: lastId,
            });
        });
    },

    onActionCellClick(e) {
        const currentOrder = e.currentTarget.dataset.item;

        const actionSheetActions = [];

        actionSheetActions.push({
            action: "detail",
            name: '查看详细',
            className: 'iconfont iconfont-view-detail',
        });

        this.setData({
            currentOrder: currentOrder,
            actionSheetActions: actionSheetActions,
            showActionSheet: true
        });
    },

    onActionSheetCloseClick(e) {
        this.setData({
            showActionSheet: false
        });
    },

    onActionSheetSelect(e) {
        const currentOrder = this.data.currentOrder;
        const orderId = currentOrder.id;
        const orderCode = currentOrder.orderCode;
        const action = e.detail.action;

        const that = this;
        if (action === "detail") {
            wx.navigateTo({
                url: './order_detail?id=' + orderId,
            });
        }

        this.setData({
            showActionSheet: false,
        });
    }
})