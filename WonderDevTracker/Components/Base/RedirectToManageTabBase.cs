using Microsoft.AspNetCore.Components;

namespace WonderDevTracker.Components.Base
{
    
    public abstract class RedirectToManageTabBase : ComponentBase
    {
        /// <summary>
        /// Override this in children to indicate which tab to select.
        /// </summary>
        /// 
        [Inject] private NavigationManager Nav { get; set; } = default!;
        protected abstract int TargetTabIndex { get; }

        protected override void OnInitialized()
        {
            // Redirect to Index with a query parameter instructing it which tab to activate
            var uri = $"/Account/Manage?tab={TargetTabIndex}";
            Nav.NavigateTo(uri, forceLoad: false);
        }
    }
}
