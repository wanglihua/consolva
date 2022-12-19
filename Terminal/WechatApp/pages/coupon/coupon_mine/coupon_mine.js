// pages/coupon/coupon_mine/coupon_mine.js

const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        couponList: [],
        couponCount: 0,
        hasBatchToPick: false,
    },

    onLoad(options) {
        this.loadMyCouponList();
    },

    loadMyCouponList() {
        const that = this;
        app.post("/client/coupon/mine/list", {}, function (res) {
            that.setData({
                couponList: that.data.couponList,
                couponCount: that.data.couponCount,
                hasBatchToPick: that.data.hasBatchToPick,
            });
        });
    },

    onCouponPickClick(e) {
        wx.navigateTo({
            url: "../coupon_pick/coupon_pick",
        });
    },

})