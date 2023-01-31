// pages/freight_estimate/freight_estimate_summary.js
const app = getApp();

Page({
    data: {
        goodList: [],
        goodsType: "",
        goodsValue: "",
        totalPackageCount: 0,
        receiveAddress: {},
        firstMode: {},
        firstModeFee: "",
        firstModeFeeList: [],
        gstFee: "",
        totalFee: "",
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
                receiveAddress: res.data.receiveAddress,
                firstMode: res.data.firstMode,
                firstModeFee: res.data.firstModeFee,
                firstModeFeeList: res.data.firstModeFeeList,
                gstFee: res.data.gstFee,
                totalFee: res.data.totalFee,
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