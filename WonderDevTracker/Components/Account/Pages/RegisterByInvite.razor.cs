namespace WonderDevTracker.Components.Account.Pages
{
        /// <summary>
        /// <para><b>Checklist – adding a new Identity field</b></para>
        /// <list type="number">
        /// <item>Add the property to <c>ApplicationUser</c>.</item>
        /// <item>Update this page:
        ///   <list type="lower-alpha">
        ///   <item>Add it to <c>InputModel</c> (with validation).</item>
        ///   <item>Copy it into <c>RegisterUser()</c>.</item>
        ///   </list>
        /// </item>
        /// <item>Add the field to the form markup.</item>
        /// <item>Update <c>Manage/Index</c> the same way (InputModel, form, save).</item>
        /// <item>Add it to <c>CustomUserClaimsPrincipalFactory</c>.</item>
        /// <item>Client project: update <c>UserInfo</c>.</item>
        /// <item>Server <c>PersistingRevalidatingAuthenticationStateProvider</c>
        ///   (<see cref="OnPersistingAsync"/>).</item>
        /// <item>Client <c>PersistingRevalidatingAuthenticationStateProvider</c>
        ///   <i>or</i>  <c>UserInfoHelpers</c>.</item>
        /// </list>
        /// </summary>
    public partial class RegisterInstruction
    {
    }
}
