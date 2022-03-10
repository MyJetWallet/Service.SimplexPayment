using Autofac;
using Service.SimplexPayment.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.SimplexPayment.Client
{
    public static class AutofacHelper
    {
        public static void RegisterSimplexPaymentClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new SimplexPaymentClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetHelloService()).As<IHelloService>().SingleInstance();
        }
    }
}
