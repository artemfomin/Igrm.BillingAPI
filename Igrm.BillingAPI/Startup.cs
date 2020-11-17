using Igrm.BillingAPI.Infrastructure;
using Igrm.BillingAPI.Repositories;
using Igrm.BillingAPI.Services;
using Igrm.BillingAPI.Strategies;
using Igrm.BillingAPI.Workers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System;
using System.Reflection;

namespace Igrm.BillingAPI
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
            services.AddDbContext<BillingAPIContext>(options =>
            {
                string id = ":memory:";

                var builder = new SqliteConnectionStringBuilder()
                {
                    DataSource = id,
                    Mode = SqliteOpenMode.Memory,
                    Cache = SqliteCacheMode.Shared
                };

                var connection = new SqliteConnection(builder.ConnectionString);
                connection.Open();
                connection.EnableExtensions(true);

                options.UseSqlite(connection);
                options.UseLazyLoadingProxies();
            });

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IOrderProcessingService, OrderProcessingService>();
            services.AddScoped<IBillingConfigurationService, BillingConfigurationService>();
            services.AddScoped<IGatewayStrategyFactory, GatewayStrategyFactory>();

            services.AddControllers();
            services.AddMemoryCache();
            
            services.AddHttpClient<IOrderProcessingService, OrderProcessingService>()
                    .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
                                                          .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                                                          .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
            
            services.AddHttpClient<IGatewayStrategyFactory, GatewayStrategyFactory>()
                    .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
                                                          .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                                                          .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Igrm.BillingAPI", Version = "v1" });
            });

            services.AddHostedService<SettlementAllocationWorker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseSerilogRequestLogging();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Igrm.BillingAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
