using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController(IProjectDTOService projectService) : ControllerBase
    {
            // Check if the user is authenticated
            UserInfo UserInfo => UserInfoHelper.GetUserInfo(User)!;

        #region GET ALL ACTIVE PROJECTS
        /// <summary>
        /// Get Projects
        /// </summary>
        /// <remarks>This method returns all active projects associated with the current user's company. Ensure the user is
        /// authenticated and authorized to access project data before calling this method. Returns a 404
        /// status code if no projects are found.</remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects()
        {
            
            var projects = await projectService.GetAllProjectsAsync(UserInfo);
            
            if(projects == null || !projects.Any()) return NotFound();
           
            return Ok(projects);
        }
        #endregion

        #region GET PROJECT BY ID
        /// <summary>
        /// Get Project by Id
        /// </summary>
        /// <param name="projectId">The ID of the project to retrieve.</param>
        /// <remarks>Returns detailed information about a specific project. Ensure the user is
        /// authenticated and authorized to access project data before calling this method. Returns a 404
        /// status code if no projects are found.</remarks>
        
        //Route parameter must match blazor parameter 
        [HttpGet("{projectId:int}")]
        public async Task<ActionResult<ProjectDTO>> GetProjectById([FromRoute]int projectId)
        {
           
            var project = await projectService.GetProjectByIdAsync(projectId, UserInfo);
            
            if (project == null) return NotFound();
            
            return Ok(project);
        }
        #endregion
    }
}
