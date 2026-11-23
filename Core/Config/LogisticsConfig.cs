namespace shunda.Core.Config;

public class LogisticsConfig {
    /// <summary>
    /// 体积重分母，5000 就表示一个立方 200公斤
    /// </summary>
    public static int VolumeWeightDenominator => 5000;

    /// <summary>
    /// 体积重每立方重量，一般是 200公斤
    /// </summary>
    public static int VolumeWeightPerCubicMeter => 100_00_00 / VolumeWeightDenominator;
}