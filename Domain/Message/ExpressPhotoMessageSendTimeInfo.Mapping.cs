using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;

namespace shunda.Domain.Message;

public class ExpressPhotoMessageSendTimeInfoMapping : EntityNoUpdateMapping<ExpressPhotoMessageSendTimeInfo> {
    public ExpressPhotoMessageSendTimeInfoMapping() {
        Table("tbl_express_photo_message_send_time");

        Property(
            x => x.TrackingNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("tracking_no");
                map.NotNullable(false);
            }
        );

        ManyToOne(
            x => x.Express,
            map => {
                map.Class(typeof(ExpressInfo));
                map.Column("express_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.Order,
            map => {
                map.Class(typeof(LogisticsOrderInfo));
                map.Column("order_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Type,
            map => {
                map.Type<ExpressPhotoTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LastSendTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("last_send_time");
                map.NotNullable(true);
            }
        );

    }
}