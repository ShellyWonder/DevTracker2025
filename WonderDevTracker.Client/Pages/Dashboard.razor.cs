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
        private List<NotificationDTO> Notifications { get; set; } = new();

        private List<NotificationDTO> DismissedNotifications { get; set; } = [];

        #region STATE
        private DashboardDTO? _data;
        private List<AppUserDTO> _members = [];
        private List<BreadcrumbItem> _breadcrumbs = [];
        private bool _loadingComplete = false;
        private bool _hasData = false;
        #endregion

        #region UI HELPERS & COMPUTED PROPERTIES
        private void ResetIndex() => IndexTracker.Reset();
        private string UserName => UserInfo?.FullName ?? "User";
        private string? CompanyName => _data?.CompanyInfo?.CompanyName;
        private bool IsEmpty => _data is null || UserInfo is null || CurrentUser is null;

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
                DismissedNotifications = await NotificationService.GetArchivedForCurrentUserAsync(UserInfo.UserId);

                await NotificationState.RefreshUnreadCountAsync(UserInfo);

                _members = (await CompanyService.GetUsersAsync(UserInfo)).ToList();

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
            if (UserInfo is null) return;

            Notifications = await NotificationService.GetForCurrentUserAsync(UserInfo!.UserId);
            DismissedNotifications = await NotificationService.GetArchivedForCurrentUserAsync(UserInfo.UserId);
            await NotificationState.RefreshUnreadCountAsync(UserInfo);
        }

        private Task HandleOpenNotificationAsync(NotificationDTO notification)
        {
            if (notification.TicketId is not null)
            {
                NavManager.NavigateTo($"/tickets/{notification.TicketId}");
                return Task.CompletedTask;
            }

            if (notification.ProjectId is not null)
            {
                NavManager.NavigateTo($"/projects/{notification.ProjectId}");
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
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