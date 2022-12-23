// components/header.js
Component({
    options: {
        styleIsolation: 'isolated'
    },
    /**
     * 组件的属性列表
     */
    properties: {},

    /**
     * 组件的初始数据
     */
    data: {
        chatIconMessageCount: 0,
    },

    pageLifetimes: {
        show: function () {
            console.log("pageLifetimes: show()")
        }
    },
    
    methods: {
        onChatIconClick(e) {
            console.log(e);

            let chatIconMessageCount = this.data.chatIconMessageCount;
            chatIconMessageCount = chatIconMessageCount + 1;
            this.setData({
                chatIconMessageCount: chatIconMessageCount,
            });
        }
    },


})
