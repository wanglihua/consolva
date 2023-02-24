// pages/instruction/instruction_warehouse_address.js
const app = getApp();

Page({
    data: {
        address: {},
    },

    onLoad(options) {
        const that = this;
        app.post("/client/warehouse/address/detail", {}, function (res) {
            const address = res.data;
            that.setData({
                address: address,
            });
        });
    },

    onCustomerCodeClick(e) {
        const that = this;
        wx.setClipboardData({
            data: that.data.address.customerCode,
            success: function (res) {
                wx.showToast({
                    title: "复制成功！",
                });
            }
        });
    },

    onPhoneClick(e) {
        const that = this;
        wx.setClipboardData({
            data: that.data.address.phone,
            success: function (res) {
                wx.showToast({
                    title: "复制成功！",
                });
            }
        });
    },

    onAreaClick(e) {
        const that = this;
        wx.setClipboardData({
            data: that.data.address.area,
            success: function (res) {
                wx.showToast({
                    title: "复制成功！",
                });
            }
        });
    },

    onAddressClick(e) {
        const that = this;
        wx.setClipboardData({
            data: that.data.address.address,
            success: function (res) {
                wx.showToast({
                    title: "复制成功！",
                });
            }
        });
    },

    onZipcodeClick(e) {
        const that = this;
        wx.setClipboardData({
            data: that.data.address.zipcode,
            success: function (res) {
                wx.showToast({
                    title: "复制成功！",
                });
            }
        });
    },

    onFullAddressCopyClick(e) {
        const that = this;
        wx.setClipboardData({
            data: that.data.address.fullAddress,
            success: function (res) {
                wx.showToast({
                    title: "复制成功！",
                });
            }
        });
    }
})
