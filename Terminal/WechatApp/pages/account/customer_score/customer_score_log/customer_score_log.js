// pages/account/customer_score/customer_score_log/customer_score_log.js

const app = getApp();

Page({

    data: {
        lastId: 0,
        logCount: 0,
        logList: [],
    },

    onLoad(options) {
        const that = this;
        app.post("/client/customer/score/log", {
            lastId: that.data.lastId,
        }, function (res) {
            const lastId = res.data.lastId;
            const logCount = res.data.logCount;
            const logList = res.data.logList;

            for (const log of logList) {
                if (log.isValidScore === true) {
                    log.validScoreDesc = "有效积分，";
                } else {
                    log.validScoreDesc = '';
                }

                if (log.orderId !== null) {
                    log.relatedOrderDesc = "积分关联订单" + log.orderCode + "，订单客户" + log.orderCustomerId + "，";
                }

                if (log.description) {
                    log.description = "，" + log.description;
                }
            }

            that.setData({
                logList: logList,
                logCount: logCount,
                lastId: lastId,
            });
        })

    }
})