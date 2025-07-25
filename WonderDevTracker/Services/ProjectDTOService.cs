using WonderDevTracker.Client;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Services.Interfaces;
using WonderDevTracker.Models;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services
{
    public class ProjectDTOService(IProjectRepository projectRepository) : IProjectDTOService
    {
        public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(UserInfo user)
        {
            IEnumerable<Project> projects = await projectRepository.GetAllProjectsAsync(user);

            IEnumerable<ProjectDTO> dtos = projects.Select(p =>p.ToDTO());
            return dtos;
        }


       
    }
}
