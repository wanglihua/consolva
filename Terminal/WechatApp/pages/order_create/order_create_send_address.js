const app = getApp();

Page({
    data: {
        addressList: [],
        selectAddressId: 0,
    },

    onLoad(options) {
        this.loadSendAddressList();
    },

    loadSendAddressList() {
        const that = this;
        app.post("/client/order/create/address/send/list", {}, function (res) {
            that.setData({
                addressList: res.data.addressList,
                selectAddressId: res.data.selectAddressId,
            });
        });
    },

    onSendAddressButtonClick(e) {
        const address = e.currentTarget.dataset.item;
        const addressId = address.id;

        const that = this;
        app.post("/client/order/create/address/send/save", {
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
                content: "请选择发货地址！",
                showCancel: false
            });
            return;
        }

        this.gotoNext();
    },

    gotoPre() {
        wx.redirectTo({
            url: "./order_create_receive_address",
        });
    },

    gotoNext() {
        wx.redirectTo({
            url: "./order_create_insurance",
        });
    }
})