// pages/express/express_claim/express_claim_detail/express_claim_detail.js

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
        wx.navigateTo({
            url: './express_claim_pick?id=' + this.data.express.id,
        });        
    },

    onBackClick() {
        wx.navigateBack();
    }
})