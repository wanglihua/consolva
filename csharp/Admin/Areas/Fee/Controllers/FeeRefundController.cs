using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Fee.Models.FeeRefund;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Fee;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.FeeRefund)]
public class FeeRefundController : Controller
{
    private ISession session;
    private CustomerCodeService customerCodeService;

    public FeeRefundController(ISession session, CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/fee/refund")]
    public IActionResult Index()
    {
        FeeRefundIndexModel model = new FeeRefundIndexModel();

        model.TypeList = FeeRefundType.GetList();
        model.StatusList = FeeRefundStatus.GetList();

        return View("FeeRefundIndex", model);
    }

    [HttpPost]
    [Route("/admin/fee/refund/list")]
    public IActionResult List(FeeRefundListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<FeeRefundInfo> logisticsOrderRefundList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<FeeRefundInfo>();

        foreach (FeeRefundInfo feeRefund in logisticsOrderRefundList)
        {
            feeRefund.CustomerCodeCache = customerCodeService.GetCustomerCode(feeRefund.Customer.Id);
        }

        FeeRefundListModel model = new FeeRefundListModel(listPager);
        model.FeeRefundList = logisticsOrderRefundList;

        // 汇总信息
        var sumData = GetListCriteria(queryParam)
            .SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count(Projections.Distinct(Projections.Property($"{nameof(FeeRefundInfo.Customer)}.{nameof(FeeRefundInfo.Customer.Id)}"))))
                    .Add(Projections.Count(Projections.Distinct(Projections.Property($"{nameof(FeeRefundInfo.Order)}.{nameof(FeeRefundInfo.Order.Id)}"))))
                    .Add(Projections.Sum(Projections.Property(nameof(FeeRefundInfo.Amount))))
            )
            .UniqueResult<object[]>();

        model.CustomerCount = (int)sumData[0];
        model.OrderCount = (int)sumData[1];
        model.AmountSum = (decimal)sumData[2];

        return View("FeeRefundList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(FeeRefundListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<FeeRefundInfo>();

        criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(FeeRefundInfo.Customer), nameof(FeeRefundInfo.Customer));
        criteria = criteria.CreateAlias(nameof(FeeRefundInfo.Order), nameof(FeeRefundInfo.Order));

        if (queryParam.TypeQuery != null && queryParam.TypeQuery != FeeRefundType.None)
        {
            criteria = criteria.Add(Restrictions.Where<FeeRefundInfo>(e => e.Type == queryParam.TypeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Like($"{nameof(FeeRefundInfo.Order)}.{nameof(FeeRefundInfo.Order.Code)}", orderCodeQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Eq($"{nameof(FeeRefundInfo.Customer)}.{nameof(FeeRefundInfo.Customer.Id)}", customerIdQuery));
        }

        string reasonQuery = queryParam.ReasonQuery;
        if (string.IsNullOrWhiteSpace(reasonQuery) == false)
        {
            reasonQuery = reasonQuery.Trim();
            criteria = criteria.Add(Restrictions.On<FeeRefundInfo>(e => e.ReasonContent).IsInsensitiveLike(reasonQuery, MatchMode.Anywhere));
        }

        if (queryParam.StatusQuery != null && queryParam.StatusQuery != FeeRefundStatus.None)
        {
            criteria = criteria.Add(Restrictions.Where<FeeRefundInfo>(e => e.Status == queryParam.StatusQuery));
        }

        if (queryParam.BeginDateQuery != null)
        {
            DateTime beginDateQueryValue = queryParam.BeginDateQuery.Value;
            criteria = criteria.Add(Restrictions.Where<FeeRefundInfo>(e => e.CreateTime >= beginDateQueryValue)); // 这里 CreateTime 和 UpdateTime 都可以
        }

        if (queryParam.EndDateQuery != null)
        {
            DateTime endDateQueryValue = queryParam.EndDateQuery.Value.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<FeeRefundInfo>(e => e.CreateTime < endDateQueryValue)); // 这里 CreateTime 和 UpdateTime 都可以
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/fee/refund/export")]
    public IActionResult Export(FeeRefundListQueryParam queryParam)
    {
        IList<FeeRefundInfo> refundList = GetListCriteria(queryParam)
            .AddOrder(new Order($"{nameof(FeeRefundInfo.Id)}", true))
            .List<FeeRefundInfo>();

        string fileName = "退赔款.xlsx";
        IWorkbook workbook = new XSSFWorkbook();

        ISheet sheet = workbook.CreateSheet("退赔款");

        int rowHeight = 22;
        ICellStyle headerCenterCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook, HorizontalAlignment.Center);
        ICellStyle contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);
        ICellStyle contentCenterCellStyle = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Center);

        IDataFormat dataFormat = workbook.CreateDataFormat();

        ICellStyle precision0Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision0Style.DataFormat = dataFormat.GetFormat("0");

        ICellStyle precision1Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision1Style.DataFormat = dataFormat.GetFormat("0.0");

        ICellStyle precision2Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision2Style.DataFormat = dataFormat.GetFormat("0.00");

        ICellStyle precision3Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision3Style.DataFormat = dataFormat.GetFormat("0.000");

        ICellStyle precision6Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision6Style.DataFormat = dataFormat.GetFormat("0.000000");

        int rowIndex = 0;
        int cellIndex = 0;

        // row 
        IRow row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        cellIndex = 0;

        // 客户    订单    类型    退赔金额    退赔款原因    退赔支付方式    状态    生成时间    

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        ICell cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("客户");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("订单");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("类型");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("退赔金额");

        sheet.SetColumnWidth(cellIndex, 50 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("退赔款原因");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("退赔支付方式");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("状态");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("生成时间");

        foreach (FeeRefundInfo refund in refundList)
        {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 客户    订单    类型    退赔金额    退赔款原因    退赔支付方式    状态    生成时间    

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(customerCodeService.GetCustomerCode(refund.Customer.Id));

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(refund.Order?.Code);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(refund.Type?.Name);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = precision2Style;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue((double)refund.Amount);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(refund.ReasonContent);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(refund.PayType?.Name);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(refund.Status?.Name);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(refund.CreateTime.ToString("yyyy-MM-dd HH:mm"));
        }


        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return File(memoryStream.ToArray(), ContentType.Xlsx, fileName);
    }
}