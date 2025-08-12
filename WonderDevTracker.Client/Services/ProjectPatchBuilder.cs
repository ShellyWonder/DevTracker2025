using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Client.Services
{
    public sealed class ProjectPatchBuilder : IProjectPatchBuilder
    {
        public ProjectDTO BuildNamePatch(string name) => new()
        {
            Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim()
        };
        
        public ProjectDTO BuildDescriptionPatch(string? description) => new()
        {
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim()
        };
        public ProjectDTO BuildDatesPatch(DateTime? startLocal, DateTime? endLocal) =>new()
        {
            // Setters invoke ProjectDTO’s local→UTC logic
            StartDateTime = startLocal,
            EndDateTime = endLocal
        };
        public ProjectDTO BuildPriorityPatch(ProjectPriority priority) => new()
        { 
            Priority = priority
        };
        
    }
}
