// components/header.js
Component({
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

    /**
     * 组件的方法列表
     */
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
