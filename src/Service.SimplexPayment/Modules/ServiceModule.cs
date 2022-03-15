using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.NoSql;
using Service.ClientProfile.Client;
using Service.SimplexPayment.Domain;
using Service.SimplexPayment.Domain.Models;
using Service.SimplexPayment.Services;

namespace Service.SimplexPayment.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMyNoSqlWriter<DepositAddressNoSqlEntity>(() => Program.Settings.MyNoSqlWriterUrl,
                DepositAddressNoSqlEntity.TableName);

            var noSqlClient = builder.CreateNoSqlClient((() => Program.Settings.MyNoSqlReaderHostPort));
            builder.RegisterClientProfileClients(noSqlClient, Program.Settings.ClientProfileGrpcServiceUrl);

            builder.RegisterType<DepositAddressRepositoryTemp>().As<IDepositAddressRepository>().SingleInstance().AutoActivate();
        }
    }
}