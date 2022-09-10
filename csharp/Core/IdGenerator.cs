namespace shunda.Core;

public static class IdGenerator
{
    private static readonly DateTime startTime = new DateTime(2022, 1, 1);
    private static readonly long nodeId = 1; // 数值从 0~99，支持 100 个节点。如果有多个节点，可通过配置初始化。

    private static long sequence = 0; // 数值从 0~9999，支持每毫秒 10000 个ID

    private static readonly object lockObj = new object();

    public static long NextId()
    {
        lock (lockObj)
        {
            sequence = (sequence + 1) % 10000;
        }

        long milliseconds = (long)((DateTime.Now - startTime).TotalMilliseconds);
        return (milliseconds * 100 + nodeId) * 10000 + sequence;
    }
}