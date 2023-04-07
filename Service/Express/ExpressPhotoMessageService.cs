using shunda.Core.Service;
using shunda.Domain.Express;
using shunda.Domain.Message;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressPhotoMessageService {
    // todo a 完成图片消息发送
    public void SendAddPhotoMessage(ExpressPhotoInfo expressPhoto) {
    }

    /// <summary>
    /// 发送订单图片添加消息通知
    /// </summary>
    /// <param name="expressPhoto"></param>
    public void SendAddOrderPhotoMessage(ExpressPhotoInfo expressPhoto) {
        /*
         todo a 后面补上
        if (expressPhoto == null || expressPhoto.Order == null || expressPhoto.Customer == null || expressPhoto.Type == null)
        {
            return;
        }

        int orderId = expressPhoto.Order.Id;
        ExpressPhotoType photoType = expressPhoto.Type;

        // 一个订单，一个图片类型，同一时间段，只发送一个消息
        LogisticsOrderPhotoMessageSendTimeInfo messageSendTimeRecord = session.QueryOver<LogisticsOrderPhotoMessageSendTimeInfo>()
            .Where(e => e.Order != null && e.Order.Id == orderId && e.Type == photoType)
            .OrderBy(e => e.LastSendTime).Desc
            .Take(1)
            .SingleOrDefault();

        if (messageSendTimeRecord == null || messageSendTimeRecord.LastSendTime < DateTime.Now.AddHours(-1)) // 没发送过，或者一个小时前发送过
        {
            CustomerMessageType customerMessageType = GetMessageType(expressPhoto.Type);
            if (customerMessageType != null && customerMessageType != CustomerMessageType.None)
            {
                if (messageSendTimeRecord == null)
                {
                    messageSendTimeRecord = new LogisticsOrderPhotoMessageSendTimeInfo();
                    messageSendTimeRecord.TrackingNo = null;
                    messageSendTimeRecord.Order = expressPhoto.Order;
                    messageSendTimeRecord.Express = expressPhoto.Express;
                    messageSendTimeRecord.Type = photoType;
                }

                messageSendTimeRecord.LastSendTime = DateTime.Now;

                session.SaveOrUpdate(messageSendTimeRecord);
                session.Flush();

                // 先保存messageSendTimeRecord，再发送消息，否则发送消息比较费时容易造成上面的保存出错
                SendMessageOrderPhoto(photoType, expressPhoto);
            }
        }
        */
    }

    /// <summary>
    /// 发送图片快递添加消息通知
    /// </summary>
    /// <param name="expressPhoto"></param>
    public void SendAddExpressPhotoMessage(ExpressPhotoInfo expressPhoto) {
        /*
        todo a 后面补上
        
        // 这里的订单是可以为空的，发送消息时，可能订单都还没创建
        if (expressPhoto == null
            || string.IsNullOrWhiteSpace(expressPhoto.ExpressTrackingNo)
            // ||expressPhoto.Order == null
            || expressPhoto.Customer == null
            || expressPhoto.Type == null)
        {
            return;
        }

        string trackingNo = expressPhoto.ExpressTrackingNo;
        ExpressPhotoType photoType = expressPhoto.Type;

        // 一个快递单，一个图片类型，同一时间段，只发送一个消息  
        LogisticsOrderPhotoMessageSendTimeInfo messageSendTimeRecord = session.QueryOver<LogisticsOrderPhotoMessageSendTimeInfo>()
            .Where(e => e.TrackingNo == trackingNo && e.Type == photoType)
            .OrderBy(e => e.LastSendTime).Desc
            .Take(1)
            .SingleOrDefault();

        if (messageSendTimeRecord == null || messageSendTimeRecord.LastSendTime < DateTime.Now.AddHours(-1)) // 没发送过，或者一个小时前发送过
        {
            CustomerMessageType customerMessageType = GetMessageType(expressPhoto.Type);
            if (customerMessageType != null && customerMessageType != CustomerMessageType.None)
            {
                if (messageSendTimeRecord == null)
                {
                    messageSendTimeRecord = new LogisticsOrderPhotoMessageSendTimeInfo();
                    messageSendTimeRecord.TrackingNo = trackingNo;
                    messageSendTimeRecord.Order = null;
                    messageSendTimeRecord.Express = null;
                    messageSendTimeRecord.Type = photoType;
                }

                messageSendTimeRecord.LastSendTime = DateTime.Now;

                session.SaveOrUpdate(messageSendTimeRecord);
                session.Flush();

                // 先保存messageSendTimeRecord，再发送消息，否则发送消息比较费时容易造成上面的保存出错
                SendMessageExpressPhoto(photoType, expressPhoto);
            }
        }
        */
    }

    private CustomerMessageType GetMessageType(ExpressPhotoType photoType) {
        /*
         todo a 后面补上
        if (photoType == ExpressPhotoType.SignIn)
        {
            return CustomerMessageType.OrderPhotoSignInTuiSong;
        }

        if (photoType == ExpressPhotoType.CheckIn)
        {
            return CustomerMessageType.OrderPhotoCheckInTuiSong;
        }

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

        if (photoType == ExpressPhotoType.Ship)
        {
            return CustomerMessageType.OrderPhotoShipTuiSong;
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

        return CustomerMessageType.None;
    }

    private void SendMessageOrderPhoto(ExpressPhotoType photoType, ExpressPhotoInfo expressPhoto) {
        /*
        if (photoType == ExpressPhotoType.None)
        {
            return;
        }

        if (expressPhoto == null || expressPhoto.Customer == null || expressPhoto.Order == null)
        {
            return;
        }

        // // 这两个消息应该改造成按快递单号发送
        //  
        // if (photoType == ExpressPhotoType.SignIn)
        // {
        //     if (expressPhoto.User?.LoginName == AutoPhotoConfig.AutoPhotoLoginName)
        //     {
        //         string sendUserName = expressPhoto.User?.UserName;
        //
        //         // 使用智能机器人作为发送人
        //         customerMessageService.SendMessageOrderPhotoSignIn(expressPhoto.Customer, expressPhoto.Order, sendUserName);
        //     }
        //     else
        //     {
        //         customerMessageService.SendMessageOrderPhotoSignIn(expressPhoto.Customer, expressPhoto.Order);
        //     }
        // }
        //
        // if (photoType == ExpressPhotoType.CheckIn)
        // {
        //     customerMessageService.SendMessageOrderPhotoCheckIn(expressPhoto.Customer, expressPhoto.Order);
        // }
        //
    

        if (photoType == ExpressPhotoType.Issue)
        {
            customerMessageService.SendMessageOrderPhotoIssue(expressPhoto.Customer, expressPhoto.Order);
        }

        if (photoType == ExpressPhotoType.CnDamage)
        {
            customerMessageService.SendMessageOrderPhotoCnDamage(expressPhoto.Customer, expressPhoto.Order);
        }

        if (photoType == ExpressPhotoType.Return)
        {
            customerMessageService.SendMessageOrderPhotoReturn(expressPhoto.Customer, expressPhoto.Order);
        }

        if (photoType == ExpressPhotoType.Ship)
        {
            customerMessageService.SendMessageOrderPhotoShip(expressPhoto.Customer, expressPhoto.Order);
        }

        if (photoType == ExpressPhotoType.AuDamage)
        {
            customerMessageService.SendMessageOrderPhotoAuDamage(expressPhoto.Customer, expressPhoto.Order);
        }

        if (photoType == ExpressPhotoType.Deliver)
        {
            customerMessageService.SendMessageOrderPhotoDeliver(expressPhoto.Customer, expressPhoto.Order);
        }
        */
    }

    private void SendMessageExpressPhoto(ExpressPhotoType photoType, ExpressPhotoInfo expressPhoto) {
        /*
        if (photoType == ExpressPhotoType.SignIn)
        {
            if (expressPhoto.User?.LoginName == AutoPhotoConfig.AutoPhotoLoginName)
            {
                string sendUserName = expressPhoto.User?.UserName;

                // 使用智能机器人作为发送人
                customerMessageService.SendMessageOrderPhotoSignIn(expressPhoto.Customer, expressPhoto.ExpressTrackingNo, sendUserName);
            }
            else
            {
                customerMessageService.SendMessageOrderPhotoSignIn(expressPhoto.Customer, expressPhoto.ExpressTrackingNo);
            }
        }

        if (photoType == ExpressPhotoType.CheckIn)
        {
            customerMessageService.SendMessageOrderPhotoCheckIn(expressPhoto.Customer, expressPhoto.ExpressTrackingNo);
        }
        */
    }
}