// pages/customer/customer_score/customer_score/customer_score.js

const app = getApp();

Page({

    data: {
        totalScore: 0,
        validScore: 0,
        vipRank: '',
        exchangeScore: '',
        exchangeScoreInputHasError: false,
        exchangeScoreInputErrorMsg: '',
    },

    onLoad(options) {
        this.loadCustomerScore();
    },

    loadCustomerScore() {
        const that = this;
        app.post("/client/customer/score", {}, function (res) {
            that.setData({
                totalScore: res.data.totalScore,
                validScore: res.data.validScore,
                vipRank: res.data.vipRank,
            });
        });
    },

    onExchangeClick(e) {
        const exchangeScore = this.data.exchangeScore;
        if (!exchangeScore) {
            this.setData({
                exchangeScoreInputHasError: true,
                exchangeScoreInputErrorMsg: '兑换积分不能为空！',
            });

            return;
        }

        this.setData({
            exchangeScoreInputHasError: false,
            exchangeScoreInputErrorMsg: '',
        });

        const exchangeScoreNum = parseFloat(exchangeScore);
        if (isNaN(exchangeScoreNum) || exchangeScoreNum <= 0) {
            this.setData({
                exchangeScoreInputHasError: true,
                exchangeScoreInputErrorMsg: '请输入正确的兑换积分！',
            });
        }

        this.setData({
            exchangeScoreInputHasError: false,
            exchangeScoreInputErrorMsg: '',
        });

        const that = this;

        wx.showModal({
            title: '兑换确认',
            content: '确实要把 ' + exchangeScore + ' 积分兑换成余额吗？',
            success(res) {
                if (res.confirm) {
                    app.post("/client/customer/score/to/account", {
                        score: exchangeScoreNum,
                    }, function (res) {
                        wx.showModal({
                            title: '成功',
                            content: res.message,
                            showCancel: false
                        });

                        that.setData({
                            exchangeScore: '',
                            totalScore: res.data.totalScore,
                            validScore: res.data.validScore,
                            vipRank: res.data.vipRank,
                        });

                        // that.loadCustomerScore(); 不用再次加载，兑换成功后的返回数据里面已经相当于重新加载了
                    });
                }
            }
        });


    },

    onBackClick(e) {
        wx.navigateBack();
    }
})