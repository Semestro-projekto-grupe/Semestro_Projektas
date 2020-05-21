using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Semestro_projektas.Data;
using Semestro_projektas.Data.Repository;
using Semestro_projektas.Models;
using Semestro_projektas.SignalR.Hubs;

namespace Semestro_projektas
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

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration["DefaultConnection"]));

            services.AddDefaultIdentity<User>(options =>
            { //slaptazodzio nustatymai (nemažinti nustatymų ir nekeisti, kitaip būtina pertvarkyti valdiklyje)
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.User.AllowedUserNameCharacters = String.Empty;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();


            services.AddTransient<IRepository, Repository>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;      //pakeist i true, kai jei veiks visom narsyklem
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            //nurodom papke kurioje laikysim resource failus
            services.AddLocalization(opts =>
            {
                opts.ResourcesPath = "Resources";
            });

            //planuojamu palaikyt kalbu sarasas
            services.Configure<RequestLocalizationOptions>(opts =>
            {
                var supportedCultures = new List<CultureInfo> {
                    new CultureInfo("en"),
                    //new CultureInfo("en-US"),
                    new CultureInfo("lt-LT"),
                  };

                //Nustatom "pagrindine" kalba
                opts.DefaultRequestCulture = new RequestCulture("lt-LT");
                // numeriu, datu ir t.t. formatui
                opts.SupportedCultures = supportedCultures;
                // UI vertimui
                opts.SupportedUICultures = supportedCultures;
            });

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);  Original
            services.AddMvc()
                    .AddViewLocalization(opts => { opts.ResourcesPath = "Resources"; })
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSignalR();
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

            //parodom kad "in pipeline" naudosim globalizacija
            //sukeitus vietom su kitais app pradeda galimai neveikt.
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
