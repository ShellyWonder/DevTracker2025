using System.Net.Http.Json;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public class WASMProjectDTOService(HttpClient http) : IProjectDTOService
    {
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

        public Task ArchiveProjectAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectDTO> CreateProjectAsync(ProjectDTO project, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProjectDTO>> GetAllArchivedProjectsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        

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

        public Task RestoreProjectByIdAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProjectAsync(ProjectDTO project, UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}
