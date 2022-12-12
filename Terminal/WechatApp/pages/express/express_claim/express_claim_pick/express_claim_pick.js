// pages/express/express_claim/express_claim_pick/express_claim_pick.js
const app = getApp();

Page({
    data: {
        express: {
            trackingNoInput: '',
            trackingNoInputHasError: false,
            trackingNoInputErrorMsg: "",

            expressGoodsList: [],
            expressSizeWeightList: [],
        },
        btnClaimDisabled: false,
        btnClaimText: "认领",
    },

    onLoad(options) {
        const id = options.id;
        const that = this;
        app.post("/client/express/claim/detail", {
            id: id
        }, function (res) {
            let express = res.data;
            express.trackingNoInput = '';
            if (express.expectTrackingNo) {
                express.trackingNoInput = express.expectTrackingNo;
            }

            express.trackingNoInputHasError = false;
            express.trackingNoInputErrorMsg = "";

            const expressGoodsList = express.expressGoodsList;
            for (const expressGoods of expressGoodsList) {
                expressGoods.inputHasError = false;
                expressGoods.inputErrorMsg = "";
            }

            express.expressGoodsList = expressGoodsList;

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

    onExpressGoodsPriceInput(e) {
        const expressGoodsId = e.currentTarget.dataset.item.id;
        const price = e.detail;

        const express = this.data.express;
        const expressGoodsList = express.expressGoodsList; // 这里的列表是引用了原来的
        for (const expressGoods of expressGoodsList) { // 这里的每个元素都是引用了原来的实例
            if (expressGoods.id === expressGoodsId) {
                expressGoods.price = price;
            }
        }

        this.setData({
            express: express,
        });
    },

    onClaimClick() {
        // 下面三个都输出 true
        // console.log(!(null));
        // console.log(!(undefined));
        // console.log(!(""));

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

        express.trackingNoInputHasError = false;
        express.trackingNoInputErrorMsg = "";
        this.setData({
            express: express,
        });

        let hasPriceInputError = false;
        const expressGoodsList = express.expressGoodsList;
        for (const expressGoods of expressGoodsList) {
            const priceNum = parseFloat(expressGoods.price);
            if (isNaN(priceNum) || priceNum <= 0) {
                hasPriceInputError = true;
                expressGoods.inputHasError = true;
                expressGoods.inputErrorMsg = "请输入正确的物品价格！";
            } else {
                expressGoods.inputHasError = false;
                expressGoods.inputErrorMsg = "";
            }
        }

        express.expressGoodsList = expressGoodsList;

        this.setData({
            express: express,
        });

        if (hasPriceInputError) {
            return;
        }


    },

    onBackClick() {
        wx.navigateBack();
    }
})