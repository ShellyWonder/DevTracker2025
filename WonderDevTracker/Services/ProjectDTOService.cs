using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services
{
    public class ProjectDTOService(IProjectRepository projectRepository) : IProjectDTOService
    {
        public Task<bool> ArchiveProjectAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task<ProjectDTO> CreateProjectAsync(ProjectDTO project, UserInfo user)
        {
            Project dbProject = new()
            {
                Name = project.Name,
                Description = project.Description,
                Created = DateTimeOffset.UtcNow,
                StartDate = project.StartDate ?? DateTimeOffset.UtcNow,
                EndDate = project.EndDate ?? DateTimeOffset.UtcNow + TimeSpan.FromDays(7),
                Archived = false,
                Priority = project.Priority,
                CompanyId = user.CompanyId,
            };
            
            dbProject = await projectRepository.CreateProjectAsync(dbProject, user)
                         ?? throw new InvalidOperationException("Project creation failed.");
            
            return dbProject.ToDTO();
        }

        public Task<IEnumerable<ProjectDTO>> GetAllArchivedProjectsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(UserInfo user)
        {
            IEnumerable<Project> projects = await projectRepository.GetAllProjectsAsync(user);

            IEnumerable<ProjectDTO> dtos = projects.Select(p =>p.ToDTO());
            return dtos;
        }

        public async Task<ProjectDTO?> GetProjectByIdAsync(int projectId, UserInfo user)
        {
          Project? project = await projectRepository.GetProjectByIdAsync(projectId, user);
            return project?.ToDTO();
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

        public async Task UpdateProjectAsync(ProjectDTO project, UserInfo user)
        {
            Project dbProject = await projectRepository.GetProjectByIdAsync(project.Id, user)
                ?? throw new InvalidOperationException($"Project with ID {project.Id} not found.");
            {

                if(!string.IsNullOrWhiteSpace(project.Name)) dbProject.Name = project.Name;
                if(project.Description is not null) dbProject.Description = project.Description;

                // Dates (treat as a unit if both are supplied; otherwise coalesce individually)
                var newStart = project.StartDate ?? dbProject.StartDate;
                var newEnd = project.EndDate ?? dbProject.EndDate;
                if (newEnd < newStart)
                    throw new InvalidOperationException("End date must be on or after start date.");

                dbProject.StartDate = newStart;
                dbProject.EndDate = newEnd;
                dbProject.Priority = project.Priority;

                await projectRepository.UpdateProjectAsync(dbProject, user);
            }
           
        }
    }
}
