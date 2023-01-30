using shunda.Core.Service;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class FeeCalculateLogService {
    private List<string> logList = new List<string>();

    private bool enabled = false;

    public void SetEnabled(bool enabled) {
        this.enabled = enabled;
    }

    public void Init() {
        if (this.enabled) {
            logList.Clear();
        }
    }

    public void AddLog(string log) {
        if (enabled) {
            logList.Add(log);
        }
    }

    public IReadOnlyList<string> GetLogList() {
        // 保证返回的不为空
        if (logList == null) {
            return new List<string>();
        }

        return logList;
    }
}