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
            that.setData({
                express: res.data,
            });
        });
    },

    onExpressGoodsPriceInput(e) {
        console.log(e);
    },

    onSavePriceClick(e) {
        const express = this.data.express;

        let goodsPriceList = [];

        if (express.goodsList && express.goodsList.length !== 0) {
            for (const goods of express.goodsList) {
                goodsPriceList.push({
                    id: goods.id,
                    price: goods.price,
                });
            }
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