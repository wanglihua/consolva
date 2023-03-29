namespace shunda.Domain.Pass.PassRes;

public partial class PassRes {
    /// <summary>
    /// 仓管APP
    /// </summary>
    public class PassResAdminApp {
        public const int CateId = PassResCate.AdminAppCateId;
        public static readonly PassResCateInfo Cate = PassResCate.AdminApp;


        public static readonly IList<PassResInfo> AllData = new List<PassResInfo>() {
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
        public const int ExpressOnShelf = CateId + 0200;

        /// <summary>
        /// 仓库需核实
        /// </summary>
        public const int ExpressWarehouseCheck = CateId + 0400;

        /// <summary>
        /// 快递下架
        /// </summary>
        public const int ExpressOffShelf = CateId + 0600;

        /// <summary>
        /// 退运下架
        /// </summary>
        public const int ExpressReturnOffShelf = CateId + 0800;

        /// <summary>
        /// 快递堆存
        /// </summary>
        public const int ExpressDump = CateId + 1000;

        /// <summary>
        /// 入流动仓
        /// </summary>
        public const int ExpressPacking = CateId + 1200;

        /// <summary>
        /// 箱号信息查询
        /// </summary>
        public const int PackBoxQuery = CateId + 1000;

        /// <summary>
        /// 箱号发货
        /// </summary>
        public const int PackBoxShip = CateId + 1400;

        /// <summary>
        /// 签收照片
        /// </summary>
        public const int PhotoSignIn = CateId + 1000;
    }
}



/*
    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:background="@android:color/white"
        android:gravity="center_horizontal"
        android:orientation="vertical">

        <TextView
            android:id="@+id/tvPhotoSignIn"
            android:layout_width="fill_parent"
            android:layout_height="wrap_content"
            android:layout_margin="1dp"
            android:background="#ffef5973"
            android:drawableStart="@drawable/order_photo"
            android:gravity="center_vertical|center_horizontal"
            android:padding="5dp"
            android:text="签收照片"
            android:textColor="#ffffffff"
            android:textSize="28.0sp"
            android:visibility="gone" />

        <TextView
            android:id="@+id/tvPhotoCheckIn"
            android:layout_width="fill_parent"
            android:layout_height="wrap_content"
            android:layout_margin="1dp"
            android:background="#fff78221"
            android:drawableStart="@drawable/order_photo"
            android:gravity="center_vertical|center_horizontal"
            android:padding="5dp"
            android:text="入库照片"
            android:textColor="#ffffffff"
            android:textSize="28.0sp"
            android:visibility="gone" />

        <TextView
            android:id="@+id/tvPhotoIssue"
            android:layout_width="fill_parent"
            android:layout_height="wrap_content"
            android:layout_margin="1dp"
            android:background="#fffe5000"
            android:drawableStart="@drawable/order_photo"
            android:gravity="center_vertical|center_horizontal"
            android:padding="5dp"
            android:text="问题件照片"
            android:textColor="#ffffffff"
            android:textSize="28.0sp"
            android:visibility="gone" />

        <TextView
            android:id="@+id/tvPhotoPack"
            android:layout_width="fill_parent"
            android:layout_height="wrap_content"
            android:layout_margin="1dp"
            android:background="#ff00bece"
            android:drawableStart="@drawable/order_photo"
            android:gravity="center_vertical|center_horizontal"
            android:padding="5dp"
            android:text="打包照片"
            android:textColor="#ffffffff"
            android:textSize="28.0sp"
            android:visibility="gone" />

        <TextView
            android:id="@+id/tvPhotoReturn"
            android:layout_width="fill_parent"
            android:layout_height="wrap_content"
            android:layout_margin="1dp"
            android:background="#ffa650b5"
            android:drawableStart="@drawable/order_photo"
            android:gravity="center_vertical|center_horizontal"
            android:padding="5dp"
            android:text="退运照片"
            android:textColor="#ffffffff"
            android:textSize="28.0sp"
            android:visibility="gone" />

        <TextView
            android:id="@+id/tvPhotoShip"
            android:layout_width="fill_parent"
            android:layout_height="wrap_content"
            android:layout_margin="1dp"
            android:background="#fff8c708"
            android:drawableStart="@drawable/order_photo"
            android:gravity="center_vertical|center_horizontal"
            android:padding="5dp"
            android:text="发货照片"
            android:textColor="#ffffffff"
            android:textSize="28.0sp"
            android:visibility="gone" />

*/