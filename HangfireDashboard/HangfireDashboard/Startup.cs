using HangfireData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HangfireDashboard
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider => Configuration);
            services.AddSingleton<IConnectionStringManager>(cs => new ConnectionStringManager(Configuration));
            services.AddScoped<IDatabaseService, DatabaseService>();
            services.AddScoped<IHangfireSack, WebHangfireSack>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //    routes.MapRoute(
            //        name: "api",
            //        template: "api/{controller}/{action}"
            //        );
            //    routes.MapSpaFallbackRoute(
            //        name: "spa-fallback",
            //        defaults: new { controller = "Home", action = "Index" });
            //});
            app.UseMvc();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "HangfireClient";
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer("start");
                }
            });
        }
    }
}
