const app = getApp();

Page({
    data: {
        amount: 0,

        currencyList: [],
        showActionSheet: false,
        actionSheetActions: [],
    },

    onLoad(options) {
        const amount = options.amount;
        this.setData({
            amount,
        });

        const that = this;

        app.post("/client/customer/account/charge", {}, function (res) {
            that.setData({
                currencyList: res.data.currencyList,
            });
        });
    },

    onWechatPayClick(e) {
        const amount = this.data.amount;
        app.post("/client/wechat/app/charge/wechat/pay/create", {
            amount: amount
        }, function (res) {
            wx.requestPayment({
                'timeStamp': res.data.timeStamp,
                'nonceStr': res.data.nonceStr,
                'package': res.data.package,
                'signType': res.data.signType,
                'paySign': res.data.paySign,
                'success': function (res) {
                    wx.redirectTo({
                        url: '/pages/customer/customer_account/customer_account',
                    })
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

    onAlipayClick(e) {
        const amount = this.data.amount;

        app.post("/client/charge/alipay/create/url", {
            amount: amount,
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
        const actionSheetActions = [];

        const currencyList = this.data.currencyList;
        for (let currency of currencyList) {
            actionSheetActions.push({
                currency: currency.code,
                name: currency.code + " " + currency.name,
            });
        }

        this.setData({
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
        const amount = this.data.amount;
        const currency = e.detail.currency;

        app.post("/client/charge/paypal/create/url", {
            amount: amount,
            currency: currency,
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

        this.setData({
            showActionSheet: false,
        });
    }
})