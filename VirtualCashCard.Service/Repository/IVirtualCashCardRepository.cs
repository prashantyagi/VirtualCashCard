using System.Threading.Tasks;

namespace VirtualCashCard.Service
{
    public interface IVirtualCashCardRepository
    {
        Task<Response<Account>> GetAccount(string cardNumber, string pin);
        Task<Response<Account>> GetAccount(string accountNumber);
        Task<Response> Withdraw(long accountId, int amount);
        Task<Response> Deposit(long accountId, int amount);
    }
}