const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        id: 0,
        express: {},
        showBigPhoto: false,
        bigPhoto: {},
    },

    onLoad(options) {
        const id = options.id;
        this.setData({
            id: id,
        });

        this.loadExpressDetail();
    },

    loadExpressDetail() {
        const id = this.data.id;
        const that = this;
        app.post('/client/express/checkin/detail', {
            id: id,
        }, function (res) {
            that.setData({
                express: res.data,
            });
        });
    },

    onGoodsValueClick(e) {
        let expressId = e.currentTarget.dataset.item.id;
        wx.navigateTo({
            url: './express_checkin_price?id=' + expressId,
        })
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