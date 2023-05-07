using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Message;
using shunda.Service.Customer;

namespace shunda.Service.Message.CustomerMessage;

/// <summary>
/// 快递单入库图片推送
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageExpressPhotoCheckinService {
    private readonly ISession session;
    private readonly CustomerMessageCommonService customerMessageCommonService;
    private readonly CustomerCodeService customerCodeService;

    public CustomerMessageExpressPhotoCheckinService(ISession session,
        CustomerMessageCommonService customerMessageCommonService,
        CustomerCodeService customerCodeService) {
        this.session = session;
        this.customerMessageCommonService = customerMessageCommonService;
        this.customerCodeService = customerCodeService;
    }

    private static CustomerMessageType messageType => CustomerMessageType.ExpressPhotoCheckin;

    public static void InitSetting(ISession session) {
        var setting = session.Query<CustomerMessageRealTimeSendSettingInfo>()
            .Where(e => e.Type == messageType)
            .Take(1)
            .SingleOrDefault();
        if (setting == null) {
            setting = new CustomerMessageRealTimeSendSettingInfo {
                Type = messageType,
                Status = CustomerMessageSettingStatus.Disabled,
                WechatSend = true,
                SmsSend = false,
                WechatMessageTemplate1 = "您好，您的【{tracking_no}】快递单新增了【{photo_type}】照片，请点击对应入库快递单详情查看！",
                SmsMessageTemplate = "您好，您的【{tracking_no}】快递单新增了【{photo_type}】照片，请点击对应入库快递单详情查看！",
                Placeholders = "快递单号：{tracking_no}，图片类型：{photo_type}",
                ClientCustomizable = false,
                ClientTitle = messageType.Name,
                ClientDesc = messageType.Description
            };

            session.Save(setting);
            session.Flush();
        }
    }

    private string TransferMessage(string message, string trackingNo) {
        if (string.IsNullOrWhiteSpace(message)) {
            return string.Empty;
        }

        message = message.Replace("{tracking_no}", trackingNo);
        message = message.Replace("{photo_type}", ExpressPhotoType.Checkin.Name);

        return message;
    }

    public string GetUrl(int expressId) {
        return $"pages/express/express_checkin/express_checkin_detail?id={expressId}";
    }

    public void SendMessage(ExpressPhotoInfo expressPhoto) {
        var customer = expressPhoto.Customer;

        string trackingNo = expressPhoto.TrackingNo;
        if (customer == null) {
            var express = session.Query<ExpressInfo>()
                .Where(e => e.TrackingNo == trackingNo)
                .Take(1)
                .SingleOrDefault();
            if (express != null) {
                customer = express.Customer;
            } else {
                var expressExpect = session.Query<ExpressExpectInfo>()
                    .Where(e => e.TrackingNo == trackingNo)
                    .Take(1)
                    .SingleOrDefault();
                if (expressExpect != null) {
                    customer = expressExpect.Customer;
                }
            }
        }

        // 快递单还没绑定快递单，就是快递单还没入库，并且也没预报
        if (customer == null) {
            // throw new ValidationException("消息发送指定的会员不能为空！");
            return;
        }

        // if (express == null) {
        //     throw new ValidationException("消息发送指定的快递单不能为空！");
        // }

        if (customer.Status == CustomerStatus.Frozen) {
            return;
        }

        CustomerMessageRealTimeSendSettingInfo setting = customerMessageCommonService.GetCustomerMessageRealTimeSendSetting(messageType);
        if (setting.Status == CustomerMessageSettingStatus.Enabled) {
            string message1 = TransferMessage(setting.WechatMessageTemplate1, trackingNo);
            string message2 = TransferMessage(setting.WechatMessageTemplate2, trackingNo);
            string message3 = TransferMessage(setting.WechatMessageTemplate3, trackingNo);

            CustomerMessageStatus messageStatus = CustomerMessageStatus.NotSend;

            bool clientShouldSend = true;
            if (setting.ClientCustomizable && (setting.WechatSend || setting.SmsSend)) {
                clientShouldSend = customerMessageCommonService.CustomerShouldSend(customer.Id, messageType);
            }

            if (clientShouldSend && (setting.WechatSend || setting.SmsSend)) {
                messageStatus = CustomerMessageStatus.Success;
            }

            string processUser = CustomerMessageCommonService.ShundaSecretary;

            int message1Id = 0;
            if (string.IsNullOrWhiteSpace(message1) == false) {
                message1Id = customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, expressPhoto.Id, message1, messageStatus, setting, processUser);
            }

            int message2Id = 0;
            if (string.IsNullOrWhiteSpace(message2) == false) {
                message2Id = customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, expressPhoto.Id, message2, messageStatus, setting, processUser);
            }

            int message3Id = 0;
            if (string.IsNullOrWhiteSpace(message3) == false) {
                message3Id = customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, expressPhoto.Id, message3, messageStatus, setting, processUser);
            }

            if (clientShouldSend) {
                if (setting.WechatSend) {
                    string messageTitle = customerMessageCommonService.GetMessageTitle(messageType, "快递单入库图片推送");
                    string customerCode = customerCodeService.GetCustomerCode(customer.Id);

                    if (string.IsNullOrWhiteSpace(message1) == false) {
                        string url = CustomerMessageClientUrlService.GetMessageDetailUrl(message1Id);
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, messageTitle, customerCode, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message2) == false) {
                        string url = CustomerMessageClientUrlService.GetMessageDetailUrl(message2Id);
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, messageTitle, customerCode, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message3) == false) {
                        string url = CustomerMessageClientUrlService.GetMessageDetailUrl(message3Id);
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, messageTitle, customerCode, processUser, url);
                    }
                }

                if (setting.SmsSend) {
                    customerMessageCommonService.SendSms(customer.Mobile, setting.SmsMessageTemplate);
                }
            }
        }
    }
}