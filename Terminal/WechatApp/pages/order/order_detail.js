const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        orderDetail: {},
    },

    onLoad(options) {
        const orderId = options.id;
        this.loadOrderDetail(orderId);
    },

    loadOrderDetail(orderId) {
        const that = this;
        app.post("/client/order/detail", {
            orderId: orderId
        }, function (res) {
            const orderDetail = res.data.orderDetail;
            that.setData({
                orderDetail: orderDetail,
            });
        });
    }
})