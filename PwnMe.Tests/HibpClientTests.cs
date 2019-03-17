using System;
using System.Threading.Tasks;
using FluentAssertions;
using PwnMe.Exceptions;
using PwnMe.Models;
using Xunit;

namespace PwnMe.Tests
{
    public class HibpClientTests : IClassFixture<HibpClient>
    {

        private readonly HibpClient _client;
        public HibpClientTests(HibpClient client)
        {
            _client = client;
        }

        [Theory]
        [InlineData("test123")]
        [InlineData("password")]
        public async Task IsPwned_BreachedPasswordGiven_ShouldPass(string password)
        {
            var isValid = await _client.IsPwned(password);
            isValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("test123", 96105)]
        public async Task GetOccurences_BreachedPasswordGiven_ShouldPass(string password, int minimumExpectedOccurencesCount)
        {
            var isValid = await _client.GetOccurences(password);
            isValid.Should().BeGreaterOrEqualTo(minimumExpectedOccurencesCount);
        }

        [Theory]
        [InlineData("test@example.com")]
        public async Task GetAccountPastes_ValidAccountGiven_ShouldPass(string account)
        {
            var pastes = await _client.GetAccountPastes(account);
            pastes.Count.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("invalidemail")]
        public void GetAccountPastes_InvalidAccountGiven_ShouldThrowException(string account)
        {
            Action act = () => _client.GetAccountPastes(account).Wait();
            act.Should().Throw<HibpApiErrorException>()
                        .WithMessage("The account does not comply with an acceptable format.");
        }

        [Theory]
        [InlineData("test@example.com")]
        public async Task GetAccountBreaches_ValidAccountGiven_ShouldPass(string account)
        {
            var breaches = await _client.GetAccountBreaches(account);
            breaches.Count.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("test@example.com", "000webhost.com")]
        public async Task GetAccountBreaches_FilteredForDomain_ValidAccountGiven_ShouldPass(string account, string domain)
        {
            var breaches = await _client.GetAccountBreaches(account, domain: domain);
            breaches.Count.Should().Be(1);
        }

        [Theory]
        [InlineData("test@example.com")]
        public async Task GetAccountBreaches_TruncatedResponse_ValidAccountGiven_ShouldPass(string account)
        {
            var breaches = await _client.GetAccountBreaches(account, truncated: true);
            breaches.Count.Should().BeGreaterThan(0);
            breaches[0].Name.Should().NotBeNull();
            breaches[0].Title.Should().BeNull();
        }

        [Fact]
        public async Task GetBreaches_ShouldPass()
        {
            var breaches = await _client.GetBreaches();
            breaches.Count.Should().BeGreaterOrEqualTo(1);
        }

        [Theory]
        [InlineData("adobe.com")]
        public async Task GetBreaches_DomainGiven_ShouldPass(string domain)
        {
            var breaches = await _client.GetBreaches(domain: domain);
            breaches.Count.Should().Be(1);
        }

    }
}