// pages/express/express_claim/express_claim_list/express_claim_list.js

const app = getApp();

Page({
    data: {
        query: "",
        lastExpressId: 0,
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
            lastExpressId: 0,
        });
        this.loadExpressClaimList();
    },
    onSearchClear() {
        this.setData({
            query: "",
            lastExpressId: 0,
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

        // console.log(expressClaimId);
        // console.log(action);

        const that = this;

        if (action === "detail") {
            wx.navigateTo({
                url: '../express_claim_detail/express_claim_detail?id=' + expressClaimId,
            });
        }

        if (action === "claim") {
            wx.navigateTo({
                url: '../express_claim_pick/express_claim_pick?id=' + expressClaimId,
            });
        }

        this.setData({
            showActionSheet: false,
        });
    }
})