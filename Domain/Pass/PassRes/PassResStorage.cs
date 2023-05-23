namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 仓储管理
    /// </summary>
    public class Storage {
        public const int CateId = PassResCate.StorageCateId;
        public static PassResCateInfo Cate => PassResCate.Storage;

        public static IList<PassResInfo> AllData => new List<PassResInfo>() {
            new PassResInfo { Id = Warehouse, Name = "仓库", Cate = Cate },
            new PassResInfo { Id = StorageLocSpec, Name = "仓位规格", Cate = Cate },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static IList<int> All => AllData.Select(e => e.Id).ToList();

        /// <summary>
        /// 仓库
        /// </summary>
        public const int Warehouse = CateId + 0200;

        /// <summary>
        /// 仓位规格
        /// </summary>
        public const int StorageLocSpec = CateId + 0400;
    }
}