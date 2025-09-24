
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Repositories;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Marketplace.SaaS.Accelerator.CustomerSite;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Configuración Fulfillment API
        var config = new SaaSApiClientConfiguration
        {
            ClientId = Configuration["SaaSApiConfiguration:ClientId"],
            ClientSecret = Configuration["SaaSApiConfiguration:ClientSecret"],
            TenantId = Configuration["SaaSApiConfiguration:TenantId"],
            FulFillmentAPIBaseURL = Configuration["SaaSApiConfiguration:FulFillmentAPIBaseURL"],
            FulFillmentAPIVersion = Configuration["SaaSApiConfiguration:FulFillmentAPIVersion"],
            Resource = Configuration["SaaSApiConfiguration:Resource"],
            SaaSAppUrl = Configuration["SaaSApiConfiguration:SaaSAppUrl"]
        };

        services.AddSingleton(config);
        services.AddHttpClient<IFulfillmentApiService, FulfillmentApiService>();

        // DbContext y repositorios
        services.AddDbContext<SaasKitContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IClientsRepository, ClientsRepository>();
        services.AddScoped<ISubscriptionsRepository, SubscriptionsRepository>();
        services.AddScoped<ILicensesRepository, LicensesRepository>();
        services.AddScoped<ISubLinesRepository, SubLinesRepository>();

        // Servicios de negocio
        services.AddScoped<ClientsService>();
        services.AddScoped<SubscriptionService>();
        services.AddScoped<LicenseService>();
        services.AddScoped<SubLinesService>();

        // Solo controladores (sin vistas)
        services.AddControllers();
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

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers(); // Solo API controllers
        });
    }
}

