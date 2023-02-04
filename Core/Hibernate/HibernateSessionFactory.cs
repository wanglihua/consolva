using System.Data;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using shunda.Core.Config;
using shunda.Core.Hibernate.MiniProfilerMySql;
using shunda.Core.Log;
using Environment = NHibernate.Cfg.Environment;

// using NHibernate.Caches.RtMemoryCache;

#if !DEBUG
using NHibernate.Driver;
#endif

namespace shunda.Core.Hibernate;

public class HibernateSessionFactory {
    private static readonly string MAPPING_CLASS_NAME_SUFFIX = "InfoMapping";

    private static ISessionFactory? sessionFactorySingleton = null;

    private static Configuration BuildConfiguration(DataBaseConfig.DataBaseType dataBaseType, IEnumerable<Type> mappingTypes) {
        return BuildConfiguration(dataBaseType, mappingTypes, null);
    }

    private static Configuration BuildConfiguration(DataBaseConfig.DataBaseType dataBaseType, Assembly mappingAssembly) {
        return BuildConfiguration(dataBaseType, new List<Type>(), mappingAssembly);
    }

    private static Configuration BuildConfiguration(DataBaseConfig.DataBaseType dataBaseType, IEnumerable<Type> mappingTypes, Assembly? mappingAssembly) {
        Configuration configuration = new Configuration();

        configuration.DataBaseIntegration(
            cfg => {
                if (dataBaseType == DataBaseConfig.DataBaseType.MySQL) {
                    cfg.ConnectionString = DataBaseConfig.MySQLConnectionString;
                    cfg.Dialect<MySQL55InnoDBDialect>();
#if DEBUG
                    // cfg.Driver<MySqlProfileDriver>();
                    cfg.Driver<MiniProfilerMySqlDataDriver>();
#else
                    cfg.Driver<MySqlDataDriver>();
#endif
                }

                if (dataBaseType == DataBaseConfig.DataBaseType.SQLite) {
                    cfg.ConnectionString = DataBaseConfig.SQLiteConnectionString;
                    cfg.Dialect<SQLiteDialect>();
#if DEBUG
                    cfg.Driver<SQLiteProfileDriver>();
#else
                    cfg.Driver<SQLite20Driver>();
#endif
                }


// #if DEBUG
//                 cfg.LogSqlInConsole = true;
//                 cfg.LogFormattedSql = true;
// #else
//                 cfg.LogSqlInConsole = false;
//                 cfg.LogFormattedSql = false;
// #endif

                cfg.LogSqlInConsole = false;
                cfg.LogFormattedSql = false;
                cfg.AutoCommentSql = false;

                cfg.BatchSize = 15;
                cfg.IsolationLevel = IsolationLevel.ReadCommitted;
#if DEBUG
                // 单元测试时保持一个 connection 可以让 sqlite 内存数据保持同一个
                // 生产环境用默认的一个事务就关闭就可以了，毕竟有连接池也不会有性能的问题
                cfg.ConnectionReleaseMode = ConnectionReleaseMode.OnClose; // 这个模式下，如果忘记调用 session.Close(); 会不会造成连接资源耗尽
#endif
                cfg.Timeout = 200;
            }
        );

        #region 二级缓存配置

        /*
        二级缓存先不启用了，有时会造成不明错误吧

        configuration.Cache(
            cfg => {
                cfg.DefaultExpiration = 5 * 60; // 感觉 5分钟就够了
                cfg.UseQueryCache = false;
                cfg.Provider<RtMemoryCacheProvider>(); // 这里面已经相当于设置了 configuration.SetProperty(Environment.UseSecondLevelCache, "true");
            }
        );
        */

        #endregion

#if DEBUG
        configuration.SetProperty(Environment.PropertyUseReflectionOptimizer, "false");
        configuration.SetProperty(Environment.QueryStartupChecking, "false");
#else
        configuration.SetProperty(Environment.PropertyUseReflectionOptimizer, "true");
        configuration.SetProperty(Environment.QueryStartupChecking, "true");
#endif

        configuration.SetInterceptor(new HibernateInterceptor());

        // use NHibernate.Mapping.ByCode;
        ModelMapper mapper = new ModelMapper();

        if (mappingTypes != null) {
            mapper.AddMappings(mappingTypes);
        }

        if (mappingAssembly != null) {
            mapper.AddMappings(mappingAssembly.GetTypes().Where(t => t.Name.EndsWith(MAPPING_CLASS_NAME_SUFFIX)));
        }

        HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
        configuration.AddMapping(domainMapping);

        // use NHibernate.Mapping.Attributes;
        // configuration.AddInputStream(HbmSerializer.Default.Serialize(mappingAssembly));

        return configuration;
    }

    private static object objLock = new object();

    public static ISessionFactory BuildSessionFactory(DataBaseConfig.DataBaseType dataBaseType, Assembly mappingAssembly) {
/*

#if DEBUG
        // using System.Runtime.Serialization.Formatters.Binary;
        // <EnableUnsafeBinaryFormatterSerialization>true</EnableUnsafeBinaryFormatterSerialization>
        Configuration configuration;

        string cacheFile = "/Users/wanglihua/projects/shunda/Domain/hibernate_configuration.cache";
        if (File.Exists(cacheFile)) {
            using (Stream stream = File.OpenRead(cacheFile)) {
                configuration = (new BinaryFormatter().Deserialize(stream) as Configuration)!;
            }
        } else {
            configuration = BuildConfiguration(dataBaseType, mappingAssembly);
            using (Stream stream = File.Create(cacheFile)) {
                new BinaryFormatter().Serialize(stream, configuration);
            }
        }
#else
        var configuration = BuildConfiguration(dataBaseType, mappingAssembly);
#endif

 */
        lock (objLock) {
            if (sessionFactorySingleton == null) {
                var configuration = BuildConfiguration(dataBaseType, mappingAssembly);
                sessionFactorySingleton = configuration.BuildSessionFactory();
            }

            return sessionFactorySingleton;
        }
    }

    public static ISessionFactory BuildSessionFactory(DataBaseConfig.DataBaseType dataBaseType, IEnumerable<Type> mappingTypes) {
        lock (objLock) {
            if (sessionFactorySingleton == null) {
                var configuration = BuildConfiguration(dataBaseType, mappingTypes);
                sessionFactorySingleton = configuration.BuildSessionFactory();
            }

            return sessionFactorySingleton;
        }
    }

    /// <summary>
    /// 只会创建指定的数据库表
    /// </summary>
    /// <param name="mappingTypes"></param>
    /// <returns></returns>
    public static ISession CreateTablesAndOpenSession(IEnumerable<Type> mappingTypes) {
        NLogConfiguration.Config();

        var cfg = BuildConfiguration(DataBaseConfig.DataBaseType.SQLite, mappingTypes);
        ISession session = OpenSession(cfg.BuildSessionFactory());

#if DEBUG
        new SchemaExport(cfg).Create(false, true, session.Connection);
#endif

        return session;
    }

    /// <summary>
    /// 会创建所有映射的数据库表
    /// </summary>
    /// <param name="mappingAssembly"></param>
    /// <returns></returns>
    public static ISession CreateTablesAndOpenSession(Assembly mappingAssembly) {
        NLogConfiguration.Config();

        var cfg = BuildConfiguration(DataBaseConfig.DataBaseType.SQLite, mappingAssembly);
        ISession session = OpenSession(cfg.BuildSessionFactory());

#if DEBUG
        new SchemaExport(cfg).Create(false, true, session.Connection);
#endif

        return session;
    }

    public static ISession OpenSession(ISessionFactory sessionFactory) {
        ISession session = sessionFactory.OpenSession(); // configuration.SetInterceptor(new HibernateInterceptor()); 中配置了 HibernateInterceptor
        // session.DefaultReadOnly = true; 这个不能加上，否则 更新数据 会不成功

        return session;
    }
}