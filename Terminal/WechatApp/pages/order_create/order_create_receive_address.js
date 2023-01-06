// pages/order_create/order_create_receive_address.js
const app = getApp();

Page({
    data: {},

    onLoad(options) {

    },

    onPreButtonClick(e) {
        this.gotoPre();
    },

    onNextButtonClick(e) {

    },

    gotoPre() {
        wx.navigateTo({
            url: "./order_create_first_mode",
        });
    },

    gotoNext() {
        wx.navigateTo({
            url: "./order_create_send_address",
        });
    }
})
