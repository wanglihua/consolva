// pages/express/express_expect/express_expect_list/express_expect_list.js
const app = getApp();

Page({

    data: {
        trackingNoQuery: "",
        expressExpectList: [],
        expressExpectCount: 0,
        currentExpressExpect: {},
        showActionSheet: false,
        actionSheetActions: [],
    },

    onLoad(options) {
        this.loadExpressExpectList();
    },

    loadExpressExpectList() {
        let that = this;

        app.post("/client/express/expect/list", {
            trackingNoQuery: that.data.trackingNoQuery,
        }, function (res) {

            let expressExpectCount = res.data.expressExpectCount;
            let expressExpectList = res.data.expressExpectList;

            that.fillListData(expressExpectList);

            that.setData({
                expressExpectList: expressExpectList,
                expressExpectCount: expressExpectCount,
            });
        });
    },

    fillListData(expressExpectList) {
        for (const expressExpect of expressExpectList) {
            let headerClassName = "item-header";
            let actions = "编辑，删除";
            if (expressExpect.enter === true) {
                headerClassName = "item-header-alt";
                actions = actions + "，录货值";
            }

            expressExpect.headerClassName = headerClassName;
            expressExpect.actions = actions;
        }
    },

    onAddExpressExpectClick() {
        wx.navigateTo({
            url: './express_expect_add',
        })
    },
    onSearch(e) {
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

        const actionSheetActions = [];

        actionSheetActions.push({
            action: "edit",
            name: '编辑',
            className: 'iconfont iconfont-edit',
        });

        actionSheetActions.push({
            action: "delete",
            name: '删除',
            className: 'iconfont iconfont-delete',
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
            className: 'iconfont iconfont-claim-pick',
            disabled: enterPriceDisabled,
        });

        this.setData({
            currentExpressExpect: currentExpressExpect,
            actionSheetActions: actionSheetActions,
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
        const expressId = this.data.currentExpressExpect.expressId;
        const trackingNo = this.data.currentExpressExpect.trackingNo;
        const trackingNoQuery = this.data.trackingNoQuery;
        const action = e.detail.action;

        const that = this;

        if (action === "edit") {
            wx.navigateTo({
                url: './express_expect_edit?id=' + expressExpectId,
            });
        }

        if (action === "delete") {
            wx.showModal({
                title: '删除确认',
                content: '确实要删除 ' + trackingNo + ' 预报吗？',
                success(res) {
                    if (res.confirm) {
                        app.post("/client/express/expect/delete", {
                            id: expressExpectId,
                            trackingNoQuery: trackingNoQuery,
                        }, function (res) {
                            wx.showToast({
                                title: res.message,
                            });

                            // 立即设置列表数据
                            const expressExpectList = res.data.expressExpectList;
                            const expressExpectCount = res.data.expressExpectCount;

                            that.fillListData(expressExpectList);

                            that.setData({
                                expressExpectList: expressExpectList,
                                expressExpectCount: expressExpectCount,
                            });
                        });
                    }
                }
            });
        }

        if (action === "enterPrice") {
            wx.navigateTo({
                url: '../express_checkin/express_checkin_price?id=' + expressId,
            });            
        }

        this.setData({
            showActionSheet: false
        });
    }
})