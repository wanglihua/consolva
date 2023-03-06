const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        couponList: [],
        couponCount: 0,
        hasBatchToPick: false,
    },

    onLoad(options) {
        // this.loadMyCouponList();
    },
    
    onShow() {
        this.loadMyCouponList();
    },

    loadMyCouponList() {
        const that = this;
        app.post("/client/coupon/mine/list", {}, function (res) {
            that.setData({
                couponList: res.data.couponList,
                couponCount: res.data.couponCount,
                hasBatchToPick: res.data.hasBatchToPick,
            });
        });
    },

    onCouponPickClick(e) {
        wx.navigateTo({
            url: "./coupon_pick",
        });
    },

})