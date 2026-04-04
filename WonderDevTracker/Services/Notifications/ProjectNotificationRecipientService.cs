using WonderDevTracker.Client;
using WonderDevTracker.Services.Interfaces;

namespace WonderDevTracker.Services.Notifications
{
    public class ProjectNotificationRecipientService(IProjectRepository projectRepository) : IProjectNotificationRecipientService
    {
        public string? GetAffectedMemberRecipient(string? userId, UserInfo actor)
        {
            if (userId == actor.UserId || string.IsNullOrWhiteSpace(userId)) return null;
                
            
            return userId;
        }

        public async Task<string?> GetProjectManagerRecipientAsync(int projectId, UserInfo actor)
        {
            var pmId = await projectRepository.GetProjectManagerIdAsync(projectId, actor);

            if (string.IsNullOrWhiteSpace(pmId) || pmId == actor.UserId) return null;

            return pmId;

        }
        public async Task<List<string>> GetProjectMemberRecipientsAsync(int projectId, UserInfo actor)
        {
           var projectMembers = await projectRepository.GetProjectMembersAsync(projectId, actor);
            if (projectMembers is null || !projectMembers.Any()) return [];
            return [.. projectMembers
               .Where(m => !string.IsNullOrWhiteSpace(m.Id))
               .Where(m => m.Id != actor.UserId)
               .Select(m => m.Id!)
               .Distinct()];
        }

        public async Task<List<string>> GetProjectMemberRecipientsExcludingAsync(int projectId, UserInfo actor, params string?[] excludedUserIds)
        {
            var projectMembers = await projectRepository.GetProjectMembersAsync(projectId, actor);
            if (projectMembers is null || !projectMembers.Any()) return [];

            var excluded = excludedUserIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToHashSet();

            excluded.Add(actor.UserId);

            return [.. projectMembers
                .Where(m => !string.IsNullOrWhiteSpace(m.Id))
                .Where(m => !excluded.Contains(m.Id))
                .Select(m => m.Id!)
                .Distinct()];
        }
    }
   
}
