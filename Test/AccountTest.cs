﻿using System;
using FluentAssertions;
using Xunit;

namespace Recurly.Test
{
    public class AccountTest : BaseTest
    {
        [Fact]
        public void CreateAccount()
        {
            var acct = new Account(Factories.GetUniqueAccountCode());
            acct.Create();
            acct.CreatedAt.Should().NotBe(default(DateTime));
        }

        [Fact]
        public void CreateAccountWithParameters()
        {
            var acct = new Account(Factories.GetUniqueAccountCode())
            {
                Username = "testuser1",
                Email = "testemail@recurly.com",
                FirstName = "Test",
                LastName = "User",
                CompanyName = "Test Company",
                AcceptLanguage = "en"
            };

            acct.Create();

            acct.Username.Should().Be("testuser1");
            acct.Email.Should().Be("testemail@recurly.com");
            acct.FirstName.Should().Be("Test");
            acct.LastName.Should().Be("User");
            acct.CompanyName.Should().Be("Test Company");
            acct.AcceptLanguage.Should().Be("en");
        }

        [Fact]
        public void CreateAccountWithBillingInfo()
        {
            // Arrange
            var accountCode = Factories.GetUniqueAccountCode();
            var account = new Account(accountCode, "BI", "User",
                "4111111111111111", DateTime.Now.Month, DateTime.Now.Year + 1);

            // Act
            account.Create();
            var infoFromService = BillingInfo.Get(accountCode);

            // Assert
            infoFromService.Should().Be(account.BillingInfo);
        }

        [Fact]
        public void LookupAccount()
        {
            var newAcct = new Account(Factories.GetUniqueAccountCode())
            {
                Email = "testemail@recurly.com"
            };
            newAcct.Create();

            var account = Account.Get(newAcct.AccountCode);
            
            account.Should().NotBeNull();
            account.AccountCode.Should().Be(newAcct.AccountCode);
            account.Email.Should().Be(newAcct.Email);
        }

        [Fact]
        public void FindNonExistentAccount()
        {
            Action get = () => Account.Get("totallynotfound!@#$");
            get.ShouldThrow<NotFoundException>();
        }

        [Fact]
        public void UpdateAccount()
        {
            var acct = new Account(Factories.GetUniqueAccountCode());
            acct.Create();

            acct.LastName = "UpdateTest123";
            acct.Update();

            var getAcct = Account.Get(acct.AccountCode);
            acct.LastName.Should().Be(getAcct.LastName);
        }

        [Fact]
        public void CloseAccount()
        {
            var accountCode = Factories.GetUniqueAccountCode();
            var acct = new Account(accountCode);
            acct.Create();

            acct.Close();

            var getAcct = Account.Get(accountCode);
            getAcct.State.Should().Be(Account.AccountState.Closed);
        }

        //[Fact]
        //public void ReopenAccount()
        //{
        //    string s = Factories.GetMockAccountName();
        //    Account acct = new Account(s);
        //    acct.Create();
        //    acct.Close();

        //    acct.Reopen();

        //    Account test = Account.Get(s);
        //    Assert.AreEqual(acct.State, Account.AccountState.Active);
        //    Assert.AreEqual(test.State, Account.AccountState.Active);
        //}
    }
}