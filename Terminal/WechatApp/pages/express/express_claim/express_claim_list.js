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

        let lastId = this.data.lastId;
        const query = this.data.query;

        let hasQuery = false;
        if (query && query !== "" && query !== " " && query !== "  ") {
            hasQuery = true;
        }

        if (hasQuery === true) {
            lastId = 0;
        }

        app.post("/client/express/claim/list", {
            query: that.data.query,
            lastId: lastId,
        }, function (res) {
            const expressClaimCount = res.data.expressClaimCount;
            let expressClaimList = res.data.expressClaimList;
            let lastId = res.data.lastId;

            for (const expressClaim of expressClaimList) {
                // expressClaim.actions = "查看详细，认领";
                expressClaim.actions = "认领";
            }

            if (hasQuery === true) {
                lastId = 0
            } else {
                expressClaimList = util.distinctConcat(that.data.expressClaimList, expressClaimList)
            }

            that.setData({
                expressClaimList: expressClaimList,
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

        // 不查看详细了
        // actionSheetActions.push({
        //     action: "detail",
        //     name: '查看详细',
        //     className: 'iconfont iconfont-view-detail',
        // });

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
        // if (action === "detail") {
        //     wx.navigateTo({
        //         url: './express_claim_detail?id=' + expressClaimId,
        //     });
        // }

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