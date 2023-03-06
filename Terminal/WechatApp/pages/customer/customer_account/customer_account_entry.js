const app = getApp();
const util = app.require('utils/util.js');

Page({

    data: {
        lastId: 0,
        entryCount: 0,
        entryList: [],
    },

    onLoad(options) {
        this.loadAccountEntryList();
    },

    onReachBottom() {
        this.loadAccountEntryList();
    },

    loadAccountEntryList() {
        const that = this;
        app.post("/client/customer/account/entry/list", {
            lastId: that.data.lastId,
        }, function (res) {
            const entryCount = res.data.entryCount;
            const entryList = util.distinctConcat(that.data.entryList, res.data.entryList);
            const lastId = res.data.lastId;

            that.setData({
                entryCount: entryCount,
                entryList: entryList,
                lastId: lastId,
            });
        });
    }
})