using Core;
using Core.Util;
using Domain;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Message;
using Domain.Pass;
using Service.Message;
using Web.Areas.Admin.Models.CustomerMessageRealTimeSendSetting;

namespace Web.Areas.Admin.Controllers
{
    [AdminArea]
    [AdminAuth(PassRes.CustomerMessage.RealTime)]
    public class CustomerMessageRealTimeSendSettingController : Controller
    {
        private ISession session = null;
        private CustomerMessageRealTimeSendSettingService customerMessageRealTimeSendSettingService = null;

        public CustomerMessageRealTimeSendSettingController(ISession session, CustomerMessageRealTimeSendSettingService customerMessageRealTimeSendSettingService)
        {
            this.session = session;
            this.customerMessageRealTimeSendSettingService = customerMessageRealTimeSendSettingService;
        }

        [HttpGet]
        [Route("/admin/customer/message/real/time/send/setting")]
        [NoHibernateAutoTransaction]
        public IActionResult Index()
        {
            CustomerMessageRealTimeSendSettingIndexModel model = new CustomerMessageRealTimeSendSettingIndexModel();

            customerMessageRealTimeSendSettingService.InitSetting();

            return View("CustomerMessageRealTimeSendSettingIndex", model);
        }

        [HttpPost]
        [Route("/admin/customer/message/real/time/send/setting/list")]
        [NoHibernateAutoTransaction]
        public IActionResult List(CustomerMessageRealTimeSendSettingListQueryParam queryParam)
        {
            int totalCount = GetListCriteria(queryParam)
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();

            ListPager listPager = new ListPager(queryParam, totalCount);

            IList<CustomerMessageRealTimeSendSettingInfo> customerMessageRealTimeSendSettingList = GetListCriteria(queryParam)
                .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
                .AddOrder(Order.Desc(nameof(Model.Id)))
                .SetFirstResult(listPager.PageIndex * listPager.PageSize)
                .SetMaxResults(listPager.PageSize)
                .List<CustomerMessageRealTimeSendSettingInfo>();

            foreach (CustomerMessageRealTimeSendSettingInfo customerMessageRealTimeSendSettingInfo in customerMessageRealTimeSendSettingList)
            {
                // customerMessageRealTimeSendSettingInfo
            }

            CustomerMessageRealTimeSendSettingListModel model = new CustomerMessageRealTimeSendSettingListModel(listPager);
            model.CustomerMessageRealTimeSendSettingList = customerMessageRealTimeSendSettingList;

            return View("CustomerMessageRealTimeSendSettingList", model);
        }

        [NonAction]
        private ICriteria GetListCriteria(CustomerMessageRealTimeSendSettingListQueryParam queryParam)
        {
            ICriteria criteria = session.CreateCriteria<CustomerMessageRealTimeSendSettingInfo>();

            criteria = criteria.SetReadOnly(true);

            if (string.IsNullOrWhiteSpace(queryParam.TypeNameQuery) == false)
            {
                string typeNameQuery = queryParam.TypeNameQuery.Trim();
                IList<CustomerMessageType> typeList = CustomerMessageType.GetList()
                    .Where(e => e.Name.Contains(typeNameQuery))
                    .ToList();

                criteria = criteria.Add(Restrictions.In($"{nameof(CustomerMessageRealTimeSendSettingInfo.Type)}", typeList.ToArray()));
            }

            return criteria;
        }

        [HttpPost]
        [Route("/admin/customer/message/real/time/send/setting/detail")]
        [NoHibernateAutoTransaction]
        public IActionResult Detail(int id)
        {
            session.DefaultReadOnly = true;

            CustomerMessageRealTimeSendSettingInfo customerMessageRealTimeSendSetting = session.Get<CustomerMessageRealTimeSendSettingInfo>(id);
            if (customerMessageRealTimeSendSetting == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }

            CustomerMessageRealTimeSendSettingDetailModel model = new CustomerMessageRealTimeSendSettingDetailModel();
            model.CustomerMessageRealTimeSendSetting = customerMessageRealTimeSendSetting;

            model.MessageTypeList = CustomerMessageType.GetList();
            model.StatusList = CustomerMessageSettingStatus.GetList();

            return View("CustomerMessageRealTimeSendSettingDetail", model);
        }

        [HttpPost]
        [Route("/admin/customer/message/real/time/send/setting/save")]
        [NoHibernateAutoTransaction]
        public IActionResult Save(CustomerMessageRealTimeSendSettingDetailModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
            }

            CustomerMessageRealTimeSendSettingInfo customerMessageRealTimeSendSettingFromUi = model.CustomerMessageRealTimeSendSetting;
            int customerMessageRealTimeSendSettingId = customerMessageRealTimeSendSettingFromUi.Id;

            CustomerMessageRealTimeSendSettingInfo customerMessageRealTimeSendSetting = session.Get<CustomerMessageRealTimeSendSettingInfo>(customerMessageRealTimeSendSettingId);
            if (customerMessageRealTimeSendSetting == null)
            {
                throw new ValidationException("配置信息不存在！");
            }

            customerMessageRealTimeSendSetting.WeiXinMessageTemplate1 = customerMessageRealTimeSendSettingFromUi.WeiXinMessageTemplate1;
            customerMessageRealTimeSendSetting.WeiXinMessageTemplate2 = customerMessageRealTimeSendSettingFromUi.WeiXinMessageTemplate2;
            customerMessageRealTimeSendSetting.WeiXinMessageTemplate3 = customerMessageRealTimeSendSettingFromUi.WeiXinMessageTemplate3;
            customerMessageRealTimeSendSetting.SmsMessageTemplate = customerMessageRealTimeSendSettingFromUi.SmsMessageTemplate;
            customerMessageRealTimeSendSetting.Status = customerMessageRealTimeSendSettingFromUi.Status;
            customerMessageRealTimeSendSetting.SmsSend = customerMessageRealTimeSendSettingFromUi.SmsSend;
            customerMessageRealTimeSendSetting.WeiXinSend = customerMessageRealTimeSendSettingFromUi.WeiXinSend;
            // 占位符 PlaceHolders 不在这里保存修改

            customerMessageRealTimeSendSetting.ClientCustomizable = customerMessageRealTimeSendSettingFromUi.ClientCustomizable;
            if (customerMessageRealTimeSendSettingFromUi.ClientTitle == null)
            {
                customerMessageRealTimeSendSettingFromUi.ClientTitle = String.Empty;
            }

            customerMessageRealTimeSendSetting.ClientTitle = customerMessageRealTimeSendSettingFromUi.ClientTitle;

            if (customerMessageRealTimeSendSettingFromUi.ClientDesc == null)
            {
                customerMessageRealTimeSendSettingFromUi.ClientDesc = string.Empty;
            }

            customerMessageRealTimeSendSetting.ClientDesc = customerMessageRealTimeSendSettingFromUi.ClientDesc;

            session.Update(customerMessageRealTimeSendSetting);

            session.Flush();

            return Json(new Dictionary<string, object>() {
                {"success", true},
                {"message", "保存成功！"}
            });
        }
    }
}