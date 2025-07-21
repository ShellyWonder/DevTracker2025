using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;
using WonderDevTracker.Services.Repositories;

namespace WonderDevTracker.Services
{
    public class ProjectDTOService(IProjectRepository projectRepository) : IProjectDTOService
    {
        public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(string userId)
        {
            IEnumerable<Project> projects = await projectRepository.GetAllProjectsAsync( userId);

            IEnumerable<ProjectDTO> dtos = projects.Select(p =>p.ToDTO());
            return dtos;
        }


       
    }
}
