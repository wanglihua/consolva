// index.js
// 获取应用实例
const app = getApp();

Page({
    id: "index",
    data: {},
    onLoad() {
        const nowMilliSeconds = new Date().getTime();
        const loginMilliSeconds = wx.getStorageSync("client-login-time");
        const that = this;
        // 最多5分钟尝试 wx.login() 一次
        if (!loginMilliSeconds || nowMilliSeconds - loginMilliSeconds > 5 * 60 * 1000) {
            wx.login({
                success: res => {
                    that.getClientHomeData(res.code);
                }
            });
        } else {
            that.getClientHomeData("");
        }
    },

    getClientHomeData(code) {
        const that = this;
        app.postWithoutLoading(
            "/client/home/data", {
                code: code
            }, function (res) {
                // 存储的是登录时间毫秒值
                wx.setStorageSync('client-login-time', new Date().getTime());
            }
        );
    }
})