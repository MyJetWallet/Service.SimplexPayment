using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.ApiSecurityManager.Autofac;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.ServiceBus;
using Service.ClientProfile.Client;
using Service.PersonalData.Client;
using Service.SimplexPayment.Domain;
using Service.SimplexPayment.Domain.Models;
using Service.SimplexPayment.Domain.Models.NoSql;
using Service.SimplexPayment.Grpc;
using Service.SimplexPayment.Jobs;
using Service.SimplexPayment.Services;

namespace Service.SimplexPayment.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMyNoSqlWriter<DepositAddressNoSqlEntity>(() => Program.Settings.MyNoSqlWriterUrl,
                DepositAddressNoSqlEntity.TableName);
            builder.RegisterMyNoSqlWriter<PendingPaymentNoSqlEntity>(() => Program.Settings.MyNoSqlWriterUrl,
                PendingPaymentNoSqlEntity.TableName);
            builder.RegisterMyNoSqlWriter<SimplexEventsNoSqlEntity>(() => Program.Settings.MyNoSqlWriterUrl,
                SimplexEventsNoSqlEntity.TableName);
            builder.RegisterMyNoSqlWriter<BuysInProgressNoSqlEntity>(() => Program.Settings.MyNoSqlWriterUrl,
                BuysInProgressNoSqlEntity.TableName);
            
            var noSqlClient = builder.CreateNoSqlClient(Program.Settings.MyNoSqlReaderHostPort, Program.LogFactory);
            builder.RegisterClientProfileClients(noSqlClient, Program.Settings.ClientProfileGrpcServiceUrl); 
            builder.RegisterPersonalDataClient(Program.Settings.PersonalDataServiceUrl);
            
            var serviceBus = builder.RegisterMyServiceBusTcpClient(() => Program.Settings.SpotServiceBusHostPort, Program.LogFactory);
            builder.RegisterMyServiceBusPublisher<SimplexIntention>(serviceBus, SimplexIntention.TopicName, true);
            
            builder
                .RegisterType<SimplexHttpClient>()
                .AsSelf()
                .SingleInstance()
                .AutoActivate();

            builder
                .RegisterType<SimplexPaymentService>()
                .AsSelf()
                .SingleInstance()
                .AutoActivate();
            
            builder
                .RegisterType<SimplexEventCleaningJob>()
                .AsSelf()
                .SingleInstance()
                .AutoActivate();
            
            builder
                .RegisterType<InProgressBuysService>()
                .As<IInProgressBuysService>()
                .SingleInstance()
                .AutoActivate();

            builder.RegisterEncryptionServiceClient();
        }
    }
}