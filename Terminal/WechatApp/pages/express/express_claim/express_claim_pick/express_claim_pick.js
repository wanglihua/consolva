// pages/express/express_claim/express_claim_pick/express_claim_pick.js
const app = getApp();

Page({
    data: {
        express: {
            expressGoodsList: [],
            expressSizeWeightList: [],
        }
    },

    onLoad(options) {
        const id = options.id;
        const that = this;
        app.post("/client/express/claim/detail", {
            id: id
        }, function (res) {
            that.setData({
                express: res.data,
            });
        });
    },

    onClaimClick() {
       
    },

    onBackClick() {
        wx.navigateBack();
    }
})