using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace shunda.Core.Log;

/// <summary>
/// 使用
/// NLogConfiguration.Config();
/// builder.Logging.AddNLogWeb();
///
/// 适用于版本 4.7.15
/// 
/// </summary>
public class NLogConfiguration
{
    public static void Config()
    {
        LoggingConfiguration config = new LoggingConfiguration();

        // string layout = "${longdate}|${uppercase:${level}}|${uppercase:${aspnet-request-method}} ${aspnet-request-url} |post-data:${aspnet-request-posted-body}|${callsite} line ${callsite-linenumber}${newline}${message}${newline}${exception}";
        string layout = "${longdate}|${uppercase:${level}}|${uppercase:${aspnet-request-method}} ${aspnet-request-url} |${callsite} line ${callsite-linenumber}${newline}${message}${newline}${exception}"; // 不能配置|post-data:${aspnet-request-posted-body} 微信支付会失败

        // fileTarget
        FileTarget fileTarget = new FileTarget();
        fileTarget.Layout = layout;
        fileTarget.KeepFileOpen = true;
        fileTarget.Encoding = Encoding.UTF8;
        fileTarget.ArchiveAboveSize = 10240000;
        fileTarget.ArchiveEvery = FileArchivePeriod.Day;

#if DEBUG
        fileTarget.FileName = "${basedir}/../../../logs/log-${shortdate}.log";
        fileTarget.MaxArchiveFiles = 3;
#else
        fileTarget.FileName = "${basedir}/logs/log-${shortdate}.log";
        fileTarget.MaxArchiveFiles = 100;
#endif
        config.AddTarget("file", fileTarget);

        // consoleTarget
#if DEBUG
        ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
        consoleTarget.Layout = layout;
        config.AddTarget("console", consoleTarget);
#endif

        // rules
#if DEBUG
        // Trace, Debug, Info, Warn, Error, Fatal

        // Microsoft
        config.LoggingRules.Add(new LoggingRule("Microsoft.*", LogLevel.Warn, fileTarget));
        config.LoggingRules.Add(new LoggingRule("Microsoft.*", LogLevel.Warn, consoleTarget));

        // NHibernate
        config.LoggingRules.Add(new LoggingRule("NHibernate.*", LogLevel.Warn, fileTarget));
        config.LoggingRules.Add(new LoggingRule("NHibernate.*", LogLevel.Warn, consoleTarget));

        // Quartz
        config.LoggingRules.Add(new LoggingRule("Quartz.*", LogLevel.Warn, fileTarget));
        config.LoggingRules.Add(new LoggingRule("Quartz.*", LogLevel.Warn, consoleTarget));

        // iepoch
        config.LoggingRules.Add(new LoggingRule("iepoch.*", LogLevel.Trace, fileTarget));
        config.LoggingRules.Add(new LoggingRule("iepoch.*", LogLevel.Trace, consoleTarget));

        // all
        config.LoggingRules.Add(new LoggingRule("*", LogLevel.Warn, fileTarget));
        config.LoggingRules.Add(new LoggingRule("*", LogLevel.Warn, consoleTarget));
#else
        config.LoggingRules.Add(new LoggingRule("*", LogLevel.Error, fileTarget));
#endif

        LogManager.Configuration = config;
    }
}