// pages/order_create/order_create_country.js
const app = getApp();

Page({
    data: {
        countryList: [],
    },

    onLoad(options) {
        this.loadCountryList();
    },

    loadCountryList() {
        const that = this;
        app.post("/client/order/create/country/list", {}, function (res) {
            that.setData({
                countryList: res.data.countryList,
            });
        });
    }
})