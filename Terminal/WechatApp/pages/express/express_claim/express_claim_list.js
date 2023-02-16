// pages/express/express_claim/express_claim_list/express_claim_list.js

const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        query: "",
        lastId: 0,
        expressClaimList: [],
        expressClaimCount: 0,
        currentExpressClaim: {},
        showActionSheet: false,
        actionSheetActions: [],
    },

    onLoad(options) {
        this.loadExpressClaimList();
    },

    onReachBottom() {
        this.loadExpressClaimList();
    },

    loadExpressClaimList() {
        const that = this;

        app.post("/client/express/claim/list", {
            query: that.data.query,
            lastId: that.data.lastId
        }, function (res) {
            const expressClaimCount = res.data.expressClaimCount;
            const expressClaimList = res.data.expressClaimList;
            const lastId = res.data.lastId;

            for (const expressClaim of expressClaimList) {
                expressClaim.actions = "查看详细，认领";
            }

            that.setData({
                expressClaimList: util.distinctConcat(that.data.expressClaimList, expressClaimList),
                expressClaimCount: expressClaimCount,
                lastId: lastId,
            });
        });
    },

    onSearch(e) {
        this.setData({
            query: e.detail,
            lastId: 0,
        });
        this.loadExpressClaimList();
    },
    onSearchClear() {
        this.setData({
            query: "",
            lastId: 0,
        });
        this.loadExpressClaimList();
    },
    onActionCellClick(e) {
        const currentExpressClaim = e.currentTarget.dataset.item;

        const actionSheetActions = [];

        actionSheetActions.push({
            action: "detail",
            name: '查看详细',
            className: 'iconfont iconfont-view-detail',
        });

        actionSheetActions.push({
            action: "claim",
            name: '认领',
            className: 'iconfont iconfont-claim-pick',
        });

        this.setData({
            currentExpressClaim: currentExpressClaim,
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
        const expressClaimId = this.data.currentExpressClaim.id;
        const action = e.detail.action;

        const that = this;
        if (action === "detail") {
            wx.navigateTo({
                url: './express_claim_detail?id=' + expressClaimId,
            });
        }

        if (action === "claim") {
            wx.navigateTo({
                url: './express_claim_pick?id=' + expressClaimId,
            });
        }

        this.setData({
            showActionSheet: false,
        });
    }
})