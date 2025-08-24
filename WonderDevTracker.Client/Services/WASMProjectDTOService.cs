using System.Net.Http.Json;
using WonderDevTracker.Client.Models.DTOs;
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

        public  async Task<ProjectDTO?> GetProjectByIdAsync(int projectId, UserInfo user)
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
        public Task ArchiveProjectAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<ProjectDTO>> GetAllArchivedProjectsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task RestoreProjectByIdAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        #endregion

        public Task<IEnumerable<AppUserDTO>> GetProjectDevelopersAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<AppUserDTO?> GetProjectManagerAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppUserDTO>> GetProjectMembersByRoleAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppUserDTO>> GetProjectMembersExceptPMAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
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

        public Task<IEnumerable<AppUserDTO>> GetUserProjectsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppUserDTO>> GetUsersNotOnProjectAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        

    }
}
