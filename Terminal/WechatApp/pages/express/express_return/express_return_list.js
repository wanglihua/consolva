const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        query: "",
        lastId: 0,

        expressReturnList: [],
        expressReturnCount: 0,

        currentExpressReturn: {},
        showActionSheet: false,
        actionSheetActions: [],
    },

    onLoad(options) {
        this.loadExpressReturnList();
    },

    onReachBottom() {
        this.loadExpressReturnList();
    },

    loadExpressReturnList() {
        const that = this;

        app.post("/client/express/return/list", {
            query: that.data.query,
            lastId: that.data.lastId,
        }, function (res) {
            const expressReturnListAdd = res.data.expressReturnList;

            for (const expressReturn of expressReturnListAdd) {
                if (!expressReturn.returnTrackingNo || expressReturn.returnTrackingNo === '') {
                    expressReturn.returnTrackingNo = '无';
                }

                if (!expressReturn.returnExpressCompany || expressReturn.returnExpressCompany === '') {
                    expressReturn.returnExpressCompany = '未知';
                }

                if (!expressReturn.returnPickupCode || expressReturn.returnPickupCode === '') {
                    expressReturn.returnPickupCode = '点击更新取件码';
                }

                expressReturn.actions = "查看详细，更新取货码";
            }

            const expressReturnCount = res.data.expressReturnCount;
            const expressReturnList = util.distinctConcat(that.data.expressReturnList, expressReturnListAdd);
            const lastId = res.data.lastId;

            that.setData({
                expressReturnList: expressReturnList,
                expressReturnCount: expressReturnCount,
                lastId: lastId,
            });
        });
    },

    onSearch(e) {
        this.setData({
            query: e.detail,
            lastId: 0,
        });
        this.loadExpressReturnList();
    },
    onSearchClear() {
        this.setData({
            query: "",
            lastId: 0,
        });
        this.loadExpressReturnList();
    },
    onActionCellClick(e) {
        const currentExpressReturn = e.currentTarget.dataset.item;

        const actionSheetActions = [];

        actionSheetActions.push({
            action: "detail",
            name: '查看详细',
            className: 'iconfont iconfont-view-detail',
        });

        this.setData({
            currentExpressReturn: currentExpressReturn,
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
        const expressReturnId = this.data.currentExpressReturn.id;
        const action = e.detail.action;

        const that = this;
        if (action === "detail") {
            wx.navigateTo({
                url: './express_return_detail?id=' + expressReturnId,
            });
        }

        this.setData({
            showActionSheet: false,
        });
    }
})