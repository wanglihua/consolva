namespace shunda.Service.AutoMeasure;

public class AutoMeasureErrorException : Exception
{
    public AutoMeasureErrorException(string msg) : base(msg)
    {
    }
}