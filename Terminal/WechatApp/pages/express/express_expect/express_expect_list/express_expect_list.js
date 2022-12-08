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
        currentExpressExpect: {},
        showActionSheet: false,
        actionSheetActions: [],
    },

    /**
     * 生命周期函数--监听页面加载
     */
    onLoad(options) {
        // this.loadExpressExpectList();
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
        this.loadExpressExpectList();
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
                let actions = "编辑，删除";
                if (expressExpect.enter === true) {
                    headerClassName = "express-expect-header-enter";
                    actions = actions + "，录货值";
                }

                expressExpect.headerClassName = headerClassName;
                expressExpect.actions = actions;
            }

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
    },

    onSearch(e) {
        this.setData({
            trackingNoQuery: e.detail,
        });
        this.loadExpressExpectList();
    },
    onSearchChange(e) {
        this.setData({
            trackingNoQuery: e.detail,
        });
        this.loadExpressExpectList();
    },
    onSearchClear() {
        this.setData({
            trackingNoQuery: "",
        });
        this.loadExpressExpectList();
    },
    onActionCellClick(e) {
        const currentExpressExpect = e.currentTarget.dataset.item;
        this.setData({
            currentExpressExpect: currentExpressExpect
        });

        const actionSheetActions = [];

        actionSheetActions.push({
            action: "edit",
            name: '编辑',
        });

        actionSheetActions.push({
            action: "delete",
            name: '删除',
            // color: "red",
        });

        let enterPriceDisabled = true;
        if (currentExpressExpect.enter === true) {
            enterPriceDisabled = false;
        }
        actionSheetActions.push({
            action: "enterPrice",
            name: '录货值',
            subname: '录入货品价格，完成最终入库',
            disabled: enterPriceDisabled,
        });

        this.setData({
            actionSheetActions: actionSheetActions
        });

        this.setData({
            showActionSheet: true
        });
    },
    onActionSheetCloseClick(e) {
        this.setData({
            showActionSheet: false
        });
    },
    onActionSheetSelect(e) {
        const expressExpectId = this.data.currentExpressExpect.id;
        const trackingNo = this.data.currentExpressExpect.trackingNo;
        const action = e.detail.action;

        const that = this;

        if (action === "edit") {
            wx.navigateTo({
                url: '../express_expect_edit/express_expect_edit?id=' + expressExpectId,
            });
        }

        if (action === "delete") {
            wx.showModal({
                title: '删除确认',
                content: '确实要删除 ' + trackingNo + ' 预报吗？',
                success(res) {
                    if (res.confirm) {
                        app.post("/client/express/expect/delete", {
                            id: expressExpectId
                        }, function (res) {
                            wx.showToast({
                                title: res.message,
                            });

                            that.setData({
                                showActionSheet: false
                            });

                            that.loadExpressExpectList();
                        });
                    }

                    if (res.cancel) {
                        that.setData({
                            showActionSheet: false
                        });
                    }
                }
            });
        }

        if (action === "enterPrice") {
            // todo 待快递单号那里完成后，这里补上
        }

        this.setData({
            showActionSheet: false
        });
    }
})