using BillsPaymentSystem.Data;
using BillsPaymentSystem.Models;
using BillsPaymentSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BillsPaymentSystem.App
{
    public class DbInitializer
    {
        public static void Seed(BillsPaymentSystemContext context)
        {
            SeedUsers(context);

            SeedCreditCards(context);

            SeedBankAccount(context);

            SeedPaymentMethod(context);
        }

        private static void SeedPaymentMethod(BillsPaymentSystemContext context)
        {
            var paymentMethods = new List<PaymentMethod>();

            for (int i = 0; i < 8; i++)
            {
                var paymentMethod = new PaymentMethod
                {
                    UserId = new Random().Next(1, 5),
                    Type = (PaymentType)new Random().Next(0, 2)
                };

                if (i % 3 == 0)
                {
                    paymentMethod.CreditCardId = 1;
                    paymentMethod.BankAccountId = 1;
                }
                else if (i % 2 == 0)
                {
                    paymentMethod.CreditCardId = new Random().Next(1, 5);
                }
                else
                {
                    paymentMethod.BankAccountId = new Random().Next(1, 5);
                }

                if (!IsValid(paymentMethod))
                {
                    continue;
                }

                paymentMethods.Add(paymentMethod);
            }

            context.PaymentMethods.AddRange(paymentMethods);
            context.SaveChanges();
        }

        private static void SeedBankAccount(BillsPaymentSystemContext context)
        {
            var bankAccounts = new List<BankAccount>();

            decimal[] balances = { 1000.00m, 2000, 3000, 4000, 5000, 6000, 0, -2 };
            //---Balance: 1000.00

            string[] bankNames = { "First Investment Bank", "DSK", "Hadjiqta gruev", "Zlatan", "JP Morgan", "Krivicki i sie", "kamizdu", "Bash oktai" };
            //-- - Bank: First Investment Bank
            string[] swiftCodes = { "FINVBGSF", "ASDQKD", "adsfq", null, "", "ASDASD", "123", "dasd" };
            //-- - SWIFT: FINVBGSF

            for (int i = 0; i < balances.Length; i++)
            {
                var bankAcc = new BankAccount
                {
                    Balance = balances[i],
                    BankName = bankNames[i],
                    SWIFT = swiftCodes[i]
                };

                if (!IsValid(bankAcc))
                {
                    continue;
                }

                bankAccounts.Add(bankAcc);
            }

            context.BankAccounts.AddRange(bankAccounts);
        }

        private static void SeedCreditCards(BillsPaymentSystemContext context)
        {
            var creditCards = new List<CreditCard>();
            for (int i = 0; i < 8; i++)
            {
                var creditCard = new CreditCard
                {
                    Limit = new Random().Next(-25000, 25000),
                    MoneyOwed = new Random().Next(-25000, 25000),
                    ExpirationDate = DateTime.Now.AddDays(new Random().Next(-200, 200))
                };

                if (!IsValid(creditCard))
                {
                    continue;
                }

                creditCards.Add(creditCard);
            }

            context.CreditCards.AddRange(creditCards);
            context.SaveChanges();

        }

        private static void SeedUsers(BillsPaymentSystemContext context)
        {
            string[] firstNames = { "Gosho", "Pesho", "Marin", "Gergana", "Ivaila", "Vesela", "", null };

            string[] lastNames = { "Goshev", "Peshev", "Drinov", "Atanasova", "Stoqnova", "Staneva", null, "" };

            string[] emails = { "ivayla.stnv@abv.bg", "AvramBakalina@gmail.com", "ChikidjiqOdd@abv.bg", "ERROR", null, "asdasd", "dasdasd", "asdasd.asd@abv.bg" };

            string[] password = { "amfura", "daiduriduridai", "12", null, "5252", "123123", "dsa", "dasd" };

            var users = new List<User>();

            for (int i = 0; i < firstNames.Length; i++)
            {
                var user = new User
                {
                    FirstName = firstNames[i],
                    LastName = lastNames[i],
                    Email = emails[i],
                    Password = password[i]
                };

                if (!IsValid(user))
                {
                    continue;
                }

                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, validationContext, validationResults, true);

            return isValid;
        }
    }
}