// pages/order/order_pay_customer_account.js
const app = getApp();

Page({
    data: {
        orderId: 0,
        couponId: 0,
        orderCode: "",
        totalAmount: "",
        hasCoupon: false,
        discountAmount: "",
        paidAmount: "",
        arrearsAmount: "",
        actualPayAmount: "",
        hasAccountToPay: true,
    },

    onLoad(options) {
        const orderId = options.orderId;
        const couponId = options.couponId;

        this.setData({
            orderId: orderId,
            couponId: couponId,
        });

        const that = this;
        app.post("/client/order/pay/customer/account/detail", {
            orderId: orderId,
            couponId: couponId,
        }, function (res) {
            that.setData({
                orderCode: res.data.orderCode,
                totalAmount: res.data.totalAmount,
                hasCoupon: res.data.hasCoupon,
                discountAmount: res.data.discountAmount,
                paidAmount: res.data.paidAmount,
                arrearsAmount: res.data.arrearsAmount,
                actualPayAmount: res.data.actualPayAmount,
                hasAccountToPay: res.data.hasAccountToPay,
            });
        });
    },

    onPayClick(e) {
        const orderId = this.data.orderId;
        const couponId = this.data.couponId;

        app.post("/client/order/pay/customer/account/pay", {
            orderId: orderId,
            couponId: couponId,
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