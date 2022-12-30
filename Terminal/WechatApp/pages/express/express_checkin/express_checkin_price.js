// pages/express/express_checkin/express_checkin_price.js
const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        express: {},
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

    onExpressGoodsPriceInput(e) {
        const expressGoodsId = e.currentTarget.dataset.item.id;
        const price = e.detail;

        const express = this.data.express;
        const expressGoodsList = express.goodsList;
        for (const expressGoods of expressGoodsList) {
            if (expressGoods.id === expressGoodsId) {
                expressGoods.price = price;

                const priceNum = parseFloat(price);
                if (priceNum && priceNum > 0) {
                    expressGoods.inputHasError = false;
                    expressGoods.inputErrorMsg = '';
                } else {
                    expressGoods.inputHasError = true;
                    expressGoods.inputErrorMsg = '价格录入有误！';
                }
            }
        }

        this.setData({
            express: express,
        });
    },

    onSavePriceClick(e) {
        const express = this.data.express;

        let goodsPriceList = [];

        if (!express.goodsList || express.goodsList.length === 0) {
            wx.showModal({
                title: '错误',
                content: "未找到物品价格信息！",
                showCancel: false
            });
            return;
        }

        for (const goods of express.goodsList) {
            if (goods.inputHasError === true) {
                wx.showModal({
                    title: '错误',
                    content: "请先纠正价格录入错误！",
                    showCancel: false
                });
                return;
            }

            goodsPriceList.push({
                id: goods.id,
                price: goods.price,
            });
        }

        app.post("/client/express/checkin/price/save", {
            expressId: express.id,
            goodsPriceList: goodsPriceList,
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