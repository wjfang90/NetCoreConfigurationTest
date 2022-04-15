using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //configuration.GetChildren();

            //ILoggerFactory loggerFactory = null;
            //var logger= loggerFactory.CreateLogger<Startup>();
            //logger.LogInformation("");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            {
                //启用目录浏览
                //services.AddDirectoryBrowser();//EnableDirectoryBrowsing 属性值为 true 时必须调用 AddDirectoryBrowse
            }

            {
                //将路由添加到 Startup.ConfigureServices 中的服务容器：
                //services.AddRouting();

            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //var s1 = "test";
            //var s = $"{ s1}";
            //var s2 = $"select * from table where name={s1}";

            //app.Map("",)



            #region 静态文件
            {
                //web根目录内的
                //app.UseStaticFiles();// For the wwwroot folder

                //web根目录目录外的
                // MyStaticFiles\images\banner1.svg

                //app.UseStaticFiles(new StaticFileOptions()
                //{
                //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "MyStaticFiles")),
                //    RequestPath = "/StaticFiles",
                //});

                //<img src="~/StaticFiles/images/banner1.svg" alt="ASP.NET" class="img-responsive" />

            }

            {
                //设置http响应头
                //var cacheprend = env.IsDevelopment() ? "600" : "60000";

                //app.UseStaticFiles(new StaticFileOptions()
                //{
                //    OnPrepareResponse = ctx => ctx.Context.Response.Headers.Append("Cache-Control", $"public,max-age={cacheprend}")
                //});
            }


            {
                //静态文件授权
                // [Authorize]
                //public IActionResult BannerImage()
                //{
                //    var file = Path.Combine(Directory.GetCurrentDirectory(),
                //                            "MyStaticFiles", "images", "banner1.svg");

                //    return PhysicalFile(file, "image/svg+xml");
                //}
            }

            {
                //启用目录浏览
                //app.UseDirectoryBrowser(new DirectoryBrowserOptions
                //{
                ///MyImages 浏览 wwwroot/images 文件夹的目录
                //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images")),
                //    RequestPath = "/MyImages"
                //});
            }
            {
                //提供默认文档
                //default.htm
                //default.html
                //index.htm
                //index.html
                //要提供默认文件，必须在 UseStaticFiles 前调用 UseDefaultFiles。 UseDefaultFiles 实际上用于重写 URL，不提供文件。 通过 UseStaticFiles 启用静态文件中间件来提供文件。
                //app.UseDefaultFiles();

                //app.UseStaticFiles();


                //自定义默认文档
                //DefaultFilesOptions dfo = new DefaultFilesOptions();
                //dfo.DefaultFileNames.Clear();
                //dfo.DefaultFileNames.Add("mydefault.html");
                //app.UseDefaultFiles(dfo);
                //app.UseStaticFiles();
            }

            {
                //UseFileServer 
                //UseFileServer 结合了 UseStaticFiles、UseDefaultFiles 和 UseDirectoryBrowser 的功能。

                //app.UseFileServer();//提供静态文件和默认文件

                //app.UseFileServer(enableDirectoryBrowsing: true);//启用目录浏览


                //app.UseFileServer(new FileServerOptions()
                //{
                //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "MyStaticFiles")),
                //    RequestPath = "/MyStaticFiles",
                //    EnableDirectoryBrowsing = true//EnableDirectoryBrowsing 属性值为 true 时必须在ConfigureServices中调用 AddDirectoryBrowse
                //});

            }

            {
                //var provider = new FileExtensionContentTypeProvider();
                ////添加文件扩展名到MIME内容类型映射
                //provider.Mappings.Add(".myapp", "application/x-msdownload");
                //provider.Mappings[".htm3"] = "text/html";

                ////替换文件扩展名
                //provider.Mappings[".rtf"] = "application/x-msdownload";

                ////删除扩展名
                //provider.Mappings.Remove(".mp4");

                //app.UseStaticFiles(new StaticFileOptions()
                //{
                //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images")),
                //    RequestPath = "/MyImages",
                //    ContentTypeProvider = provider
                //});

            }

            {
                //非标准内容类型
                //请求的文件含未知内容类型时，以图像形式返回请求。
                //app.UseStaticFiles(new StaticFileOptions()
                //{
                //    ServeUnknownFileTypes = true,
                //    DefaultContentType = "image/jpg"
                //});
            }
            #endregion


            {
                //路由配置
                //new Microsoft.AspNetCore.Routing.RouteBuilder(app).MapRoute("routeName", "templateName");
            }

            {
                ////url重写中间件

                //////通过使用扩展方法为每条规则创建 RewriteOptions 类的实例，建立 URL 重写和重定向规则
                //using (StreamReader apacheModRewriteStreamReader = File.OpenText("appacheRewrite.txt"))
                //using (StreamReader iisUrlRewriteStreamReader = File.OpenText("IISUrlRewrite.xml"))
                //{

                //    var options = new RewriteOptions()
                //                //使用 AddRedirect 将请求重定向。
                //                .AddRedirect("redirect-rule/(.*)", "redirected/$1")

                //                .AddRewrite(@"^rewrite-rule/(\d+)/(\d+)", "rewritten?var1=$1&var2=$2", skipRemainingRules: true)

                //    //URL 重定向到安全的终结点.将状态代码设置为“301(永久移动)”并将端口更改为 5001。
                //    .AddRedirectToHttps(301, 5001)

                //    //将不安全的请求重定向到采用安全 HTTPS 协议（端口 443 上的 https://）的相同主机和路径
                //    .AddRedirectToHttpsPermanent()

                //                .AddApacheModRewrite(apacheModRewriteStreamReader)
                //                .AddIISUrlRewrite(iisUrlRewriteStreamReader)
                //                //基于方法的规则
                //                .Add(MethodRules.RedirectXMLRequests)
                //                //使用 Add(IRule) 在派生自 IRule 的类中实现自己的规则逻辑
                //                .Add(new RedirectImageRequests(".png", "/png-images"))
                //                .Add(new RedirectImageRequests(".jpg", "/jpg-images"));


                //    app.UseRewriter(options);

                //}

            }

            {
                ////环境变量
                ////在 Windows 和 macOS 上，环境变量和值不区分大小写。 默认情况下，Linux 环境变量和值要区分大小写。
                ////Properties\launchSettings.json 文件中设置环境变量

                env.IsDevelopment();
                env.IsProduction(); //default
                env.IsStaging();


                ////在 IIS 中托管应用并添加或更改 ASPNETCORE_ENVIRONMENT 环境变量时，请采用下列方法之一，让新值可供应用拾取：
                ////重启应用的应用池。
                ////在命令提示符处依次执行 net stop was / y 和 net start w3svc。
                ////重启服务器。
            }

        }
    }


}
