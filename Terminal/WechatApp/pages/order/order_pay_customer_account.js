// pages/order/order_pay_customer_account.js
const app = getApp();

Page({
    data: {
        orderId: 0,
        orderCode: "",
        totalAmount: "",
        paidAmount: "",
        arrearsAmount: "",
        actualPayAmount: "",
        hasAccountToPay: true,
    },

    onLoad(options) {
        const orderId = options.orderId;

        const that = this;
        app.post("/client/order/pay/customer/account/detail", {
            orderId: orderId,
        }, function (res) {
            that.setData({
                orderId: orderId,
                orderCode: res.data.orderCode,
                totalAmount: res.data.totalAmount,
                paidAmount: res.data.paidAmount,
                arrearsAmount: res.data.arrearsAmount,
                actualPayAmount: res.data.actualPayAmount,
                hasAccountToPay: res.data.hasAccountToPay,
            });
        });
    },

    onPayClick(e) {
        const orderId = this.data.orderId;
        app.post("/client/order/pay/customer/account/pay", {
            orderId: orderId,
        }, function (res) {
            wx.showModal({
                title: '成功',
                content: res.message,
                showCancel: false,
                success: function (result) {
                    wx.redirectTo({
                        url: "/pages/order/order_detail?id=" + orderId,
                    });
                }
            });
        });
    },
})