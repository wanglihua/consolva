using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Fee.Models.ManualDecreaseFee;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Fee;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.ManualDecreaseFee)]
public class ManualDecreaseFeeController : Controller
{
    private ISession session;
    private CustomerCodeService customerCodeService;

    public ManualDecreaseFeeController(ISession session,
        CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/manual/decrease/fee")]
    public IActionResult Index()
    {
        ManualDecreaseFeeIndexModel model = new ManualDecreaseFeeIndexModel();

        model.FeeReasonList = session.Query<ManualFeeReasonInfo>().Where(e => e.FeeType == ManualFeeType.ManualDecrease).ToList();

        return View("ManualDecreaseFeeIndex", model);
    }

    [HttpPost]
    [Route("/admin/manual/decrease/fee/list")]
    public IActionResult List(ManualDecreaseFeeListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsOrderFeeInfo> logisticsOrderFeeList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsOrderFeeInfo>();

        foreach (LogisticsOrderFeeInfo orderFee in logisticsOrderFeeList)
        {
            orderFee.CustomerCodeCache = customerCodeService.GetCustomerCode(orderFee.Customer.Id);
        }

        ManualDecreaseFeeListModel model = new ManualDecreaseFeeListModel(listPager);
        model.LogisticsOrderFeeList = logisticsOrderFeeList;

        // 汇总信息
        var sumData = GetListCriteria(queryParam)
            .SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.Count(Projections.Distinct(Projections.Property($"{nameof(LogisticsOrderFeeInfo.Customer)}.{nameof(LogisticsOrderFeeInfo.Customer.Id)}"))))
                    .Add(Projections.Count(Projections.Distinct(Projections.Property($"{nameof(LogisticsOrderFeeInfo.Order)}.{nameof(LogisticsOrderFeeInfo.Order.Id)}"))))
                    .Add(Projections.Sum(Projections.Property(nameof(LogisticsOrderFeeInfo.Fee))))
            )
            .UniqueResult<object[]>();

        model.CustomerCount = (int)sumData[0];
        model.OrderCount = (int)sumData[1];
        model.FeeSum = (decimal)sumData[2];

        return View("ManualDecreaseFeeList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ManualDecreaseFeeListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<LogisticsOrderFeeInfo>();

        criteria = criteria.CreateAlias(nameof(LogisticsOrderFeeInfo.Order), nameof(LogisticsOrderFeeInfo.Order));
        // criteria = criteria.CreateAlias(nameof(LogisticsOrderFeeInfo.Customer), nameof(LogisticsOrderFeeInfo.Customer));

        criteria = criteria.Add(Restrictions.Where<LogisticsOrderFeeInfo>(e => e.Type == LogisticsOrderFeeType.ManualDecrease));

        criteria = criteria.SetReadOnly(true);

        if (queryParam.ManualReasonIdQuery != 0)
        {
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderFeeInfo>(e => e.ManualReason!.Id == queryParam.ManualReasonIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Like($"{nameof(LogisticsOrderFeeInfo.Order)}.{nameof(LogisticsOrderFeeInfo.Order.Code)}", orderCodeQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsOrderFeeInfo.Customer)}.{nameof(LogisticsOrderFeeInfo.Customer.Id)}", customerIdQuery));
        }

        if (queryParam.BeginDateQuery != null)
        {
            DateTime beginDateQueryValue = queryParam.BeginDateQuery.Value;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderFeeInfo>(e => e.CreateTime >= beginDateQueryValue));
        }

        if (queryParam.EndDateQuery != null)
        {
            DateTime endDateQueryValue = queryParam.EndDateQuery.Value.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderFeeInfo>(e => e.CreateTime < endDateQueryValue));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/manual/decrease/fee/export")]
    public IActionResult Export(ManualDecreaseFeeListQueryParam queryParam)
    {
        IList<LogisticsOrderFeeInfo> feeList = GetListCriteria(queryParam)
            .AddOrder(new Order($"{nameof(LogisticsOrderFeeInfo.Id)}", true))
            .List<LogisticsOrderFeeInfo>();

        foreach (LogisticsOrderFeeInfo orderFee in feeList)
        {
            orderFee.CustomerCodeCache = customerCodeService.GetCustomerCode(orderFee.Customer.Id);
        }

        string fileName = "减少费用.xlsx";
        IWorkbook workbook = new XSSFWorkbook();

        ISheet sheet = workbook.CreateSheet("减少费用");

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

        // 客户 订单 费用类型 金额 减少费用原因 描述 操作用户 生成时间

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        ICell cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell?.SetCellType(CellType.String);
        cell.SetCellValue("客户");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell?.SetCellType(CellType.String);
        cell.SetCellValue("订单");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell?.SetCellType(CellType.String);
        cell.SetCellValue("费用类型");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell?.SetCellType(CellType.String);
        cell.SetCellValue("金额");

        sheet.SetColumnWidth(cellIndex, 50 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell?.SetCellType(CellType.String);
        cell.SetCellValue("减少费用原因");

        sheet.SetColumnWidth(cellIndex, 50 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell?.SetCellType(CellType.String);
        cell.SetCellValue("描述");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell?.SetCellType(CellType.String);
        cell.SetCellValue("操作用户");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell?.SetCellType(CellType.String);
        cell.SetCellValue("生成时间");

        foreach (LogisticsOrderFeeInfo fee in feeList)
        {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 客户 订单 费用类型 金额 减少费用原因 描述 操作用户 生成时间    

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue(fee.CustomerCodeCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue(fee.Order?.Code);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue(fee.Type?.Name);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = precision2Style;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue((double)fee.Fee);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue(fee.ManualReasonContent);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue(fee.Description);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue($"{fee.ManualUser?.LoginName}({fee.ManualUser?.UserName})");

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue(fee.CreateTime.ToString("yyyy-MM-dd HH:mm"));
        }


        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return File(memoryStream.ToArray(), ContentType.Xlsx, fileName);
    }
}