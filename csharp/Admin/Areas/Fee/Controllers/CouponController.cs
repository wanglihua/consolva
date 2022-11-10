using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Fee.Models.Coupon;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Domain.Customer;
using shunda.Domain.Order;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Coupon;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.Coupon)]
public class CouponController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    private CustomerCodeService customerCodeService;
    private CouponBatchService couponBatchService;
    private CouponCodeService couponCodeService;

    public CouponController(ISession session,
        CustomerCodeService customerCodeService,
        CouponBatchService couponBatchService,
        CouponCodeService couponCodeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.couponBatchService = couponBatchService;
        this.couponCodeService = couponCodeService;
    }

    [HttpGet]
    [Route("/admin/coupon")]
    public IActionResult Index()
    {
        CouponIndexModel model = new CouponIndexModel();
        model.Batch = null;
        model.StatusList = CouponStatus.GetList();

        return View("CouponIndex", model);
    }

    [HttpGet]
    [Route("/admin/coupon/{batchId:int}")]
    public IActionResult IndexBatch(int batchId)
    {
        var batch = session.Get<CouponBatchInfo>(batchId);
        if (batch == null)
        {
            throw new ValidationException($"指定的优惠券批次#{batchId}，不存在！");
        }

        CouponIndexModel model = new CouponIndexModel();
        model.Batch = batch;
        model.StatusList = CouponStatus.GetList();

        return View("CouponIndex", model);
    }

    [HttpPost]
    [Route("/admin/coupon/list")]
    public IActionResult List(CouponListQueryParam queryParam)
    {
        var listCriteria = GetListCriteria(queryParam);

        int totalCount = ((ICriteria)listCriteria.Clone())
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<CouponInfo> couponList = ((ICriteria)listCriteria.Clone())
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CouponInfo>();

        foreach (CouponInfo coupon in couponList)
        {
        }

        CouponListModel model = new CouponListModel(listPager);
        model.CouponList = couponList;

        return View("CouponList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CouponListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<CouponInfo>();
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(CouponInfo.Batch), nameof(CouponInfo.Batch));
        criteria = criteria.CreateAlias(nameof(CouponInfo.Customer), nameof(CouponInfo.Customer), JoinType.LeftOuterJoin);
        criteria = criteria.CreateAlias(nameof(CouponInfo.Order), nameof(CouponInfo.Order), JoinType.LeftOuterJoin);

        if (queryParam.BatchId != 0)
        {
            criteria = criteria.Add(Restrictions.Eq($"{nameof(CouponInfo.Batch)}.{nameof(CouponBatchInfo.Id)}", queryParam.BatchId));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CodeQuery) == false)
        {
            string codeQuery = queryParam.CodeQuery;
            criteria = criteria.Add(Restrictions.Eq($"{nameof(CouponInfo.Code)}", codeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Eq($"{nameof(CouponInfo.Customer)}.{nameof(CustomerInfo.Id)}", customerIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            var orderId = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCodeQuery)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(CouponInfo.Order)}.{nameof(LogisticsOrderInfo.Id)}", orderId));
        }

        if (queryParam.StatusQuery != null && queryParam.StatusQuery != CouponStatus.None)
        {
            var statusQuery = queryParam.StatusQuery;
            criteria = criteria.Add(Restrictions.Eq($"{nameof(CouponInfo.Status)}", statusQuery));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/coupon/show/grant")]
    public IActionResult ShowGrant(int batchId)
    {
        CouponGrantModel model = new CouponGrantModel();
        model.BatchId = batchId;
        model.Quantity = 1; // 默认为1

        return View("CouponGrant", model);
    }

    [HttpPost]
    [Route("/admin/coupon/save/grant")]
    [Transactional]
    public IActionResult SaveGrant(CouponGrantModel model)
    {
        if (!this.ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int batchId = model.BatchId;
        var batch = session.Get<CouponBatchInfo>(batchId);
        if (batch == null)
        {
            throw new ValidationException($"指定的优惠券批次#{batchId}不存在！");
        }

        string customerCode = model.CustomerCode;
        int customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerCode);

        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null)
        {
            throw new ValidationException($"指定的客户#{customerCode}不存在！");
        }

        if (model.Quantity <= 0)
        {
            throw new ValidationException("发放数量应该大于0！");
        }

        // 检查批次总数量是否已经达到，这里发放时不再做其他判断
        int batchPickedCount = session.Query<CouponInfo>()
            .Where(e => e.Batch.Id == batchId)
            .Select(e => e.Id)
            .Count();

        if (batchPickedCount >= batch.TotalQuantity)
        {
            throw new ValidationException($"指定的优惠券批次#{batch.Name}，已 领取/发放 完！");
        }

        if (batchPickedCount + model.Quantity > batch.TotalQuantity)
        {
            throw new ValidationException($"指定的优惠券批次#{batch.Name}，未领取数量，不够本次发放！");
        }

        // 发放
        IList<CouponInfo> couponList = new List<CouponInfo>();

        for (int index = 0; index < model.Quantity; index++)
        {
            var now = DateTime.Now;
            var coupon = new CouponInfo();
            coupon.Batch = batch;
            coupon.Code = string.Empty; // 后面会做修正
            coupon.Customer = customer;
            coupon.PickTime = now;
            coupon.GetMethod = CouponGetMethod.Grant;
            coupon.Status = CouponStatus.Pick;

            session.Save(coupon);

            couponList.Add(coupon);
        }

        session.Flush();

        foreach (var coupon in couponList)
        {
            // 用新生成的ID来更新Code，保证不重复。
            coupon.Code = couponCodeService.Generate(batchId, batch.Type, coupon.Id);
            session.Update(coupon);
        }

        session.Flush();


        // 更新批次已领取总数量
        couponBatchService.SumAndSaveBatchTotalPicked(batchId);

        if (batch.Status == CouponBatchStatus.Release) // 发布状态才设置
        {
            if (batch.TotalQuantity <= batch.PickQuantity)
            {
                batch.Status = CouponBatchStatus.Picked;
                session.Update(batch);

                session.Flush();
            }
        }

        return Json(new {
            success = true,
            message = "优惠券发放成功！",
        });
    }

    private class CouponImportItem
    {
        public string CustomerCode { get; set; } = string.Empty;

        public CustomerInfo Customer { get; set; } = new();

        public int Quantity { get; set; }
    }

    [HttpPost]
    [Route("/admin/coupon/import/grant")]
    // 内部有事务 
    public IActionResult ImportGrant(int batchId, IFormFile excelFile)
    {
        if (excelFile == null)
        {
            throw new ValidationException("导入失败，文件未上传！");
        }

        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        ISheet sheet = workbook.GetSheetAt(0);
        if (sheet == null)
        {
            throw new ValidationException("导入的Excel文件应该至少有一个工作表！");
        }

        var batch = session.Get<CouponBatchInfo>(batchId);
        if (batch == null)
        {
            throw new ValidationException($"指定的优惠券批次#{batchId}不存在！");
        }

        IList<CouponImportItem> importItemList = new List<CouponImportItem>();

        // 略过标题行，所以 rowIndex = 1 开始
        int lastRowNum = sheet.LastRowNum;
        for (int rowIndex = 1; rowIndex <= lastRowNum; rowIndex++)
        {
            IRow row = sheet.GetRow(rowIndex);

            if (row == null)
            {
                break;
            }

            // 设置单元格的格式为 String，这样后面用 StringCellValue 取值的时候，就不会出错了
            int cellIndex = 0;
            int lastCellNum = row.LastCellNum;
            for (cellIndex = 0; cellIndex < lastCellNum; cellIndex++)
            {
                row.GetCell(cellIndex)?.SetCellType(CellType.String);
            }

            // 客户ID 充值金额

            cellIndex = 0;

            CouponImportItem importItem = new CouponImportItem();

            // 客户ID
            string customerCode = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(customerCode))
            {
                break;
            }

            importItem.CustomerCode = customerCode;
            int customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerCode);
            if (customerId == 0)
            {
                throw new ValidationException($"导入的第 {rowIndex} 行，客户ID输入有误！");
            }

            CustomerInfo customer = session.Get<CustomerInfo>(customerId);
            if (customer == null)
            {
                throw new ValidationException($"导入的第 {rowIndex} 行，指定的客户ID {customerCode} 不存在！");
            }

            importItem.Customer = customer;

            // 发放数量
            string qtyStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;

            int qty = 1; // 默认数量为1
            if (string.IsNullOrWhiteSpace(qtyStr) == false)
            {
                if (int.TryParse(qtyStr, out qty) == false)
                {
                    throw new ValidationException($"导入的第 {rowIndex} 行，发放数量输入有误！");
                }

                if (qty < 1)
                {
                    throw new ValidationException($"导入的第 {rowIndex} 行，发放数量应大于1！");
                }
            }

            importItem.Quantity = qty;

            importItemList.Add(importItem);
        }

        // 检查批次总数量是否已经达到，这里发放时不再做其他判断
        int batchPickedCount = session.Query<CouponInfo>()
            .Where(e => e.Batch.Id == batchId)
            .Select(e => e.Id)
            .Count();
        if (batchPickedCount >= batch.TotalQuantity)
        {
            throw new ValidationException($"指定的优惠券批次#{batch.Name}，已 领取/发放 完！");
        }

        if (batchPickedCount + importItemList.Count() > batch.TotalQuantity)
        {
            throw new ValidationException($"指定的优惠券批次#{batch.Name}，未领取数量，不够本次发放！");
        }

        // 发放

        Transactional.Wrap(session, () => {
            var now = DateTime.Now;
            IList<CouponInfo> couponList = new List<CouponInfo>();
            foreach (var importItem in importItemList)
            {
                CustomerInfo customer = importItem.Customer;

                for (int index = 0; index < importItem.Quantity; index++)
                {
                    var coupon = new CouponInfo();
                    coupon.Batch = batch;
                    coupon.Code = Guid.NewGuid().ToString("N"); // 后面会做修正，这里也不能给个空的，否则数据库唯一索引约束会报错
                    coupon.Customer = customer;
                    coupon.PickTime = now;
                    coupon.GetMethod = CouponGetMethod.Grant;
                    coupon.Status = CouponStatus.Pick;

                    session.Save(coupon);

                    couponList.Add(coupon);
                }
            }

            session.Flush();

            foreach (var coupon in couponList)
            {
                // 用新生成的ID来更新Code，保证不重复。
                coupon.Code = couponCodeService.Generate(batchId, batch.Type, coupon.Id);
                session.Update(coupon);
            }

            session.Flush();

            // 更新批次已领取总数量
            couponBatchService.SumAndSaveBatchTotalPicked(batchId);

            if (batch.Status == CouponBatchStatus.Release) // 发布状态才设置
            {
                if (batch.TotalQuantity <= batch.PickQuantity)
                {
                    batch.Status = CouponBatchStatus.Picked;
                    session.Update(batch);

                    session.Flush();
                }
            }
        });

        return Json(new {
            success = true,
            message = "优惠券导入发放成功！",
        });
    }

    [HttpPost]
    [Route("/admin/coupon/remove/grant")]
    [Transactional]
    public IActionResult RemoveGrant(IList<int> idList)
    {
        if (idList == null || idList.Count == 0)
        {
            throw new ValidationException("请选择要收回发放的优惠券！");
        }

        var couponList = session.Query<CouponInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        foreach (var coupon in couponList)
        {
            if (coupon.GetMethod == CouponGetMethod.Pick)
            {
                throw new ValidationException($"优惠券#{coupon.Code}客户领取的，不能收回发放！");
            }
        }

        foreach (var coupon in couponList)
        {
            session.Delete(coupon);
        }

        session.Flush();

        // 更新批次已领取总数量

        var batchIdList = couponList.Select(e => e.Batch.Id).ToList(); // 其实一般也只有一个

        foreach (var batchId in batchIdList)
        {
            var batch = session.Get<CouponBatchInfo>(batchId);
            if (batch == null)
            {
                throw new ValidationException($"指定的优惠券批次#{batchId}不存在！");
            }

            couponBatchService.SumAndSaveBatchTotalPicked(batchId);

            // 按现在的逻辑，只有 release 的才能进入到 Picked 状态
            // 所以才能回退到 release 的状态s
            if (batch.Status == CouponBatchStatus.Picked)
            {
                if (batch.PickQuantity < batch.TotalQuantity)
                {
                    batch.Status = CouponBatchStatus.Release;
                    session.Update(batch);

                    session.Flush();
                }
            }
        }

        session.Flush();

        return Json(new {
            success = true,
            message = "优惠券收回发放成功！",
        });
    }
}