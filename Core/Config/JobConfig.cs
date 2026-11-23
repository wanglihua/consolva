namespace shunda.Core.Config;

public class JobConfig
{
    /// <summary>
    /// Job 里面一个循环的休息时间，毫秒
    /// </summary>
    public static int SleepTimeOneRepeat => 0; // 0，只是别人需要时让出时间片就行
}