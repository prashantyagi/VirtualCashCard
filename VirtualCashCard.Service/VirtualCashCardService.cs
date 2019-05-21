using System;
using System.Threading.Tasks;

namespace VirtualCashCard.Service
{
    public class VirtualCashCardService : IVirtualCashCardService
    {
        private readonly IVirtualCashCardRepository _repository;

        public VirtualCashCardService(IVirtualCashCardRepository repository)
        {
            // repository should be dependency injected
            _repository = repository;
        }

        async Task<Response> IVirtualCashCardService.Withdraw(string cardNumber, string pin, int amount)
        {
            var response = new Response();

            try
            {
                if (string.IsNullOrWhiteSpace(cardNumber))
                {
                    response.ErrorMessage = "Invalid card number.";
                    return response;
                }

                if (string.IsNullOrWhiteSpace(pin))
                {
                    response.ErrorMessage = "Invalid pin number.";
                    return response;
                }

                if (amount <= 0)
                {
                    response.ErrorMessage = "Invalid amount.";
                    return response;
                }

                var getAccountResponse = await _repository.GetAccount(cardNumber, pin);

                if (getAccountResponse == null || !getAccountResponse.Success)
                {
                    response.ErrorMessage = getAccountResponse.ErrorMessage;
                    return response;
                }

                var withdrawResponse = await _repository.Withdraw(getAccountResponse.Data.Id, amount);
                if (!withdrawResponse.Success)
                {
                    response.ErrorMessage = withdrawResponse.ErrorMessage;
                    return response;
                }

                response.Success = true;
            }
            catch(Exception) 
            {
                // Log exception using DI Logger (Nuget Common Logging)
                response.ErrorMessage = "Unexpected error while withdrawing.";
            }

            return response;
        }

        async Task<Response> IVirtualCashCardService.Deposit(string accountNumber, int amount)
        {
            var response = new Response();

            try
            {
                if (string.IsNullOrWhiteSpace(accountNumber))
                {
                    response.ErrorMessage = "Invalid account number.";
                    return response;
                }

                if (amount <= 0)
                {
                    response.ErrorMessage = "Invalid amount.";
                    return response;
                }

                var getAccountResponse = await _repository.GetAccount(accountNumber);

                if (getAccountResponse == null || !getAccountResponse.Success)
                {
                    response.ErrorMessage = getAccountResponse.ErrorMessage;
                    return response;
                }

                var depositResponse = await _repository.Deposit(getAccountResponse.Data.Id, amount);
                if (!depositResponse.Success)
                {
                    response.ErrorMessage = depositResponse.ErrorMessage;
                    return response;
                }

                response.Success = true;
            }
            catch (Exception)
            {
                // Log exception using DI Logger (Nuget Common Logging)
                response.ErrorMessage = "Unexpected error while withdrawing.";
            }

            return response;
        }
    }
}
