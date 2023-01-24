using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.Express.Models.ExpressRemark;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressList)]
public class ExpressRemarkController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly ExpressRemarkService expressRemarkService;

    public ExpressRemarkController(ISession session,
        LoginUserService loginUserService,
        ExpressRemarkService expressRemarkService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.expressRemarkService = expressRemarkService;
    }

    [HttpPost]
    [Route("/admin/express/remark/detail")]
    public IActionResult Detail(int expressId) {
        session.DefaultReadOnly = true;

        ExpressInfo express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单#{expressId}不存在！");
        }

        ExpressRemarkInfo expressRemark = new ExpressRemarkInfo();

        ExpressRemarkDetailModel model = new ExpressRemarkDetailModel {
            Id = expressRemark.Id,
            Content = expressRemark.Content,
            ExpressId = express.Id,
            TrackingNo = express.TrackingNo,
        };

        return View("ExpressRemarkDetail", model);
    }

    [HttpPost]
    [Route("/admin/express/remark/save")]
    public IActionResult Save(ExpressRemarkSaveModel model) {
        int expressId = model.ExpressId;
        ExpressInfo express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单#{expressId}不存在！");
        }

        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        int expressRemarkId = model.Id;

        bool isCreate = expressRemarkId == 0;

        ExpressRemarkInfo expressRemark;

        if (isCreate) {
            expressRemark = new ExpressRemarkInfo();
        } else {
            expressRemark = session.Get<ExpressRemarkInfo>(expressRemarkId);
            if (expressRemark == null) {
                throw new ValidationException($"指定的快递单备注#{expressRemarkId}不存在！");
            }
        }

        expressRemark.Type = ExpressRemarkType.Service;
        expressRemark.Express = express;
        expressRemark.Content = model.Content;
        expressRemark.AddUser = loginUserService.GetLoginUser();

        if (isCreate) {
            session.Save(expressRemark);
        } else {
            session.Update(expressRemark);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }
}