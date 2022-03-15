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
            
            GetEnvVariables();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ConfigureJetWallet(env, endpoints =>
            {
                endpoints.MapGrpcSchema<SimplexPaymentService, ISimplexPaymentService>();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureJetWallet();
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<ServiceModule>();
        }
        
        private void GetEnvVariables()
        {
            var key = Environment.GetEnvironmentVariable(Program.EncodingKeyStr);
            
            if (string.IsNullOrEmpty(key))
                throw new Exception($"Env Variable {Program.EncodingKeyStr} is not found");

            Program.EncodingKey = Encoding.UTF8.GetBytes(key);
        }
    }
}
