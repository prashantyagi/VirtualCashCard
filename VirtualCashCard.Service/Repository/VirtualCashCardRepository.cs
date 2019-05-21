using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualCashCard.Service
{
    // Note - No unit test for repo, this is to show a working solution
    public class VirtualCashCardRepository : IVirtualCashCardRepository
    {
        // singleton class to show multi-threading use
        private static readonly VirtualCashCardRepository instance = new VirtualCashCardRepository();

        public static VirtualCashCardRepository Instance { get; } = instance;

        static VirtualCashCardRepository()
        {
        }

        private VirtualCashCardRepository()
        {
            _accountBalances = new ConcurrentDictionary<long, decimal>();

            _accounts.ForEach(x => _accountBalances.TryAdd(x.Id, 0m));
        }

        private readonly ConcurrentDictionary<long, decimal> _accountBalances;

        // mock account collection
        private readonly List<Account> _accounts = new List<Account>
            {
                new Account{Id = 1, AccountNumber = "00000001", CardNumber = "444444444441"},
                new Account{Id = 2, AccountNumber = "00000002", CardNumber = "444444444442"},
                new Account{Id = 3, AccountNumber = "00000003", CardNumber = "444444444443"},
                new Account{Id = 4, AccountNumber = "00000004", CardNumber = "444444444444"},
                new Account{Id = 5, AccountNumber = "00000005", CardNumber = "444444444445"}
            };

        async Task<Response<Account>> IVirtualCashCardRepository.GetAccount(string cardNumber, string pin)
        {
            return await Task.Run(() =>
            {
                var response = new Response<Account>();
                try
                {
                    response.Data = _accounts.FirstOrDefault(x => x.CardNumber == cardNumber && pin == x.Id.ToString());
                    if (response.Data != null)
                    {
                        response.Success = true;
                    }
                    else 
                    {
                        response.ErrorMessage = "No associated account found.";
                    }
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.ToString();
                }
                return response;
            });
        }

        async Task<Response<Account>> IVirtualCashCardRepository.GetAccount(string accountNumber)
        {
            return await Task.Run(() =>
            {
                var response = new Response<Account>();
                try
                {
                    response.Data = _accounts.FirstOrDefault(x => x.AccountNumber == accountNumber);
                    if (response.Data != null)
                    {
                        response.Success = true;
                    }
                    else
                    {
                        response.ErrorMessage = "No associated account found.";
                    }
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.ToString();
                }
                return response;
            });
        }

        async Task<Response> IVirtualCashCardRepository.Withdraw(long accountId, int amount)
        {
            return await Task.Run(() =>
            {
                var response = new Response();
                try
                {
                    _accountBalances.TryGetValue(accountId, out decimal oldValue);
                    if(amount > oldValue) 
                    {
                        response.ErrorMessage = "Insufficient balance.";
                        return response;
                    }

                    decimal newBalance = oldValue - amount;
                    _accountBalances.TryUpdate(accountId, newBalance, oldValue);
                    Console.WriteLine($"Withdrew {amount} from {accountId}, prev balance {oldValue} was, new balance is {newBalance}");

                    response.Success = true;
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.ToString();
                }
                return response;
            });
        }

        async Task<Response> IVirtualCashCardRepository.Deposit(long accountId, int amount)
        {
            return await Task.Run(() =>
            {
                var response = new Response();
                try
                {
                    _accountBalances.TryGetValue(accountId, out decimal oldValue);
                    decimal newBalance = oldValue + amount;
                    _accountBalances.TryUpdate(accountId, newBalance, oldValue);
                    Console.WriteLine($"Deposited {amount} into {accountId}, prev balance {oldValue} was, new balance is {newBalance}");
                    response.Success = true;
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = ex.ToString();
                }
                return response;
            });
        }
    }
}