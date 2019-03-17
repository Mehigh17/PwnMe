using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "PwnMe-C#-API-Wrapper");

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

            throw GetApiException(response.StatusCode);
        }

        public async Task<IReadOnlyList<Breach>> GetAccountBreaches(string account, bool truncated = false, string domain = "", bool includeUnverified = false)
        {
            var uriBuilder = new UriBuilder($"{ApiEndpointUri}/breachedaccount/{account}");
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["truncateResponse"] = truncated.ToString();
            parameters["includeUnverified"] = includeUnverified.ToString();
            if (!string.IsNullOrEmpty(domain))
            {
                parameters["domain"] = domain;
            }
            uriBuilder.Query = parameters.ToString();

            var response = await _httpClient.GetAsync(uriBuilder.Uri);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var breaches = JsonConvert.DeserializeObject<Breach[]>(data);
                return breaches;
            }

            throw GetApiException(response.StatusCode);
        }

        public async Task<IReadOnlyList<Breach>> GetBreaches(string domain = "")
        {
            var uriBuilder = new UriBuilder($"{ApiEndpointUri}/breaches");
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(domain))
            {
                parameters["domain"] = domain;
            }
            uriBuilder.Query = parameters.ToString();

            var response = await _httpClient.GetAsync(uriBuilder.Uri);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var breaches = JsonConvert.DeserializeObject<Breach[]>(data);
                return breaches;
            }

            throw GetApiException(response.StatusCode);
        }

        private Exception GetApiException(HttpStatusCode code)
        {
            var message = String.Empty;
            switch (code)
            {
                case HttpStatusCode.BadRequest:
                    message = "The account does not comply with an acceptable format.";
                    break;
                case HttpStatusCode.Forbidden:
                    message = "No user agent has been specified in the request.";
                    break;
                case HttpStatusCode.NotFound:
                    message = "The account could not be found.";
                    break;
                case (HttpStatusCode)429:
                    message = "The rate limit has been exceeded.";
                    break;
                default:
                    message = $"Unhandled result from the API. (StatusCode {code})";
                    break;
            }
            return new HibpApiErrorException(message);
        }

    }
}