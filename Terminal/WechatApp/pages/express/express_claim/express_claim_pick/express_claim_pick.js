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
            this.setData({
                trackingNoInputHasError: true,
                trackingNoInputErrorMsg: "单号补全不能为空！",
            });
            return;
        }
    },

    onBackClick() {
        wx.navigateBack();
    }
})