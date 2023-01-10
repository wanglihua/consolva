// pages/freight_estimate/freight_estimate_country.js
const app = getApp();

Page({
    data: {
        countryList: [],
        selectCountryId: 0,
    },

    onLoad(options) {
        this.loadCountryList();
    },

    loadCountryList() {
        const that = this;
        app.post("/client/freight/estimate/country/list", {}, function (res) {
            that.setData({
                countryList: res.data.countryList,
                selectCountryId: res.data.selectCountryId,
            });
        });
    },

    onCountryButtonClick(e) {
        const country = e.currentTarget.dataset.item;
        const countryId = country.id;

        const that = this;
        app.post("/client/freight/estimate/country/save", {
            countryId: countryId
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            that.setData({
                selectCountryId: countryId,
            });

            that.gotoNext();
        });
    },

    onPreButtonClick(e) {
        this.gotoPre();
    },

    onNextButtonClick(e) {
        const selectCountryId = this.data.selectCountryId;
        if (selectCountryId === 0) {
            wx.showModal({
                title: '错误',
                content: "请选择目的国家！",
                showCancel: false
            });
            return;
        }

        this.gotoNext();
    },

    gotoPre() {
        wx.redirectTo({
            url: "./freight_estimate_goods",
        });
    },

    gotoNext() {
        wx.redirectTo({
            url: "./freight_estimate_first_mode",
        });
    },
})