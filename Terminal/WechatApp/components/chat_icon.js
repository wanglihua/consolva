const app = getApp();
const util = app.require('utils/util.js');

Component({
    options: {
        styleIsolation: 'isolated'
    },

    properties: {},

    data: {
        chatIconMessageCount: 0,
        showChatIcon: true,
        showChatForm: false,
    },

    pageLifetimes: {
        show: function () {
        }
    },

    methods: {
        /*
        onChatIconClick(e) {
            // let chatIconMessageCount = this.data.chatIconMessageCount;
            // chatIconMessageCount = chatIconMessageCount + 1;
            // this.setData({
            //     chatIconMessageCount: chatIconMessageCount,
            // });

            this.setData({
                showChatIcon: false,
                showChatForm: true,
            });
        },
        */
        onChatIconClick(e) {
            app.post("/client/customer/service/detail", {}, function (res) {
                wx.navigateTo({
                    url: "/pages/home/home_web_view?url=" + res.data.url + "&title=" + res.data.title,
                });
            });
        },
        onChatFormClose(e) {
            this.setData({
                showChatIcon: true,
                showChatForm: false,
            });
        },
    }
})
