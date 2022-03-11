using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.NoSql;
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

            builder.RegisterType<DepositAddressRepositoryTemp>().As<IDepositAddressRepository>().SingleInstance();
        }
    }
}