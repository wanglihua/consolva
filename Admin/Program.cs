using Alipay.AopSdk.AspnetCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using NHibernate;
using NLog.Web;
using Senparc.CO2NET.Cache;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin.Cache;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.RegisterServices;
using shunda.Admin.Filters;
using shunda.Admin.Middlewares;
using shunda.Core;
using shunda.Core.Binder;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Core.Hibernate.MiniProfilerMySql;
using shunda.Core.Json;
using shunda.Core.Log;
using shunda.Core.Service;
using shunda.Domain;
using shunda.Domain.Pass;
using shunda.Service;
using StackExchange.Profiling;
using ISession = NHibernate.ISession;

namespace shunda.Admin;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost
            .UseUrls("http://*:9001")
            .UseKestrel(opt => {
                // https://q.cnblogs.com/q/109103/  Reading the request body timed out due to data arriving too slowly. See MinRequestBodyDataRate.
                opt.Limits.MinRequestBodyDataRate = null;
                opt.Limits.MinResponseDataRate = null;
            });

        NLogConfiguration.Config();
        builder.Logging.AddNLogWeb();
        NHibernateLogger.SetLoggersFactory(new HibernateLoggerFactory());

        // Add services to the container.
        var services = builder.Services;

        services.AddScoped<RequestLogActionFilter>();
        services.AddScoped<LoginTimeoutResetActionFilter>();
        services.AddScoped<HibernateActionFilter>();
        services.AddScoped<HeaderActionFilter>();
        services.AddScoped<AdminAuthActionFilter>();
        services.AddScoped<ExceptionFilter>();

        var mvcBuilder = services.AddControllersWithViews()
            .AddMvcOptions(options => {
                // 执行顺序是从上到下的
                options.Filters.AddService<RequestLogActionFilter>(); // 这个Filter执行时 sessionId 应该已经有了
                options.Filters.AddService<LoginTimeoutResetActionFilter>(); // 这个Filter执行时 sessionId 应该已经有了
                options.Filters.AddService<HibernateActionFilter>();
                options.Filters.AddService<HeaderActionFilter>();
                options.Filters.AddService<AdminAuthActionFilter>();
                options.Filters.AddService<ExceptionFilter>();

                // 数据绑定相关
                options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(name => $"{name}为必填项！");
                options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value, name) => $"{value}不是合法的{name}的值！");
                options.ModelBinderProviders.Insert(0, new IntModelBinderProvider());
                options.ModelBinderProviders.Insert(0, new UIntModelBinderProvider());
                options.ModelBinderProviders.Insert(0, new DateTimeFormatModelBinderProvider());
            })
            // .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = new AppJsonNamingPolicy(); }) 改用 NewtonsoftJson 后，这里实际已经不起作用
            .AddNewtonsoftJson(options => {
                // options.SerializerSettings.ContractResolver = new DefaultContractResolver(); // 这个ContractResolver【不会】改变属性大小写
                // options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // 这个ContractResolver【会】改变属性大小写
                options.SerializerSettings.ContractResolver = new NHibernateContractResolver(); // 当前基于 CamelCasePropertyNamesContractResolver 实现
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm";
            });

#if DEBUG
        mvcBuilder.AddRazorRuntimeCompilation();
#endif
        services.Configure<FormOptions>(options => {
            options.ValueCountLimit = HttpFormConfig.ValueCountLimit;
            options.ValueLengthLimit = HttpFormConfig.ValueLengthLimit;
        });

        services.Configure<CookiePolicyOptions>(options => {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => false;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });


        services.AddMemoryCache();

        // 微信开发相关
        services.AddSenparcGlobalServices(builder.Configuration);
        services.AddSenparcWeixinServices(builder.Configuration);

        // 支付宝支付 在这里添加支付宝服务是为了，使用他的服务类初始化的时候不报错
        services.AddAlipay(options => {
            options.PrivateKey = AlipayConfig.PrivateKey;
            options.AlipayPublicKey = AlipayConfig.PublicKey;
            options.AppId = AlipayConfig.AppId;
            options.Uid = AlipayConfig.UId;
            options.CharSet = AlipayConfig.CharSet;
            options.Gatewayurl = AlipayConfig.GatewayUrl;
            options.SignType = AlipayConfig.SignType;
            options.IsKeyFromFile = false;
        });

        services.AddHttpClient();

        // 这个可以保证 Html.Action() 的正确执行
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        // 支持跨域
        // services.AddCors();

        // 添加 Core 项目中的 service
        services.AddCoreServices();

        // 添加 Service 项目中的 service
        services.AddAppServices();

        // 添加 Admin 项目中的 service
        ServiceUtil.AddAssemblyService(services, typeof(Program).Assembly);

        // 必须 Singleton
        services.AddSingleton(provider => HibernateSessionFactory.BuildSessionFactory(DataBaseConfig.DataBaseType.MySQL, new DomainMock().GetType().Assembly));

        // 必须 Scoped 
        services.AddScoped(provider => HibernateSessionFactory.OpenSession(provider.GetService<ISessionFactory>()!));

#if DEBUG
        services.AddMiniProfiler(options => {
            options.RouteBasePath = "/admin/profiler"; // /admin/profiler/results-index   /admin/profiler/results
            options.TrackConnectionOpenClose = true;
            options.ColorScheme = ColorScheme.Auto;
            options.EnableMvcFilterProfiling = true;
            options.EnableMvcViewProfiling = true;
            options.SqlFormatter = new MiniProfilerMySqlFormatter();
        });
#endif

        var app = builder.Build();

        app.UseExceptionHandler(SiteConfig.AdminErrorHandlerUrl);

        // app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // 支持跨域
        // app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

        app.UseMiddleware<SessionIdMiddleware>();
        app.UseMiddleware<RequestLogMiddleware>(); // 还是放在有了 sessionId 后放心点
        app.UseMiddleware<HibernateMiddleware>();

#if DEBUG
        app.UseMiniProfiler();
#endif

        app.UseEndpoints(endpoints => {
            endpoints.MapControllerRoute(
                "defaultWithArea",
                "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            endpoints.MapControllerRoute(
                "default",
                "{controller=Home}/{action=Index}/{id?}");

            // endpoints.MapHub<HandleHub>("/hub/handle");
        });

        CacheStrategyDomainWarehouse.RegisterCacheStrategyDomain(LocalContainerCacheStrategy.Instance);
        AccessTokenContainer.RegisterAsync(WechatOfficialAccountConfig.AppId, WechatOfficialAccountConfig.AppSecret);

        app.Run();
    }
}