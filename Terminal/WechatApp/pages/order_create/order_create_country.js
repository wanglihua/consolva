// pages/order_create/order_create_country.js
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
        app.post("/client/order/create/country/list", {}, function (res) {
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
        app.post("/client/order/create/country/save", {
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

    gotoNext() {
        wx.redirectTo({
            url: "./order_create_first_mode",
        });
    }
})