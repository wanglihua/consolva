const app = getApp();

Page({
    data: {
        expressList: [],
        goodsType: '',
        totalPackageCount: 0,
        totalGoodsValue: '',
        firstMode: {},
        receiveAddress: {},
        // sendAddress: {},
        insurance: {},
        gst: {},
        repackType: {},
        remark: "",
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
                // sendAddress: res.data.sendAddress,
                insurance: res.data.insurance,
                gst: res.data.gst,
                repackType: res.data.repackType,
                remark: res.data.remark,
            });
        })
    },

    onPreButtonClick(e) {
        this.gotoPre();
    },

    onNextButtonClick(e) {
        this.gotoNext();
    },

    gotoPre() {
        wx.redirectTo({
            url: "./order_create_repack_type",
        });
    },

    gotoNext() {
        wx.navigateTo({
            url: "./order_create_agreement",
        });
    }
})
