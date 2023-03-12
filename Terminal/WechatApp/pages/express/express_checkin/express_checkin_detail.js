const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        express: {},
        showBigPhoto: false,
        bigPhoto: {},
    },

    onLoad(options) {
        const id = options.id;

        const that = this;
        app.post('/client/express/checkin/detail', {
            id: id,
        }, function (res) {
            that.setData({
                express: res.data,
            });
        });
    },

    onCopyClick(e) {
        const copyText = e.currentTarget.dataset.item;
        wx.setClipboardData({
            data: copyText,
            success: function (res) {
                wx.showToast({
                    title: "复制成功！",
                });
            }
        });
    },

    onSmallPhotoClick(e) {
        console.log(e);
        const photo = e.currentTarget.dataset.item;
        this.setData({
            showBigPhoto: true,
            bigPhoto: photo,
        });
    },

    onBigPhotoClose(e) {
        this.setData({
            showBigPhoto: false,
        });
    },

    onBackClick(e) {
        wx.navigateBack();
    },
})