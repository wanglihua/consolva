// pages/express/express_expect/express_expect_add/express_expect_add.js

const app = getApp();

Page({

    /**
     * 页面的初始数据
     */
    data: {
        trackingNo: "",
        description: "",

        trackingNoHasError: false,
        trackingNoErrorMsg: "",
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

    onSaveClick() {
        if (this.data.trackingNo === "") {
            this.setData({
                trackingNoHasError: true,
                trackingNoErrorMsg: "快递单号不能为空！",
            });
            return;
        }

        this.setData({
            trackingNoHasError: false,
            trackingNoErrorMsg: "",
        });        

        app.post("/client/express/expect/add/save", {
            trackingNo: this.data.trackingNo,
            description: this.data.description,
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            setTimeout(function () {
                wx.navigateBack();
            }, 1000);
        });
    },
    onCancelClick() {
        wx.navigateBack();
    }
})