using System.Collections.Generic;
using System.Linq;
using Core;
using Domain.LogisticsOrder;
using NHibernate;
using NHibernate.Linq;

namespace Service.LogisticsOrder
{
    [Service(ServiceType.Scoped)]
    public class LogisticsExpressService
    {
        private ISession session = null;

        public LogisticsExpressService(ISession session)
        {
            this.session = session;
        }

        public IList<LogisticsExpressItem> GetLogisticsExpressItemList(string expressTrackingNo)
        {
            LogisticsExpressInfo logisticsExpress = session.Query<LogisticsExpressInfo>()
                .Where(e => e.TrackingNo == expressTrackingNo)
                .Take(1)
                .SingleOrDefault();

            IList<LogisticsExpressItem> expressItemList = new List<LogisticsExpressItem>();
            if (logisticsExpress == null)
            {
                return expressItemList;
            }

            int logisticsExpressId = logisticsExpress.Id;
            IList<LogisticsExpressChildInfo> expressChildList = session.Query<LogisticsExpressChildInfo>()
                .Where(e => e.Parent.Id == logisticsExpressId)
                .ToList();

            return GetLogisticsExpressItemList(new List<LogisticsExpressInfo>() {logisticsExpress}, expressChildList);
        }

        public IList<LogisticsExpressItem> GetLogisticsExpressItemList(IList<string> expressTrackingNoList)
        {
            IList<LogisticsExpressInfo> logisticsExpressList = session.Query<LogisticsExpressInfo>()
                .Where(e => expressTrackingNoList.Contains(e.TrackingNo))
                .ToList();

            IList<int> logisticsExpressIdList = logisticsExpressList.Select(e => e.Id).ToList();
            IList<LogisticsExpressChildInfo> expressChildListAll = session.Query<LogisticsExpressChildInfo>()
                .Fetch(e => e.Parent)
                .Where(e => logisticsExpressIdList.Contains(e.Parent.Id))
                .ToList();

            return GetLogisticsExpressItemList(logisticsExpressList, expressChildListAll);
        }

        public IList<LogisticsExpressItem> GetLogisticsExpressItemList(IList<LogisticsExpressInfo> logisticsExpressList, IList<LogisticsExpressChildInfo> expressChildList)
        {
            logisticsExpressList = logisticsExpressList.OrderBy(e => e.Id).ToList(); // 按 ID 排序让顺序固定
            expressChildList = expressChildList.OrderBy(e => e.Id).ToList(); // 按 ID 排序让顺序固定

            IList<LogisticsExpressItem> logisticsExpressItemList = new List<LogisticsExpressItem>();

            foreach (LogisticsExpressInfo logisticsExpress in logisticsExpressList)
            {
                if (logisticsExpress.Width != null)
                {
                    LogisticsExpressItem expressItem = new LogisticsExpressItem();

                    expressItem.LogisticsExpressId = logisticsExpress.Id;
                    expressItem.ChildExpressId = null;
                    expressItem.ExpressTrackingNo = logisticsExpress.TrackingNo;
                    expressItem.ChildTrackingNo = logisticsExpress.TrackingNo;
                    expressItem.Width = logisticsExpress.Width ?? 0;
                    expressItem.Length = logisticsExpress.Length ?? 0;
                    expressItem.Height = logisticsExpress.Height ?? 0;
                    expressItem.SingleWeight = logisticsExpress.Weight ?? 0;
                    expressItem.Quantity = logisticsExpress.PackageCount ?? 1;

                    logisticsExpressItemList.Add(expressItem);
                }

                IList<LogisticsExpressChildInfo> expressChildListForExpress = expressChildList.Where(e => e.Parent.Id == logisticsExpress.Id).ToList();

                foreach (LogisticsExpressChildInfo expressChild in expressChildListForExpress)
                {
                    LogisticsExpressItem expressItem = new LogisticsExpressItem();

                    if (expressChild.Weight != null)
                    {
                        expressItem.LogisticsExpressId = logisticsExpress.Id;
                        expressItem.ChildExpressId = expressChild.Id;
                        expressItem.ExpressTrackingNo = logisticsExpress.TrackingNo;
                        expressItem.ChildTrackingNo = expressChild.TrackingNo;
                        expressItem.Width = expressChild.Width ?? 0;
                        expressItem.Length = expressChild.Length ?? 0;
                        expressItem.Height = expressChild.Height ?? 0;
                        expressItem.SingleWeight = expressChild.Weight ?? 0;
                        expressItem.Quantity = expressChild.PackageCount ?? 1;

                        logisticsExpressItemList.Add(expressItem);
                    }
                }
            }

            logisticsExpressItemList = logisticsExpressItemList.OrderBy(e => e.ExpressTrackingNo).ThenBy(e => e.ChildTrackingNo).ToList();

            return logisticsExpressItemList;
        }
    }
}