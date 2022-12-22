// pages/coupon/coupon_pick/coupon_pick.js

const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        batchList: [],
    },

    onLoad(options) {
        this.loadPickCouponList();
    },

    loadPickCouponList() {
        const that = this;
        app.post("/client/coupon/pick/list", {}, function (res) {
            that.setData({
                batchList: res.data.batchList,
            });
        });
    },

    onCouponPickClick(e) {
        console.log(e);
    }
})