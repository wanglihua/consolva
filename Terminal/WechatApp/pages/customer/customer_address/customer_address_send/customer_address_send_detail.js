// pages/customer/customer_address/customer_address_send/customer_address_send_detail.js
const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        provinceOptions: [],
        showStreetSelect: false,
        streetFieldValue: '',
        streetCascaderValue: '',

        id: 0,
    },

    onLoad(options) {
        const id = options.id;
        const that = this;
        app.post("/client/customer/send/address/detail", {
            id: id,
        }, function (res) {
            const data = res.data;
            // const streetId = data.streetId;
            //
            // let streetOptions = [];
            // let streetFieldValue = '';
            // let streetCascaderValue = '';
            // for (const street of data.streetList) {
            //     streetOptions.push({
            //         value: street.id.toString(),
            //         text: street.name,
            //     });
            //
            //     if (street.id === streetId) {
            //         streetFieldValue = street.name;
            //         streetCascaderValue = street.id;
            //     }
            // }

            that.setData({
                id: data.id,

                // streetFieldValue: streetFieldValue,
                // streetCascaderValue: streetCascaderValue,
                // streetOptions: streetOptions,
                provinceOptions: data.provinceList,
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
        console.log(e);

        const value = e.detail.value;
        const tabIndex = e.detail.tabIndex;

        const that = this;
        if (tabIndex < 3) {
            app.post("/client/customer/send/address/detail/region/list", {
                regionId: parseInt(value),
                tabIndex: tabIndex,
            }, function (res) {
                console.log(res);
                that.setData({
                    provinceOptions: res.data,
                });
            });
        }
    },

    onStreetSelectFinish(e) {
        console.log(e);
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
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            setTimeout(function () {
                wx.navigateBack({
                    complete: function (e) {
                        const pages = getCurrentPages();
                        const prevPage = pages[pages.length - 1];
                        if (prevPage.loadSendAddressList) {
                            prevPage.loadSendAddressList();
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