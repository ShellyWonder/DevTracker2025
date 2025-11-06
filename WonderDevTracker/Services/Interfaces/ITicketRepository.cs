using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Models;

namespace WonderDevTracker.Services.Interfaces
{
    public interface ITicketRepository
    {
        #region GET METHODS
        /// <summary>
        /// Get Ticket By Id
        /// </summary>
        /// <remarks>Asynchronously retrieves a ticket by its unique identifier for the specified user.</remarks>
        /// <param name="ticketId">The unique identifier of the ticket to retrieve. </param>
        /// <param name="userInfo">Current user's claims</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a TicketDTO representing the
        /// ticket if found and accessible to the user; otherwise, null.</returns>
        public Task<Ticket?> GetTicketByIdAsync(int ticketId, UserInfo userInfo);

        /// <summary>
        /// Get Ticket Comment By Id
        /// </summary>
        /// <remarks>Retrieves a ticket comment by its id</remarks>
        /// <param name="id">The unique identifier of the ticket comment to retrieve. </param>
        /// <param name="userInfo">Current user's claims</param>
        /// 
        public Task<TicketComment?> GetCommentByIdAsync(int id, UserInfo userInfo);

        /// <summary>
        /// Get Open Tickets
        /// </summary>
        /// <remarks>
        /// Get a list of open tickets for the specified user belonging to that user's company.
        /// </remarks>
        /// <param name="userInfo">"The current user's claims"</param>
        public Task<IEnumerable<Ticket>> GetOpenTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Get Resolved Tickets
        /// </summary>
        /// <remarks>
        /// Get a list of resolved tickets for the specified user's company.
        /// </remarks>
        /// <param name="userInfo">"The current user's claims"</param>
        Task<IEnumerable<Ticket>> GetResolvedTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Get Archived Tickets
        /// </summary>
        /// <remarks>
        /// Get a list of Archived tickets for the specified user's company.
        /// </remarks>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<Ticket>> GetArchivedTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Get Tickets Assigned to User
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        /// <remarks>Get all the tickets currently assigned to current user in a specific company.
        /// If the user is a Project Manager this query returns all tickets in assigned project(s).
        /// Admins will see tickets they submitted.
        /// </remarks>
        Task<IEnumerable<Ticket>> GetTicketsAssignedToUserAsync(UserInfo userInfo);
        #endregion

        #region CREATE METHODS
        /// <summary>
        /// Add Ticket
        /// </summary>
        /// <remarks>
        /// Creates a new project ticket in the database asynchronously.
        /// </remarks>
        /// <param name="ticket">New project ticket to be saved in Db</param>
        /// <param name="userInfo">Current user's claims</param>
        /// <returns>New ticket after being saved in Db</returns>
        Task<Ticket?> AddTicketAsync(Ticket ticket, UserInfo userInfo);


        /// <summary>
        /// Create Ticket Comment
        /// </summary>
        /// <param name="comment">The comment data to be added to the ticket. Must contain valid ticket and comment details.</param>
        /// <param name="userInfo">User's current claims. Used to associate the comment with the correct user
        /// identity.</param>
        /// <remarks>Creates a new comment for a ticket asynchronously using the specified comment data and user information.
        /// Returns a task that represents the asynchronous operation. The task result contains a TicketCommentDTO representing
        /// the created comment.</remarks>
        Task<TicketComment> CreateCommentAsync(TicketComment comment, UserInfo userInfo);

        /// <summary>
        /// Add Ticket Attachment
        /// </summary>
        /// <param name="attachment">document being attached to the ticket</param>
        /// <param name="userInfo">Current user's claims</param>
        /// <remarks>
        /// Uploads new ticket attachment for a particular ticket.
        /// User must be:
        /// 1. Assigned to ticket
        /// 2. Assigned project manager
        /// 3. Company admin
        /// </remarks>
        Task<TicketAttachment> AddTicketAttachmentAsync(TicketAttachment attachment, UserInfo userInfo);
        #endregion

        #region ARCHIVE/RESTORE METHODS
        /// <summary>
        /// Archive Ticket
        /// </summary>
        /// <remarks>
        /// Archives a ticket by its ID asynchronously. 
        /// Only user with 'Admin' or "ProjectManager" roles can archive a ticket.
        /// </remarks>
        /// <param name="ticketId">Specific company ticket's id</param>
        /// <param name="user">Current user claims</param>
        /// <returns>True if the ticket was archived, otherwise false</returns>
        public Task ArchiveTicketAsync(int ticketId, UserInfo user);

        /// <summary>
        /// Restores an archived ticket by its ID asynchronously  to active status
        /// </summary>
        /// <remarks>
        /// Only user with 'Admin' or "ProjectManager" roles may restore a ticket.
        /// </remarks>
        /// <param name="ticketId">Specific company ticket's id</param>
        /// <param name="user">Current user claims</param>
        public Task RestoreTicketByIdAsync(int ticketId, UserInfo user);
        #endregion

        #region UPDATE METHODS
        /// <summary>
        /// Update Ticket
        /// </summary>
        /// <remarks>Updates a ticket in the database</remarks>
        /// <param name="ticket">Ticket to be updated</param>
        /// <param name="user">Current user's claims</param>
        public Task UpdateTicketAsync(Ticket ticket, UserInfo user);

        /// <summary>
        /// Update Comment
        /// </summary>
        /// <param name="comment">Ticket comment to be updated </param>
        /// <param name="user">Current user's claims</param>
        /// <remarks>Updates active ticket comments</remarks>
        public Task UpdateCommentAsync(TicketComment comment, UserInfo user);
        #endregion

        #region DELETE METHODS
        /// <summary>
        ///Delete Comment
        /// </summary>
        /// <param name="id">The id of the comment to delete. Must correspond to an existing comment.</param>
        /// <param name="user">Current user's claims.</param>
        /// <remarks> asynchronous delete operation of a specified comment if  user is comment owner or company admin.</remarks>
        Task DeleteCommentAsync(int id, UserInfo user);

        #endregion
    }
}
