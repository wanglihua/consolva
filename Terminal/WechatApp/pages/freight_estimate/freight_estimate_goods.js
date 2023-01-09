// pages/freight_estimate/freight_estimate_goods.js
const app = getApp();

Page({
    data: {
        goodsTypeOptions: [],
        showGoodsTypeSelect: false,
        goodsTypeFieldValue: '',
        goodsTypeCascaderValue: '',

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
            const goodsTypeList = res.data.goodsTypeList;
            const goodsTypeId = res.data.goodsTypeId;

            let goodsTypeOptions = [];
            let goodsTypeFieldValue = '';
            let goodsTypeCascaderValue = '';
            for (const goodsType of goodsTypeList) {
                goodsTypeOptions.push({
                    value: goodsType.id.toString(),
                    text: goodsType.name,
                });

                if (goodsType.id === goodsTypeId) {
                    goodsTypeFieldValue = goodsType.name;
                    goodsTypeCascaderValue = goodsType.id.toString();
                }
            }

            that.setData({
                goodsTypeId: goodsTypeId,
                goodsValue: res.data.goodsValue,
                goodsList: res.data.goodsList,

                goodsTypeOptions: goodsTypeOptions,
                goodsTypeFieldValue: goodsTypeFieldValue,
                goodsTypeCascaderValue: goodsTypeCascaderValue,
            });
        });
    },

    onGoodsTypeFieldTap(e) {
        this.setData({
            showGoodsTypeSelect: true,
        });
    },

    onGoodsTypeSelectClose(e) {
        this.setData({
            showGoodsTypeSelect: false,
        });
    },

    onGoodsTypeSelectFinish(e) {
        const {selectedOptions, value} = e.detail;
        const fieldValue = selectedOptions
            .map((option) => option.text)
            .join('/');
        this.setData({
            goodsTypeFieldValue: fieldValue,
            goodsTypeCascaderValue: value,
            goodsTypeId: parseInt(value),
            showGoodsTypeSelect: false,
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