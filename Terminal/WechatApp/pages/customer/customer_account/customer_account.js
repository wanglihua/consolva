// pages/customer/customer_account/customer_account/customer_account.js

const app = getApp();

Page({

    data: {
        amountInputHasError: false,
        amountInputErrorMsg: '',
        currentAmount: '',
        chargeAmount: '',
    },

    onLoad(options) {
        this.loadCustomerAccount();
    },

    loadCustomerAccount() {
        const that = this;
        app.post("/client/customer/account", {}, function (res) {
            const amount = res.data.amount;
            that.setData({
                currentAmount: amount,
            });
        });
    },

    onChargeClick(e) {
        const chargeAmount = this.data.chargeAmount;
        if (!chargeAmount) {
            this.setData({
                amountInputHasError: true,
                amountInputErrorMsg: "充值金额不能为空！",
            });

            return;
        }

        this.setData({
            amountInputHasError: false,
            amountInputErrorMsg: "",
        });

        const chargeAmountNum = parseFloat(chargeAmount);
        if (isNaN(chargeAmountNum) || chargeAmountNum <= 0) {
            this.setData({
                amountInputHasError: true,
                amountInputErrorMsg: "请输入正确的充值金额！",
            });

            return;
        }

        this.setData({
            amountInputHasError: false,
            amountInputErrorMsg: "",
        });

        app.post("/client/wechat/app/charge/wechat/pay/create", {
            amount: chargeAmountNum
        }, function (res) {
            console.log(res);
            
            wx.requestPayment({
                'timeStamp': res.data.timeStamp,
                'nonceStr': res.data.nonceStr,
                'package': res.data.package,
                'signType': res.data.signType,
                'paySign': res.data.paySign,
                'success': function (res) {
                    this.setData({
                        chargeAmount: '',
                    });
                    this.loadCustomerAccount();
                },
                'fail': function (res) {
                    wx.showModal({
                        title: '错误',
                        content: res.errMsg,
                        showCancel: false
                    });
                }
            });
        });
    },

    onBackClick(e) {
        wx.navigateBack();
    }
})