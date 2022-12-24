// pages/customer/customer_address/customer_address_send/customer_address_send_list.js
const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        sendAddressList: [],
        sendAddressCount: 0,
        receiveAddressCount: 0,
    },

    onLoad(options) {
        this.loadSendAddressList();
    },

    loadSendAddressList() {
        const that = this;
        app.post("/client/customer/send/address/list", {}, function (res) {
            const sendAddressList = res.data.sendAddressList;
            const sendAddressCount = res.data.sendAddressCount;
            const receiveAddressCount = res.data.receiveAddressCount;

            that.setData({
                sendAddressList: sendAddressList,
                sendAddressCount: sendAddressCount,
                receiveAddressCount: receiveAddressCount,
            });
        });
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