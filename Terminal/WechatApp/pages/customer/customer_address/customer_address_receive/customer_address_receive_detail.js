// pages/customer/customer_address/customer_address_receive/customer_address_receive_detail.js
const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        countryOptions: [
            {
                text: '澳大利亚',
                value: '2000',
            },
            {
                text: '美国',
                value: '3000',
            },
        ],
        showCountrySelect: false,
        countryFieldValue: '美国',
        countryCascaderValue: '3000',

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
            that.setData({
                id: res.data.id,
                countryId: res.data.countryId,
                firstName: res.data.firstName,
                lastName: res.data.lastName,
                mobile: res.data.mobile,
                address: res.data.address,
                city: res.data.city,
                state: res.data.state,
                zipcode: res.data.zipcode,
                tfn: res.data.tfn,
                company: res.data.company,
                unitLoc: res.data.unitLoc,
            });
        });
    },

    onCountryFieldTap(e) {
        // console.log(e);

        this.setData({
            showCountrySelect: true,
        });
    },

    onCountrySelectClose(e) {
        // console.log(e);
        this.setData({
            showCountrySelect: false,
        });
    },

    onCountrySelectFinish(e) {
        console.log(e);
        const {selectedOptions, value} = e.detail;
        const fieldValue = selectedOptions
            .map((option) => option.text)
            .join('/');
        this.setData({
            countryFieldValue: fieldValue,
            countryCascaderValue: value,
            showCountrySelect: false,
        })
    },

    onSaveClick(e) {
        // console.log(e);

        // todo validate


        const that = this;
        app.post("/client/customer/receive/address/save", {
            id: that.data.id,
            countryId: that.data.countryId,
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
                wx.navigateBack({
                    complete: function (e) {
                        const pages = getCurrentPages();
                        const prevPage = pages[pages.length - 1];
                        if (prevPage.loadReceiveAddressList) {
                            prevPage.loadReceiveAddressList();
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