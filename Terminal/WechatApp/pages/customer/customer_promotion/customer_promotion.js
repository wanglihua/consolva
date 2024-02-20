const app = getApp();

Page({

    data: {
        promotionImageDesc: "",
        // 初始化为3个空的对象
        promotionImageList: [
            {
                imageNum: 1,
                imageUrl: "",
                smallImageUrl: ""
            },
            {
                imageNum: 2,
                imageUrl: "",
                smallImageUrl: ""
            },
            {
                imageNum: 3,
                imageUrl: "",
                smallImageUrl: ""
            },
        ]
    },

    onLoad(options) {
        let that = this;

        app.post("/client/customer/promotion", {
            trackingNoQuery: that.data.trackingNoQuery,
        }, function (res) {
            const promotionImageDesc = res.data.promotionImageDesc;
            const promotionImageList = res.data.promotionImageList;

            that.setData({
                promotionImageDesc: promotionImageDesc,
                promotionImageList: promotionImageList,
            });
        });
    },

    onImageClick(e) {
        let imageUrl = e.currentTarget.dataset.item.imageUrl;

        wx.showLoading({
            title: '图片下载中',
            mask: true
        });

        wx.downloadFile({
            url: imageUrl,
            success: function (res) {
                wx.hideLoading({
                    fail() {
                    }
                });

                wx.openDocument({
                    filePath: res.tempFilePath,
                    showMenu: true,
                    fileType: "jpg",
                    success: function (result) {
                        wx.saveImageToPhotosAlbum({
                            filePath: res.tempFilePath,
                            success: function (result) {
                                wx.showModal({
                                    title: '提示',
                                    content: '图片已保存到您的手机相册',
                                    showCancel: false
                                });
                            },
                            fail: function (err) {
                                if (err.errMsg === 'saveImageToPhotosAlbum:fail auth deny') {
                                    wx.openSetting({
                                        success(settingData) {
                                            if (settingData.authSetting['scope.writePhotosAlbum']) {
                                                wx.showModal({
                                                    title: '提示',
                                                    content: '获取图片保存权限成功!',
                                                    showCancel: false
                                                });
                                            } else {
                                                wx.showModal({
                                                    title: '提示',
                                                    content: '获取图片保存权限失败!',
                                                    showCancel: false
                                                });
                                            }
                                        },
                                        fail: function (err) {
                                            wx.showModal({
                                                title: '提示',
                                                content: '打开权限设置失败!' + err.errMsg,
                                                showCancel: false
                                            });
                                        }
                                    });
                                } else {
                                    wx.showModal({
                                        title: '提示',
                                        content: '图片保存失败，请尝试设置小程序权限!',
                                        showCancel: false
                                    });
                                }
                            },
                        });
                    },
                    fail: function (err) {
                        wx.showModal({
                            title: '提示',
                            content: '打开图片失败, 或请尝试设置小程序权限!' + err.errMsg,
                            showCancel: false
                        });
                    }
                });
            },
            fail: function (res) {
                wx.hideLoading({
                    fail() {
                    }
                });
                wx.showModal({
                    title: '错误',
                    content: '下载图片失败！',
                    showCancel: false
                });                
            },
            complete: function (res) {

            }
        });
    }
})