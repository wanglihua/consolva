using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Misc.Models.Notice;
using shunda.Client.Services;
using shunda.Core.Web;
using shunda.Domain.Client;
using shunda.Service.Client;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Misc.Controllers;

[MiscArea]
public class NoticeController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CustomerClientContentViewRecordService customerClientContentViewRecordService;

    public NoticeController(
        ISession session,
        LoginCustomerService loginCustomerService,
        CustomerClientContentViewRecordService customerClientContentViewRecordService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.customerClientContentViewRecordService = customerClientContentViewRecordService;
    }

    [HttpPost]
    [Route("/client/notice/list")]
    public IActionResult List([FromBody] NoticeListParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var listQuery = session.Query<ClientNoticeInfo>();

        if (param.LastId != ClientListLastIdModel.LAST_ID_NOT_SET_VALUE) {
            // 跟降序排序匹配
            int lastIdQuery = param.LastId;
            listQuery = listQuery.Where(e => e.Id < lastIdQuery); // 降序排序id大的会被先取得
        }

        var noticeList = listQuery
            .OrderByDescending(e => e.Id)
            .Take(15)
            .ToList();

        var noticeItemList = noticeList
            .Select(e => new NoticeListItem {
                Id = e.Id,
                Title = e.Title,
                Url = e.Url,
                Date = e.UpdateTime.ToString("yyyy-MM-dd"),
                Important = e.Important,
            })
            .ToList();

        var lastData = noticeList.LastOrDefault();
        int lastId = lastData?.Id ?? ClientListLastIdModel.LAST_ID_NO_MORE_DATA_VALUE;

        // 只需要这个时候设置就可以了，就是界面打开第一次加载列表的时候
        if (param.LastId == ClientListLastIdModel.LAST_ID_NOT_SET_VALUE) {
            customerClientContentViewRecordService.RecordView(loginCustomerId, ClientContentType.Notice);
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new NoticeListModel {
                NoticeList = noticeItemList,
                LastId = lastId,
            },
        });
    }
}