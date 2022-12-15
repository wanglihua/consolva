// pages/customer/customer_account/customer_account_entry/customer_account_entry.js

const app = getApp();

Page({

    data: {
        lastId: 0,
        entryCount: 0,
        entryList: [],
    },

    onLoad(options) {
        const that = this;
        app.post("/client/customer/account/entry/list", {
            lastId: that.data.lastId,
        }, function (res) {
            const entryCount = res.data.entryCount;
            const entryList = res.data.entryList;
            const lastId = res.data.lastId;

            that.setData({
                entryCount: entryCount,
                entryList: entryList,
                lastId: lastId,
            });
        });
    }
})