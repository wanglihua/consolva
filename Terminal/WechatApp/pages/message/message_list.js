const app = getApp();
const util = app.require('utils/util.js');

Page({

    data: {
        lastId: 0,
        messageList: [],
        messageCount: 0,
    },

    onLoad(options) {
        this.loadMessageList();
    },

    onReachBottom() {
        this.loadMessageList();
    },

    loadMessageList() {
        const that = this;
        app.post("/client/customer/message/list", {
            lastId: that.data.lastId,
        }, function (res) {
            const lastId = res.data.lastId;
            const messageCount = res.data.messageCount;
            const messageList = res.data.messageList;

            that.setData({
                messageList: util.distinctConcat(that.data.messageList, messageList),
                messageCount: messageCount,
                lastId: lastId,
            });
        });
    }
})