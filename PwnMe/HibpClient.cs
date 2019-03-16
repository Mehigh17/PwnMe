using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PwnMe.Exceptions;
using PwnMe.Models;

namespace PwnMe
{
    public class HibpClient : IHibpClient
    {

        private const string ApiRangeSearchEndpointUri = "https://api.pwnedpasswords.com/range";
        private const string ApiEndpointUri = "https://haveibeenpwned.com/api/v2";

        private const int HashHeaderSize = 5;

        private readonly HttpClient _httpClient;
        private readonly SHA1 _hasher;

        public HibpClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "PwnMe C# API Wrapper");

            _hasher = SHA1.Create();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _hasher.Dispose();
        }

        public async Task<bool> IsPwned(string password)
        {
            return await GetOccurences(password) > 0;
        }

        public async Task<int> GetOccurences(string password)
        {
            var passwordBytes = Encoding.ASCII.GetBytes(password);
            var hashBytes = _hasher.ComputeHash(passwordBytes);
            var hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty).ToUpper();
            var requestUri = $"{ApiRangeSearchEndpointUri}/{hash.Substring(0, HashHeaderSize)}";
            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var separators = new[] { "\r\n" };
                var content = await response.Content.ReadAsStringAsync();
                var hashOccurences = content.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                var hashRemainder = hash.Substring(HashHeaderSize, hash.Length - HashHeaderSize);
                var occurence = hashOccurences.FirstOrDefault(ho => ho.Contains(hashRemainder)).Split(':');

                if (int.TryParse(occurence[1], out var occurenceCount))
                {
                    return occurenceCount;
                }
                return 0;
            }

            return 0;
        }

        public async Task<IReadOnlyList<AccountPaste>> GetAccountPastes(string account)
        {
            var requestUri = $"{ApiEndpointUri}/pasteaccount/{account}";
            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var pastes = JsonConvert.DeserializeObject<AccountPaste[]>(data);
                return pastes;
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new HibpApiErrorException("The account does not comply with an acceptable format (i.e. it's an empty string).");
                case HttpStatusCode.Forbidden:
                    throw new HibpApiErrorException("No user agent has been specified in the request.");
                case HttpStatusCode.NotFound:
                    throw new HibpApiErrorException("The account could not be found and has therefore not been pwned.");
                case (HttpStatusCode)429:
                    throw new HibpApiErrorException("The rate limit has been exceeded.");
                default:
                    throw new Exception($"Unhandled result from the API. (StatusCode {response.StatusCode})");
            }
        }
    }
}