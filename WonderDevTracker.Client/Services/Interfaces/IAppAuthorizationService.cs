namespace WonderDevTracker.Client.Services.Interfaces
{
    public interface IAppAuthorizationService
    {
        Task<bool> IsUserAdminPMAsync(int projectId, UserInfo user);
        
    }
}
