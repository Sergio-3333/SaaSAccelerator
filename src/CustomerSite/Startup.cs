using System;
using Azure.Identity;
using Marketplace.Saas.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Repositories;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Marketplace.SaaS.Accelerator.CustomerSite.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Marketplace.SaaS;

namespace Marketplace.SaaS.Accelerator.AdminSite;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        // Configuración del cliente SaaS
        var config = new SaaSApiClientConfiguration
        {
            AdAuthenticationEndPoint = Configuration["SaaSApiConfiguration:AdAuthenticationEndPoint"],
            ClientId = Configuration["SaaSApiConfiguration:ClientId"] ?? Guid.Empty.ToString(),
            ClientSecret = Configuration["SaaSApiConfiguration:ClientSecret"] ?? string.Empty,
            FulFillmentAPIBaseURL = Configuration["SaaSApiConfiguration:FulFillmentAPIBaseURL"],
            MTClientId = Configuration["SaaSApiConfiguration:MTClientId"] ?? Guid.Empty.ToString(),
            FulFillmentAPIVersion = Configuration["SaaSApiConfiguration:FulFillmentAPIVersion"],
            GrantType = Configuration["SaaSApiConfiguration:GrantType"],
            Resource = Configuration["SaaSApiConfiguration:Resource"],
            SaaSAppUrl = Configuration["SaaSApiConfiguration:SaaSAppUrl"],
            SignedOutRedirectUri = Configuration["SaaSApiConfiguration:SignedOutRedirectUri"],
            TenantId = Configuration["SaaSApiConfiguration:TenantId"] ?? Guid.Empty.ToString(),
            IsAdminPortalMultiTenant = Configuration["SaaSApiConfiguration:IsAdminPortalMultiTenant"]
        };

        var creds = new ClientSecretCredential(config.TenantId, config.ClientId, config.ClientSecret);
        var isMultiTenant = config.IsAdminPortalMultiTenant?.ToLower().Trim() ?? "false";

        // Autenticación con Azure AD
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddOpenIdConnect(options =>
            {
                options.Authority = isMultiTenant == "false"
                    ? $"{config.AdAuthenticationEndPoint}/{config.TenantId}/v2.0"
                    : $"{config.AdAuthenticationEndPoint}/common/v2.0";

                options.ClientId = config.MTClientId;
                options.ResponseType = OpenIdConnectResponseType.IdToken;
                options.CallbackPath = "/Home/Index";
                options.SignedOutRedirectUri = config.SignedOutRedirectUri;
                options.TokenValidationParameters.NameClaimType = ClaimConstants.CLAIM_SHORT_NAME;
                options.TokenValidationParameters.ValidateIssuer = false;
            })
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.Cookie.MaxAge = options.ExpireTimeSpan;
                options.SlidingExpiration = true;
            });

        // Fulfillment API
        var fulfillmentBaseApi = Uri.TryCreate(config.FulFillmentAPIBaseURL, UriKind.Absolute, out var uri)
            ? uri
            : new Uri("https://marketplaceapi.microsoft.com/api");

        services.AddSingleton<IFulfillmentApiService, FulfillmentApiService>();

        services.AddSingleton<SaaSApiClientConfiguration>(config);

        // DbContext y repositorios
        services.AddDbContext<SaasKitContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IClientsRepository, ClientsRepository>();
        services.AddScoped<IClientsService, ClientsService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ILicensesRepository, LicensesRepository>();
        services.AddScoped<ILicenseService, LicenseService>();

        // HomeController (si usas vistas)
        services.AddScoped<SaaSClientLogger<HomeController>>();

        // Sesión y MVC
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(5);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddMvc(option =>
        {
            option.EnableEndpointRouting = false;
            option.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        });

        services.AddControllersWithViews();

        services.Configure<CookieTempDataProviderOptions>(options =>
        {
            options.Cookie.IsEssential = true;
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        app.UseSession();
        app.UseAuthentication();
        app.UseMvc(routes =>
        {
            routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
