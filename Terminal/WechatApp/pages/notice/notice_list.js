// pages/notice/notice_list.js
const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        lastId: 0,
        noticeList: [],
    },

    onLoad(options) {
        this.loadNoticeList();
    },

    onReachBottom() {
        this.loadNoticeList();
    },

    loadNoticeList() {
        const that = this;
        app.post("/client/notice/list", {
            lastId: that.data.lastId,
        }, function (res) {
            const lastId = res.data.lastId;
            const noticeList = res.data.noticeList;

            that.setData({
                noticeList: util.distinctConcat(that.data.noticeList, noticeList),
                lastId: lastId,
            });
        });
    }    
})