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
/// param name="TModel">The type of the model being edited.</param>
/// </summary>
namespace WonderDevTracker.Client.Components.UIComponents.ProjectComponents.Modals

{
    public abstract partial class PropertyEditDialogBase<TModel> : AuthenticatedComponentBase
    {
        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        protected IProjectDTOService ProjectService { get; set; } = default!;

        [CascadingParameter]
        private IMudDialogInstance? MudDialog { get; set; }

        [Parameter]
        public RenderFragment<TModel>? Body { get; set; }

        [Parameter]
        public required int ProjectId { get; set; }

        [Parameter]
        public EventCallback<ProjectDTO> OnSaved { get; set; }

        [Parameter]
        public Func<TModel, ProjectDTO>? BuildPatch { get; set; }

        protected bool _submitting;
        protected TModel Model = default!;

        protected void Close() => MudDialog?.Close(DialogResult.Cancel());

        // Each derived dialog returns a minimal DTO with only the changed fields set
        protected virtual Task<ProjectDTO> BuildPatchAsync()
        {
            if (Model is null) throw new InvalidOperationException("Model is null.");
            if (BuildPatch is null) throw new InvalidOperationException("BuildPatch delegate was not provided.");
            return Task.FromResult(BuildPatch(Model));
        }

        /// We own the EditContext so the Save button outside <EditForm> can call _editContext.Validate().
        /// Created in OnParametersSet when Model is present, then passed into <EditForm>.
        protected EditContext? _editContext;


        // Ensure the EditContext matches the current Model (and only create when necessary)
        protected override void OnParametersSet()
        {
            if (Model is null)
                throw new InvalidOperationException("Model must be set by the derived dialog before rendering.");

            if (_editContext is null || !ReferenceEquals(_editContext.Model, Model))
                _editContext = new EditContext(Model);
        }
        protected async Task SubmitAsync()
        {
            if (_submitting || _editContext is null) return;

            // Validate the form since the button is outside the <EditForm>
            var valid = _editContext.Validate();
            if (!valid) return;

            await HandleSubmitAsync();
        }
        protected async Task HandleSubmitAsync()
        {
            //saves resources if the user is clicking the save button multiple times
            if (_submitting) return;
            _submitting = true;

            try
            {
                if (UserInfo is null) throw new UnauthorizedAccessException("User not authenticated.");
                //contains only changed fields(plus Id),
                var patch = await BuildPatchAsync();
                patch.Id = ProjectId;

                //API/Service executes a partial update(PATCH semantics) and leave untouched fields alone.
                await ProjectService.UpdateProjectAsync(patch, UserInfo);

                // Immediately retrieves the full, fresh project:
                var updated = await ProjectService.GetProjectByIdAsync(ProjectId, UserInfo)
                              ?? throw new InvalidOperationException("Project not found after update.");

                Snackbar.Add("Update saved successfully.", Severity.Success);
                if (OnSaved.HasDelegate) await OnSaved.InvokeAsync(updated);

                MudDialog?.Close(DialogResult.Ok(updated));
            }
            catch (UnauthorizedAccessException ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
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

