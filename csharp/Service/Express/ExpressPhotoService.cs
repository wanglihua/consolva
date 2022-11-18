using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Express;
using shunda.Domain.Message;
using shunda.Domain.LogisticsOrder;

namespace shunda.Service.Express;

/// <summary>
/// 这个类大部分处理数据业务处理相关的
/// 跟图片和文件有关的逻辑放在 ExpressPhotoFileService 中
/// </summary>
[Service(ServiceType.Scoped)]
public class ExpressPhotoService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;

    // private CustomerMessageService customerMessageService = null;
    private ExpressPhotoFileService expressPhotoFileService;

    public ExpressPhotoService(ISession session,
        // CustomerMessageService customerMessageService,
        ExpressPhotoFileService expressPhotoFileService)
    {
        this.session = session;
        // this.customerMessageService = customerMessageService;
        this.expressPhotoFileService = expressPhotoFileService;
    }

    public IList<ExpressPhotoInfo> GetExpressPhotoList(IList<string> trackingNoList)
    {
        if (trackingNoList == null || trackingNoList.Count == 0)
        {
            return new List<ExpressPhotoInfo>();
        }

        IList<ExpressPhotoInfo> orderPhotoList = session.Query<ExpressPhotoInfo>()
            .Where(e => trackingNoList.Contains(e.TrackingNo))
            .OrderBy(e => e.Type)
            .ToList();

        foreach (ExpressPhotoInfo orderPhoto in orderPhotoList)
        {
            orderPhoto.BigImageFullUrlCache = expressPhotoFileService.GetBigImageFullUrlForSite(orderPhoto, OssSiteLocType.China);
            orderPhoto.SmallImageFullUrlCache = expressPhotoFileService.GetSmallImageFullUrlForSite(orderPhoto, OssSiteLocType.China);
        }

        return orderPhotoList;
    }

    // todo b 调用
    /// <summary>
    /// 绑定图片到快递单号，添加和更新图片时调用
    /// </summary>
    /// <param name="photoId"></param>
    public void BindPhotoToExpress(int photoId)
    {
        ExpressPhotoInfo? expressPhoto = session.Get<ExpressPhotoInfo>(photoId);
        if (expressPhoto == null)
        {
            throw new ValidationException($"指定的快递单号图片#{photoId}不存在！");
        }

        string trackingNo = expressPhoto.TrackingNo.Trim();

        ExpressInfo? express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        BindPhotoToExpress(photoId, express);
    }

    /// <summary>
    /// 绑定图片到快递单号，添加和更新图片时调用
    /// express == null 时，也可以用来解绑
    /// </summary>
    /// <param name="photoId"></param>
    /// <param name="express">可以为空</param>
    public void BindPhotoToExpress(int photoId, ExpressInfo? express)
    {
        ExpressPhotoInfo? expressPhoto = session.Get<ExpressPhotoInfo>(photoId);
        if (expressPhoto == null)
        {
            throw new ValidationException($"指定的快递单号图片#{photoId}不存在！");
        }

        if (express == null)
        {
            expressPhoto.Express = null;
            expressPhoto.Customer = null;
            expressPhoto.Order = null;
            expressPhoto.OrderCode = String.Empty;
        }
        else
        {
            expressPhoto.Express = express;
            expressPhoto.Customer = express.Customer; // 有可能为 null，也有可能不是
            expressPhoto.Order = express.Order; // 有可能为 null，也有可能不是
            if (express.Order != null)
            {
                expressPhoto.OrderCode = express.Order?.Code ?? string.Empty;
            }
            else
            {
                expressPhoto.OrderCode = string.Empty;
            }
        }

        session.Update(expressPhoto);
        session.Flush();
    }

    /// <summary>
    /// 绑定图片到订单，用于订单图片的情形，就是 express 为空时
    /// </summary>
    /// <param name="photoId"></param>
    /// <param name="order"></param>
    public void BindPhotoToOrder(int photoId, LogisticsOrderInfo order)
    {
        ExpressPhotoInfo? expressPhoto = session.Get<ExpressPhotoInfo>(photoId);
        if (expressPhoto == null)
        {
            throw new ValidationException($"指定的快递单号图片#{photoId}不存在！");
        }

        expressPhoto.Order = order;
        expressPhoto.OrderCode = order?.Code ?? string.Empty;
        expressPhoto.Customer = order?.Customer;

        session.Save(expressPhoto);
        session.Flush();
    }

    // todo b 调用
    /// <summary>
    /// 绑定快递单号到图片，添加和更新快递单时调用
    /// </summary>
    /// <param name="expressId"></param>
    public void BindExpressToPhoto(int expressId)
    {
        ExpressInfo? express = session.Get<ExpressInfo>(expressId);
        if (express == null)
        {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        string trackingNo = express.TrackingNo.Trim();

        IList<ExpressPhotoInfo> expressPhotoList = session.Query<ExpressPhotoInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .ToList();

        foreach (ExpressPhotoInfo expressPhoto in expressPhotoList)
        {
            expressPhoto.Express = express;
            expressPhoto.Customer = express.Customer; // 有可能为 null，也有可能不是
            expressPhoto.Order = express.Order; // 有可能为 null，也有可能不是
            if (express.Order != null)
            {
                expressPhoto.OrderCode = express.Order.Code ?? string.Empty;
            }
            else
            {
                expressPhoto.OrderCode = string.Empty;
            }

            session.Update(expressPhoto);
        }

        session.Flush();
    }

    // todo b 调用
    /// <summary>
    /// 给图片从快递单号解绑，用户快递单号删除的时候调用
    /// </summary>
    /// <param name="expressId"></param>
    public void UnbindPhotoFromExpress(int expressId)
    {
        IList<ExpressPhotoInfo> expressPhotoList = session.Query<ExpressPhotoInfo>()
            .Where(e => e.Express!.Id == expressId)
            .ToList();

        foreach (ExpressPhotoInfo expressPhoto in expressPhotoList)
        {
            expressPhoto.Express = null;
            expressPhoto.Customer = null;
            expressPhoto.Order = null;
            expressPhoto.OrderCode = String.Empty;

            session.Update(expressPhoto);
        }

        session.Flush();
    }

    /// <summary>
    /// 挂接 订单图片的 orderId 和 expressId
    /// 这里是针对，只有一个express_tracking_no 的数据，order_id, express_id 空缺的情况
    /// </summary>
    public void BindOrderPhotoEntity(bool sleepInRepeat, bool useInnerTransaction)
    {
        // todo e 这里做实时绑定，这个方法感觉也不需要了

        /*

        // 绑定订单

        string sqlBindOrder = @"
SELECT op.id          AS photo_id,
       oe.id          AS express_id,
       lo.id          AS order_id,
       lo.customer_id AS customer_id
FROM tbl_logistics_order_photo op
         INNER JOIN tbl_logistics_order_goods_express oe ON op.express_tracking_no = oe.express_tracking_no
         INNER JOIN tbl_logistics_order lo ON oe.order_id = lo.id
WHERE op.create_time > DATE_SUB(CURDATE(), INTERVAL 365 DAY)
  AND lo.create_time > DATE_SUB(CURDATE(), INTERVAL 10 DAY)
  AND op.order_id IS NULL
  AND lo.order_status_id != 100 # 100 LogisticsOrderStatus.Draft
  AND lo.code NOT LIKE 'DRAFT%'
  AND lo.order_status_id IS NOT NULL
";

        IList<object[]> photoOrderBindDataList = session.CreateSQLQuery(sqlBindOrder).List<object[]>();

        foreach (var bindData in photoOrderBindDataList)
        {
            ITransaction transaction = null;

            try
            {
                int photoId = (int)bindData[0];
                int expressId = (int)bindData[1];
                int orderId = (int)bindData[2];
                int customerId = (int)bindData[3];

                ExpressPhotoInfo? expressPhoto = session.Get<ExpressPhotoInfo>(photoId);
                if (expressPhoto == null)
                {
                    continue;
                }

                if (useInnerTransaction)
                {
                    transaction = session.BeginTransaction();
                }

                expressPhoto.Express = session.Load<LogisticsOrderGoodsExpressInfo>(expressId);
                expressPhoto.Order = session.Load<LogisticsOrderInfo>(orderId);
                expressPhoto.Customer = session.Load<CustomerInfo>(customerId);

                session.Update(expressPhoto);
                session.Flush();

                if (useInnerTransaction)
                {
                    transaction?.Commit();
                }

                // 先绑定在发送消息
                SendAddOrderPhotoMessage(expressPhoto);
            }
            catch (Exception exp)
            {
                if (useInnerTransaction)
                {
                    transaction?.Rollback();
                }

                log.Error(exp);
            }

            if (sleepInRepeat)
            {
                Thread.Sleep(JobConfig.SleepTimeOneRepeat);
            }

            session.Flush();
        }

        // 绑定客户
        // 这里的天数是不是可以做些调整
        string sqlBindCustomer = @"
SELECT op.id          AS photo_id,
       cg.customer_id AS customer_id
FROM tbl_logistics_order_photo op
         INNER JOIN tbl_logistics_claim_goods cg ON cg.tracking_no = op.express_tracking_no
WHERE op.create_time > DATE_SUB(CURDATE(), INTERVAL 100 DAY)
  AND cg.create_time > DATE_SUB(CURDATE(), INTERVAL 100 DAY)
  AND cg.status_id != 100 # 100 LogisticsClaimGoodsStatus.WaitPick
  AND cg.customer_id > 0
  AND op.customer_id IS NULL
";
        IList<object[]> photoCustomerBindDataList = session.CreateSQLQuery(sqlBindCustomer).List<object[]>();

        foreach (var bindData in photoCustomerBindDataList)
        {
            ITransaction transaction = null;

            try
            {
                int photoId = (int)bindData[0];
                int customerId = (int)bindData[1];

                ExpressPhotoInfo? expressPhoto = session.Get<ExpressPhotoInfo>(photoId);
                if (expressPhoto == null)
                {
                    continue;
                }

                if (useInnerTransaction)
                {
                    transaction = session.BeginTransaction();
                }

                expressPhoto.Customer = session.Load<CustomerInfo>(customerId);

                session.Update(expressPhoto);
                session.Flush();

                if (useInnerTransaction)
                {
                    transaction?.Commit();
                }

                // 先绑定在发送消息
                SendAddExpressPhotoMessage(expressPhoto);
            }
            catch (Exception exp)
            {
                if (useInnerTransaction)
                {
                    transaction?.Rollback();
                }

                log.Error(exp);
            }

            if (sleepInRepeat)
            {
                Thread.Sleep(JobConfig.SleepTimeOneRepeat);
            }

            session.Flush();
        }
        */
    }

    /// <summary>
    /// 发送订单图片添加消息通知
    /// </summary>
    /// <param name="expressPhoto"></param>
    public void SendAddOrderPhotoMessage(ExpressPhotoInfo expressPhoto)
    {
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
    public void SendAddExpressPhotoMessage(ExpressPhotoInfo expressPhoto)
    {
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

    private CustomerMessageType GetMessageType(ExpressPhotoType photoType)
    {
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

    private void SendMessageOrderPhoto(ExpressPhotoType photoType, ExpressPhotoInfo expressPhoto)
    {
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

    private void SendMessageExpressPhoto(ExpressPhotoType photoType, ExpressPhotoInfo expressPhoto)
    {
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