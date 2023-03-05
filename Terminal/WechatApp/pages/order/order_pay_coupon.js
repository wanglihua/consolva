// pages/order/order_pay_coupon.js
const app = getApp();

Page({
    data: {
        orderId: 0,
        couponCount: 0,
        couponList: [],
        couponId: 0,
    },

    onLoad(options) {
        const orderId = options.orderId;
        this.setData({
            orderId: orderId,
        });

        const that = this;
        app.post("/client/order/pay/coupon/list", {
            orderId: orderId,
        }, function (res) {
            that.setData({
                couponCount: res.data.couponCount,
                couponList: res.data.couponList,
                couponId: res.data.couponId,
            });
        });
    },

    onCouponButtonClick(e) {
        
    }
})