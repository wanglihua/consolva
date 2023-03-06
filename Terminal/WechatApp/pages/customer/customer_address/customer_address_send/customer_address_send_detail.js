const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        regionOptions: [],
        showStreetSelect: false,
        streetFieldValue: '',
        streetCascaderValue: '',

        id: 0,
        name: '',
        mobile: '',
        address: '',
        zipcode: '',
    },

    onLoad(options) {
        const id = options.id;
        const that = this;
        app.post("/client/customer/send/address/detail", {
            id: id,
        }, function (res) {
            const data = res.data;

            that.setData({
                id: data.id,
                name: data.name,
                mobile: data.mobile,
                address: data.address,
                zipcode: data.zipcode,

                streetFieldValue: data.streetFieldValue,
                streetCascaderValue: data.streetCascaderValue,
                regionOptions: data.regionList,
            });
        });
    },

    onStreetFieldTap(e) {
        this.setData({
            showStreetSelect: true,
        });
    },

    onStreetSelectClose(e) {
        this.setData({
            showStreetSelect: false,
        });
    },

    onStreetSelectChange(e) {
        const value = e.detail.value;
        const tabIndex = e.detail.tabIndex;

        const that = this;
        if (tabIndex < 3) {
            app.post("/client/customer/send/address/detail/region/list", {
                regionId: parseInt(value),
                tabIndex: tabIndex,
            }, function (res) {
                that.setData({
                    regionOptions: res.data,
                });
            });
        }
    },

    onStreetSelectFinish(e) {
        const {selectedOptions, value} = e.detail;
        const fieldValue = selectedOptions
            .map((option) => option.text)
            .join('/');
        this.setData({
            streetFieldValue: fieldValue,
            streetCascaderValue: value,
            showStreetSelect: false,
        })
    },

    onSaveClick(e) {
        let streetId = parseInt(this.data.streetCascaderValue);

        const that = this;
        app.post("/client/customer/send/address/save", {
            id: that.data.id,
            name: that.data.name,
            mobile: that.data.mobile,
            streetId: parseInt(that.data.streetCascaderValue),
            address: that.data.address,
            zipcode: that.data.zipcode,
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            setTimeout(function () {
                const pages = getCurrentPages();
                wx.navigateBack({
                    complete: function (e) {
                        if (pages.length >= 2) {
                            const prevPage = pages[pages.length - 2];
                            if (prevPage.loadSendAddressList) {
                                prevPage.loadSendAddressList();
                            }
                        }
                    }
                });
            }, 1000);
        });
    },

    onCancelClick(e) {
        wx.navigateBack();
    }
})