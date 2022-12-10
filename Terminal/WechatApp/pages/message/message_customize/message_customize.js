// pages/message/message_customize/message_customize.js
const app = getApp();

Page({

    /**
     * 页面的初始数据
     */
    data: {
        messageCustomizeList: [],
        messageCustomizeCount: 0,
    },

    /**
     * 生命周期函数--监听页面加载
     */
    onLoad(options) {
        this.loadMessageCustomizeList();
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

    onSwitchChange(e) {
        const messageCustomize = e.currentTarget.dataset.item;
        const typeId = messageCustomize.typeId;
        const send = e.detail;

        // 这个设置让点击后马上就能看到视觉效果
        const messageCustomizeList = this.data.messageCustomizeList;

        for (const messageCustomize of messageCustomizeList) {
            if (messageCustomize.typeId === typeId) {
                messageCustomize.send = send;
            }
        }

        this.setData({
            messageCustomizeList: messageCustomizeList,
        });

        const that = this;
        app.post("/client/customer/message/customize/save", {
            typeId: typeId,
            send: send
        }, function (res) {
            // 不提示，体验更好
            // wx.showToast({
            //     title: res.message,
            // });

            // 这是界面显示和数据库设置强制同步，同时也更新了 send hint，前面客户端设置时只设置了send，没有设置 send hint
            that.setData({
                messageCustomizeList: res.data.messageCustomizeList,
                messageCustomizeCount: res.data.messageCustomizeCount,
            });
        });
    },

    loadMessageCustomizeList() {
        let that = this;

        app.post("/client/customer/message/customize/list", {
            trackingNoQuery: that.data.trackingNoQuery,
        }, function (res) {
            const messageCustomizeCount = res.data.messageCustomizeCount;
            const messageCustomizeList = res.data.messageCustomizeList;

            that.setData({
                messageCustomizeList: messageCustomizeList,
                messageCustomizeCount: messageCustomizeCount,
            });
        });
    },

    saveMessageCustomize(e) {
    },
})