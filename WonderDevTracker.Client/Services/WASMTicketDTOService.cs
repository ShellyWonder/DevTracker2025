using System.Net.Http.Json;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class WASMTicketDTOService(HttpClient http) : ITicketDTOService
    {
        public async Task<TicketDTO?> AddTicketAsync(TicketDTO ticket, UserInfo userInfo)
        {
            var response = await http.PostAsJsonAsync("api/Tickets", ticket);
            response.EnsureSuccessStatusCode();
            var createdTicket = await response.Content.ReadFromJsonAsync<TicketDTO>()
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse);
            return createdTicket;
        }

        public  async Task<TicketAttachmentDTO> AddTicketAttachmentAsync(TicketAttachmentDTO attachment, byte[] fileData, string contentType, UserInfo userInfo)
        {
            //encode multipart form data content
            using var formData = new MultipartFormDataContent();
           formData.Headers.ContentDisposition = new("form-data");
            using var fileContent = new ByteArrayContent(fileData);
            fileContent.Headers.ContentType = new(contentType);

            formData.Add(fileContent, "file", attachment.FileName ?? String.Empty);

            //add metadata fields
            formData.Add(new StringContent(attachment.FileName ?? String.Empty), nameof(attachment.FileName));
            formData.Add(new StringContent(attachment.Description ?? String.Empty), nameof(attachment.Description));
            formData.Add(new StringContent(attachment.Created.ToString()), nameof(attachment.Created));
            formData.Add(new StringContent(attachment.UserId ?? String.Empty), nameof(attachment.UserId));
            formData.Add(new StringContent(attachment.TicketId.ToString()), nameof(attachment.TicketId));
            formData.Add(new StringContent("/api/attachments"), nameof(attachment.AttachmentUrl));
            //Note: Not formulated as PostAsJsonAsync because of file upload
            using var response = await http.PostAsync($"/api/tickets/{attachment.TicketId}/attachments", formData);
            response.EnsureSuccessStatusCode();

            //Note: Using ReadFromJsonAsync because response is JSON
            var createdAttachment = await response.Content.ReadFromJsonAsync<TicketAttachmentDTO>()
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse);
            return createdAttachment;

        }

        public async Task ArchiveTicketAsync(int ticketId, UserInfo user)
        {
            var response = await http.PatchAsync($"api/Tickets/{ticketId}/archive", null);//null body
            response.EnsureSuccessStatusCode();
        }

        public async Task<TicketCommentDTO> CreateCommentAsync(TicketCommentDTO comment, UserInfo userInfo)
        {
            var response = await http.PostAsJsonAsync($"/api/Tickets/{comment.TicketId}/comments", comment);
            response.EnsureSuccessStatusCode();
            var createdComment = await response.Content.ReadFromJsonAsync<TicketCommentDTO>()
                                               ?? throw new HttpIOException(HttpRequestError.InvalidResponse);
            return createdComment;
        }

        public async Task DeleteCommentAsync(int commentId, UserInfo user)
        {
            var response = await http.DeleteAsync($"/api/Tickets/comments/{commentId}");
            response.EnsureSuccessStatusCode();
        }

        
        public async Task DeleteTicketAttachmentAsync(int attachmentId, UserInfo user)
        {
            var response = await http.DeleteAsync($"/api/Tickets/attachments/{attachmentId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TicketDTO>> GetArchivedTicketsAsync(UserInfo userInfo)
        {
            try
            {
                List<TicketDTO>? tickets = await http.GetFromJsonAsync<List<TicketDTO>>($"api/Tickets?filter={TicketsFilter.Archived}")
                    ?? [];
                return tickets;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return [];
            }
        }

        public async Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo user)
        {
            try
            {
                List<TicketDTO>? tickets = await http.GetFromJsonAsync<List<TicketDTO>>("api/Tickets")
                    ?? [];
                return tickets;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return [];
            }
        }

        public async Task<IEnumerable<TicketDTO>> GetResolvedTicketsAsync(UserInfo user)
        {
            try
            {
                List<TicketDTO>? tickets = await http.GetFromJsonAsync<List<TicketDTO>>($"api/Tickets?filter={TicketsFilter.Resolved}")
                    ?? [];
                return tickets;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return [];
            }
        }

        public async Task<TicketDTO?> GetTicketByIdAsync(int ticketId, UserInfo userInfo)
        {
            try
            {
                TicketDTO? ticket = await http.GetFromJsonAsync<TicketDTO>($"api/Tickets/{ticketId}");
                return ticket;

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<IEnumerable<TicketDTO>> GetTicketsAssignedToUserAsync(UserInfo userInfo)
        {
            try
            {
                List<TicketDTO>? tickets = await http.GetFromJsonAsync<List<TicketDTO>>($"api/Tickets?filter={TicketsFilter.Assigned}")
                    ?? [];
                return tickets;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return [];
            }
        }

        public async Task RestoreTicketByIdAsync(int ticketId, UserInfo user)
        {
            var response = await http.PatchAsync($"api/Tickets/{ticketId}/restore", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateCommentAsync(TicketCommentDTO comment, UserInfo user)
        {

            var response = await http.PutAsJsonAsync($"api/Tickets/{comment.TicketId}/comments/{comment.Id}", comment);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateTicketAsync(TicketDTO ticket, UserInfo user)
        {
            var response = await http.PutAsJsonAsync($"api/Tickets/{ticket.Id}", ticket);
            response.EnsureSuccessStatusCode();
        }
    }
}
