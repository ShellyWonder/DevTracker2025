using WonderDevTracker.Client.Models.DTOs;

namespace WonderDevTracker.Client.Helpers.Animation
{
    public class CardAnimationHelper(IEnumerable<ProjectDTO> projects)
    {

        public class ProjectWithIndex
        {
            public ProjectDTO Project { get; set; } = default!;
            public int Index { get; set; }
        }
        private readonly IEnumerable<ProjectDTO>? _projects = projects;

        public List<ProjectWithIndex> GetProjectsWithIndex()
        {
            return [.. _projects!.Select((p, i) => new ProjectWithIndex
            {
                Project = p,
                Index = i
            })];
        }
    }
}
