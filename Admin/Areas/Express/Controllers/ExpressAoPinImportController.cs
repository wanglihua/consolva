using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Admin.Areas.Express.Models.ExpressAoPinImport;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressList)]
public class ExpressAoPinImportController : Controller {
    private readonly ISession session;
    private readonly CheckinTrackingNoService checkinTrackingNoService;

    public ExpressAoPinImportController(ISession session, CheckinTrackingNoService checkinTrackingNoService) {
        this.session = session;
        this.checkinTrackingNoService = checkinTrackingNoService;
    }

    [HttpGet]
    [Route("/admin/express/ao/pin/import")]
    public IActionResult Index() {
        return View("ExpressAoPinImportIndex");
    }

    [HttpPost]
    [Route("/admin/express/ao/pin/import/do")]
    public IActionResult Import(IFormFile formFile) {
        var streamReader = new StreamReader(formFile.OpenReadStream());
        string json = streamReader.ReadToEnd();
        var expressImportList = JsonConvert.DeserializeObject<List<ExpressAoPinImportExpress>>(json);

        if (expressImportList == null) {
            throw new ValidationException("未能解析接受数据！");
        }

        foreach (var ei in expressImportList) {
            string trackingNo = ei.TrackingNo;
            var expressInDb = session.Query<ExpressInfo>().Where(e => e.TrackingNo == trackingNo).Take(1).SingleOrDefault();
            if (expressInDb != null) {
                continue;
            }

            ExpressStatus expressStatus = ExpressStatus.WaitPick;
            if (ei.CustomerId != 0) {
                expressStatus = ExpressStatus.Checkin;
            }
            
            DateTime? claimTime = 

            var express = new ExpressInfo {
                TrackingNo = ei.TrackingNo,
                CheckinTrackingNo = checkinTrackingNoService.GetNextMainTrackinigNo(),
                Status = expressStatus,
                ShelfStatus = ExpressShelfStatus.ToOnShelf,
                AddTime = ei.AddTime,
                
            };
        }

        Console.WriteLine(json);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}