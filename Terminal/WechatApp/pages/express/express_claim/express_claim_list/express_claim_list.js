// pages/express/express_claim/express_claim_list/express_claim_list.js
Page({

    /**
     * 页面的初始数据
     */
    data: {
        query: "",
        lastExpressId: 0,
        expressClaimList: [],
        expressClaimCount: 0,
        currentExpressClaim: {},
        showActionSheet: false,
        actionSheetActions: [],
    },

    /**
     * 生命周期函数--监听页面加载
     */
    onLoad(options) {

    },

    /**
     * 生命周期函数--监听页面初次渲染完成
     */
    onReady() {

    },

    /**
     * 生命周期函数--监听页面显示
     */
    onShow() {

    },

    /**
     * 生命周期函数--监听页面隐藏
     */
    onHide() {

    },

    /**
     * 生命周期函数--监听页面卸载
     */
    onUnload() {

    },

    /**
     * 页面相关事件处理函数--监听用户下拉动作
     */
    onPullDownRefresh() {

    },

    /**
     * 页面上拉触底事件的处理函数
     */
    onReachBottom() {

    },

    /**
     * 用户点击右上角分享
     */
    onShareAppMessage() {

    },

    loadExpressClaimList() {
        let that = this;

        app.post("/client/express/claim/list", {
            query: that.data.query,
            lastExpressId: that.data.lastExpressId
        }, function (res) {

            let expressClaimCount = res.data.expressClaimCount;
            let expressClaimList = res.data.expressClaimList;
            let lastExpressId = res.data.lastExpressId;

            for (const expressClaim of expressClaimList) {
                expressClaim.actions = "查看详细，认领";
            }

            that.setData({
                expressClaimList: expressClaimList,
                expressClaimCount: expressClaimCount,
                lastExpressId: lastExpressId,
            });
        });
    },

    onSearch(e) {
        this.setData({
            query: e.detail,
        });
        this.loadExpressClaimList();
    },
    onSearchClear() {
        this.setData({
            query: "",
        });
        this.loadExpressClaimList();
    },
})