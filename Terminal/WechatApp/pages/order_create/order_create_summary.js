// pages/order_create/order_create_summary.js
const app = getApp();

Page({
    data: {
        expressList: [],
        goodsType: '',
        totalPackageCount: 0,
        totalGoodsValue: '',
        firstMode: {},
        receiveAddress: {},
        sendAddress: {},
        insurance: {},
        gst: {},
    },

    onLoad(options) {
        this.loadSummary();
    },

    loadSummary() {
        const that = this;
        app.post("/client/order/create/summary", {}, function (res) {
            that.setData({
                expressList: res.data.expressList,
                goodsType: res.data.goodsType,
                totalPackageCount: res.data.totalPackageCount,
                totalGoodsValue: res.data.totalGoodsValue,
                firstMode: res.data.firstMode,
                receiveAddress: res.data.receiveAddress,
                sendAddress: res.data.sendAddress,
                insurance: res.data.insurance,
                gst: res.data.gst,
            });
        })
    },

    onPreButtonClick(e) {
        this.gotoPre();
    },

    onNextButtonClick(e) {

    },

    gotoPre() {
        wx.redirectTo({
            url: "./order_create_insurance",
        });
    },

    gotoNext() {
        wx.redirectTo({
            url: "",
        });
    }
})
