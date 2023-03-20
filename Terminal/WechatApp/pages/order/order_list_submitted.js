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
                expressClaim.actions = "查看详细";
            }

            that.setData({
                orderList: util.distinctConcat(that.data.orderList, orderList),
                orderCount: orderCount,
                lastId: lastId,
            });
        });
    },

    onCopyClick(e) {
        const copyText = e.currentTarget.dataset.item;
        wx.setClipboardData({
            data: copyText,
            success: function (res) {
                wx.showToast({
                    title: "复制成功！",
                });
            }
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

        // actionSheetActions.push({
        //     action: "cancel",
        //     name: '取消订单',
        //     className: 'iconfont iconfont-reject',
        // });

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

        /* 暂时只能联系客服取消订单
        if (action === "cancel") {
            wx.showModal({
                title: '订单取消确认',
                content: '确实要取消订单 ' + orderCode + ' 吗？',
                success(res) {
                    if (res.confirm) {
                        app.post("/client/order/action/cancel", {orderId: orderId}, function (res) {
                            wx.showToast({
                                title: res.message,
                            });

                            // 重置lastId
                            that.setData({
                                lastId: 0,
                            });

                            // 重新加载订单列表
                            that.loadOrderList();
                        });
                    }
                }
            });
        }
         */

        this.setData({
            showActionSheet: false,
        });
    }
})