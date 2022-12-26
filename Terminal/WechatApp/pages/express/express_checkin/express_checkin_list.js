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
})