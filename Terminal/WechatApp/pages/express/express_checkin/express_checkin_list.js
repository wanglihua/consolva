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

        if (currentExpress.forbidden === false) {
            if (currentExpress.waitOrder === true) {
                actionSheetActions.push({
                    action: "removeFromOrder",
                    name: '暂不转运',
                    subname: '从待转运列表移除快递单',
                    className: 'iconfont iconfont-remove-from-order',
                });

                const waitOrderCount = this.data.expressWaitOrderCount;
                if (waitOrderCount >= 2) {
                    actionSheetActions.push({
                        action: "removeFromOrderAll",
                        name: '全部暂不转运',
                        subname: '从待转运列表移除全部快递单',
                        className: 'iconfont iconfont-remove-from-order',
                    });
                }
            } else {
                actionSheetActions.push({
                    action: "addToOrder",
                    name: '我要转运',
                    subname: '添加快递单到待转运列表',
                    className: 'iconfont iconfont-add-to-order',
                });

                const checkinCount = this.data.expressCheckinCount;
                if (checkinCount >= 2) {
                    actionSheetActions.push({
                        action: "addToOrderAll",
                        name: '我要全部转运',
                        subname: '添加全部快递单到待转运列表',
                        className: 'iconfont iconfont-add-to-order',
                    });
                }
            }
        }

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

        if (currentExpress.waitOrder === true) {
            actionSheetActions.push({
                action: "applyReturn",
                name: '申请退运',
                subname: '待转运不能申请退运',
                className: 'iconfont iconfont-express-return',
                disabled: true,
            });
        } else {
            actionSheetActions.push({
                action: "applyReturn",
                name: '申请退运',
                className: 'iconfont iconfont-express-return',
            });
        }

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