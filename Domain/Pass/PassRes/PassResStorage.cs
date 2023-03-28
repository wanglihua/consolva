namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 仓储管理
    /// </summary>
    public class Storage {
        public const int CateId = PassResCate.StorageCateId;
        public static readonly PassResCateInfo Cate = PassResCate.Storage;

        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo { Id = Warehouse, Name = "仓库", Cate = Cate },
            new PassResInfo { Id = StorageLocSpec, Name = "仓位规格", Cate = Cate },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 仓库
        /// </summary>
        public const int Warehouse = CateId + 02000;

        /// <summary>
        /// 仓位规格
        /// </summary>
        public const int StorageLocSpec = CateId + 04000;
    }
}