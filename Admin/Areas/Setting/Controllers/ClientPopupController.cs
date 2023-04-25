using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Client;
using shunda.Admin.Areas.Setting.Models.ClientPopup;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Setting.Controllers;

/// <summary>
/// 客户端弹窗 控制器
/// </summary>
[SettingArea]
[AdminAuth(PassRes.Setting.ClientPopup)]
public class ClientPopupController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public ClientPopupController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/setting/client/popup")]
    public IActionResult Index() {
        ClientPopupIndexModel model = new ClientPopupIndexModel();
        return View("ClientPopupIndex", model);
    }

    [HttpPost]
    [Route("/admin/setting/client/popup/list")]
    public IActionResult List(ClientPopupListQueryParam queryParam) {
        var listQuery = GetQueryOver(queryParam);
        listQuery = listQuery.ThenByAlias(() => clientPopupAlias!.ShouldPopup).Desc;

        IList<ClientPopupInfo> clientPopupList = listQuery
            .List<ClientPopupInfo>();

        /*
        foreach (ClientPopupInfo clientPopup in clientPopupList) {
        }
        */

        ClientPopupListModel model = new ClientPopupListModel();
        model.ClientPopupList = clientPopupList;
        model.ClientPopupAlias = nameof(clientPopupAlias);

        return View("ClientPopupList", model);
    }

    private ClientPopupInfo? clientPopupAlias = null;

    [NonAction]
    private IQueryOver<ClientPopupInfo, ClientPopupInfo> GetQueryOver(ClientPopupListQueryParam queryParam) {
        var query = session.QueryOver<ClientPopupInfo>(() => clientPopupAlias!);

        return query;
    }

    [HttpGet]
    [Route("/admin/setting/client/popup/detail/{id:int}")]
    public IActionResult Detail(int id) {
        ClientPopupInfo? clientPopup;

        if (id <= 0) {
            clientPopup = new ClientPopupInfo();
        } else {
            clientPopup = session.Get<ClientPopupInfo>(id);
            if (clientPopup == null) {
                throw new ValidationException($"指定的 客户端弹窗#{id} 不存在！");
            }
        }

        ClientPopupDetailModel model = new ClientPopupDetailModel {
            ClientPopupId = clientPopup.Id,
            ShouldPopup = clientPopup.ShouldPopup,
            Content = clientPopup.Content
        };

        return View("ClientPopupDetail", model);
    }

    [HttpPost]
    [Route("/admin/setting/client/popup/save")]
    [Transactional]
    public IActionResult Save(ClientPopupSaveModel model) {
        ValidationUtil.Validate<ClientPopupSaveModelValidator, ClientPopupSaveModel>(model);

        int clientPopupId = model.ClientPopupId;

        bool isCreate = clientPopupId <= 0;

        ClientPopupInfo clientPopup;

        if (isCreate) {
            clientPopup = new ClientPopupInfo();
        } else {
            clientPopup = session.Get<ClientPopupInfo>(clientPopupId);
            if (clientPopup == null) {
                throw new ValidationException($"指定的 客户端弹窗#{clientPopupId} 不存在！");
            }
        }

        bool shouldPopup = model.ShouldPopup ?? false;
        if (shouldPopup) {
            // 如果当前这个是要弹出的，就把别的都改成不弹出
            var clientPopupList = session.Query<ClientPopupInfo>().ToList();
            foreach (var cp in clientPopupList) {
                cp.ShouldPopup = false;
                session.Update(cp);
            }

            session.Flush();
        }

        clientPopup.ShouldPopup = shouldPopup;

        if (clientPopup.Content != model.Content) {
            clientPopup.Content = model.Content;
            clientPopup.EditTime = DateTime.Now;
        }

        if (isCreate) {
            session.Save(clientPopup);
        } else {
            session.Update(clientPopup);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
            Data = new {
                create = isCreate,
                id = clientPopup.Id,
            }
        });
    }

    [HttpPost]
    [Route("/admin/setting/client/popup/delete")]
    [Transactional]
    public IActionResult Delete(IdListParam param) {
        param.Validate("请选择要删除的客户端弹窗！");

        var idList = param.IdList;

        foreach (int id in idList) {
            ClientPopupInfo clientPopup = session.Load<ClientPopupInfo>(id);
            session.Delete(clientPopup);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "删除成功！",
        });
    }
}