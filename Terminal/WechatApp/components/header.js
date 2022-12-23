// components/header.js
Component({
    options: {
        styleIsolation: 'isolated'
    },

    properties: {},

    data: {},

    pageLifetimes: {
        show: function () {
            // console.log("pageLifetimes: show()")
        }
    },

    methods: {
        onClickLeft() {
            wx.showToast({title: '点击返回', icon: 'none'});
        },
        onClickRight() {
            wx.showToast({title: '点击按钮', icon: 'none'});
        },
    },
})
