using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Express.Models.ExpressReturn;
using shunda.Client.Models;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressReturnController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public ExpressReturnController(ISession session, LoginCustomerService loginCustomerService) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/express/return/list")]
    public IActionResult List([FromBody] ExpressReturnListParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        param.LoginCustomerId = loginCustomerId;

        var listQuery = GetListQuery(param);

        if (param.LastId != ListLastIdModel.LAST_ID_NOT_SET_VALUE) {
            // 跟降序排序匹配
            int lastIdQuery = param.LastId;
            listQuery = listQuery.Where(e => e.Id < lastIdQuery); // 降序排序id大的会被先取得
        }

        IList<ExpressReturnInfo> expressReturnList = listQuery
            .OrderByDescending(e => e.Id) // 按Id降序排序
            .Take(15)
            .ToList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new ExpressReturnListModel {
            },
        });
    }

    [NonAction]
    private IQueryable<ExpressReturnInfo> GetListQuery(ExpressReturnListParam param) {
        int loginCustomerId = param.LoginCustomerId;

        var query = session.Query<ExpressReturnInfo>()
            .Where(e => e.Customer.Id == loginCustomerId);

        if (string.IsNullOrWhiteSpace(param.Query) == false) {
            string queryStr = param.Query.Trim();
            query = query.Where(e => e.TrackingNo.Contains(queryStr)
                                     || e.CheckinTrackingNo.Contains(queryStr)
                                     || e.ReturnTrackingNo.Contains(queryStr));
        }

        return query;
    }

    /// <summary>
    /// todo 这里只做申请就可以退运，还没有集成退运支付
    /// </summary>
    [HttpPost]
    [Route("/client/express/return/apply")]
    [Transactional]
    public IActionResult Apply([FromBody] IdParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int expressId = param.Id;
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        if ((express.Customer?.Id ?? 0) != loginCustomerId) {
            throw new ValidationException($"指定的快递单号{express.TrackingNo}不属于当前登录会员#{loginCustomerId}！");
        }

        if (express.Status != ExpressStatus.CheckIn) {
            throw new ValidationException("指定的快递单号{express.TrackingNo}不是已入库状态！");
        }

        var expressReturn = session.Query<ExpressReturnInfo>()
            .Where(e => e.Express.Id == expressId)
            .Take(1)
            .SingleOrDefault();
        if (expressReturn != null) {
            throw new ValidationException($"快递单号{express.TrackingNo}已存在退运记录！");
        }

        // 更新 express 
        express.Status = ExpressStatus.WaitReturn;

        session.Update(express);
        session.Flush();

        // 创建 express return
        expressReturn = new ExpressReturnInfo {
            Express = express,
            TrackingNo = express.TrackingNo,
            CheckinTrackingNo = express.CheckinTrackingNo,
            ExpressCompany = express.ExpressCompany,
            GoodsType = express.GoodsType,
            AddTime = express.AddTime,
            AddUser = express.AddUser,
            Warehouse = express.Warehouse,
            Customer = express.Customer,
            Status = express.Status,
            ReturnTrackingNo = string.Empty,
            ReturnExpressCompany = string.Empty,
            ReturnApplyTime = DateTime.Now,
            ReturnPickupCode = string.Empty,
            ReturnUser = null,
            ReturnTime = null,
        };

        session.Save(expressReturn);
        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}