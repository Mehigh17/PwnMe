using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PwnMe.Models;

namespace PwnMe
{
    public interface IHibpClient : IDisposable
    {

        /// <summary>
        /// Gets all the pastes for a given valid account.
        /// </summary>
        /// <param name="account">Valid email address</param>
        /// <returns>A list of account pastes</returns>
        Task<IReadOnlyList<AccountPaste>> GetAccountPastes(string account);

        /// <summary>
        /// Gets the count of how many times a password has occured in the database.
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Number of found occurences for that password</returns>
        Task<int> GetOccurences(string password);

        /// <summary>
        /// Checks if a password has been pwned by getting the number of occurences for it.
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns></returns>
        Task<bool> IsPwned(string password);

        /// <summary>
        /// Gets all the breaches for a given account.
        /// </summary>
        /// <param name="account">Valid email address</param>
        /// <param name="truncated">Returns only the name of the breach.</param>
        /// <param name="domain">Filters the result set to only breaches against the domain specified.</param>
        /// <param name="includeUnverified">Returns breaches that have been flagged as "unverified".</param>
        /// <returns>A list of account breaches</returns>
        Task<IReadOnlyList<Breach>> GetAccountBreaches(string account, bool truncated = false, string domain = "", bool includeUnverified = false);

        /// <summary>
        /// Gets all the breaches in the HIBP database.
        /// </summary>
        /// <param name="domain">Filters the result set to only breaches against the domain specified.</param>
        /// <returns>A list of all the breaches</returns>
        Task<IReadOnlyList<Breach>> GetBreaches(string domain = "");
    }
}