Page({
    data: {
        amount: 0,
    },

    onLoad(options) {
        const amount = options.amount;
        this.setData({
            amount,
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

    },
})