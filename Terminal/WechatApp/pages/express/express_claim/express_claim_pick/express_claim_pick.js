// pages/express/express_claim/express_claim_pick/express_claim_pick.js
const app = getApp();

Page({
    data: {
        express: {
            expressGoodsList: [],
            expressSizeWeightList: [],
        },
        trackingNoInputHasError: false,
        trackingNoInputErrorMsg: "",
    },

    onLoad(options) {
        const id = options.id;
        const that = this;
        app.post("/client/express/claim/detail", {
            id: id
        }, function (res) {
            let express = res.data;
            express.trackingNoInput = '';
            if (express.expectTrackingNo !== '') {
                express.trackingNoInput = express.expectTrackingNo;
            }

            that.setData({
                express: express,
            });
        });
    },

    onClaimClick() {
        // 下面三个都输出 true
        // console.log(!(null));
        // console.log(!(undefined));
        // console.log(!(""));

        const trackingNoInput = this.data.express.trackingNoInput;
        if (!trackingNoInput) {
            const express = this.data.express;
            express.trackingNoInputHasError = true;
            express.trackingNoInputErrorMsg = "单号补全不能为空！";
            this.setData({
                express: express,
            });
            return;
        }

        let hasPriceInputError = false;
        const expressGoodsList = this.data.express.expressGoodsList;
        for (const expressGoods of expressGoodsList) {
            // error="{{ expressGoods.inputHasError }}" error-message="{{ expressGoods.inputErrorMsg }}" 

            const priceNum = parseFloat(expressGoods.price);
            if (isNaN(priceNum) || priceNum <= 0) {
                hasPriceInputError = true;
                expressGoods.inputHasError = true;
                expressGoods.inputErrorMsg = "请输入正确的物品价格！";
            }
        }

        if (hasPriceInputError) {
            const express = this.data.express;
            express.expressGoodsList = expressGoodsList

            this.setData({
                express: express,
            });

            return;
        }


    },

    onBackClick() {
        wx.navigateBack();
    }
})