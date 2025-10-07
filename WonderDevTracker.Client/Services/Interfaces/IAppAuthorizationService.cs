namespace WonderDevTracker.Client.Services.Interfaces
{
    public interface IAppAuthorizationService
    {
        Task<bool> IsUserAdminPM(int projectId, UserInfo user);
        
    }
}
