using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Client;
using shunda.Domain.Message;

namespace shunda.Service.Message;

[Service(ServiceType.Scoped)]
public class CustomerMessageClientViewRecordService {
    private readonly ISession session;

    public CustomerMessageClientViewRecordService(ISession session) {
        this.session = session;
    }

    public int GetLastViewMessageId(int customerId) {
        return session.Get<CustomerMessageClientViewRecordInfo>(customerId)?.LastViewMessageId ?? 0;
    }

    public void RecordView(int customerId) {
        // 客户消息列表中的最后一条消息 id，也是客户在客户端查看的最近的一条消息 id
        // 如果还没有消息，那么这个 lastMessageId 的值就是 0
        var lastMessageId = session.Query<CustomerMessageInfo>()
            .Where(e => e.Customer.Id == customerId)
            .OrderByDescending(e => e.Id)
            .Select(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        RecordView(customerId, lastMessageId);
    }

    public void RecordView(int customerId, int lastMessageId) {
        var viewRecord = session.Get<CustomerMessageClientViewRecordInfo>(customerId);
        if (viewRecord == null) {
            viewRecord = new CustomerMessageClientViewRecordInfo() {
                CustomerId = customerId,
                LastViewMessageId = lastMessageId,
                LastViewTime = DateTime.Now,
            };

            session.Save(viewRecord);
        } else {
            if (lastMessageId > viewRecord.LastViewMessageId) // 这个判断必须要有，因为分页查看时，可能会记录一个比较小的值，要排除这种可能
            {
                viewRecord.LastViewMessageId = lastMessageId;
                viewRecord.LastViewTime = DateTime.Now;

                session.Update(viewRecord);
            }
        }

        session.Flush();
    }
}