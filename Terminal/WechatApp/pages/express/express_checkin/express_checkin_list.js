// pages/express/express_checkin/express_checkin_list/express_checkin_list.js
const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        query: '',

        expressWaitOrderList: [],
        expressCheckinList: [],
        expressWaitOrderCount: 0,
        expressCheckinCount: 0,
        expressTotalCount: 0,

        currentExpress: {},
        showActionSheet: false,
        actionSheetActions: [],
    },

    onLoad(options) {
        this.loadExpressCheckinList();
    },

    loadExpressCheckinList() {
        const that = this;
        app.post("/client/express/checkin/list", {
            query: that.data.query,
        }, function (res) {
            const expressWaitOrderList = res.data.expressWaitOrderList;
            const expressCheckinList = res.data.expressCheckinList;
            const expressWaitOrderCount = res.data.expressWaitOrderCount;
            const expressCheckinCount = res.data.expressCheckinCount;
            const expressTotalCount = res.data.expressTotalCount;

            for (const express of expressWaitOrderList) {
                express.actions = "转运，查看详细，修改价格等";

                if (express.forbidden === true) {
                    express.headerValueClass = 'item-header-value-forbidden';
                } else {
                    express.headerValueClass = 'item-header-value';
                }
            }

            for (const express of expressCheckinList) {
                express.actions = "转运，查看详细，修改价格等";

                if (express.forbidden === true) {
                    express.headerValueClass = 'item-header-value-forbidden';
                } else {
                    express.headerValueClass = 'item-header-value';
                }
            }

            that.setData({
                expressWaitOrderList: expressWaitOrderList,
                expressCheckinList: expressCheckinList,
                expressWaitOrderCount: expressWaitOrderCount,
                expressCheckinCount: expressCheckinCount,
                expressTotalCount: expressTotalCount,
            });

        });
    },

    onSearch(e) {
        this.setData({
            query: e.detail,
        });
        this.loadExpressCheckinList();
    },

    onSearchClear() {
        this.setData({
            query: "",
        });
        this.loadExpressCheckinList();
    },

    onActionCellClick(e) {
        const currentExpress = e.currentTarget.dataset.item;

        const actionSheetActions = [];

        actionSheetActions.push({
            action: "detail",
            name: '查看详细',
            className: 'iconfont iconfont-view-detail',
        });

        actionSheetActions.push({
            action: "editPrice",
            name: '修改价格',
            className: 'iconfont iconfont-edit',
        });

        this.setData({
            currentExpress: currentExpress,
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
        const expressId = this.data.currentExpress.id;
        const action = e.detail.action;

        const that = this;
        if (action === "detail") {
            wx.navigateTo({
                url: './express_checkin_detail?id=' + expressId,
            });
        }

        this.setData({
            showActionSheet: false,
        });
    },
})