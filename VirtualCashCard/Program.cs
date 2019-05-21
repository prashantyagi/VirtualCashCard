using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualCashCard.Service;
using System.Linq;

namespace VirtualCashCard
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Process started...");
            var accountList = new List<Account> 
            { 
                new Account{Id = 1, AccountNumber = "00000001", CardNumber = "444444444441"},
                new Account{Id = 2, AccountNumber = "00000002", CardNumber = "444444444442"},
                //new Account{Id = 3, AccountNumber = "00000003", CardNumber = "444444444443"},
                //new Account{Id = 4, AccountNumber = "00000004", CardNumber = "444444444444"},
                //new Account{Id = 5, AccountNumber = "00000005", CardNumber = "444444444445"}
            };

            // singleton class to show multi-threading use
            IVirtualCashCardRepository repository = VirtualCashCardRepository.Instance;

            var randomAmount = new Random();

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
               {
                   accountList.ForEach((account) =>
                   {
                       IVirtualCashCardService service = new VirtualCashCardService(repository);
                       var depositAmount = randomAmount.Next(1, 99);
                       //Console.WriteLine($"Account - {account.AccountNumber} - Depositing - {depositAmount}");
                       var depositResponse = service.Deposit(account.AccountNumber, depositAmount).Result;
                       Console.WriteLine(depositResponse.ErrorMessage);

                       var withdrawAmount = randomAmount.Next(1, 99);
                       //Console.WriteLine($"Card - {account.CardNumber} - Withdrawing - {withdrawAmount}");
                       var withdrawlResponse = service.Withdraw(account.CardNumber, account.Id.ToString(), randomAmount.Next(1, 99)).Result;
                       Console.WriteLine(withdrawlResponse.ErrorMessage);
                   });
               }));
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("Process finished.");

            Console.ReadLine();
        }
    }
}
