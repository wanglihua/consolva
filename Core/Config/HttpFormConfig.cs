namespace shunda.Core.Config;

public class HttpFormConfig
{
    public static int ValueCountLimit => 1024 * 10; // 10240 items max

    public static int ValueLengthLimit => 1024 * 1024 * 100; // 100MB max len form data
}