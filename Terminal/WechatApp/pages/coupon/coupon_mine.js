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
            let couponList = res.data.couponList;
            for (let index = 0; index < couponList.length; index++) {
                let coupon = couponList[index];
                if (coupon.follow === true) {
                    coupon.rule = coupon.rule + "(" + coupon.batchName + ")";
                }
            }

            that.setData({
                couponList: couponList,
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

    onCouponHeaderClick(e) {
        const coupon = e.currentTarget.dataset.item;
        const couponId = coupon.id;

        const couponList = this.data.couponList;
        for (let index = 0; index < couponList.length; index++) {
            let c = couponList[index];
            if (c.id === couponId) {
                if (c.show === true) {
                    c.show = false;
                } else {
                    c.show = true;
                }
            }
        }

        this.setData({
            couponList: couponList,
        });
    },

})