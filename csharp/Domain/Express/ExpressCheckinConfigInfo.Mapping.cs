using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.Express;

public class ExpressCheckinConfigInfoMapping : EntityMapping<ExpressCheckinConfigInfo>
{
    public ExpressCheckinConfigInfoMapping()
    {
        Table("tbl_express_checkin_config");

        Property(
            x => x.ExpressCountPerStorageLoc,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("express_count_per_storage_loc");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.SigninPhotoMinQtyPerExpress,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("signin_photo_min_qty_per_express");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CheckinPhotoMinQtyPerExpress,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("checkin_photo_min_qty_per_express");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.AutoRecordCheckinVideo,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("auto_record_checkin_video");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ClientShowVideo,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("client_show_video");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CheckinPhotoClientShow,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("checkin_photo_client_show");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LabelW10H05Status,
            map => {
                map.Type<ExpressCheckinLabelConfigStatusHibernateType>();
                map.Column("label_w10_h05_status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LabelW10H05PrintCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("label_w10_h05_print_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LabelW05H05Status,
            map => {
                map.Type<ExpressCheckinLabelConfigStatusHibernateType>();
                map.Column("label_w05_h05_status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LabelW05H05PrintCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("label_w05_h05_print_count");
                map.NotNullable(true);
            }
        );

    }
}