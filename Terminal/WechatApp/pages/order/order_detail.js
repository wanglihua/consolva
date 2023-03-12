const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        orderDetail: {},
        orderShow: {
            show: false,
            text: "显示",
        },
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
    },

    onOrderHeaderClick(e) {
        const orderShow = this.data.orderShow;
        if (orderShow.show) {
            orderShow.show = false;
            orderShow.text = "显示";
        } else {
            orderShow.show = true;
            orderShow.text = "隐藏";
        }

        this.setData({
            orderShow: orderShow,
        });
    },

    onFirstModeHeaderClick(e) {
    },

    onReceiveAddressHeaderClick(e) {
    },

    onExpressListHeaderClick(e) {
    },

    onOrderPackListHeaderClick(e) {
    },

    onFeeHeaderClick(e) {

    }
})