const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        express: {
            goodsValueInput: '',
            goodsValueInputHasError: false,
            goodsValueInputErrorMsg: "",
        },
    },

    onLoad(options) {
        const id = options.id;

        const that = this;
        app.post("/client/express/checkin/price", {
            id: id,
        }, function (res) {
            const express = res.data;
            if (express.goodsList && express.goodsList.length > 0) {
                for (const expressGoods of express.goodsList) {
                    expressGoods.inputHasError = false;
                    expressGoods.inputErrorMsg = '';
                }
            }

            that.setData({
                express: res.data,
            });
        });
    },

    onGoodsValueInput(e) {
        const express = this.data.express;
        express.goodsValueInput = e.detail;

        this.setData({
            express: express,
        });
    },

    onSavePriceClick(e) {
        const express = this.data.express;

        const goodsValueInput = this.data.express.goodsValueInput;
        if (!goodsValueInput) {
            express.goodsValueInputHasError = true;
            express.goodsValueInputErrorMsg = "货品总值不能为空！";
            this.setData({
                express: express,
            });
            return;
        }

        express.goodsValueInputHasError = false;
        express.goodsValueInputErrorMsg = "";
        this.setData({
            express: express,
        });

        app.post("/client/express/checkin/price/save", {
            expressId: express.id,
            goodsValueInput: goodsValueInput,
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            setTimeout(function () {
                const pages = getCurrentPages();
                wx.navigateBack({
                    complete: function (e) {
                        if (pages.length >= 2) {
                            const prevPage = pages[pages.length - 2];
                            if (prevPage.loadExpressCheckinList) {
                                prevPage.loadExpressCheckinList();
                            }

                            if (prevPage.loadExpressExpectList) {
                                prevPage.loadExpressExpectList();
                            }
                        }
                    }
                });
            }, 1000);
        });
    },

    onBackClick(e) {
        wx.navigateBack();
    },
})