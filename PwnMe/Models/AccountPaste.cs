using System;

namespace PwnMe.Models
{
    public class AccountPaste
    {

        /// <summary>
        /// The paste service the record was retrieved from.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The ID of the paste as it was given at the source service.

        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The title of the paste as observed on the source site.

        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The date and time that the paste was posted.
        /// </summary>
        public DateTime PublicationDate { get; set; }

        /// <summary>
        /// The number of emails that were found when processing the paste.
        /// </summary>
        public int EmailCount { get; set; }
    }
}