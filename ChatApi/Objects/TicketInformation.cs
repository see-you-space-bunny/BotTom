namespace ChatApi
{
    /// <summary>
    /// Return information provided when requesting a ticket from F-list
    /// </summary>
    public class TicketInformation
    {
        /// <summary>
        /// Full list of account's characters
        /// </summary>
        public string[] Characters { get; set; }

        /// <summary>
        /// Account's default character
        /// </summary>
        public string Default_Character { get; set; }

        /// <summary>
        /// Acquired ticket
        /// </summary>
        public string Ticket { get; set; }

        /// <summary>
        /// Error message, if any
        /// </summary>
        public string Error { get; set; }
    }
}
