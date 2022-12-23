// components/chat_icon.js
const app = getApp();
const util = app.require('utils/util.js');

Component({
    options: {
        styleIsolation: 'isolated'
    },

    properties: {},

    data: {
        chatIconMessageCount: 0,
    },

    pageLifetimes: {
        show: function () {
            // console.log("pageLifetimes: show()")
        }
    },

    methods: {
        onChatIconClick(e) {
            // console.log(e);

            let chatIconMessageCount = this.data.chatIconMessageCount;
            chatIconMessageCount = chatIconMessageCount + 1;
            this.setData({
                chatIconMessageCount: chatIconMessageCount,
            });
        }
    }
})
