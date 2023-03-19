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
        arrearsAmount: ""
    },

    onLoad(options) {
        const orderId = options.orderId;
        let couponId = 0;
        if (options.couponId) {
            couponId = options.couponId;
        }

        this.setData({
            orderId: orderId,
            couponId: couponId,
        });

        const that = this;
        app.post("/client/order/pay/type/list", {
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
            });
        });
    },

    onWechatPayClick(e) {
        const orderId = this.data.orderId;
        const couponId = this.data.couponId;

        // 优惠券在这个请求里面保存到订单
        app.post("/client/wechat/app/order/wechat/pay/create", {
            orderId: orderId,
            couponId: couponId,
        }, function (res) {
            if (res.needPay === false) {
                wx.showModal({
                    title: '成功',
                    content: "订单已支付成功，无需继续微信支付！",
                    showCancel: false,
                    success: function (res) {
                        wx.redirectTo({
                            url: "/pages/order/order_detail?id=" + orderId,
                        });
                    }
                });
            } else {
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
            }
        });
    },

    onAlipayClick(e) {
        const orderId = this.data.orderId;
        const couponId = this.data.couponId;

        app.post("/client/order/alipay/create/url", {
            orderId: orderId,
            couponId: couponId,
        }, function (res) {
            const url = res.data.url;
            wx.setClipboardData({
                data: url,
                success: function (res) {
                    wx.showModal({
                        title: '成功',
                        content: "支付宝支付URL已复制，请粘贴到手机浏览器或电脑浏览器地址栏中完成支付！",
                        showCancel: false,
                        success: function (res) {
                        }
                    });
                }
            });
        });
    },

    onPaypalClick(e) {
        const orderId = this.data.orderId;
        const couponId = this.data.couponId;

        app.post("/client/order/paypal/create/url", {
            orderId: orderId,
            couponId: couponId,
        }, function (res) {
            const url = res.data.url;
            wx.setClipboardData({
                data: url,
                success: function (res) {
                    wx.showModal({
                        title: '成功',
                        content: "PayPal支付URL已复制，请粘贴到手机浏览器或电脑浏览器地址栏中完成支付！",
                        showCancel: false,
                        success: function (res) {
                        }
                    });
                }
            });
        });
    },
})