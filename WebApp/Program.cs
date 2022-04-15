using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApp.config;

namespace WebApp
{
    public class Program
    {

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();

            //CreateWebHostBuilder2(args).Build().Run();

            //CreateWebHostBuilder3().Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
        .ConfigureServices(s =>
        {
            s.AddMvc();
        })
        .Configure(s =>
        {
            s.UseMvcWithDefaultRoute();
            s.UseStaticFiles();
        })
        .ConfigureAppConfiguration((webHostContext, config) =>
        {
            var tvshow = new TvShow();
            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddXmlFile("tvshow.xml", optional: false, reloadOnChange: false);
            config.Build().GetSection("tvshow").Bind(tvshow);
        })

        .UseStartup<Startup>();



        ////基础 -- 配置
        public static IWebHostBuilder CreateWebHostBuilder0(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            Dictionary<string, string> configArray = new Dictionary<string, string>()
            {
                {"array:entries:0","value0" },
                {"array:entries:1","value1" },
                {"array:entries:2","value2" },
                {"array:entries:4","value4" },
                {"array:entries:5","value5" }
            };


            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddInMemoryCollection(configArray);
            config.AddJsonFile("json_file.json", optional: false, reloadOnChange: false);
            config.AddXmlFile("xml_file.xml", optional: false, reloadOnChange: false);
            //config.AddEFConfiguration(options => options.UseInMemoryDatabase("InMemoryDb"));
            config.AddCommandLine(args);
        })
        .UseStartup<Startup>();



        /// <summary>
        /// 基础 -- 配置--交换映射
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder2(string[] args)
        {
            Dictionary<string, string> configSwitchMapping = new Dictionary<string, string>()
            {
                { "-CLKey1", "CommandLineKey1" },
                { "-CLKey2", "CommandLineKey2" }
            };

            return WebHost.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddCommandLine(args, configSwitchMapping);
                })
                .UseStartup<Startup>();
        }

        /// <summary>
        /// 基础 -- 配置--环境变量提供程序
        /// </summary>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder3()
        {

            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                //前缀  要筛选前缀 PREFIX_ 上的环境变量
                //.AddEnvironmentVariables(prefix: "PREFIX_")
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseStartup<Startup>();

            return host;
        }

        /// <summary>
        /// 基础--配置--文件配置提供程序
        /// </summary>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder4()
        {
            //INI 配置文件的通用示例：
            //[section0]
            //key0 = value
            //key1 = value

            //[section1]
            //subsection: key = value

            //[section2: subsection0]
            //key = value

            //配置文件使用 value 加载以下键：冒号可用作 INI 文件配置中的节分隔符
            //section0: key0
            //section0:key1
            //section1:subsection: key
            //section2:subsection0: key
            //section2:subsection1: key

            //INI 配置提供程序
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddIniFile("ini_file.ini", optional: true, reloadOnChange: true)
                .Build();

            var host = new WebHostBuilder()
               .UseConfiguration(config)
               .UseKestrel()
               .UseStartup<Startup>();

            //JSON 配置提供程序
            //使用 CreateDefaultBuilder 初始化新的 WebHostBuilder 时，会自动调用 AddJsonFile 两次。 调用该方法来从以下文件加载配置：
            //appsettings.json – 首先读取此文件。 该文件的环境版本可以替代 appsettings.json 文件提供的值。
            //appsettings.{ Environment}.json– 根据 IHostingEnvironment.EnvironmentName 加载文件的环境版本。
            //此外，CreateDefaultBuilder 也会加载：
            //环境变量。
            //用户机密(Secret Manager)（在开发环境中）。
            //命令行参数。
            var webHost = WebHost.CreateDefaultBuilder()
                .ConfigureAppConfiguration((webhostcontext, config1) =>
                {
                    config1.SetBasePath(Directory.GetCurrentDirectory());
                    config1.AddIniFile("json_file.json", optional: true, reloadOnChange: true);
                })
            .UseKestrel()
            .UseStartup<Startup>();


            //XML 配置提供程序
            //XML 配置文件可以为重复节使用不同的元素名称：

            //<? xml version = "1.0" encoding = "UTF-8" ?>
            //< configuration >   
            //< section0 >   
            //< key0 > value </ key0 >   
            //< key1 > value </ key1 >   
            //</ section0 >   
            //< section1 >   
            //< key0 > value </ key0 >   
            //< key1 > value </ key1 >   
            //</ section1 >
            //</ configuration >

            //以前的配置文件使用 value 加载以下键：
            //section0: key0
            //section0:key1
            //section1:key0
            //section1:key1

            var webHost2 = WebHost.CreateDefaultBuilder()
                .ConfigureAppConfiguration((webHostContext, config2) =>
                {
                    config2.SetBasePath(Directory.GetCurrentDirectory());
                    config2.AddXmlFile("xml_file.xml", optional: false, reloadOnChange: false);
                })
                .UseKestrel()
                .UseStartup<Startup>();




            return host;
        }

        /// <summary>
        /// 基础--配置-- Key-per-file 配置提供程序
        /// </summary>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder5()
        {
            //KeyPerFileConfigurationProvider 使用目录的文件作为配置键值对。 该键是文件名。 该值包含文件的内容。 Key - per - file 配置提供程序用于 Docker 托管方案
            var path = Path.Combine(Directory.GetCurrentDirectory(), "path/to/files");
            var webHost3 = WebHost.CreateDefaultBuilder()
                .ConfigureAppConfiguration((webHostContext, config3) =>
                {
                    config3.SetBasePath(Directory.GetCurrentDirectory())
                    .AddKeyPerFile(directoryPath: path, optional: false);
                })
                .UseKestrel()
                .UseStartup<Startup>();

            return webHost3;
        }

        /// <summary>
        /// 基础--配置--内存配置提供程序
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder6(string[] args) =>
            WebHost.CreateDefaultBuilder()
            .ConfigureAppConfiguration((webHostContext, config) =>
            {
                var dict = new Dictionary<string, string>()
                {
                    {"MemoryCollectionKey1","value1" },
                    {"MemoryCollectionKey2","value2" }
                };
                config.AddInMemoryCollection(dict);

                {
                    //GetValue
                    //用键 NumberKey 从配置中提取字符串值，键入该值作为 int，并将值存储在变量 intValue 中。 如果在配置键中找不到 NumberKey，则 intValue 会接收 99 的默认值：
                    var getValue = config.Build().GetValue("NumberKey", 99);


                    //GetSection、GetChildren 和 Exists

                    //json内容；
                    //{
                    //    "section0": {
                    //        "key0": "value",
                    //        "key1": "value"
                    //    },
                    //    "section1": {
                    //        "key0": "value",
                    //        "key1": "value"
                    //    },
                    //    "section2": {
                    //        "subsection0" : {
                    //            "key0": "value",
                    //            "key1": "value"
                    //        },
                    //        "subsection1" : {
                    //            "key0": "value",
                    //            "key1": "value"
                    //        }
                    //    }
                    //}

                    //将文件读入配置时，会创建以下唯一的分层键来保存配置值：
                    //section0: key0
                    //section0:key1
                    //section1:key0
                    //section1:key1
                    //section2:subsection0: key0
                    //section2:subsection0: key1
                    //section2:subsection1: key0
                    //section2:subsection1: key1

                    //若要返回仅包含 section1 中键值对的 IConfigurationSection
                    var section1 = config.Build().GetSection("section1");

                    //若要获取 section2:subsection0 中键的值
                    var section2_subsection0 = config.Build().GetSection("section2:subsection0");

                    //GetChildren
                    var section2 = config.Build().GetSection("section2");
                    var section2_children = section2.GetChildren();

                    //exists 确定配置节是否存在
                    var sectionExits = config.Build().GetSection("section2:subsection2").Exists();

                }

                {
                    //绑定至类
                    var starShip = new Starship();
                    config.Build().GetSection("starship").Bind(starShip);

                    //Starship = starShip;
                }

            })
            .UseStartup<Startup>();


        /// <summary>
        /// 基础--配置--绑定至类
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder7(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
             .ConfigureAppConfiguration((webHostContext, config) =>
             {
                 var starShip = new Starship();
                 config.SetBasePath(Directory.GetCurrentDirectory());
                 config.AddJsonFile("starship.json", optional: false, reloadOnChange: false);
                 config.Build().GetSection("starship").Bind(starShip);
             })
            .UseStartup<Startup>();

        public static IWebHostBuilder CreateWebHostBuilder8(string[] args) =>
            WebHost.CreateDefaultBuilder()
            .ConfigureAppConfiguration((webHostContext, config) =>
            {
                var tvshow = new TvShow();
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddXmlFile("tvshow.xml", optional: false, reloadOnChange: false);
                config.Build().GetSection("tvshow").Bind(tvshow);
            })
            .UseStartup<Startup>();

    }



}
