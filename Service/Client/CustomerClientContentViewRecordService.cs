using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Client;

namespace shunda.Service.Client;

[Service(ServiceType.Scoped)]
public class CustomerClientContentViewRecordService {
    private readonly ISession session;

    public CustomerClientContentViewRecordService(ISession session) {
        this.session = session;
    }

    public void RecordView(int customerId, ClientContentType contentType) {
        RecordView(customerId, contentType, 0);
    }

    public void RecordView(int customerId, ClientContentType contentType, int relatedId) {
        DateTime now = DateTime.Now;

        var record = session.Query<CustomerClientContentViewRecordInfo>()
            .Where(e => e.CustomerId == customerId
                        && e.Type == contentType
                        && e.RelatedId == relatedId)
            .Take(1)
            .SingleOrDefault();
        if (record == null) {
            record = new CustomerClientContentViewRecordInfo {
                CustomerId = customerId,
                Type = contentType,
                RelatedId = relatedId,
                LastReadTime = now,
            };

            session.Save(record);
        } else {
            record.LastReadTime = now;

            session.Update(record);
        }

        session.Flush();
    }
}