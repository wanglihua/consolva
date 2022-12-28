// pages/express/express_checkin/express_checkin_price.js
const app = getApp();
const util = app.require('utils/util.js');

Page({
    data: {
        express: {},
    },

    onLoad(options) {
        const id = options.id;

        const that = this;
        app.post("/client/express/checkin/price", {
            id: id,
        }, function (res) {
            that.setData({
                express: res.data,
            });
        });
    }
})