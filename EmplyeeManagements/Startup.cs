using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EmplyeeManagements.Models;
using EmplyeeManagements.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmplyeeManagements
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
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


            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder().
                RequireAuthenticatedUser().Build();

                config.Filters.Add(new AuthorizeFilter(policy));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);




            services.AddDbContext<AppDbContext>(options => options
            .UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));
            
            services.AddIdentity<ApplicationUser, IdentityRole>(options => 
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;



            }).AddEntityFrameworkStores<AppDbContext>();


            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");

            });


            services.AddAuthorization(options =>
            {

                options.AddPolicy("DeleteRolePolicy",
                    policy => policy.RequireClaim("Delete Role", "True"));

                options.AddPolicy("EditRolePolicy1",
                    policy => policy.RequireAssertion(context =>
                  context.User.IsInRole("Admin") &&
                   context.User.HasClaim(claim => claim.Type == "Edit Role"
                  && claim.Value == "True")
                   || context.User.IsInRole("Super Admin")

                   ));



                options.AddPolicy("EditRolePolicy",
                   policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirements())

                  );

                options.AddPolicy("AdminPolicy",
               policy => policy.RequireRole("Admin"));


                options.AddPolicy("ManageAdministration", policy =>
                    policy.RequireAssertion(context =>
                    context.User.IsInRole("Super Admin") ||
                    context.User.IsInRole("Admin")));



               




            });






            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddSingleton<IAuthorizationHandler, CanOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminManageHandler>();
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

                app.UseStatusCodePagesWithReExecute("/Error/{0}");


                app.UseExceptionHandler("/Error");

            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
