// pages/order_create/order_create_insurance.js
const app = getApp();

Page({
    data: {

    },

    onLoad(options) {

    },

    onPreButtonClick(e) {
        this.gotoPre();
    },

    onNextButtonClick(e) {

    },

    gotoPre() {
        wx.redirectTo({
            url: "./order_create_send_address",
        });
    },

    gotoNext() {
        wx.redirectTo({
            url: "./order_create_summary",
        });
    }
})
