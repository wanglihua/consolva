// pages/order/order_list_submitted.js

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

        app.post("/client/order/list/submitted", {
            lastId: that.data.lastId
        }, function (res) {
            const orderCount = res.data.orderCount;
            const orderList = res.data.orderList;
            const lastId = res.data.lastId;

            for (const expressClaim of orderList) {
                expressClaim.actions = "查看详细，取消订单";
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

        actionSheetActions.push({
            action: "claim",
            name: '认领',
            className: 'iconfont iconfont-claim-pick',
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
        const orderId = this.data.currentOrder.id;
        const action = e.detail.action;

        const that = this;
        if (action === "detail") {
            wx.navigateTo({
                url: './express_claim_detail?id=' + orderId,
            });
        }

        if (action === "claim") {
            wx.navigateTo({
                url: './express_claim_pick?id=' + orderId,
            });
        }

        this.setData({
            showActionSheet: false,
        });
    }
})