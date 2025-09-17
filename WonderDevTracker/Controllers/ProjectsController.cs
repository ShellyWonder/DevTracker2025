using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
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
        /// <remarks>This method returns projects associated with the current user's company
        /// according to the project collection category(Active, Assigned and Archived).
        /// 
        /// Ensure the user is authenticated and authorized to access project data before calling this method. </remarks>
        /// 
        ///
        /// <param name="filter">Optional filter to specify which projects to retrieve: **Active (default)**, Archived, or Assigned.</param>
        
        [HttpGet] //api/projects?fiter=assigned
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects([FromQuery] ProjectsFilter filter = ProjectsFilter.Active)
        {
            //use switch expression to determine which projects to return
            var projects = filter switch
            {
                ProjectsFilter.Archived => await projectService.GetAllArchivedProjectsAsync(UserInfo),
                ProjectsFilter.Assigned => await projectService.GetAssignedProjectsAsync(UserInfo),
                _ => await projectService.GetAllProjectsAsync(UserInfo)
            };

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
        public async Task<ActionResult<ProjectDTO>> GetProjectById([FromRoute] int projectId)
        {

            var project = await projectService.GetProjectByIdAsync(projectId, UserInfo);

            if (project == null) return NotFound();

            return Ok(project);
        }
        #endregion

        #region CREATE PROJECT
        /// <summary>
        /// Create Project
        /// </summary>
        /// <remarks>Creates a new company project in the database. 
        /// Only users with 'Admin' or 'ProjectManager' roles in their respective companies can create(submit) a new project.
        /// If the user is a project manager, the project will be automatically assigned to them as the project lead.
        /// </remarks>
        /// <param name="project">Details of the project to be created.</param>

        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<ActionResult<ProjectDTO>> CreateProject([FromBody] ProjectDTO project)
        {
            ProjectDTO createdProject = await projectService.CreateProjectAsync(project, UserInfo);

            //create project with a 201 status code and a Location header pointing to its route
            return CreatedAtAction(
                actionName: nameof(GetProjectById),
                routeValues: new { projectId = createdProject.Id },
                value: createdProject
                );
        }
        #endregion

        #region UPDATE PROJECT
        /// <summary>
        /// Update Project
        /// </summary>
        /// <remarks>Update details of an existing project.User must be company admin or assigned PM of project</remarks>
        /// <param name="project">Updated project details.</param>
        /// <param name="projectId">The ID of the project to update.</param>

        [HttpPut("{projectId:int}")]
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<IActionResult> UpdateProject([FromRoute] int projectId, [FromBody] ProjectDTO project)
        {

            if (projectId != project.Id) return BadRequest("Project ID mismatch.");

            await projectService.UpdateProjectAsync(project, UserInfo);
            return NoContent();
        }

        #endregion

        #region ARCHIVE PROJECT
        /// <summary>
        /// Archive Project
        /// </summary>
        /// <remarks>Archives the specified project, including all project tickets, marking it as no longer active.
        /// Only users with the roles <see cref="Role.Admin"/> or <see
        /// cref="Role.ProjectManager"/> are authorized to perform this action.</remarks>
        /// <param name="projectId">The id of the project to archive.</param>

        [HttpPatch("{projectId:int}/archive")]
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<IActionResult> ArchiveProject([FromRoute] int projectId)
        {

            await projectService.ArchiveProjectAsync(projectId, UserInfo);
            return NoContent();
        }
        #endregion

        #region RESTORE PROJECT
        /// <summary>
        /// Restore Archived Project
        /// </summary>
        /// <remarks>Restore an archived project to active status. Its tickets revert to their individual status before the project was archived.</remarks>
        /// <param name="projectId">Id of the project to archive</param>
        [HttpPatch("{projectId:int}/restore")]
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<IActionResult> RestoreProject([FromRoute] int projectId)
        {

            await projectService.RestoreProjectByIdAsync(projectId, UserInfo);
            return NoContent();
        }
        #endregion

        #region UPDATE PROJECT MEMBERS
        /// <summary>
        /// Add Project Member
        /// </summary>
        /// <param name="projectId">project id</param>
        /// <param name="userId">Company member(user) id</param>
        /// <remarks>Adds a company member(user) to a specific project. 
        /// User making the change must be an Admin
        /// or the assigned Project Manager to add members to the project.
        /// 
        /// *NOTE:Project Manager must be assigned or removed through SetProjectManager enpoint.
        /// Admins cannot be assigned to projects.*
        /// </remarks>

        [HttpPut("{projectId:int}/members/{userId}")] //api/ projects/1/members/123
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<IActionResult> AddProjectMember([FromRoute] int projectId, [FromRoute] string userId)
        {
            await projectService.AddProjectMemberAsync(projectId, userId, UserInfo);
            return NoContent();
        }
        #endregion

        #region GET PROJECT MEMBERS
        /// <summary>
        /// Get Project Members
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <remarks>Retrieves all members assigned to a specific project.
        /// 
        /// *NOTE: Only Admin or assigned Project Manager may add a member.*
        /// Admins cannot belong to projects.
        /// Project Managers can only be assigned to a project
        /// via the *SetProjectManager* endpoint.
        /// </remarks>
        [HttpGet("{projectId:int}/members")]
        public async Task<ActionResult<IEnumerable<AppUserDTO>>> GetProjectMembers([FromRoute] int projectId)
        {
            var members = await projectService.GetProjectMembersAsync(projectId, UserInfo);

            return Ok(members);
        }

        #endregion

        #region REMOVE PROJECT MEMBER

        /// <summary>
        /// Remove Project Member
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="userId">Id of member(user) being removed from the project.</param>
        /// <remarks>Remove an assigned member for a project.
        /// 
        /// *NOTE: Only Admin or assigned Project Manager may remove a member.*
        /// Admins cannot belong to projects.
        /// Project Managers can only be removed from a project
        /// via the *SetProjectManager* endpoint.
        /// </remarks>
        [HttpDelete("{projectId:int}/members/{userId}")] //api/projects/1/members/123
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<IActionResult> RemoveProjectMember([FromRoute] int projectId, [FromRoute] string userId)
        {
            await projectService.RemoveProjectMemberAsync(projectId, userId, UserInfo);
            return NoContent();
        }
        #endregion

        #region GET PROJECT MANAGER
        /// <summary>
        /// Get Project Manager
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <remarks>Get the manager assigned to the project if one exists</remarks>

        [HttpGet("{projectId:int}/pm")]
        public async Task<ActionResult<AppUserDTO?>> GetProjectManager([FromRoute] int projectId)
        {
            var pm = await projectService.GetProjectManagerAsync(projectId, UserInfo);

            if (pm is null) return NotFound();

            return Ok(pm);
        }
        #endregion

        #region SET PROJECT MANAGER
        /// <summary>
        /// Assign Project Manager
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="userId"> Id of the member in the project manager role </param>
        /// <remarks>Assign a Project Manager to this project.
        /// *NOTE: Only Admin or assigned Project Manager may remove a project manager.*
        /// Admins cannot belong to projects.</remarks>

        //api/Projects/{projectId}/pm/{userId}
        [HttpPut("{projectId:int}/pm/{userId}")]
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<IActionResult> AssignProjectManager([FromRoute] int projectId, [FromRoute] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return BadRequest("UserId is required.");
            await projectService.SetProjectManagerAsync(projectId, userId, UserInfo);
            return NoContent();
        }
        #endregion

        #region CLEAR PROJECT MANAGER
        /// <summary>
        /// Clear Project Manager
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <remarks>Remove a Project Manager from this project.
        /// *NOTE: Only Admin or assigned Project Manager may remove a project manager.*
        /// Admins cannot belong to projects.</remarks>
        [HttpDelete("{projectId:int}/pm")]
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<IActionResult> ClearProjectManager([FromRoute] int projectId)
        {
            await projectService.SetProjectManagerAsync(projectId, null, UserInfo);
            return NoContent();
        }
        #endregion
    }

}
