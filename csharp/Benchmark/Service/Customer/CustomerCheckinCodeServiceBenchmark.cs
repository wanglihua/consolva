using BenchmarkDotNet.Attributes;
using shunda.Service.Customer;

#nullable disable

namespace shunda.Benchmark.Service.Customer;

// [HtmlExporter]
public class CustomerCheckinCodeServiceBenchmark
{
    private CustomerCheckinCodeService customerCheckinCodeService = new CustomerCheckinCodeService(null);

    [Benchmark]
    public void RandCode()
    {
        string code = customerCheckinCodeService.RandCode();
    }
}