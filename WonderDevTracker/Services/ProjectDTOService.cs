using Microsoft.AspNetCore.Identity;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services
{
    public class ProjectDTOService(IProjectRepository projectRepository,
                                    UserManager<ApplicationUser> userManager) : IProjectDTOService
    {
        #region CREATE METHODS
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
        #endregion

        #region GET METHODS
        public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(UserInfo user)
        {
            IEnumerable<Project> projects = await projectRepository.GetAllProjectsAsync(user);

            IEnumerable<ProjectDTO> dtos = projects.Select(p => p.ToDTO());
            return dtos;
        }

        public async Task<ProjectDTO?> GetProjectByIdAsync(int projectId, UserInfo user)
        {
            Project? project = await projectRepository.GetProjectByIdAsync(projectId, user);

            if (project is null) return null;

            //Get project members and convert to DTOs
            List<AppUserDTO> members = [];

            foreach (var member in project.Members ?? Enumerable.Empty<ApplicationUser>())
            {
                AppUserDTO dto = await member.ToDTOWithRole(userManager);
                members.Add(dto);
            }

            ProjectDTO projectDTO = project.ToDTO();

            projectDTO.Members = members;
            //map projectManagerId so it is explicitly available to client;
            //Blazor can now easily highlight the PM by comparing member.id with projectManagerId.
            projectDTO.ProjectManagerId = await projectRepository.GetProjectManagerIdAsync(projectId, user);
            return projectDTO;
        }

        public Task<IEnumerable<AppUserDTO>> GetProjectDevelopersAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task<AppUserDTO?> GetProjectManagerAsync(int projectId, UserInfo user)
        {
            ApplicationUser? projectManager = await projectRepository.GetProjectManagerAsync(projectId, user)
                ?? throw new InvalidOperationException($"Project with ID {projectId} does not have a project manager assigned.");
            AppUserDTO dto = projectManager.ToDTO();
            dto.Role = Role.ProjectManager;
            return dto;
        }

        public Task<IEnumerable<AppUserDTO>> GetProjectMembersByRoleAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AppUserDTO>> GetProjectMembersAsync(int projectId, UserInfo user)
        {
            IEnumerable<ApplicationUser> members = await projectRepository.GetProjectMembersAsync(projectId, user);

            List<AppUserDTO> dtos = [];

            foreach (var member in members)
            {
                AppUserDTO dto = await member.ToDTOWithRole(userManager);
                dtos.Add(dto);
            }
            return dtos;
        }
        public Task<ProjectDTO?> GetProjectsByPriorityAsync(ProjectDTO priority, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppUserDTO>> GetProjectSubmittersAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProjectDTO>> GetAssignedProjectsAsync(UserInfo user)
        {
           IEnumerable<Project> projects = await projectRepository.GetAssignedProjectsAsync(user);
            IEnumerable<ProjectDTO> dtos = projects.Select(p => p.ToDTO());
            return dtos;
        }

        public Task<IEnumerable<ProjectDTO>> GetUnassignedProjectsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppUserDTO>> GetUsersNotOnProjectAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ARCHIVE METHODS
        public async Task ArchiveProjectAsync(int projectId, UserInfo user)
        {
            await projectRepository.ArchiveProjectAsync(projectId, user);

        }

        public async Task<IEnumerable<ProjectDTO>> GetAllArchivedProjectsAsync(UserInfo user)
        {
            
            IEnumerable<Project> projects = await projectRepository.GetAllArchivedProjectsAsync(user);
            IEnumerable<ProjectDTO> dtos = projects.Select(p => p.ToDTO());
            return dtos;
        }

        public async Task RestoreProjectByIdAsync(int projectId, UserInfo user)
        {
            await projectRepository.RestoreProjectAsync(projectId, user);
        }
        #endregion

        #region UPDATE METHODS
        public async Task UpdateProjectAsync(ProjectDTO project, UserInfo user)
        {
            Project dbProject = await projectRepository.GetProjectByIdAsync(project.Id, user)
                ?? throw new InvalidOperationException($"Project with ID {project.Id} not found.");
            {

                if (!string.IsNullOrWhiteSpace(project.Name)) dbProject.Name = project.Name;
                if (project.Description is not null) dbProject.Description = project.Description;

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
        #endregion

        #region ADD/REMOVE METHODS

        public async Task AddProjectMemberAsync(int projectId, string userId, UserInfo user)
        {
            await projectRepository.AddProjectMemberAsync(projectId, userId, user);
        }

        public async Task RemoveProjectMemberAsync(int projectId, string userId, UserInfo user)
        {
            await projectRepository.RemoveProjectMemberAsync(projectId, userId, user);
        }

        public async Task AssignProjectManagerAsync(int projectId, string userId, UserInfo user)
        {
            await projectRepository.SetProjectManagerAsync(projectId, userId, user);
        }
        public async Task RemoveProjectManagerAsync(int projectId, UserInfo user)
        {
            await projectRepository.SetProjectManagerAsync(projectId, null, user);
        }

        public async Task SetProjectManagerAsync(int projectId, string? managerId, UserInfo user)
        {
            // Use the unified setter to avoid legacy paths
            await projectRepository.SetProjectManagerAsync(projectId, managerId, user);
        }

        #endregion
    }
}
