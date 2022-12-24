// pages/customer/customer_address/customer_address_receive/customer_address_receive_list.js
const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        receiveAddressList: [],
        receiveAddressCount: 0,
        sendAddressCount: 0,
    },

    onLoad(options) {
        this.loadReceiveAddressList();
    },

    loadReceiveAddressList() {
        const that = this;
        app.post("/client/customer/receive/address/list", {}, function (res) {
            const receiveAddressList = res.data.receiveAddressList;
            const receiveAddressCount = res.data.receiveAddressCount;
            const sendAddressCount = res.data.sendAddressCount;

            for (const address of receiveAddressList) {
                if (address.isDefault) {
                    address.defaultStr = '默认地址';
                } else {
                    address.defaultStr = '';
                }
            }

            that.setData({
                receiveAddressList: receiveAddressList,
                receiveAddressCount: receiveAddressCount,
                sendAddressCount: sendAddressCount,
            });
        });
    },

    onTabClick(e) {
        // console.log(e.detail.name);
        if (e.detail.name === "send") {
            wx.redirectTo({
                url: "../customer_address_send/customer_address_send_list",
            });
        }
    },

    onAddCustomerReceiveAddress(e) {
        console.log(e);
        wx.navigateTo({
            url: "./customer_address_receive_detail?id=0",
        });
    },
})