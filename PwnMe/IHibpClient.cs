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
    }
}