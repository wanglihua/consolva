using Core;
using Core.Util;
using Domain;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain.LogisticsOrder;
using Domain.Pass;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Service;
using Service.Customer;
using Web.Areas.Admin.Models.LogisticsOrderRefund;

namespace Web.Areas.Admin.Controllers
{
    [AdminArea]
    [AdminAuth(PassRes.OrderFee.Refund)]
    public class LogisticsOrderRefundController : Controller
    {
        private ISession session = null;
        private CustomerService customerService = null;

        public LogisticsOrderRefundController(ISession session, CustomerService customerService)
        {
            this.session = session;
            this.customerService = customerService;
        }

        [HttpGet]
        [Route("/admin/logistics/order/refund")]
        [NoHibernateAutoTransaction]
        public IActionResult Index()
        {
            LogisticsOrderRefundIndexModel model = new LogisticsOrderRefundIndexModel();

            model.TypeList = LogisticsOrderRefundType.GetList();
            model.StatusList = LogisticsOrderRefundStatus.GetList();

            return View("LogisticsOrderRefundIndex", model);
        }

        [HttpPost]
        [Route("/admin/logistics/order/refund/list")]
        [NoHibernateAutoTransaction]
        public IActionResult List(LogisticsOrderRefundListQueryParam queryParam)
        {
            int totalCount = GetListCriteria(queryParam)
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();

            ListPager listPager = new ListPager(queryParam, totalCount);

            IList<LogisticsOrderRefundInfo> logisticsOrderRefundList = GetListCriteria(queryParam)
                .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
                .AddOrder(Order.Desc(nameof(Model.Id)))
                .SetFirstResult(listPager.PageIndex * listPager.PageSize)
                .SetMaxResults(listPager.PageSize)
                .List<LogisticsOrderRefundInfo>();

            foreach (LogisticsOrderRefundInfo logisticsOrderRefundInfo in logisticsOrderRefundList)
            {
                // logisticsOrderRefundInfo
            }

            LogisticsOrderRefundListModel model = new LogisticsOrderRefundListModel(listPager);
            model.LogisticsOrderRefundList = logisticsOrderRefundList;

            // 汇总信息
            var sumData = GetListCriteria(queryParam)
                .SetProjection(
                    Projections.ProjectionList()
                        .Add(Projections.Count(Projections.Distinct(Projections.Property($"{nameof(LogisticsOrderRefundInfo.Customer)}.{nameof(LogisticsOrderRefundInfo.Customer.Id)}"))))
                        .Add(Projections.Count(Projections.Distinct(Projections.Property($"{nameof(LogisticsOrderRefundInfo.Order)}.{nameof(LogisticsOrderRefundInfo.Order.Id)}"))))
                        .Add(Projections.Sum(Projections.Property(nameof(LogisticsOrderRefundInfo.Amount))))
                )
                .UniqueResult<object[]>();

            model.CustomerCount = (int) sumData[0];
            model.OrderCount = (int) sumData[1];
            model.AmountSum = (decimal) sumData[2];

            return View("LogisticsOrderRefundList", model);
        }

        [NonAction]
        private ICriteria GetListCriteria(LogisticsOrderRefundListQueryParam queryParam)
        {
            ICriteria criteria = session.CreateCriteria<LogisticsOrderRefundInfo>();

            criteria.SetReadOnly(true);

            criteria = criteria.CreateAlias(nameof(LogisticsOrderRefundInfo.Customer), nameof(LogisticsOrderRefundInfo.Customer));
            criteria = criteria.CreateAlias(nameof(LogisticsOrderRefundInfo.Order), nameof(LogisticsOrderRefundInfo.Order));

            if (queryParam.TypeQuery != null && queryParam.TypeQuery != LogisticsOrderRefundType.None)
            {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderRefundInfo>(e => e.Type == queryParam.TypeQuery));
            }

            if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
            {
                string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
                criteria = criteria.Add(Restrictions.Like($"{nameof(LogisticsOrderRefundInfo.Order)}.{nameof(LogisticsOrderRefundInfo.Order.Code)}", orderCodeQuery, MatchMode.Anywhere));
            }

            if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
            {
                int customerIdQuery = customerService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
                criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsOrderRefundInfo.Customer)}.{nameof(LogisticsOrderRefundInfo.Customer.Id)}", customerIdQuery));
            }

            string reasonQuery = queryParam.ReasonQuery;
            if (string.IsNullOrWhiteSpace(reasonQuery) == false)
            {
                reasonQuery = reasonQuery.Trim();
                criteria = criteria.Add(Restrictions.On<LogisticsOrderRefundInfo>(e => e.ReasonContent).IsInsensitiveLike(reasonQuery, MatchMode.Anywhere));
            }

            if (queryParam.StatusQuery != null && queryParam.StatusQuery != LogisticsOrderRefundStatus.None)
            {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderRefundInfo>(e => e.Status == queryParam.StatusQuery));
            }

            if (queryParam.BeginDateQuery != null)
            {
                DateTime beginDateQueryValue = queryParam.BeginDateQuery.Value;
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderRefundInfo>(e => e.CreateTime >= beginDateQueryValue)); // 这里 CreateTime 和 UpdateTime 都可以
            }

            if (queryParam.EndDateQuery != null)
            {
                DateTime endDateQueryValue = queryParam.EndDateQuery.Value.AddDays(1);
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderRefundInfo>(e => e.CreateTime < endDateQueryValue)); // 这里 CreateTime 和 UpdateTime 都可以
            }

            return criteria;
        }

        [HttpPost]
        [Route("/admin/logistics/order/refund/export")]
        [NoHibernateAutoTransaction]
        public IActionResult Export(LogisticsOrderRefundListQueryParam queryParam)
        {
            IList<LogisticsOrderRefundInfo> refundList = GetListCriteria(queryParam)
                .AddOrder(new Order($"{nameof(LogisticsOrderRefundInfo.Id)}", true))
                .List<LogisticsOrderRefundInfo>();

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
            cell.SetCellValue("类型");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("退赔金额");

            sheet.SetColumnWidth(cellIndex, 50 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("退赔款原因");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("退赔支付方式");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("状态");

            sheet.SetColumnWidth(cellIndex, 16 * 256);
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = headerCenterCellStyle;
            cell?.SetCellType(CellType.String);
            cell.SetCellValue("生成时间");

            foreach (LogisticsOrderRefundInfo refund in refundList)
            {
                row = sheet.CreateRow(rowIndex++);
                row.HeightInPoints = rowHeight;

                cellIndex = 0;

                // 客户    订单    类型    退赔金额    退赔款原因    退赔支付方式    状态    生成时间    

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(refund.Customer?.CustomerCode);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(refund.Order?.Code);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(refund.Type?.Name);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = precision2Style;
                cell.SetCellType(CellType.Numeric);
                cell.SetCellValue((double) refund.Amount);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(refund.ReasonContent);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(refund.PayType?.Name);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(refund.Status?.Name);

                cell = row.CreateCell(cellIndex++);
                cell.CellStyle = contentCenterCellStyle;
                cell?.SetCellType(CellType.String);
                cell.SetCellValue(refund.CreateTime.ToString("yyyy-MM-dd HH:mm"));
            }


            MemoryStream memoryStream = new MemoryStream();
            workbook.Write(memoryStream);
            memoryStream.Flush();

            return File(memoryStream.ToArray(), ContentType.Xlsx, fileName);
        }
    }
}