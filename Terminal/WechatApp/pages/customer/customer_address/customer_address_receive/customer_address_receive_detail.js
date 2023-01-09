// pages/customer/customer_address/customer_address_receive/customer_address_receive_detail.js
const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        countryOptions: [],
        showCountrySelect: false,
        countryFieldValue: '',
        countryCascaderValue: '',

        id: 0,
        countryId: 0,
        firstName: '',
        lastName: '',
        mobile: '',
        address: '',
        city: '',
        state: '',
        zipcode: '',
        tfn: '',
        company: '',
        unitLoc: '',
    },

    onLoad(options) {
        const id = options.id;
        const that = this;
        app.post("/client/customer/receive/address/detail", {
            id: id,
        }, function (res) {
            const data = res.data;
            const countryId = data.countryId;

            let countryOptions = [];
            let countryFieldValue = '';
            let countryCascaderValue = '';
            for (const country of data.countryList) {
                countryOptions.push({
                    value: country.id.toString(),
                    text: country.name,
                });

                if (country.id === countryId) {
                    countryFieldValue = country.name;
                    countryCascaderValue = country.id.toString();
                }
            }

            that.setData({
                id: data.id,
                countryId: data.countryId,
                firstName: data.firstName,
                lastName: data.lastName,
                mobile: data.mobile,
                address: data.address,
                city: data.city,
                state: data.state,
                zipcode: data.zipcode,
                tfn: data.tfn,
                company: data.company,
                unitLoc: data.unitLoc,

                countryFieldValue: countryFieldValue,
                countryCascaderValue: countryCascaderValue,
                countryOptions: countryOptions,
            });
        });
    },

    onCountryFieldTap(e) {
        this.setData({
            showCountrySelect: true,
        });
    },

    onCountrySelectClose(e) {
        this.setData({
            showCountrySelect: false,
        });
    },

    onCountrySelectFinish(e) {
        const {selectedOptions, value} = e.detail;
        const fieldValue = selectedOptions
            .map((option) => option.text)
            .join('/');
        this.setData({
            countryFieldValue: fieldValue,
            countryCascaderValue: value,
            showCountrySelect: false,
        });
    },

    onSaveClick(e) {
        // todo validate

        let countryId = parseInt(this.data.countryCascaderValue);

        const that = this;
        app.post("/client/customer/receive/address/save", {
            id: that.data.id,
            countryId: countryId,
            firstName: that.data.firstName,
            lastName: that.data.lastName,
            mobile: that.data.mobile,
            address: that.data.address,
            city: that.data.city,
            state: that.data.state,
            zipcode: that.data.zipcode,
            tfn: that.data.tfn,
            company: that.data.company,
            unitLoc: that.data.unitLoc,
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
                            if (prevPage.loadReceiveAddressList) {
                                prevPage.loadReceiveAddressList();
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