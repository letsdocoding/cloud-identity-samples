using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace letsdocoding_cognitologin
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
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId = "3g8ck87t9b9p03s7t7g86escta";
                    options.ClientSecret = "1tadusvi0fipp2j9jf147j1r9sl69nsq6h2c1dqlabfjiihbl975";
                    options.Authority = "https://cognito-idp.us-west-2.amazonaws.com/us-west-2_2pVIDwjSK";
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.ResponseType = "code";
                    options.Scope.Add("http://leave.letsdocoding.com/leaves.cancel");
                    //options.Scope.Add("http://leave.letsdocoding.com/leaves.apply");
                    options.SaveTokens = true;

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        NameClaimType =  "cognito:username"
                    };

                    options.Events = new OpenIdConnectEvents()
                    {
                        OnRedirectToIdentityProviderForSignOut = context =>
                        {
                            var logoutUri = $"https://letsdocoding-identity.auth.us-west-2.amazoncognito.com/logout?client_id=3g8ck87t9b9p03s7t7g86escta";
                            logoutUri += $"&logout_uri={context.Request.Scheme}://{context.Request.Host}";

                            //var postLogoutUri = context.Properties.RedirectUri;
                            //if (!string.IsNullOrEmpty(postLogoutUri))
                            //{
                            //    if (postLogoutUri.StartsWith("/"))
                            //    {
                            //        // transform to absolute
                            //        var request = context.Request;
                            //        postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                            //    }
                            //    logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                            //}

                            context.Response.Redirect(logoutUri);
                            context.HandleResponse();

                            return Task.CompletedTask;
                        }
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
