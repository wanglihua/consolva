// pages/customer/customer_address/customer_address_send/customer_address_send_list.js
const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {},

    onLoad(options) {

    },

    onTabClick(e) {
        // console.log(e.detail.name);
        if (e.detail.name === "receive") {
            wx.redirectTo({
                url: "../customer_address_receive/customer_address_receive_list",
            });
        }
    },
})