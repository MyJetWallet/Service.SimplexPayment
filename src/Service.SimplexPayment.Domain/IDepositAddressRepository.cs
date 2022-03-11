using System.Threading.Tasks;

namespace Service.SimplexPayment.Domain;

public interface IDepositAddressRepository
{
    Task<(string address, string tag)> GetAddressAndTag(string asset);
}