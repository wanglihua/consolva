const app = getApp();

Page({
    data: {
        goodsTypeOptions: [],
        showGoodsTypeSelect: false,
        goodsTypeFieldValue: '',
        goodsTypeCascaderValue: '',

        goodsTypeId: 0,
        goodsValue: '',
        goodsList: [],
    },

    onLoad(options) {
        this.loadGoodsDetail();
    },

    loadGoodsDetail() {
        const that = this;
        app.post("/client/freight/estimate/goods/detail", {}, function (res) {
            const goodsTypeList = res.data.goodsTypeList;
            const goodsTypeId = res.data.goodsTypeId;

            let goodsTypeOptions = [];
            let goodsTypeFieldValue = '';
            let goodsTypeCascaderValue = '';
            for (const goodsType of goodsTypeList) {
                goodsTypeOptions.push({
                    value: goodsType.id.toString(),
                    text: goodsType.name,
                });

                if (goodsType.id === goodsTypeId) {
                    goodsTypeFieldValue = goodsType.name;
                    goodsTypeCascaderValue = goodsType.id.toString();
                }
            }

            that.setData({
                goodsTypeId: goodsTypeId,
                goodsValue: res.data.goodsValue,
                goodsList: res.data.goodsList,

                goodsTypeOptions: goodsTypeOptions,
                goodsTypeFieldValue: goodsTypeFieldValue,
                goodsTypeCascaderValue: goodsTypeCascaderValue,
            });
        });
    },

    onGoodsTypeFieldTap(e) {
        this.setData({
            showGoodsTypeSelect: true,
        });
    },

    onGoodsTypeSelectClose(e) {
        this.setData({
            showGoodsTypeSelect: false,
        });
    },

    onGoodsTypeSelectFinish(e) {
        const {selectedOptions, value} = e.detail;
        const fieldValue = selectedOptions
            .map((option) => option.text)
            .join('/');
        this.setData({
            goodsTypeFieldValue: fieldValue,
            goodsTypeCascaderValue: value,
            goodsTypeId: parseInt(value),
            showGoodsTypeSelect: false,
        });
    },

    onGoodsValueInput(e) {
        this.setData({
            goodsValue: e.detail,
        });
    },

    onGoodsLengthInput(e) {
        const goodsId = e.currentTarget.dataset.item.id;
        const goodsList = this.data.goodsList;
        for (const goods of goodsList) {
            if (goods.id === goodsId) {
                goods.length = e.detail;
            }
        }

        this.setData({
            goodsList: goodsList,
        });
    },

    onGoodsWidthInput(e) {
        const goodsId = e.currentTarget.dataset.item.id;
        const goodsList = this.data.goodsList;
        for (const goods of goodsList) {
            if (goods.id === goodsId) {
                goods.width = e.detail;
            }
        }

        this.setData({
            goodsList: goodsList,
        });
    },

    onGoodsHeightInput(e) {
        const goodsId = e.currentTarget.dataset.item.id;
        const goodsList = this.data.goodsList;
        for (const goods of goodsList) {
            if (goods.id === goodsId) {
                goods.height = e.detail;
            }
        }

        this.setData({
            goodsList: goodsList,
        });
    },

    onGoodsWeightInput(e) {
        const goodsId = e.currentTarget.dataset.item.id;
        const goodsList = this.data.goodsList;
        for (const goods of goodsList) {
            if (goods.id === goodsId) {
                goods.weight = e.detail;
            }
        }

        this.setData({
            goodsList: goodsList,
        });
    },

    onGoodsPackageCountInput(e) {
        const goodsId = e.currentTarget.dataset.item.id;
        const goodsList = this.data.goodsList;
        for (const goods of goodsList) {
            if (goods.id === goodsId) {
                goods.packageCount = e.detail;
            }
        }

        this.setData({
            goodsList: goodsList,
        });
    },

    onRemoveGoodsButtonClick(e) {
        const goodsId = e.currentTarget.dataset.item.id;
        const goodsList = this.data.goodsList;

        const newGoodsList = [];
        for (const goods of goodsList) {
            if (goodsId !== goods.id) {
                newGoodsList.push(goods);
            }
        }

        this.setData({
            goodsList: newGoodsList,
        });
    },

    onAddGoodsButtonClick(e) {
        const goodsList = this.data.goodsList;

        let minGoodsId = -1;
        for (const goods of goodsList) {
            if (minGoodsId > goods.id) {
                minGoodsId = goods.id;
            }
        }

        const newGoodsId = minGoodsId - 1;

        goodsList.push({
            id: newGoodsId,
            width: '',
            length: '',
            height: '',
            weight: '',
            packageCount: '',
        });

        this.setData({
            goodsList: goodsList,
        });
    },

    onNextButtonClick(e) {
        const that = this;
        app.postWithoutLoading("/client/freight/estimate/goods/save", {
            goodsTypeId: that.data.goodsTypeId,
            goodsValue: that.data.goodsValue,
            goodsList: that.data.goodsList,
        }, function (res) {
            wx.showToast({
                title: res.message,
            });

            that.gotoNext();
        });
    },

    gotoNext() {
        wx.redirectTo({
            url: "./freight_estimate_country",
        });
    },
})