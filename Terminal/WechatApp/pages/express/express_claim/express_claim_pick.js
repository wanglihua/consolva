// pages/express/express_claim/express_claim_pick/express_claim_pick.js
const app = getApp();

Page({
    data: {
        express: {
            trackingNoInput: '',
            goodsValueInput: '',
            trackingNoInputHasError: false,
            trackingNoInputErrorMsg: "",
            goodsValueInputHasError: false,
            goodsValueInputErrorMsg: "",

            expressGoodsList: [],
            expressSizeWeightList: [],
        },
        pageTitle: "申领件认领",
        btnClaimDisabled: false,
        btnClaimText: "认领",
    },

    onLoad(options) {
        const id = options.id;
        const that = this;
        app.post("/client/express/claim/detail", {
            id: id
        }, function (res) {
            const express = res.data;
            express.trackingNoInput = '';
            if (express.expectTrackingNo) {
                express.trackingNoInput = express.expectTrackingNo;
            }

            express.trackingNoInputHasError = false;
            express.trackingNoInputErrorMsg = "";
            express.goodsValueInputHasError = false;
            express.goodsValueInputErrorMsg = "";

            that.setData({
                express: express,
            });

            if (express.claimed) {
                that.setData({
                    btnClaimDisabled: true,
                    btnClaimText: "已认领",
                });
            }
        });
    },

    onTackingNoInput(e) {
        const express = this.data.express;
        express.trackingNoInput = e.detail;

        this.setData({
            express: express,
        });
    },

    onGoodsValueInput(e) {
        const express = this.data.express;
        express.goodsValueInput = e.detail;

        this.setData({
            express: express,
        });
    },

    onClaimClick() {
        const express = this.data.express;

        const trackingNoInput = this.data.express.trackingNoInput;
        if (!trackingNoInput) {
            express.trackingNoInputHasError = true;
            express.trackingNoInputErrorMsg = "单号补全不能为空！";
            this.setData({
                express: express,
            });
            return;
        }

        const goodsValueInput = this.data.express.goodsValueInput;
        if (!goodsValueInput) {
            express.goodsValueInputHasError = true;
            express.goodsValueInputErrorMsg = "货品总值不能为空！";
            this.setData({
                express: express,
            });
            return;
        }

        express.trackingNoInputHasError = false;
        express.trackingNoInputErrorMsg = "";
        express.goodsValueInputHasError = false;
        express.goodsValueInputErrorMsg = "";
        this.setData({
            express: express,
        });

        app.post("/client/express/claim/pick", {
            expressId: express.id,
            trackingNoInput: trackingNoInput,
            goodsValueInput: goodsValueInput,
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            setTimeout(function () {
                if (express.enter) {
                    const pages = getCurrentPages();
                    wx.navigateBack({
                        complete: function (e) {
                            if (pages.length >= 2) {
                                const prevPage = pages[pages.length - 2];
                                if (prevPage.loadExpressExpectList) {
                                    prevPage.loadExpressExpectList();
                                }
                            }
                        }
                    });
                } else {
                    wx.navigateTo({
                        url: './express_claim_list',
                    });
                }
            }, 1000);
        });
    },

    onBackClick() {
        wx.navigateBack();
    }
})