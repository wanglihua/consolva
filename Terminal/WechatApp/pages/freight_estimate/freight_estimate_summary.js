// pages/freight_estimate/freight_estimate_summary.js
const app = getApp();

Page({
    data: {
        goodList: [],
        goodsType: "",
        goodsValue: "",
        totalPackageCount: 0,
        addressReceive: {},
        firstMode: {},
        feeList: [],
        gstFee: "",
    },

    onLoad(options) {
        this.loadSummary();
    },

    loadSummary() {
        const that = this;
        app.post("/client/freight/estimate/summary", {}, function (res) {
            that.setData({
                goodsList: res.data.goodsList,
                goodsType: res.data.goodsType,
                goodsValue: res.data.goodsValue,
                totalPackageCount: res.data.totalPackageCount,
                addressReceive: res.data.addressReceive,
                firstMode: res.data.firstMode,
                feeList: res.data.feeList,
                gstFee: res.data.gstFee,
            });
        });
    },


    onPreButtonClick(e) {
        wx.redirectTo({
            url: "./freight_estimate_receive_address",
        });
    },

    onNextButtonClick(e) {
        wx.redirectTo({
            url: "./freight_estimate_goods",
        });
    }
})