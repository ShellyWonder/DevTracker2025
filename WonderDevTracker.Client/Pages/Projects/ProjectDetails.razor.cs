using Microsoft.AspNetCore.Components;
using MudBlazor;
using WonderDevTracker.Client.Components.BaseComponents;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Client.Pages.Projects
{
    public  partial class ProjectDetails : AuthenticatedComponentBase
    {
        #region PARAMETERS
        [Parameter]
        public int ProjectId { get; set; }
        #endregion

        #region COLLECTIONS & LOOKUPS
        private IReadOnlyList<AppUserDTO> projectManagers = Array.Empty<AppUserDTO>();
        #endregion

        #region STATE
        private ProjectDTO? _project;
        private List<BreadcrumbItem> breadcrumbs = [];
        private bool _loadingComplete = false;
        private bool _userCanEditProject = false;
        private bool _processingRequest = false;
        #endregion

        #region UI HELPERS & COMPUTED
        private void ResetIndex() => IndexTracker.Reset();
        #endregion

        #region TOKENS
        // Refresh ProjectMembers component when there is a change to project Manager assignment.
        private Guid _membersRefreshToken = Guid.NewGuid();
        #endregion
        
        #region LIFECYCLE METHODS
        protected override async Task OnInitializedWithAuthAsync()
        {
            try
            {
                _project = await ProjectService.GetProjectByIdAsync(ProjectId, UserInfo!);

                if (_project is not null)
                {
                    projectManagers = await CompanyService.GetUsersInRoleAsync(Role.ProjectManager, UserInfo!);
                    _userCanEditProject = await AuthService.IsUserAdminPMAsync(ProjectId, UserInfo!);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Snackbar.Add("Error loading project details.", Severity.Error);
            }
            _loadingComplete = true;


            breadcrumbs = [
                new BreadcrumbItem("Home", href: "/"),
                new BreadcrumbItem("Projects", href: "/projects"),
            ];
            if (_project?.Archived == true) breadcrumbs.Add(new BreadcrumbItem("Archived Projects", href: "/projects/archived"));

            breadcrumbs.Add(new BreadcrumbItem(_project?.Name ?? "Project Not Found", href: null, disabled: true));

        }
        #endregion

        #region ARCHIVE / RESTORE HANDLERS
        private async Task HandleArchiveAsync()
        {
            if (!_userCanEditProject || _project is null || _processingRequest) return;

            var projectName = _project.Name ?? "Project"; // Cache the name

            try
            {
                _processingRequest = true;
                await InvokeAsync(StateHasChanged);

                //archive project
                await ProjectService.ArchiveProjectAsync(_project.Id, UserInfo!);

                // //refresh project details to show archived status
                _project = await ProjectService.GetProjectByIdAsync(ProjectId, UserInfo!);

                if (_project != null) _project.Archived = true;


                await InvokeAsync(() => Snackbar.Add($"The Project, '{projectName}' has been archived", Severity.Success));

                // Navigate back to projects list
                NavManager.NavigateTo("/projects");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                Snackbar.Add($"{projectName} is not available to be archived");
            }
            finally
            {
                _processingRequest = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task HandleRestoreAsync()
        {
            if (!_userCanEditProject || _project is null || _processingRequest) return;
            try
            {
                _processingRequest = true;
                await InvokeAsync(StateHasChanged);

                string projectName = _project.Name ?? "Project";

                //restore project
                await ProjectService.RestoreProjectByIdAsync(_project.Id, UserInfo!);

                //refresh project details to show restored status
                _project = await ProjectService.GetProjectByIdAsync(ProjectId, UserInfo!);

                if (_project != null) _project.Archived = false;

                Snackbar.Add($"The Project '{projectName}' has been restored", Severity.Success);
                // Navigate back to projects list
                NavManager.NavigateTo("/projects");

                if (_project == null)
                {
                    Snackbar.Add("The project could not be found after restoring.", Severity.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Snackbar.Add($"The Project is not available to be restored");
            }
            finally
            {
                _processingRequest = false;
                await InvokeAsync(StateHasChanged);
            }
        }
        #endregion

        #region HANDLE MEMBERS
        private async Task HandleSavePMAsync(string? newPMId)
        {
            if (_project is null) return;

            try
            {
                if (string.IsNullOrWhiteSpace(newPMId))
                {
                    await ProjectService.RemoveProjectManagerAsync(ProjectId, UserInfo!);
                }
                else
                {
                    await ProjectService.AssignProjectManagerAsync(ProjectId, newPMId, UserInfo!);
                }

                // Refetch authoritative DTO (ProjectManagerId already populated)
                _project = await ProjectService.GetProjectByIdAsync(ProjectId, UserInfo!);

                _membersRefreshToken = Guid.NewGuid();

                Snackbar.Add("Project Manager updated.", Severity.Success);
                StateHasChanged();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);

                await InvokeAsync(() => Snackbar.Add("Unable to update Project Manager.", Severity.Error));
            }


        }
        #endregion

    }
}
