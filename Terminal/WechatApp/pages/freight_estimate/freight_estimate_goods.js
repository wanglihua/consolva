// pages/freight_estimate/freight_estimate_goods.js
const app = getApp();

Page({
    data: {
        showGoodsTypeSelect: false,
        
        goodsTypeList: [],
        goodsTypeId: 0,
        goodsValue: '',
        goodsList: [],
    },

    onLoad(options) {
        this.loadGoodsDetail();
    },

    loadGoodsDetail() {
        const that = this;
        app.post("/client/freight/estimate/goods/detail", {}, function (res) {
            that.setData({
                goodsTypeList: res.data.goodsTypeList,
                goodsTypeId: res.data.goodsTypeId,
                goodsValue: res.data.goodsValue,
                goodsList: res.data.goodsList,
            });
        });
    },

    onGoodsValueInput(e) {
        this.setData({
            goodsValue: e.detail,
        });
    },

    onAddGoodsButtonClick(e) {

    },

    onNextButtonClick(e) {
        const that = this;
        app.post("/client/freight/estimate/goods/save", {
            goodsTypeId: that.data.goodsTypeId,
            goodsValue: that.data.goodsValue,
            goodsList: that.data.goodsList,
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            that.gotoNext();
        });
    },

    gotoNext() {
        wx.redirectTo({
            url: "./freight_estimate_country",
        });
    },
})