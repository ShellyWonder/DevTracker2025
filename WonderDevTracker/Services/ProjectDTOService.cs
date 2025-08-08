using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services
{
    public class ProjectDTOService(IProjectRepository projectRepository) : IProjectDTOService
    {
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

        public async Task UpdateProjectAsync(ProjectDTO project, UserInfo user)
        {
            Project dbProject = await projectRepository.GetProjectByIdAsync(project.Id, user)
                ?? throw new InvalidOperationException($"Project with ID {project.Id} not found.");
            {

                dbProject.Name = project.Name;
                dbProject.Description = project.Description;
                dbProject.StartDate = project.StartDate ?? dbProject.StartDate;
                dbProject.EndDate = project.EndDate ?? dbProject.EndDate;
                dbProject.Priority = project.Priority;

                await projectRepository.UpdateProjectAsync(dbProject, user);
            }
           
        }
    }
}
