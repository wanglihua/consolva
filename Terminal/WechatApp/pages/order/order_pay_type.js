// pages/order/order_pay_type.js
const app = getApp();

Page({

    data: {
        orderId: 0,
        orderCode: "",
        totalAmount: "",
        paidAmount: "",
        arrearsAmount: ""
    },

    onLoad(options) {
        const orderId = options.orderId;

        const that = this;
        app.post("/client/order/pay/type/list", {
            orderId: orderId,
        }, function (res) {

            that.setData({
                orderId: orderId,
                orderCode: res.data.orderCode,
                totalAmount: res.data.totalAmount,
                paidAmount: res.data.paidAmount,
                arrearsAmount: res.data.arrearsAmount,
            });
        });
    },

    onWechatPayClick(e) {
        const orderId = this.data.orderId;

        app.post("/client/wechat/app/order/wechat/pay/create", {
            orderId: orderId,
        }, function (res) {
            wx.requestPayment({
                'timeStamp': res.data.timeStamp,
                'nonceStr': res.data.nonceStr,
                'package': res.data.package,
                'signType': res.data.signType,
                'paySign': res.data.paySign,
                'success': function (res) {
                    wx.redirectTo({
                        url: "/pages/order/order_detail?id=" + orderId,
                    });
                },
                'fail': function (res) {
                    if (res.errMsg !== "requestPayment:fail cancel") {
                        wx.showModal({
                            title: '错误',
                            content: res.errMsg,
                            showCancel: false
                        });
                    }
                }
            });
        });
    },
})