using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using MyJetWallet.Sdk.GrpcSchema;
using MyJetWallet.Sdk.Postgres;
using MyJetWallet.Sdk.Service;
using Service.SimplexPayment.Grpc;
using Service.SimplexPayment.Modules;
using Service.SimplexPayment.Postgres;
using Service.SimplexPayment.Services;
using MyJetWallet.ApiSecurityManager.Autofac;

namespace Service.SimplexPayment
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureJetWallet<ApplicationLifetimeManager>(Program.Settings.ZipkinUrl);

            DatabaseContext.LoggerFactory = Program.LogFactory;
            services.AddDatabase(DatabaseContext.Schema, Program.Settings.PostgresConnectionString,
                o => new DatabaseContext(o));
            DatabaseContext.LoggerFactory = null;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ConfigureJetWallet(env, endpoints =>
            {
                endpoints.MapGrpcSchema<SimplexPaymentServiceGrpc, ISimplexPaymentService>();
                endpoints.RegisterGrpcServices();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureJetWallet();
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<ServiceModule>();
        }
    }
}
