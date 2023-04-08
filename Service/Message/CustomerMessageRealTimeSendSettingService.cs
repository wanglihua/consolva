using NHibernate;
using shunda.Core.Service;
using shunda.Service.Message.CustomerMessage;

namespace shunda.Service.Message;

[Service(ServiceType.Scoped)]
public class CustomerMessageRealTimeSendSettingService {
    private readonly ISession session;

    public CustomerMessageRealTimeSendSettingService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 初始化设置
    /// </summary>
    public void InitSetting() {
        CustomerMessageAccountChangeService.InitSetting(session);
        CustomerMessageCouponFollowService.InitSetting(session);
        CustomerMessageExpressCheckinService.InitSetting(session);
        CustomerMessageExpressExpectEnterService.InitSetting(session);
        CustomerMessageExpressPhotoCheckinService.InitSetting(session);
        CustomerMessageExpressPhotoSignService.InitSetting(session);
        CustomerMessageMoneyEarnedService.InitSetting(session);
        CustomerMessageOrderDeliveredService.InitSetting(session);
        CustomerMessageOrderPhotoShipService.InitSetting(session);
        CustomerMessagePackedService.InitSetting(session);
        CustomerMessageTransferedService.InitSetting(session);

        session.Flush();

        /*
        // var messageTypeList = CustomerMessageType.GetList();
        var settingList = session.Query<CustomerMessageRealTimeSendSettingInfo>()
            // .Where(e => messageTypeList.Contains(e.Type))
            .ToList();

        {
            // 我赚到的钱提醒

            var setting = settingList.Where(e => e.Type == CustomerMessageType.WoZhuanDaoDeQianTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.WoZhuanDaoDeQianTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                setting.WechatMessageTemplate1 = "【叮咚一响，黄金万两】 哇，老板，又有积分进账了。本次入账：{score}积分，是您的第{gap}代下线孝敬您的！";
                setting.SmsMessageTemplate = "【叮咚一响，黄金万两】 哇，老板，又有积分进账了。本次入账：{score}积分，是您的第{gap}代下线孝敬您的！";
                setting.Placeholders = "订单类别(海运或空运)：{order_cate}，积分：{score}，代差：{gap}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 我的问题件提醒

            var setting = settingList.Where(e => e.Type == CustomerMessageType.WoDeWeiTiJianTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.WoDeWeiTiJianTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                setting.WechatMessageTemplate1 = "亲，【我的问题件】刚刚有了新情况哦，快去处理一下吧，以确保订单物品及早转运";
                setting.SmsMessageTemplate = "亲，【我的问题件】刚刚有了新情况哦，快去处理一下吧，以确保订单物品及早转运";
                setting.Placeholders = "";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 全部入库提交打包提醒

            var setting = settingList.Where(e => e.Type == CustomerMessageType.QuanBuRuKuTiJiaDaBoTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.QuanBuRuKuTiJiaDaBoTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                setting.WechatMessageTemplate1 = "老板，您的{order_code}订单现已是【已全部入库】状态了呢。如您不再需要添加新的快递，请及时点击【提交打包】按钮哦。早打包，早收到宝贝呢！";
                setting.SmsMessageTemplate = "老板，您的{order_code}订单已全部入库。请及时点击【提交打包】按钮，及早转运！";
                setting.Placeholders = "订单编码：{order_code}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 已打包提醒

            var setting = settingList.Where(e => e.Type == CustomerMessageType.YiDaBaoTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.YiDaBaoTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                setting.WechatMessageTemplate1 = "亲，{order_code}订单现已打包完毕。总费用为￥{total_fee}。你可点击【查看订单详情】查看打包数据及计费明细。友情提醒：如您对费用有异议，请联系您的专属客服！如无异议，请及时支付费用！";
                setting.SmsMessageTemplate = "亲，{order_code}订单现已打包完毕。总费用为￥{total_fee}。如对费用有异议，请联系客服！并请及时支付费用！";
                setting.Placeholders = "订单编码：{order_code}，总费用：{total_fee}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 空运订单已打包提醒

            var setting = settingList.Where(e => e.Type == CustomerMessageType.OrderAirYiDaBaoTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.OrderAirYiDaBaoTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                setting.WechatMessageTemplate1 = "亲，{order_code}订单现已打包完毕。总费用为￥{total_fee}。你可点击【查看订单详情】查看打包数据及计费明细。友情提醒：如您对费用有异议，请联系您的专属客服！如无异议，请及时支付费用！";
                setting.SmsMessageTemplate = "亲，{order_code}订单现已打包完毕。总费用为￥{total_fee}。如对费用有异议，请联系客服！并请及时支付费用！";
                setting.Placeholders = "订单编码：{order_code}，总费用：{total_fee}，费用计算描述：{fee_cal_desc}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 已转运提醒

            var setting = settingList.Where(e => e.Type == CustomerMessageType.YiZhuanYunTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.YiZhuanYunTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                setting.WechatMessageTemplate1 = "亲，{order_code}订单现已转运。所在船航批次为{customer_ship_container_code}，预计{arrive_time}到港。转运后，到港前，您可修改二程取货方式。到港后，不再允许修改二程取货方式！如遇台风、海关查验、船晚开，等不可抗力，时间顺延！";
                setting.SmsMessageTemplate = "亲，{order_code}订单现已在{customer_ship_container_code}船航批次转运，预计{arrive_time}到港。";
                setting.Placeholders = "订单编码：{order_code}，客户端船航批次：{customer_container_code}，到港时间：{arrive_time}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 支付二程方式变更费用提醒

            var setting = settingList.Where(e => e.Type == CustomerMessageType.ZhiFuErChengFangShiBianGengFeiYongTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.ZhiFuErChengFangShiBianGengFeiYongTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                setting.WechatMessageTemplate1 = "亲，【{order_code}】订单所在船航批次【{customer_container_code}】还有3天将抵达目的港。3天后将不能修改二程取货方式。如您仍未支付费用，请及时支付费用，如已支付请忽略！如遇海关查验、码头罢工，等不可抗力，时间顺延！";
                setting.SmsMessageTemplate = "亲，【{order_code}】订单所在船航批次【{customer_container_code}】将抵达目的港。3天后不能修改取货方式。如仍未支付费用，请及时支付，如已支付请忽略！";
                setting.Placeholders = "订单编码：{order_code}，客户端船航批次：{customer_container_code}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // VIP升级推送

            var setting = settingList.Where(e => e.Type == CustomerMessageType.VipShengJiTuiSong).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.VipShengJiTuiSong;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 老板，恭喜您啊。在您的众多下线的努力下，您现在已经是【VIP等级】了。从此，您将享受：集运头程【x】折扣；集运二程【x】折扣；澳拼商城【x】折扣。请继续转发您的专属海报，向【下一级VIP】出发哦！
                setting.WechatMessageTemplate1 = "老板，恭喜您啊。在您的众多下线的努力下，您现在已经是【{new_vip_level}】了。从此，您将享受：集运头程【{logistics_first_discount}】折扣；集运二程【{logistics_second_discount}】折扣；澳拼商城【{mall_discount}】折扣。请继续转发您的专属海报，向【{next_vip_level}】出发哦！";
                setting.SmsMessageTemplate = "老板，恭喜您啊。在您的众多下线的努力下，您现在已经是【{new_vip_level}】了！";
                setting.Placeholders = "最新VIP等级：{new_vip_level}，集运头程折扣：{logistics_first_discount}，集运二程折扣：{logistics_second_discount}，澳拼商城折扣：{mall_discount}，下一级VIP：{next_vip_level}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 在线客服推送

            var setting = settingList.Where(e => e.Type == CustomerMessageType.ZaiXianKeFuTuiSong).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.ZaiXianKeFuTuiSong;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 您好，【在线客服】有新的消息哦。请从公众号右侧菜单进入【在线客服】！
                setting.WechatMessageTemplate1 = "您好，【在线客服】有新的消息哦。请从公众号右侧菜单进入【在线客服】！";
                setting.SmsMessageTemplate = "您好，【在线客服】有新的消息哦。请从公众号右侧菜单进入【在线客服】！";
                setting.Placeholders = "";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // GST推送

            var setting = settingList.Where(e => e.Type == CustomerMessageType.GstTuiSong).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.GstTuiSong;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 您好，您的{0}订单，现在申报货值达到￥2500，货值达到或超过￥2500后，会按照货值的10%收取GST，澳洲进口时，交给澳洲海关哦。
                setting.WechatMessageTemplate1 = "您好，您的{order_code}订单，现在申报货值达到￥2500，货值达到或超过￥2500后，会按照货值的10%收取GST，澳洲进口时，交给澳洲海关哦。";
                setting.SmsMessageTemplate = "您好，您的{order_code}订单，现在申报货值达到￥2500，货值达到或超过￥2500后，会按照货值的10%收取GST，澳洲进口时，交给澳洲海关哦。";
                setting.Placeholders = "订单编码：{order_code}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 订单入库视频推送

            var setting = settingList.Where(e => e.Type == CustomerMessageType.OrderVideoCheckInTuiSong).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.OrderVideoCheckInTuiSong;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 您好，您的【{tracking_no}】快递单新增了【{video_type}】视频，请点击对应入库快递单查看！
                setting.WechatMessageTemplate1 = "您好，您的【{tracking_no}】快递单新增了【{video_type}】视频，请点击对应入库快递单查看！";
                setting.SmsMessageTemplate = "您好，您的【{tracking_no}】快递单新增了【{video_type}】视频，请点击对应入库快递单查看！";
                setting.Placeholders = "快递单号：{tracking_no}，视频类型：{video_type}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 订单问题图片推送

            var setting = settingList.Where(e => e.Type == CustomerMessageType.OrderPhotoIssueTuiSong).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.OrderPhotoIssueTuiSong;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 您好，您的【{0}】订单新增了【{1}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！
                setting.WechatMessageTemplate1 = "您好，您的【{order_code}】订单新增了【{photo_type}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！";
                setting.SmsMessageTemplate = "您好，您的【{order_code}】订单新增了【{photo_type}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！";
                setting.Placeholders = "订单编码：{order_code}，图片类型：{photo_type}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 订单中国损毁图片推送

            var setting = settingList.Where(e => e.Type == CustomerMessageType.OrderPhotoCnDamageTuiSong).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.OrderPhotoCnDamageTuiSong;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 您好，您的【{0}】订单新增了【{1}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！
                setting.WechatMessageTemplate1 = "您好，您的【{order_code}】订单新增了【{photo_type}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！";
                setting.SmsMessageTemplate = "您好，您的【{order_code}】订单新增了【{photo_type}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！";
                setting.Placeholders = "订单编码：{order_code}，图片类型：{photo_type}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 订单退运图片推送

            var setting = settingList.Where(e => e.Type == CustomerMessageType.OrderPhotoReturnTuiSong).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.OrderPhotoReturnTuiSong;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 您好，您的【{0}】订单新增了【{1}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！
                setting.WechatMessageTemplate1 = "您好，您的【{order_code}】订单新增了【{photo_type}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！";
                setting.SmsMessageTemplate = "您好，您的【{order_code}】订单新增了【{photo_type}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！";
                setting.Placeholders = "订单编码：{order_code}，图片类型：{photo_type}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 订单发货图片推送

            var setting = settingList.Where(e => e.Type == CustomerMessageType.OrderPhotoShipTuiSong).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.OrderPhotoShipTuiSong;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 您好，您的【{0}】订单新增了【{1}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！
                setting.WechatMessageTemplate1 = "您好，您的【{order_code}】订单新增了【{photo_type}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！";
                setting.SmsMessageTemplate = "您好，您的【{order_code}】订单新增了【{photo_type}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！";
                setting.Placeholders = "订单编码：{order_code}，图片类型：{photo_type}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 订单澳洲损毁图片推送

            var setting = settingList.Where(e => e.Type == CustomerMessageType.OrderPhotoAuDamageTuiSong).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.OrderPhotoAuDamageTuiSong;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 您好，您的【{0}】订单新增了【{1}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！
                setting.WechatMessageTemplate1 = "您好，您的【{order_code}】订单新增了【{photo_type}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！";
                setting.SmsMessageTemplate = "您好，您的【{order_code}】订单新增了【{photo_type}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！";
                setting.Placeholders = "订单编码：{order_code}，图片类型：{photo_type}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 订单交付图片推送

            var setting = settingList.Where(e => e.Type == CustomerMessageType.OrderPhotoDeliverTuiSong).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.OrderPhotoDeliverTuiSong;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 您好，您的【{0}】订单新增了【{1}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！
                setting.WechatMessageTemplate1 = "您好，您的【{order_code}】订单新增了【{photo_type}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！";
                setting.SmsMessageTemplate = "您好，您的【{order_code}】订单新增了【{photo_type}】照片，请点击对应订单的【点击查看详情】里面的【录像/照片】查看！";
                setting.Placeholders = "订单编码：{order_code}，图片类型：{photo_type}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 超期未入库快递单提醒

            var setting = settingList.Where(e => e.Type == CustomerMessageType.ExpressOverdueCheckinTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.ExpressOverdueCheckinTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 您的预报的快递单号:【{tracking_no}】，创建 10 天后未能入库，请检查是否预报有误！ 如有疑问，请联系系统内【客服】
                setting.WechatMessageTemplate1 = "您的预报的快递单号:【{tracking_no}】，创建 10 天后未能入库，请检查是否预报有误！ 如有疑问，请联系系统内【客服】";
                setting.SmsMessageTemplate = "您的预报的快递单号:【{tracking_no}】，创建 10 天后未能入库，请检查是否预报有误！ 如有疑问，请联系系统内【客服】";
                setting.Placeholders = "快递单号：{tracking_no}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 澳洲快递单号提醒

            var setting = settingList.Where(e => e.Type == CustomerMessageType.AuExpressTrackingNoTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.AuExpressTrackingNoTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 您的 {order_code} 订单， 现 {express_company} 快递公司已取件，跟踪单号为 {tracking_no}，跟踪网址为 {tracking_url}
                setting.WechatMessageTemplate1 = "您的 {order_code} 订单， 现 {express_company} 快递公司已取件，跟踪单号为 {tracking_no}，跟踪网址为 {tracking_url}";
                setting.SmsMessageTemplate = "您的 {order_code} 订单， 现 {express_company} 快递公司已取件，跟踪单号为 {tracking_no}，跟踪网址为 {tracking_url}";
                setting.Placeholders = "订单编码：{order_code}，澳洲快递单号：{tracking_no}，澳洲快递公司：{express_company}，澳洲快递跟踪网址：{tracking_url}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 国际快递单号提醒

            var setting = settingList.Where(e => e.Type == CustomerMessageType.InternationalExpressTrackingNoTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.InternationalExpressTrackingNoTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 您的 {order_code} 订单， 现 {express_company} 快递公司已取件，跟踪单号为 {tracking_no}，跟踪网址为 {tracking_url}
                setting.WechatMessageTemplate1 = "您的 {order_code} 订单， 现 {express_company} 快递公司已取件，跟踪单号为 {tracking_no}，跟踪网址为 {tracking_url}";
                setting.SmsMessageTemplate = "您的 {order_code} 订单， 现 {express_company} 快递公司已取件，跟踪单号为 {tracking_no}，跟踪网址为 {tracking_url}";
                setting.Placeholders = "订单编码：{order_code}，国际快递单号：{tracking_no}，国际快递公司：{express_company}，国际快递跟踪网址：{tracking_url}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 国际快递更新提醒

            var setting = settingList.Where(e => e.Type == CustomerMessageType.InternationalExpressStateUpdateTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.InternationalExpressStateUpdateTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 您的 订单{order_code} 跟踪单号{tracking_no}，有新的状态更新：{tracking_content}
                setting.WechatMessageTemplate1 = "您的 订单{order_code} 跟踪单号{tracking_no}，有新的状态更新：{tracking_content}";
                setting.SmsMessageTemplate = "您的 订单{order_code} 跟踪单号{tracking_no}，有新的状态更新：{tracking_content}";
                setting.Placeholders = "订单编码：{order_code}，国际快递单号：{tracking_no}，状态更新内容：{tracking_content}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 订单未支付提醒

            var setting = settingList.Where(e => e.Type == CustomerMessageType.OrderNotPayTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.OrderNotPayTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 您好，您新创建的转运单【{order_code}】，目前尚处于欠费状态。海运转运单，只有不欠费后，才会安排打包哦。空运转运单，只有不欠费后，才会安排转运哦
                setting.WechatMessageTemplate1 = "您好，您新创建的转运单【{order_code}】，目前尚处于欠费状态。海运转运单，只有不欠费后，才会安排打包哦。空运转运单，只有不欠费后，才会安排转运哦";
                setting.SmsMessageTemplate = "您好，您新创建的转运单【{order_code}】，目前尚处于欠费状态。海运转运单，只有不欠费后，才会安排打包哦。空运转运单，只有不欠费后，才会安排转运哦";
                setting.Placeholders = "订单编码：{order_code}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 未创建订单提醒

            var setting = settingList.Where(e => e.Type == CustomerMessageType.OrderNotCreateTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.OrderNotCreateTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 亲，澳拼发现您在【已入库】中还有未创建订单的快递，不要忘记建单付款哦！ 无欠费后 澳拼立马安排打包发货，如有任何疑问，请咨询系统【在线客服】
                setting.WechatMessageTemplate1 = "亲，澳拼发现您在【已入库】中还有未创建订单的快递，不要忘记建单付款哦！ 无欠费后 澳拼立马安排打包发货，如有任何疑问，请咨询系统【在线客服】";
                setting.SmsMessageTemplate = "亲，澳拼发现您在【已入库】中还有未创建订单的快递，不要忘记建单付款哦！ 无欠费后 澳拼立马安排打包发货，如有任何疑问，请咨询系统【在线客服】";
                setting.Placeholders = "";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 客服评价消息

            var setting = settingList.Where(e => e.Type == CustomerMessageType.CustomerChatRateMessage).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.CustomerChatRateMessage;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 客服满意度评价，点击评价
                setting.WechatMessageTemplate1 = "客服满意度评价，点击评价";
                setting.SmsMessageTemplate = "客服满意度评价，点击评价";
                setting.Placeholders = "会员ID：{customer_code}，客服：{user_name}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 国内仓储即将超期提醒
            var setting = settingList.Where(e => e.Type == CustomerMessageType.CnStorageOverdueTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.CnStorageOverdueTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                // 亲，您已入库的快递单号:【{tracking_no}】，将于 10 天后仓储达到 90 天，届时澳拼将不再有责保管，如遇丢件、损坏等，无法进行理赔，请尽快创建订单出运。
                setting.WechatMessageTemplate1 = "亲，您已入库的快递单号:【{tracking_no}】，将于 10 天后仓储达到 90 天，届时澳拼将不再有责保管，如遇丢件、损坏等，无法进行理赔，请尽快创建订单出运。";
                setting.SmsMessageTemplate = "亲，您已入库的快递单号:【{tracking_no}】，将于 10 天后仓储达到 90 天，届时澳拼将不再有责保管，如遇丢件、损坏等，无法进行理赔，请尽快创建订单出运。";
                setting.Placeholders = "快递单号：{tracking_no}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 空运包裹签收提醒

            var setting = settingList.Where(e => e.Type == CustomerMessageType.OrderDeliveredAirTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.OrderDeliveredAirTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                setting.WechatMessageTemplate1 = "亲，您的空运订单{order_code}，显示已妥投签收，如果有问题请联系空运客服。另外，分享微信朋友圈积攒10个以上，联系空运客服可得20RMB空运优惠券！";
                setting.SmsMessageTemplate = "亲，您的空运订单{order_code}，显示已妥投签收，如果有问题请联系空运客服。另外，分享微信朋友圈积攒10个以上，联系空运客服可得20RMB空运优惠券！";
                setting.Placeholders = "订单编码：{order_code}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 仓储费率提醒

            var setting = settingList.Where(e => e.Type == CustomerMessageType.AuFreeStorageDayReminderTiXing).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.AuFreeStorageDayReminderTiXing;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                setting.WechatMessageTemplate1 = "亲，【{order_code}】订单您还未在【{target_warehouse_or_pickup_station}】提货,免费仓储还剩【{days_remaining}】天，超期后将自动加收仓储费{storage_fee_rate}RMB/立方米/天（最低收费{min_fee_per_day}RMB/天），且无法申请减免，请务必尽快于营业时间提货。";
                setting.SmsMessageTemplate = "亲，【{order_code}】订单您还未在【{target_warehouse_or_pickup_station}】提货,免费仓储还剩【{days_remaining}】天，超期后将自动加收仓储费{storage_fee_rate}RMB/立方米/天（最低收费{min_fee_per_day}RMB/天），且无法申请减免，请务必尽快于营业时间提货。";
                setting.Placeholders = "订单编码：{order_code}，澳仓或取货点名称：{target_warehouse_or_pickup_station}，剩余免费仓储天数：{days_remaining}，额外仓储费率（RMB/立方米/天）：{storage_fee_rate}, 最低仓储收费（RMB/天）：{min_fee_per_day}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 待支付仓储费通知

            var setting = settingList.Where(e => e.Type == CustomerMessageType.AuExtraStorageDayFeeTongZhi).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.AuExtraStorageDayFeeTongZhi;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                setting.WechatMessageTemplate1 = "亲，【{order_code}】订单截止至{fee_date}，已产生仓储费【{storage_fee}RMB】，请在澳拼系统内支付费用后，当天提货，以免再次产生新仓储费。";
                setting.SmsMessageTemplate = "亲，【{order_code}】订单截止至{fee_date}，已产生仓储费【{storage_fee}RMB】，请在澳拼系统内支付费用后，当天提货，以免再次产生新仓储费。";
                setting.Placeholders = "订单编码：{order_code}，计费日期（推送消息的前1天）：{fee_date}，仓储费(RMB)：{storage_fee}";

                session.Save(setting);
                session.Flush();
            }
        }

        {
            // 入库超大超重HOLD通知

            var setting = settingList.Where(e => e.Type == CustomerMessageType.CheckinExtraSizeWeightHoldTongZhi).Take(1).SingleOrDefault();
            if (setting == null)
            {
                setting = new CustomerMessageRealTimeSendSettingInfo();
                setting.Type = CustomerMessageType.CheckinExtraSizeWeightHoldTongZhi;
                setting.Status = CustomerMessageSettingStatus.Disabled;
                setting.WechatSend = true;
                setting.SmsSend = false;

                setting.WechatMessageTemplate1 = "您好，您的货物，单号{tracking_no}，现已到达澳拼收货仓，因含超大或超重件，可能有额外运输费用，请尽快联系客服确认！";
                setting.SmsMessageTemplate = "您好，您的货物，单号{tracking_no}，现已到达澳拼收货仓，因含超大或超重件，可能有额外运输费用，请尽快联系客服确认！";
                setting.Placeholders = "快递单号：{tracking_no}";

                session.Save(setting);
                session.Flush();
            }
        }

        session.Flush();
        
        */
    }
}