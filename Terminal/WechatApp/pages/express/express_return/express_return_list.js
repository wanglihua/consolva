// pages/express/express_return/express_return_list/express_return_list.js
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
    }
})