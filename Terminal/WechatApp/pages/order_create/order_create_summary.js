// pages/order_create/order_create_summary.js
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
            url: "./order_create_insurance",
        });
    },

    gotoNext() {
        wx.redirectTo({
            url: "",
        });
    }    
})
