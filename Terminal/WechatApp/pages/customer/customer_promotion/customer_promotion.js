const app = getApp();

Page({

    data: {
        promotionImageDesc: "",
        // 初始化为9个空的对象
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
            {
                imageNum: 4,
                imageUrl: "",
                smallImageUrl: ""
            },
            {
                imageNum: 5,
                imageUrl: "",
                smallImageUrl: ""
            },
            {
                imageNum: 6,
                imageUrl: "",
                smallImageUrl: ""
            },
            {
                imageNum: 7,
                imageUrl: "",
                smallImageUrl: ""
            },
            {
                imageNum: 8,
                imageUrl: "",
                smallImageUrl: ""
            },
            {
                imageNum: 9,
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
        console.log(e.currentTarget.dataset.item.imageNum);
        console.log(e.currentTarget.dataset.item.imageUrl);
    }
})