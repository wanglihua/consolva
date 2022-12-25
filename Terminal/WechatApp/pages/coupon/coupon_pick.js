// pages/coupon/coupon_pick/coupon_pick.js

const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        batchCount: 0,
        batchList: [],
    },

    onLoad(options) {
        this.loadPickCouponList();
    },

    loadPickCouponList() {
        const that = this;
        app.post("/client/coupon/pick/list", {}, function (res) {
            that.setData({
                batchCount: res.data.batchCount,
                batchList: res.data.batchList,
            });
        });
    },

    onCouponPickClick(e) {
        const batchId = e.currentTarget.dataset.item.id;

        const that = this;
        app.post("/client/coupon/pick/save", {
            batchId: batchId,
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            that.setData({
                batchCount: res.data.batchCount,
                batchList: res.data.batchList,
            });
        });
    }
})