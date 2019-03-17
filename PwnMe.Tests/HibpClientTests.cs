using System;
using System.Threading.Tasks;
using FluentAssertions;
using PwnMe.Exceptions;
using PwnMe.Models;
using Xunit;

namespace PwnMe.Tests
{
    public class HibpClientTests
    {

        [Theory]
        [InlineData("test123")]
        [InlineData("password")]
        public async Task IsPwned_BreachedPasswordGiven_ShouldPass(string password)
        {
            var service = new HibpClient();
            var isValid = await service.IsPwned(password);

            isValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("test123", 96105)]
        public async Task GetOccurences_BreachedPasswordGiven_ShouldPass(string password, int minimumExpectedOccurencesCount)
        {
            var service = new HibpClient();
            var isValid = await service.GetOccurences(password);

            isValid.Should().BeGreaterOrEqualTo(minimumExpectedOccurencesCount);
        }

        [Theory]
        [InlineData("test@example.com")]
        public async Task GetAccountPastes_ValidAccountGiven_ShouldPass(string account)
        {
            var client = new HibpClient();
            var pastes = await client.GetAccountPastes(account);

            pastes.Count.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("invalidemail")]
        public void GetAccountPastes_InvalidAccountGiven_ShouldThrowException(string account)
        {
            var client = new HibpClient();
            Action act = () => client.GetAccountPastes(account).Wait();

            act.Should().Throw<HibpApiErrorException>()
                        .WithMessage("The account does not comply with an acceptable format.");
        }

        [Theory]
        [InlineData("test@example.com")]
        public async Task GetAccountBreaches_ValidAccountGiven_ShouldPass(string account)
        {
            var client = new HibpClient();
            var breaches = await client.GetAccountBreaches(account);
            breaches.Count.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("test@example.com", "000webhost.com")]
        public async Task GetAccountBreaches_FilteredForDomain_ValidAccountGiven_ShouldPass(string account, string domain)
        {
            var client = new HibpClient();
            var breaches = await client.GetAccountBreaches(account, domain: domain);
            breaches.Count.Should().Be(1);
        }

        [Theory]
        [InlineData("test@example.com")]
        public async Task GetAccountBreaches_TruncatedResponse_ValidAccountGiven_ShouldPass(string account)
        {
            var client = new HibpClient();
            var breaches = await client.GetAccountBreaches(account, truncated: true);
            breaches.Count.Should().BeGreaterThan(0);
            breaches[0].Name.Should().NotBeNull();
            breaches[0].Title.Should().BeNull();
        }

        [Fact]
        public async Task GetBreaches_ShouldPass()
        {
            var client = new HibpClient();
            var breaches = await client.GetBreaches();
            breaches.Count.Should().BeGreaterOrEqualTo(1);
        }

        [Theory]
        [InlineData("adobe.com")]
        public async Task GetBreaches_DomainGiven_ShouldPass(string domain)
        {
            var client = new HibpClient();
            var breaches = await client.GetBreaches(domain: domain);
            breaches.Count.Should().Be(1);
        }

    }
}