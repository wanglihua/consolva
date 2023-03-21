Component({
    properties: {
        index: {
            type: Number,
            value: 0,
        },
        headerClass: {
            type: String,
            value: "",
        },
        headerTip: {
            type: String,
            value: "",
        },
        express: {
            type: Object,
            value: {},
        }
    },
    data: {},
    methods: {
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
    }
});
