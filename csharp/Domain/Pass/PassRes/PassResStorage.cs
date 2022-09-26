namespace shunda.Domain.Pass.PassRes;

public partial class PassRes
{
    /// <summary>
    /// 仓储管理
    /// </summary>
    public class Storage
    {
        /// <summary>
        /// 类别
        /// </summary>
        public const int CateId = 08000_00000;

        public static readonly PassResCateInfo Cate = new PassResCateInfo() { Id = CateId, Name = "仓储管理" };

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo() { Id = Warehouse, Name = "仓库", CateIdForCache = CateId },
            new PassResInfo() { Id = StorageLocSpec, Name = "仓位规格", CateIdForCache = CateId },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 仓库
        /// </summary>
        public const int Warehouse = CateId + 01000;

        /// <summary>
        /// 仓位规格
        /// </summary>
        public const int StorageLocSpec = CateId + 02000;
    }
}