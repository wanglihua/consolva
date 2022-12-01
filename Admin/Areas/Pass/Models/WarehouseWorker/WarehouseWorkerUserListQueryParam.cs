using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.WarehouseWorker;

public class WarehouseWorkerUserListQueryParam : ListQueryParam
{
    public override int PageSize
    {
        get { return 10; }
    }

    public int WarehouseId { get; set; }

    [ModelBinder(typeof(WarehouseWorkerTypeModelBinder))]
    public WarehouseWorkerType? Type { get; set; }

    /// <summary>
    /// 登录名或者用户名查询
    /// </summary>
    public string? NameQuery { get; set; }

    /// <summary>
    /// 按群组查询
    /// </summary>
    public int GroupIdQuery { get; set; }
}