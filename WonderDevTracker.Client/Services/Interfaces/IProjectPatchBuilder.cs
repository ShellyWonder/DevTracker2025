using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Models.Enums;

namespace WonderDevTracker.Client.Services.Interfaces
{
    public interface IProjectPatchBuilder
    {
        ProjectDTO BuildNamePatch(string name);
        ProjectDTO BuildDescriptionPatch(string? description);
        ProjectDTO BuildDatesPatch(DateTime? startLocal, DateTime? endLocal);
        ProjectDTO BuildPriorityPatch(ProjectPriority priority);
    }
}
