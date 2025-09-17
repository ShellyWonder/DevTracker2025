using System.Net.Http.Json;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class WASMProjectDTOService(HttpClient http) : IProjectDTOService
    {
        #region GET PROJECTS
        public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(UserInfo user)
        {

            try
            {
                List<ProjectDTO>? projects = await http.GetFromJsonAsync<List<ProjectDTO>>("api/Projects") ?? [];
                return projects;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return [];
            }

        }

        public async Task<ProjectDTO?> GetProjectByIdAsync(int projectId, UserInfo user)
        {
            try
            {
                ProjectDTO? project = await http.GetFromJsonAsync<ProjectDTO>($"api/Projects/{projectId}");
                return project;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<IEnumerable<ProjectDTO>> GetAssignedProjectsAsync(UserInfo user)
        {
            try
            {
                List<ProjectDTO>? projects = await http.GetFromJsonAsync<List<ProjectDTO>>($"api/Projects?filter={ProjectsFilter.Assigned}") ?? [];
                return projects;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return [];
            }
        }
        #endregion

        #region CREATE PROJECT
        public async Task<ProjectDTO> CreateProjectAsync(ProjectDTO project, UserInfo user)
        {
            var response = await http.PostAsJsonAsync("api/Projects", project);
            response.EnsureSuccessStatusCode();

            var createdProject = await response.Content.ReadFromJsonAsync<ProjectDTO>() ?? throw new HttpIOException(HttpRequestError.InvalidResponse);

            return createdProject;

        }
        #endregion

        #region UPDATE PROJECT
        public async Task UpdateProjectAsync(ProjectDTO project, UserInfo user)
        {
            var response = await http.PutAsJsonAsync($"api/Projects/{project.Id}", project);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region ARCHIVE/RESTORE PROJECT

        public async Task ArchiveProjectAsync(int projectId, UserInfo user)
        {
            var response = await http.PatchAsync($"api/Projects/{projectId}/archive", null);
            response.EnsureSuccessStatusCode();

        }


        public async Task<IEnumerable<ProjectDTO>> GetAllArchivedProjectsAsync(UserInfo user)
        {
            try
            {
                List<ProjectDTO>? projects = await http.GetFromJsonAsync<List<ProjectDTO>>($"api/Projects?filter={ProjectsFilter.Archived}") ?? [];
                return projects;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return [];
            }
        }

        public async Task RestoreProjectByIdAsync(int projectId, UserInfo user)
        {
            var response = await http.PatchAsync($"api/Projects/{projectId}/restore", null);
            response.EnsureSuccessStatusCode();
        }

        #endregion

        public Task<IEnumerable<AppUserDTO>> GetProjectDevelopersAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task<AppUserDTO?> GetProjectManagerAsync(int projectId, UserInfo user)
        {
            var response = await http.GetFromJsonAsync<AppUserDTO?>($"api/Projects/{projectId}/pm");
            return response;

        }

        public async Task AssignProjectManagerAsync(int projectId, string userId, UserInfo user)
        {
            // No body needed; route carries userId
            var response = await http.PutAsync($"api/Projects/{projectId}/pm/{userId}", content: null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveProjectManagerAsync(int projectId, UserInfo user)
        {
            var response = await http.DeleteAsync($"api/Projects/{projectId}/pm");
            response.EnsureSuccessStatusCode();
        }

        public Task<IEnumerable<AppUserDTO>> GetProjectMembersByRoleAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AppUserDTO>> GetProjectMembersAsync(int projectId, UserInfo user)
        {
            try
            {
                List<AppUserDTO> users = await http.GetFromJsonAsync<List<AppUserDTO>>($"api/Projects/{projectId}/members") ?? [];
                return (users);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }

        public Task<ProjectDTO?> GetProjectsByPriorityAsync(ProjectDTO priority, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppUserDTO>> GetProjectSubmittersAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProjectDTO>> GetUnassignedProjectsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

       

        public Task<IEnumerable<AppUserDTO>> GetUsersNotOnProjectAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task AddProjectMemberAsync(int projectId, string userId, UserInfo user)
        {
            var response = await http.PutAsync($"api/Projects/{projectId}/members/{userId}", null);  //null because no body needed
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveProjectMemberAsync(int projectId, string userId, UserInfo user)
        {
            var response = await http.DeleteAsync($"api/Projects/{projectId}/members/{userId}");
            response.EnsureSuccessStatusCode();
        }

        
        public Task RemoveProjectMemberAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task SetProjectManagerAsync(int projectId, string? userId, UserInfo user)
        {
            var response = await http.PutAsJsonAsync($"api/Projects/{projectId}/pm", new { UserId = userId });
            response.EnsureSuccessStatusCode();
        }

        
    }
    
}
