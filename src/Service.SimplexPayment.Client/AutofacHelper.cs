using Autofac;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using Service.SimplexPayment.Domain.Models.NoSql;
using Service.SimplexPayment.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.SimplexPayment.Client
{
    public static class AutofacHelper
    {
        public static void RegisterSimplexPaymentClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new SimplexPaymentClientFactory(grpcServiceUrl, null);

            builder.RegisterInstance(factory.GetSimplexService()).As<ISimplexPaymentService>().SingleInstance();
        }
        
        public static void RegisterSimplexInProgressClient(this ContainerBuilder builder, string grpcServiceUrl, IMyNoSqlSubscriber myNoSqlSubscriber)
        {
            var subs = new MyNoSqlReadRepository<BuysInProgressNoSqlEntity>(myNoSqlSubscriber, BuysInProgressNoSqlEntity.TableName);

            var factory = new SimplexPaymentClientFactory(grpcServiceUrl, subs);
            
            builder
                .RegisterInstance(subs)
                .As<IMyNoSqlServerDataReader<BuysInProgressNoSqlEntity>>()
                .SingleInstance();

            builder
                .RegisterInstance(factory.GetInProgressClient())
                .As<IInProgressBuysService>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}
