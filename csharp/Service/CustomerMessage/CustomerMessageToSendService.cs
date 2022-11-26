using System.Linq;
using Core;
using Domain.Customer;
using Domain.LogisticsOrder;
using Domain.Message;
using NHibernate;
using NHibernate.Linq;

namespace shunda.Service.CustomerMessage
{
    [Service(ServiceType.Scoped)]
    public class CustomerMessageToSendService
    {
        private ISession session = null;

        public CustomerMessageToSendService(ISession session)
        {
            this.session = session;
        }

        /// <summary>
        /// 目的仓批次可自提推送
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="order"></param>
        /// <param name="shipContainerCustomerCode"></param>
        public void AddMessageMuDiCangPiCiKeZiTi(CustomerInfo customer, LogisticsOrderInfo order, string shipContainerCustomerCode)
        {
            // Customs/AQIS Processing 的，就不添加这个消息发送定时
            int orderId = order.Id;
            var orderExtra = session.Query<LogisticsOrderExtraInfo>()
                .Where(e => e.LogisticsOrder.Id == orderId)
                .Take(1)
                .SingleOrDefault();
            if (orderExtra != null && orderExtra.CustomsAqisProcessing == true)
            {
                return;
            }

            CustomerMessageToSendInfo customerMessageToSend = new CustomerMessageToSendInfo();
            customerMessageToSend.Type = CustomerMessageType.MuDiCangPiCiKeZiTiTuiSong;
            customerMessageToSend.Customer = customer;
            customerMessageToSend.Order = order;
            customerMessageToSend.ShipContainerCustomerCode = shipContainerCustomerCode;

            session.Save(customerMessageToSend);
            session.Flush();
        }

        /// <summary>
        /// 取货点可取货推送
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="order"></param>
        /// <param name="shipContainerCustomerCode"></param>
        public void AddMessageQuHuoDianKeQuHuo(CustomerInfo customer, LogisticsOrderInfo order, string shipContainerCustomerCode)
        {
            // Customs/AQIS Processing 的，就不添加这个消息发送定时
            int orderId = order.Id;
            var orderExtra = session.Query<LogisticsOrderExtraInfo>()
                .Where(e => e.LogisticsOrder.Id == orderId)
                .Take(1)
                .SingleOrDefault();
            if (orderExtra != null && orderExtra.CustomsAqisProcessing == true)
            {
                return;
            }

            CustomerMessageToSendInfo customerMessageToSend = new CustomerMessageToSendInfo();
            customerMessageToSend.Type = CustomerMessageType.QuHuoDianKeQuHuoTuiSong;
            customerMessageToSend.Customer = customer;
            customerMessageToSend.Order = order;
            customerMessageToSend.ShipContainerCustomerCode = shipContainerCustomerCode;

            session.Save(customerMessageToSend);
            session.Flush();
        }

        /// <summary>
        /// 即将送货上门推送
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="order"></param>
        /// <param name="shipContainerCustomerCode"></param>
        public void AddMessageJiJiangSongHuoShangMen(CustomerInfo customer, LogisticsOrderInfo order, string shipContainerCustomerCode)
        {
            // Customs/AQIS Processing 的，就不添加这个消息发送定时
            int orderId = order.Id;
            var orderExtra = session.Query<LogisticsOrderExtraInfo>()
                .Where(e => e.LogisticsOrder.Id == orderId)
                .Take(1)
                .SingleOrDefault();
            if (orderExtra != null && orderExtra.CustomsAqisProcessing == true)
            {
                return;
            }

            CustomerMessageToSendInfo customerMessageToSend = new CustomerMessageToSendInfo();
            customerMessageToSend.Type = CustomerMessageType.JiJiangSongHuoShangMenTuiSong;
            customerMessageToSend.Customer = customer;
            customerMessageToSend.Order = order;
            customerMessageToSend.ShipContainerCustomerCode = shipContainerCustomerCode;

            session.Save(customerMessageToSend);
            session.Flush();
        }

        public void DeleteMessage(LogisticsOrderInfo order, CustomerMessageType customerMessageType)
        {
            int orderId = order?.Id ?? 0;
            session.Query<CustomerMessageToSendInfo>()
                .Where(e => e.Order.Id == orderId && e.Type == customerMessageType)
                .Delete();
        }
    }
}