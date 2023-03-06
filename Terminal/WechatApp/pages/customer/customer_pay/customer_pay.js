const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        lastId: 0,
        payCount: 0,
        payList: [],
    },

    onLoad(options) {
        this.loadPayList();
    },

    onReachBottom() {
        this.loadPayList();
    },

    loadPayList() {
        const that = this;
        app.post("/client/customer/pay/list", {
            lastId: that.data.lastId,
        }, function (res) {
            const payCount = res.data.payCount;
            const payList = util.distinctConcat(that.data.payList, res.data.payList)
            const lastId = res.data.lastId;

            that.setData({
                payCount: payCount,
                payList: payList,
                lastId: lastId,
            });
        });
    }
})