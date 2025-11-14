using Microsoft.AspNetCore.Components;
using MudBlazor;
using WonderDevTracker.Client.Components.BaseComponents;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Models.Records;

namespace WonderDevTracker.Client.Pages.Tickets
{
    public partial class TicketDetails : AuthenticatedComponentBase
    {
        #region PARAMETERS
        [Parameter]
        public int Id { get; set; }

        [Parameter]
        public int TicketId { get; set; }
        #endregion

        #region COLLECTIONS & LOOKUPS
        private List<TicketAttachmentDTO> _attachments = new();
        private List<BreadcrumbItem> _breadcrumbs = [];
        private IEnumerable<AppUserDTO> _developers = [];
        private IReadOnlyList<AppUserDTO> _projectManagers = [];
        #endregion

        #region STATE
        private TicketDTO? _ticket;
        private bool _loadingComplete = false;
        private bool _userCanEditTicket = false;
        private bool _userCanArchiveTicket = false;
        private bool _userCanAssignTicket = false;
        private bool _processingArchiveRestore = false;
        private bool CanComment => _userCanEditTicket || !_ticket!.Archived;
        #endregion

        #region UI REFERENCES & TOKENS
        private MudMessageBox? _attachmentDialog = default!;

        // Refresh ProjectMembers component when there is a change to project Manager assignment.
        private Guid _membersRefreshToken = Guid.NewGuid();
        #endregion

        #region UI HELPERS & COMPUTED
        private void ResetIndex() => IndexTracker.Reset();
        private string TicketTitle => _ticket?.Title ?? "Ticket";
        private async Task ReloadTicketAsync() => _ticket = await TicketService.GetTicketByIdAsync(Id, UserInfo!);
        #endregion

        #region LIFECYCLE METHODS
        protected override async Task OnParametersSetAsync()
        {
            _ticket = null;
            _loadingComplete = false;
            _userCanEditTicket = false;
            _userCanArchiveTicket = false;
            _userCanAssignTicket = false;
            try
            {
                _ticket = await TicketService.GetTicketByIdAsync(Id, UserInfo!);
                //ensures everytime ticket is loaded or reloaded, attachments are immediately available as a guaranteed List<T>.
                _attachments = _ticket?.Attachments?.ToList() ?? new();

                if (_ticket is not null)
                {
                    _breadcrumbs = [new BreadcrumbItem("Home", href: "/")];
                    if (_ticket.Project is null)
                    {
                        _breadcrumbs.Add(new BreadcrumbItem("Tickets", href: "/tickets/open"));
                        if (_ticket.Archived) _breadcrumbs.Add(new BreadcrumbItem("Archived Tickets", href: "/tickets/archived"));

                    }
                    else
                    {
                        _breadcrumbs.Add(new BreadcrumbItem("Projects", href: "/projects"));
                        _breadcrumbs.Add(new BreadcrumbItem(_ticket.Project.Name ?? "Unknown Project", href: $"/projects/{_ticket.Project.Id}"));
                    }
                    _breadcrumbs.Add(new BreadcrumbItem(_ticket.Title ?? "Ticket Not Found", href: null, disabled: true));

                    // Check if user has permission to alter ticket
                    _userCanEditTicket = await AuthService.CanEditTicketAsync(_ticket!, UserInfo!);
                    _userCanArchiveTicket = await AuthService.IsUserAdminPMAsync(_ticket.ProjectId, UserInfo!);
                    _userCanAssignTicket = await AuthService.IsUserAdminPMAsync(_ticket.ProjectId, UserInfo!);

                    IEnumerable<AppUserDTO> projectMembers = await ProjectService.GetProjectMembersAsync(_ticket.ProjectId, UserInfo!);
                    _developers = projectMembers.Where(m => m.Role == Role.Developer);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Snackbar.Add("Error loading ticket details.", Severity.Error);
            }
            _loadingComplete = true;
            StateHasChanged();

            if (_ticket is null) NavManager.NavigateTo("tickets/open");
        }
        #endregion

        #region ARCHIVE / RESTORE HANDLERS
        private async Task HandleArchiveAsync()
        {
            if (_userCanArchiveTicket == false || _ticket is null) return;

            try
            {
                _processingArchiveRestore = true;
                //archive title
                await TicketService.ArchiveTicketAsync(_ticket.Id, UserInfo!);

                // //refresh ticket details to show archived status
                await ReloadTicketAsync();
                _ticket?.Archived = true;


                await InvokeAsync(() => Snackbar.Add($"The Ticket, '{TicketTitle}' has been archived", Severity.Success));

                // Navigate back to projects list
                NavManager.NavigateTo("/projects");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                Snackbar.Add($"{TicketTitle} is not available to be archived");
            }
            finally
            {
                _processingArchiveRestore = false;
                StateHasChanged();
            }
        }

        private async Task HandleRestoreAsync()
        {
            if (_ticket is null) return;
            try
            {
                _processingArchiveRestore = true;

                //restore project
                await TicketService.RestoreTicketByIdAsync(_ticket.Id, UserInfo!);

                //refresh ticket details to show restored status
                await ReloadTicketAsync();

                if (_ticket != null) _ticket.Archived = false;

                Snackbar.Add($"The ticket '{TicketTitle}' has been restored", Severity.Success);
                // Navigate back to projects list
                NavManager.NavigateTo("/tickets/open");

                if (_ticket == null)
                {
                    Snackbar.Add("The ticket could not be found after restoring.", Severity.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Snackbar.Add($"The ticket is not available to be restored");
            }
            finally
            {
                _processingArchiveRestore = false;
                StateHasChanged();
            }
        }
        #endregion

        #region DEV ASSIGNMENT HANDLERS
        private async Task HandleAssignDevAsync(string? newDevId)
        {
            //Authorization check
            if (_userCanAssignTicket == false || _ticket is null) return;
            // No change? No-op.
            if (_ticket?.DeveloperUserId == newDevId) return;

            try
            {
                // Persist change
                _ticket!.DeveloperUserId = newDevId;
                await TicketService.UpdateTicketAsync(_ticket, UserInfo!);

                // Re-fresh the page
                await OnParametersSetAsync();

                Snackbar.Add("Developer updated.", Severity.Success);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Snackbar.Add("Failed to update assignee.", Severity.Error);

                // revert local change keeping UI strictly consistent on failure
                await ReloadTicketAsync();
            }

            StateHasChanged();
        }

        #endregion

        #region META ENUM HANDLERS (Priority, Status, Type)
        private async Task SavePriorityAsync(TicketPriority? value)
        {
            if (value is null || _ticket is null) return;
            _ticket?.Priority = value.Value;
            await TicketService.UpdateTicketAsync(_ticket!, UserInfo!);

            await ReloadTicketAsync();

            Snackbar.Add("Priority updated.", Severity.Success);
        }

        private async Task SaveStatusAsync(TicketStatus? value)
        {
            if (value is null || _ticket is null) return;
            _ticket?.Status = value.Value;

            await TicketService.UpdateTicketAsync(_ticket!, UserInfo!);

            await ReloadTicketAsync();
            Snackbar.Add("Status updated.", Severity.Success);
        }

        private async Task SaveTypeAsync(TicketType? value)
        {
            if (value is null || _ticket is null) return;
            _ticket?.Type = value.Value;

            await TicketService.UpdateTicketAsync(_ticket!, UserInfo!);
            await ReloadTicketAsync();
            Snackbar.Add("Type updated.", Severity.Success);
        }
        #endregion

        #region COMMENT HANDLERS
        private async Task HandleAddComment(TicketCommentDTO comment)
        {
            if (_ticket is null || _ticket.Archived || !_userCanEditTicket) return;
            comment.TicketId = Id;
            comment.UserId = UserInfo!.UserId;
            comment.Created = DateTimeOffset.UtcNow;

            try
            {
                await TicketService.CreateCommentAsync(comment, UserInfo!);
                // Refresh ticket to show new comment
                await ReloadTicketAsync();

                Snackbar.Add($"Comment added to {_ticket!.Title}.", Severity.Success);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Snackbar.Add($"Failed to add comment to {_ticket!.Title}.", Severity.Error);
            }
        }

        private async Task HandleEditComment(TicketCommentDTO comment)
        {

            if (!CanModifyComment(comment)) return;
            try
            {
                await TicketService.UpdateCommentAsync(comment, UserInfo!);
                // // Refresh ticket to show updated comment
                await ReloadTicketAsync();

                Snackbar.Add("Comment updated.", Severity.Success);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Snackbar.Add("Failed to update comment.", Severity.Error);
            }
        }

        private async Task HandleDeleteComment(int commentId)
        {
            TicketCommentDTO? comment = _ticket?.Comments?.FirstOrDefault(c => c.Id == commentId);

            if (!CanDeleteComment(comment!)) return;

            try
            {

                await TicketService.DeleteCommentAsync(commentId, UserInfo!);
                // Refresh ticket to show updated comments
                await ReloadTicketAsync();
                Snackbar.Add("Comment deleted.", Severity.Success);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Snackbar.Add("Failed to delete comment.", Severity.Error);
            }
        }
        #endregion

        #region ATTACHMENT HANDLERS
        private async Task HandleAttachmentUploadAsync(AttachmentUploadRequest req)
        {
            if (_ticket is null || !_userCanEditTicket || req.File is null) return;

            try
            {
                // Read contents of selected file into a byte array;
                byte[]? fileData = await BrowserFileHelper.GetFileDataAsync(req.File);
                string? contentType = req.File.ContentType;
                var attachment = new TicketAttachmentDTO
                {
                    UserId = UserInfo!.UserId,
                    TicketId = _ticket.Id,
                    FileName = req.File.Name,
                    Description = req.Description,
                    Created = DateTimeOffset.UtcNow,
                    AttachmentUrl = string.Empty // will be set by service
                };
                // Save to db;
                await TicketService.AddTicketAttachmentAsync(attachment!, fileData!, contentType!, UserInfo!);
                // Update local list (defensive against null)
                await ReloadTicketAsync();

                Snackbar.Add("Attachment successfully uploaded.", Severity.Success);
                // // close dialog to reset uploader & inputs
                _attachmentDialog?.Close();


            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
                Snackbar.Add(ex.Message, Severity.Error);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Snackbar.Add("Upload failed. Please try again.", Severity.Error);
            }

        }

        private async Task HandleDeleteAttachmentAsync(int attachmentId)
        {
            if (_ticket is null) return;
            TicketAttachmentDTO? attachment = _ticket.Attachments?.FirstOrDefault(a => a.Id == attachmentId);

            if (attachment is not null && CanDeleteTicketAttachment(attachment!))
            {
                try
                {
                    await TicketService.DeleteTicketAttachmentAsync(attachmentId, UserInfo!);

                    await ReloadTicketAsync();

                    Snackbar.Add("Attachment deleted.", Severity.Success);
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Snackbar.Add("Delete failed.", Severity.Error);
                }
            }
        }
        #endregion

    }

}

