// pages/express/express_expect/express_expect_list/express_expect_list.js
const app = getApp();

Page({

    /**
     * 页面的初始数据
     */
    data: {
        trackingNoQuery: "",
        expressExpectList: [],
        expressExpectCount: 0,
    },

    /**
     * 生命周期函数--监听页面加载
     */
    onLoad(options) {
        this.loadExpressExpectList();
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

    loadExpressExpectList() {
        let that = this;

        app.post("/client/express/expect/list", {
            trackingNoQuery: this.data.trackingNoQuery,
        }, function (res) {

            let expressExpectCount = res.data.expressExpectCount;
            let expressExpectList = res.data.expressExpectList;

            for (const expressExpect of expressExpectList) {
                let headerClassName = "express-expect-header";
                if (expressExpect.enter === true) {
                    headerClassName = "express-expect-header-enter";
                }

                expressExpect.headerClassName = headerClassName;
            }

            console.log(expressExpectList);

            that.setData({
                expressExpectList: expressExpectList,
                expressExpectCount: expressExpectCount,
            });
        });
    },

    onAddExpressExpectClick() {
        wx.navigateTo({
            url: '../express_expect_add/express_expect_add',
        })
    }
})