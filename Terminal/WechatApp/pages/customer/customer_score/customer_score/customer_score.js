// pages/customer/customer_score/customer_score/customer_score.js

const app = getApp();

Page({

    data: {
        totalScore: 0,
        validScore: 0,
        vipRank: '',
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
    }
})