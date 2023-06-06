using System.Data.Common;

namespace shunda.Core.Hibernate.MiniProfilerMySql;

public class MiniProfilerMySqlClientSqlCommandSet : IDisposable {
    private readonly IList<DbCommand> commandList = new List<DbCommand>();

    public void Append(DbCommand command) {
        commandList.Add(command);
    }

    public int ExecuteNonQuery() {
        int affectedCount = 0;
        foreach (DbCommand command in commandList) {
            affectedCount += command.ExecuteNonQuery();
        }

        commandList.Clear();

        return affectedCount;
    }

    public int CountOfCommands {
        get { return commandList.Count; }
    }

    public void Dispose() {
        commandList.Clear();
    }
}