using BenchmarkDotNet.Attributes;
using shunda.Core;

namespace shunda.Benchmark.Core;

#nullable disable

public class CaptchaServiceBenchmark
{
    private CaptchaService captchaService = new CaptchaService(null);
    
    [Benchmark()]
    public void RandNum()
    {
        string code = captchaService.RandNum();
    }
}