using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Domain.Express;
using shunda.Domain.Message;
using shunda.Service.Message.CustomerMessage;
using ISession = NHibernate.ISession;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressPhotoMessageService {
    private readonly ISession session;
    private readonly CustomerMessageExpressPhotoSignService customerMessageExpressPhotoSignService;
    private readonly CustomerMessageExpressPhotoCheckinService customerMessageExpressPhotoCheckinService;
    private readonly CustomerMessageOrderPhotoShipService customerMessageOrderPhotoShipService;

    public ExpressPhotoMessageService(
        ISession session,
        CustomerMessageExpressPhotoSignService customerMessageExpressPhotoSignService,
        CustomerMessageExpressPhotoCheckinService customerMessageExpressPhotoCheckinService,
        CustomerMessageOrderPhotoShipService customerMessageOrderPhotoShipService
    ) {
        this.session = session;
        this.customerMessageExpressPhotoSignService = customerMessageExpressPhotoSignService;
        this.customerMessageExpressPhotoCheckinService = customerMessageExpressPhotoCheckinService;
        this.customerMessageOrderPhotoShipService = customerMessageOrderPhotoShipService;
    }

    public void SendSignAndCheckinMessageIfNotSentWhenCheckinOrPick(int expressId) {
        // 发送可能的快递单图片
        var expressPhotoSign = session.Query<ExpressPhotoInfo>()
            .Where(e => e.Express!.Id == expressId && e.Type == ExpressPhotoType.Sign)
            .Take(1)
            .SingleOrDefault();
        if (expressPhotoSign != null) {
            // 这个是看有没发送过，不是看一个小时内有没发送
            var expressSendTimeSign = session.Query<ExpressPhotoMessageSendTimeInfo>()
                .Where(e => e.Express!.Id == expressId && e.Type == ExpressPhotoType.Sign)
                .Take(1)
                .SingleOrDefault();

            if (expressSendTimeSign == null) {
                SendAddPhotoMessage(expressPhotoSign);
            }
        }

        var expressPhotoCheckin = session.Query<ExpressPhotoInfo>()
            .Where(e => e.Express!.Id == expressId && e.Type == ExpressPhotoType.Checkin)
            .Take(1)
            .SingleOrDefault();
        if (expressPhotoCheckin != null) {
            var expressSendTimeCheckin = session.Query<ExpressPhotoMessageSendTimeInfo>()
                .Where(e => e.Express!.Id == expressId && e.Type == ExpressPhotoType.Checkin)
                .Take(1)
                .SingleOrDefault();

            if (expressSendTimeCheckin == null) {
                SendAddPhotoMessage(expressPhotoCheckin);
            }
        }
    }

    /// <summary>
    /// 添加图片时发送消息推送
    /// </summary>
    /// <param name="expressPhoto"></param>
    public void SendAddPhotoMessage(ExpressPhotoInfo expressPhoto) {
        if (expressPhoto == null) {
            return;
        }

        if (expressPhoto.Type == null || expressPhoto.Type == ExpressPhotoType.None) {
            return;
        }

        // 快递单图片
        if (expressPhoto.Type == ExpressPhotoType.Sign || expressPhoto.Type == ExpressPhotoType.Checkin) {
            if (string.IsNullOrWhiteSpace(expressPhoto.TrackingNo)
                // || expressPhoto.Customer == null 有预报的，这时是可以发送的
                // || expressPhoto.Express == null 有预报的，这时是可以发送的
               ) {
                return;
            }

            SendAddExpressPhotoMessageOnce(expressPhoto);
        }

        // 订单图片
        if (expressPhoto.Type == ExpressPhotoType.Ship) {
            if (string.IsNullOrWhiteSpace(expressPhoto.TrackingNo) || expressPhoto.Customer == null || expressPhoto.Order == null) {
                return;
            }

            SendAddOrderPhotoMessageOnce(expressPhoto);
        }
    }

    /// <summary>
    /// 发送图片快递添加消息通知
    /// </summary>
    /// <param name="expressPhoto"></param>
    private void SendAddExpressPhotoMessageOnce(ExpressPhotoInfo expressPhoto) {
        if (string.IsNullOrWhiteSpace(expressPhoto.TrackingNo)
            // || expressPhoto.Customer == null 有预报的，这时是可以发送的
            // || expressPhoto.Express == null 有预报的，这时是可以发送的
           ) {
            return;
        }

        string trackingNo = expressPhoto.TrackingNo;
        ExpressPhotoType photoType = expressPhoto.Type;

        // 一个快递单，一个图片类型，同一时间段，只发送一个消息  
        var messageSendTimeRecord = session.QueryOver<ExpressPhotoMessageSendTimeInfo>()
            .Where(e => e.TrackingNo == trackingNo && e.Type == photoType)
            .OrderBy(e => e.LastSendTime).Desc
            .Take(1)
            .SingleOrDefault();

        if (messageSendTimeRecord == null || messageSendTimeRecord.LastSendTime < DateTime.Now.AddHours(-1)) // 没发送过，或者一个小时前发送过
        {
            CustomerMessageType customerMessageType = GetMessageType(expressPhoto.Type);
            if (customerMessageType != null && customerMessageType != CustomerMessageType.None) {
                if (messageSendTimeRecord == null) {
                    messageSendTimeRecord = new ExpressPhotoMessageSendTimeInfo {
                        TrackingNo = trackingNo,
                        Order = null,
                        Express = expressPhoto.Express,
                        Type = photoType
                    };
                }

                messageSendTimeRecord.LastSendTime = DateTime.Now;

                session.SaveOrUpdate(messageSendTimeRecord);
                session.Flush();

                SendAddExpressPhotoMessage(photoType, expressPhoto);
            }
        }
    }

    private void SendAddExpressPhotoMessage(ExpressPhotoType photoType, ExpressPhotoInfo expressPhoto) {
        if (photoType == ExpressPhotoType.Sign) {
            if (expressPhoto.User?.LoginName == AutoPhotoConfig.AutoPhotoLoginName) {
                string sendUserName = expressPhoto.User?.UserName ?? string.Empty;

                // 使用智能机器人作为发送人
                customerMessageExpressPhotoSignService.SendMessage(expressPhoto, sendUserName);
            } else {
                customerMessageExpressPhotoSignService.SendMessage(expressPhoto);
            }
        }

        if (photoType == ExpressPhotoType.Checkin) {
            customerMessageExpressPhotoCheckinService.SendMessage(expressPhoto);
        }
    }

    /// <summary>
    /// 发送订单图片添加消息通知
    /// </summary>
    /// <param name="expressPhoto"></param>
    private void SendAddOrderPhotoMessageOnce(ExpressPhotoInfo expressPhoto) {
        if (expressPhoto == null
            || expressPhoto.Order == null
            || expressPhoto.Customer == null
            || expressPhoto.Type == null
            || expressPhoto.Type == ExpressPhotoType.None
           ) {
            return;
        }

        int orderId = expressPhoto.Order.Id;
        ExpressPhotoType photoType = expressPhoto.Type;

        // 一个订单，一个图片类型，同一时间段，只发送一个消息
        var messageSendTimeRecord = session.QueryOver<ExpressPhotoMessageSendTimeInfo>()
            .Where(e => e.Order != null && e.Order.Id == orderId && e.Type == photoType)
            .OrderBy(e => e.LastSendTime).Desc
            .Take(1)
            .SingleOrDefault();

        if (messageSendTimeRecord == null || messageSendTimeRecord.LastSendTime < DateTime.Now.AddHours(-1)) // 没发送过，或者一个小时前发送过
        {
            CustomerMessageType customerMessageType = GetMessageType(expressPhoto.Type);
            if (customerMessageType != null && customerMessageType != CustomerMessageType.None) {
                if (messageSendTimeRecord == null) {
                    messageSendTimeRecord = new ExpressPhotoMessageSendTimeInfo();
                    messageSendTimeRecord.TrackingNo = null;
                    messageSendTimeRecord.Order = expressPhoto.Order;
                    messageSendTimeRecord.Express = expressPhoto.Express;
                    messageSendTimeRecord.Type = photoType;
                }

                messageSendTimeRecord.LastSendTime = DateTime.Now;

                session.SaveOrUpdate(messageSendTimeRecord);
                session.Flush();

                SendAddOrderPhotoMessage(photoType, expressPhoto);
            }
        }
    }

    private void SendAddOrderPhotoMessage(ExpressPhotoType photoType, ExpressPhotoInfo expressPhoto) {
        if (expressPhoto.Order == null || expressPhoto.Customer == null) {
            return;
        }

        if (photoType == ExpressPhotoType.Issue) {
            customerMessageOrderPhotoShipService.SendMessage(expressPhoto.Customer, expressPhoto.Order);
        }

        /*
        if (photoType == ExpressPhotoType.Issue) {
            customerMessageService.SendMessageOrderPhotoIssue(expressPhoto.Customer, expressPhoto.Order);
        }

        if (photoType == ExpressPhotoType.CnDamage) {
            customerMessageService.SendMessageOrderPhotoCnDamage(expressPhoto.Customer, expressPhoto.Order);
        }

        if (photoType == ExpressPhotoType.Return) {
            customerMessageService.SendMessageOrderPhotoReturn(expressPhoto.Customer, expressPhoto.Order);
        }

        if (photoType == ExpressPhotoType.Ship) {
            customerMessageService.SendMessageOrderPhotoShip(expressPhoto.Customer, expressPhoto.Order);
        }

        if (photoType == ExpressPhotoType.AuDamage) {
            customerMessageService.SendMessageOrderPhotoAuDamage(expressPhoto.Customer, expressPhoto.Order);
        }

        if (photoType == ExpressPhotoType.Deliver) {
            customerMessageService.SendMessageOrderPhotoDeliver(expressPhoto.Customer, expressPhoto.Order);
        }

        */
    }

    private CustomerMessageType GetMessageType(ExpressPhotoType photoType) {
        if (photoType == ExpressPhotoType.Sign) {
            return CustomerMessageType.ExpressPhotoSign;
        }

        if (photoType == ExpressPhotoType.Checkin) {
            return CustomerMessageType.ExpressCheckin;
        }

        if (photoType == ExpressPhotoType.Ship) {
            return CustomerMessageType.OrderPhotoShip;
        }

        return CustomerMessageType.None;

        /*
        if (photoType == ExpressPhotoType.Issue)
        {
            return CustomerMessageType.OrderPhotoIssueTuiSong;
        }

        if (photoType == ExpressPhotoType.CnDamage)
        {
            return CustomerMessageType.OrderPhotoCnDamageTuiSong;
        }

        if (photoType == ExpressPhotoType.Return)
        {
            return CustomerMessageType.OrderPhotoReturnTuiSong;
        }

        if (photoType == ExpressPhotoType.AuDamage)
        {
            return CustomerMessageType.OrderPhotoAuDamageTuiSong;
        }

        if (photoType == ExpressPhotoType.Deliver)
        {
            return CustomerMessageType.OrderPhotoDeliverTuiSong;
        }
        */
    }
}