using System.Linq;
using System.Threading.Tasks;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataWriter;
using Service.SimplexPayment.Domain;
using Service.SimplexPayment.Domain.Models;

namespace Service.SimplexPayment.Services
{
    public class DepositAddressRepositoryTemp : IDepositAddressRepository
    {
        private readonly IMyNoSqlServerDataWriter<DepositAddressNoSqlEntity> _writer;

        public DepositAddressRepositoryTemp(IMyNoSqlServerDataWriter<DepositAddressNoSqlEntity> writer)
        {
            _writer = writer;
        }

        public async Task<(string address, string tag)> GetAddressAndTag(string asset)
        {
            var addresses = await _writer.GetAsync();
            var address = addresses.FirstOrDefault(t => t.RowKey == asset);
            return address != null ? (address.Address, address.Tag) : (null, null);
        }
    }
}