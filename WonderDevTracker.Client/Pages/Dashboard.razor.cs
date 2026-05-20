using Microsoft.AspNetCore.Components;
using MudBlazor;
using WonderDevTracker.Client.Components.BaseComponents;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.DTOs.DashboardDTO;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Models.ViewModels;

namespace WonderDevTracker.Client.Pages
{
    public partial class Dashboard : AuthenticatedComponentBase
    {
        #region PARAMETERS
        [Parameter]
        public List<NotificationDTO> Notifications { get; set; } = new();

        #endregion

        #region STATE
        private DashboardDTO? _data;
        private List<NotificationDTO> _notifications = [];
        private List<TicketDTO> _tickets = [];
        private List<TicketDTO> _recentTickets = [];
        private List<ProjectDTO> _projects = [];
        private List<AppUserDTO> _members = [];
        private List<ProjectDTO> _recentProjects = [];
        private List<BreadcrumbItem> _breadcrumbs = [];
        private bool _loadingComplete = false;
        private bool _hasData = false;
        #endregion

        #region UI HELPERS & COMPUTED PROPERTIES
        private void ResetIndex() => IndexTracker.Reset();
        private string UserName => UserInfo?.FullName ?? "User";
        private string? CompanyName => _data?.CompanyInfo?.CompanyName;

        private Role DashboardRole =>
                IsAdmin
                    ? Role.Admin
                    : IsProjectManager
                        ? Role.ProjectManager
                        : UserIsInRole(Role.Developer)
                            ? Role.Developer
                            : Role.Submitter;

        private string DashboardRoleLabel =>
            IsAdmin
                ? "Administrator Dashboard"
            : IsProjectManager
                ? "Project Manager Dashboard"
                : UserIsInRole(Role.Developer)
                    ? "Developer Dashboard"
                    : "Submitter Dashboard";

        private string DashboardSubtitle =>
                    IsAdmin
                        ? "Here's your company-wide project and ticket overview."
                        : IsProjectManager
                            ? "Here's what's happening with your assigned projects and team activity."
                            : UserIsInRole(Role.Developer)
                                ? "Here's what's happening with your assigned tickets."
                                    : UserIsInRole(Role.Submitter)
                                    ? "Here's an overview of your submitted tickets and updates."
                                    : "Here's your dashboard overview.";

        private AppUserDTO? CurrentUser => _members.FirstOrDefault(m => m.Id == UserInfo?.UserId);
        #endregion

        #region LIFECYCLE
        protected override async Task OnInitializedWithAuthAsync()
        {
            try
            {
                if (UserInfo is null)
                {
                    NavManager.NavigateTo("/Account/Login", forceLoad: true);
                    return;
                }

                _data = await CompanyService.GetDashboardDataAsync(UserInfo);
                Notifications = await NotificationService.GetForCurrentUserAsync(UserInfo.UserId);
                await NotificationState.RefreshUnreadCountAsync(UserInfo);

                _projects = (await ProjectService.GetAllProjectsAsync(UserInfo)).ToList();
                _tickets = (await TicketService.GetOpenTicketsAsync(UserInfo)).ToList();
                _members = (await CompanyService.GetUsersAsync(UserInfo)).ToList();

                _recentProjects = _projects
                    .Where(p => !p.Archived)
                    .OrderBy(p => p.EndDate)
                    .Take(6)
                    .ToList();

                _recentTickets = _tickets
                    .Where(t => !t.Archived)
                    .OrderByDescending(t => t.Created)
                    .Take(6)
                    .ToList();

                _hasData = _data is not null;

                _breadcrumbs = [
                            new BreadcrumbItem("Home", href: "/"),
                        new BreadcrumbItem("Dashboard", href: null, disabled: true),
            ];
                _breadcrumbs.Add(new BreadcrumbItem(UserInfo?.FullName ?? "User Not Found", href: null, disabled: true));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Snackbar.Add("Error loading dashboard details.", Severity.Error);
                _hasData = false;
            }
            finally
            {

                _loadingComplete = true;
            }

        }
        #endregion

        #region EVENT HANDLERS
        private async Task RefreshNotificationsAsync()
        {
            Notifications = await NotificationService.GetForCurrentUserAsync(UserInfo!.UserId);
            await NotificationState.RefreshUnreadCountAsync(UserInfo);
        }
        #endregion

        private DashboardActionPermissionsViewModel ActionsPermissions => new()
        {
            CanManageProjects = UserIsAnyRole(Role.Admin, Role.ProjectManager),
            CanManageMembers = UserIsAnyRole(Role.Admin, Role.ProjectManager),
            CanSubmitTickets = IsAuthenticated,
            CanViewArchivedTickets = UserIsAnyRole(Role.Admin, Role.ProjectManager),
            CanViewMyTickets = UserIsAnyRole(Role.ProjectManager, Role.Developer),
            CanViewInvites = UserIsInRole(Role.Admin),
            CanViewOpenTickets = UserIsInRole(Role.Admin)
        };
    }
}