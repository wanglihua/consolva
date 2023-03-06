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

            const noticeCount = noticeList.length;
            for (let index = 0; index < noticeCount; index++) {
                let notice = noticeList[index];
                if (notice.listTop === true) {
                    notice.top = "【置顶】";
                }

                notice.titleClass = "item-notice-title";
                if (notice.important === true) {
                    notice.titleClass = "item-notice-title-important";
                }
            }

            that.setData({
                noticeList: util.distinctConcat(that.data.noticeList, noticeList),
                lastId: lastId,
            });
        });
    }
})