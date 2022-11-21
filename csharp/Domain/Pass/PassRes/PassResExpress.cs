namespace shunda.Domain.Pass.PassRes;

public partial class PassRes
{
    /// <summary>
    /// 快递管理
    /// </summary>
    public class Express
    {
        /// <summary>
        /// 快递管理类别
        /// </summary>
        public const int CateId = 01100_00000;

        public static readonly PassResCateInfo Cate = new PassResCateInfo() { Id = CateId, Name = "快递管理" };

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo() { Id = ExpressList, Name = "快递列表", CateIdForCache = CateId },
            new PassResInfo() { Id = ExpressServiceRemark, Name = "快递客服备注", CateIdForCache = CateId },
            new PassResInfo() { Id = ExpressForceDelete, Name = "快递强制删除", CateIdForCache = CateId },
            new PassResInfo() { Id = ExpressExpect, Name = "预报单号", CateIdForCache = CateId },
            new PassResInfo() { Id = ExpressPhoto, Name = "快递单图片", CateIdForCache = CateId },
            new PassResInfo() { Id = ExpressPhotoDelete, Name = "快递单图片删除", CateIdForCache = CateId },
            new PassResInfo() { Id = ExpressSizeWeight, Name = "快递单尺寸重量", CateIdForCache = CateId },
            new PassResInfo() { Id = ExpressRemarkPresetSelect, Name = "快递备注预设", CateIdForCache = CateId },
        };

        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 快递列表
        /// </summary>
        public const int ExpressList = CateId + 10000;

        /// <summary>
        /// 快递客服备注
        /// </summary>
        public const int ExpressServiceRemark = CateId + 11000;

        /// <summary>
        /// 快递强制删除
        /// </summary>
        public const int ExpressForceDelete = CateId + 20000;

        /// <summary>
        /// 预报单号
        /// </summary>
        public const int ExpressExpect = CateId + 30000;

        /// <summary>
        /// 快递单图片
        /// </summary>
        public const int ExpressPhoto = CateId + 40000;

        /// <summary>
        /// 快递单图片删除
        /// </summary>
        public const int ExpressPhotoDelete = CateId + 50000;

        /// <summary>
        /// 快递单尺寸重量
        /// </summary>
        public const int ExpressSizeWeight = CateId + 60000;

        /// <summary>
        /// 快递备注预设
        /// </summary>
        public const int ExpressRemarkPresetSelect = CateId + 70000;
    }
}