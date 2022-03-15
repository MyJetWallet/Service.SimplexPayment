using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.ClientProfile.Grpc;
using Service.ClientProfile.Grpc.Models.Requests;
using Service.SimplexPayment.Domain.Models;
using Service.SimplexPayment.Grpc;
using Service.SimplexPayment.Grpc.Models;
using Service.SimplexPayment.Postgres;
using SimpleTrading.Common.Helpers;
using HexConverterUtils = MyJetWallet.Sdk.Service.HexConverterUtils;

namespace Service.SimplexPayment.Services
{
    public class SimplexPaymentServiceGrpc: ISimplexPaymentService
    {
        private readonly SimplexPaymentService _simplexPaymentService;

        public SimplexPaymentServiceGrpc(SimplexPaymentService simplexPaymentService)
        {
            _simplexPaymentService = simplexPaymentService;
        }

        public async Task<ExecuteQuoteResponse> RequestPayment(RequestPaymentRequest requestPaymentRequest) => await _simplexPaymentService.RequestPayment(requestPaymentRequest);
    }
}
