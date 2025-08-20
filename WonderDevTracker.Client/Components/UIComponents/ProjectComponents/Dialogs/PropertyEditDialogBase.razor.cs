using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using WonderDevTracker.Client.Components.BaseComponents;
using WonderDevTracker.Client.Models.DTOs;
using WonderDevTracker.Client.Services.Interfaces;


/// <summary>
/// Provides a base class for property edit dialogs, handling common functionality such as submission and validation.
/// Workflow Pattern : assemble minimal changes → send → re-fetch → notify/close
/// Benefits:
//  1. Keeps each dialog tiny and focused: build only the fields user want to edit.
//  2. Reduces over-posting risk: not sending unrelated properties.
//  3. The re-fetch guarantees the UI reflects exact server state after save.
//  4. Clean parent interaction via OnSaved and dialog Ok(...) payload.
/// param name="TModel">The model type being edited.</param>
/// </summary>
namespace WonderDevTracker.Client.Components.UIComponents.ProjectComponents.Dialogs

{
    public abstract partial class PropertyEditDialogBase<TModel> : AuthenticatedComponentBase
       where TModel : class, new()
    {
        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        protected IProjectDTOService ProjectService { get; set; } = default!;

        [CascadingParameter]
        private IMudDialogInstance? MudDialog { get; set; }

        [Parameter]
        public ProjectDTO Project { get; set; } = default!;

        protected abstract RenderFragment<TModel>? Body { get; }

        [Parameter]
        public EventCallback<ProjectDTO> OnSaved { get; set; }

        protected abstract Task<ProjectDTO> BuildPatchAsync();

        protected bool _submitting;

        protected TModel? Model { get; set; }

        protected void Close() => MudDialog?.Close(DialogResult.Cancel());

        protected EditContext? _editContext;

        protected override void OnParametersSet()
        {
            if (Model is null)
                throw new InvalidOperationException($"{nameof(Model)} must be set by the derived dialog before rendering.");

            if (_editContext is null || !ReferenceEquals(_editContext.Model, Model))
                _editContext = new EditContext(Model);
        }

        protected async Task SubmitAsync()
        {
            if (_submitting || _editContext is null) return;

            var valid = _editContext.Validate();
            if (!valid) return;

            await HandleSubmitAsync();
        }

        protected async Task HandleSubmitAsync()
        {
            if (_submitting) return;
            _submitting = true;

            try
            {
                if (UserInfo is null) throw new UnauthorizedAccessException("User not authenticated.");

                var patch = await BuildPatchAsync();
                patch.Id = Project.Id;

                await ProjectService.UpdateProjectAsync(patch, UserInfo);

                var updated = await ProjectService.GetProjectByIdAsync(Project.Id, UserInfo)
                              ?? throw new InvalidOperationException("Project not found after update.");

                Snackbar.Add("Update saved successfully.", Severity.Success);
                if (OnSaved.HasDelegate) await OnSaved.InvokeAsync(updated);

                MudDialog?.Close(DialogResult.Ok(updated));
            }
            catch (UnauthorizedAccessException ex)
            {
                Snackbar.Add($"Save failed: {ex.Message}", Severity.Error);
                MudDialog?.Close(DialogResult.Cancel());
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Save failed: {ex.Message}", Severity.Error);
            }
            finally
            {
                _submitting = false;
            }
        }
    }
}