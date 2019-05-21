using System.Threading.Tasks;

namespace VirtualCashCard.Service
{
    public interface IVirtualCashCardService
    {
        Task<Response> Withdraw(string cardNumber, string pin, int amount);
        Task<Response> Deposit(string accountNumber, int amount);
    }
}