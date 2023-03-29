namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 仓管APP
    /// </summary>
    public class PassResAdminApp {
        public const int CateId = PassResCate.AdminAppCateId;
        public static readonly PassResCateInfo Cate = PassResCate.AdminApp;


        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
            new PassResInfo { Id = ExpressQuery, Name = "快递单号查询", Cate = Cate },
        };

        /// <summary>
        /// 所有
        /// </summary>
        public static readonly IList<int> All = AllData.Select(e => e.Id).ToList();

        /// <summary>
        ///  快递单号查询
        /// </summary>
        public const int ExpressQuery = CateId + 0200;

        /// <summary>
        /// 快递上架
        /// </summary>
        public const int ExpressOnShelf = CateId + 0400;

        /// <summary>
        /// 仓库需核实
        /// </summary>
        public const int ExpressWarehouseCheck = CateId + 0600;

        /// <summary>
        /// 快递下架
        /// </summary>
        public const int ExpressOffShelf = CateId + 0800;

        /// <summary>
        /// 退运下架
        /// </summary>
        public const int ExpressReturnOffShelf = CateId + 1000;

        /// <summary>
        /// 快递堆存
        /// </summary>
        public const int ExpressDump = CateId + 1200;

        /// <summary>
        /// 入流动仓
        /// </summary>
        public const int ExpressPacking = CateId + 1400;

        /// <summary>
        /// 箱号信息查询
        /// </summary>
        public const int PackBoxQuery = CateId + 1600;

        /// <summary>
        /// 箱号发货
        /// </summary>
        public const int PackBoxShip = CateId + 1800;

        /// <summary>
        /// 签收照片
        /// </summary>
        public const int PhotoSignIn = CateId + 2000;

        /// <summary>
        /// 入库照片
        /// </summary>
        public const int PhotoCheckIn = CateId + 2200;

        /// <summary>
        /// 问题件照片
        /// </summary>
        public const int PhotoIssue = CateId + 2400;

        /// <summary>
        /// 打包照片
        /// </summary>
        public const int PhotoPack = CateId + 2600;

        /// <summary>
        /// 退运照片
        /// </summary>
        public const int PhotoReturn = CateId + 2800;

        /// <summary>
        /// 发货照片
        /// </summary>
        public const int PhotoShip = CateId + 3000;
    }
}