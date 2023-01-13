using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Setting.Models.ClientInstruction;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Client;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Setting.Controllers;

[SettingArea]
[AdminAuth(PassRes.Setting.ClientInstruction)]
public class ClientInstructionController : Controller {
    private ISession session;
    private LoginUserService loginUserService;

    public ClientInstructionController(ISession session, LoginUserService loginUserService) {
        this.session = session;
        this.loginUserService = loginUserService;
    }

    [HttpGet]
    [Route("/admin/client/instruction")]
    [Transactional]
    public IActionResult Index() {
        int loginUserId = loginUserService.GetLoginUserId();
        UserInfo loginUser = session.Get<UserInfo>(loginUserId);

        IList<ClientInstructionInfo> clientInstructionList = session.Query<ClientInstructionInfo>().ToList();

        foreach (ClientInstructionType type in ClientInstructionType.GetList()) {
            var clientInstruction = clientInstructionList
                .Where(e => e.Type == type)
                .Take(1)
                .SingleOrDefault();

            if (clientInstruction == null) {
                clientInstruction = new ClientInstructionInfo {
                    Type = type,
                    Title = type.Name,
                    Content = string.Empty,
                    Creator = loginUser
                };

                session.Save(clientInstruction);
            }
        }

        session.Flush();

        ClientInstructionIndexModel model = new ClientInstructionIndexModel();
        return View("ClientInstructionIndex", model);
    }

    [HttpPost]
    [Route("/admin/client/instruction/list")]
    public IActionResult List(ClientInstructionListQueryParam queryParam) {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ClientInstructionInfo> clientInstructionList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<ClientInstructionInfo>();

        foreach (ClientInstructionInfo clientInstructionInfo in clientInstructionList) {
            // clientInstructionInfo
        }

        ClientInstructionListModel model = new ClientInstructionListModel(listPager);
        model.ClientInstructionList = clientInstructionList;

        return View("ClientInstructionList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ClientInstructionListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<ClientInstructionInfo>();

        criteria = criteria.SetReadOnly(true);

        return criteria;
    }

    [HttpGet]
    [Route("/admin/client/instruction/detail/{id:int}")]
    public IActionResult Detail(int id) {
        session.DefaultReadOnly = true;

        ClientInstructionInfo clientInstruction = session.Get<ClientInstructionInfo>(id);
        if (clientInstruction == null) {
            throw new ValidationException("指定的数据项不存在！");
        }

        ClientInstructionDetailModel model = new ClientInstructionDetailModel {
            Id = clientInstruction.Id,
            Type = clientInstruction.Type,
            Title = clientInstruction.Title,
            Content = clientInstruction.Content,
        };

        return View("ClientInstructionDetail", model);
    }

    [HttpPost]
    [Route("/admin/client/instruction/save")]
    [Transactional]
    public IActionResult Save(ClientInstructionDetailModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        int clientInstructionId = model.Id;

        var clientInstruction = session.Get<ClientInstructionInfo>(clientInstructionId);
        if (clientInstruction == null) {
            throw new ValidationException($"指定的客户端文本菜单#{clientInstructionId}不存在！");
        }

        clientInstruction.Title = model.Title;
        clientInstruction.Content = model.Content;
        clientInstruction.PublishTime = DateTime.Now;

        clientInstruction.Creator = session.Load<UserInfo>(loginUserService.GetLoginUserId());

        session.Update(clientInstruction);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
            { "id", clientInstruction.Id }
        });
    }

    /*
     预置的，没有删除
    [HttpPost]
    [Route("/admin/client/instruction/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (int id in idList)
        {
            ClientInstructionInfo clientInstruction = session.Load<ClientInstructionInfo>(id);
            session.Delete(clientInstruction);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            {"success", true},
            {"message", "删除成功！"},
        });
    }
    */
}