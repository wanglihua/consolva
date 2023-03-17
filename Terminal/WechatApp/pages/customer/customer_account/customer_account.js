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

        wx.navigateTo({
            url: './cusotmer_account_charge_pay_type?amount=' + chargeAmount,
        });
    },

    onBackClick(e) {
        wx.navigateBack();
    }
})