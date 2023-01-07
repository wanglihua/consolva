// pages/order_create/order_create_receive_address.js
const app = getApp();

Page({
    data: {
        addressList: [],
        selectAddressId: 0,
        country: '',
    },

    onLoad(options) {
        this.loadReceiveAddressList();
    },

    loadReceiveAddressList() {
        const that = this;
        app.post("/client/order/create/address/receive/list", {}, function (res) {
            that.setData({
                addressList: res.data.addressList,
                selectAddressId: res.data.selectAddressId,
                country: res.data.country,
            });
        });
    },

    onReceiveAddressButtonClick(e) {
        const address = e.currentTarget.dataset.item;
        const addressId = address.id;

        const that = this;
        app.post("/client/order/create/address/receive/save", {
            addressId: addressId,
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            that.setData({
                selectAddressId: addressId,
            });

            that.gotoNext();
        })
    },

    onPreButtonClick(e) {
        this.gotoPre();
    },

    onNextButtonClick(e) {
        const selectAddressId = this.data.selectAddressId;
        if (selectAddressId === 0) {
            wx.showModal({
                title: '错误',
                content: "请选择收货地址！",
                showCancel: false
            });
            return;
        }

        this.gotoNext();
    },

    gotoPre() {
        wx.redirectTo({
            url: "./order_create_first_mode",
        });
    },

    gotoNext() {
        wx.redirectTo({
            url: "./order_create_send_address",
        });
    }
})
