const app = getApp();

Page({
    data: {
        messageCustomizeList: [],
        messageCustomizeCount: 0,
    },

    onLoad(options) {
        this.loadMessageCustomizeList();
    },

    onSwitchChange(e) {
        const messageCustomize = e.currentTarget.dataset.item;
        const typeId = messageCustomize.typeId;
        const send = e.detail;

        // 这个设置让点击后马上就能看到视觉效果
        const messageCustomizeList = this.data.messageCustomizeList;

        for (const messageCustomize of messageCustomizeList) {
            if (messageCustomize.typeId === typeId) {
                messageCustomize.send = send;
            }
        }

        this.setData({
            messageCustomizeList: messageCustomizeList,
        });

        const that = this;
        app.post("/client/customer/message/customize/save", {
            typeId: typeId,
            send: send
        }, function (res) {
            // 不提示，体验更好
            // wx.showToast({
            //     title: res.message,
            // });

            // 这是界面显示和数据库设置强制同步，同时也更新了 send hint，前面客户端设置时只设置了send，没有设置 send hint
            that.setData({
                messageCustomizeList: res.data.messageCustomizeList,
                messageCustomizeCount: res.data.messageCustomizeCount,
            });
        });
    },

    loadMessageCustomizeList() {
        let that = this;

        app.post("/client/customer/message/customize/list", {
            trackingNoQuery: that.data.trackingNoQuery,
        }, function (res) {
            const messageCustomizeCount = res.data.messageCustomizeCount;
            const messageCustomizeList = res.data.messageCustomizeList;

            that.setData({
                messageCustomizeList: messageCustomizeList,
                messageCustomizeCount: messageCustomizeCount,
            });
        });
    },

    saveMessageCustomize(e) {
    },
})