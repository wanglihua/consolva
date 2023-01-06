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
        
    }
})