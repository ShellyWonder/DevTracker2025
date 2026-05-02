using MudBlazor;
using WonderDevTracker.Client.Models.DTOs.DashboardDTO;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Models.ViewModels;

namespace WonderDevTracker.Client.Helpers;

public static class DashboardStatFactory
{
    public static IReadOnlyList<DashboardStatItem> BuildStats(
        DashboardDTO dashboard,
        Role role)
    {
        return role switch
        {
            Role.Admin => BuildCompanyStats(dashboard),
            Role.ProjectManager => BuildPMStats(dashboard),
            Role.Developer => BuildDevStats(dashboard),
            Role.Submitter => BuildSubmitterStats(dashboard),
            _ => BuildDefaultStats(dashboard)
        };
    }

    private static IReadOnlyList<DashboardStatItem> BuildCompanyStats(DashboardDTO dashboard)
    {
        var stats = dashboard.CompanyStats;

        return
        [
            new DashboardStatItem
        {
            Title = "Total Projects",
            Value = stats.TotalProjectCount,
            Icon = Icons.Material.Filled.Folder,
            Color = Color.Primary
        },
        new()
        {
            Title = "Total Tickets",
            Value = stats.TotalTicketCount,
            Icon = Icons.Material.Filled.ConfirmationNumber,
            Color = Color.Secondary
        },
        new()
        {
            Title = "Open Tickets",
            Value = stats.OpenTicketCount,
            Icon = Icons.Material.Filled.PendingActions,
            Color = Color.Warning
        },
        new()
        {
            Title = "Resolved Tickets",
            Value = stats.ResolvedTicketCount,
            Icon = Icons.Material.Filled.CheckCircle,
            Color = Color.Success
        }
    ];}

    private static IReadOnlyList<DashboardStatItem> BuildPMStats(DashboardDTO dashboard)
    {
        var stats = dashboard.PMStats;
        return
        [
            new()
        {
            Title = "Managed Projects",
            Value = stats.ManagedProjectCount,
            Icon = Icons.Material.Filled.Folder,
            Color = Color.Primary
        },
        new()
        {
            Title = "Project Tickets",
            Value = stats.ManagedProjectTicketCount,
            Icon = Icons.Material.Filled.ConfirmationNumber,
            Color = Color.Secondary
        },
        new()
        {
            Title = "Open Tickets",
            Value = stats.OpenManagedTicketCount,
            Icon = Icons.Material.Filled.PendingActions,
            Color = Color.Warning
        },
        new()
        {
            Title = "Resolved Tickets",
            Value = stats.ResolvedManagedTicketCount,
            Icon = Icons.Material.Filled.CheckCircle,
            Color = Color.Success
        }
    ]; 
    }

    private static IReadOnlyList<DashboardStatItem> BuildDevStats(DashboardDTO dashboard)
    {
        var stats = dashboard.DevStats;

        return
        [
            new()
        {
            Title = "Assigned Tickets",
            Value = stats.AssignedTicketCount,
            Icon = Icons.Material.Filled.AssignmentInd,
            Color = Color.Primary
        },
        new()
        {
            Title = "Open Tickets",
            Value = stats.OpenAssignedTicketCount,
            Icon = Icons.Material.Filled.PendingActions,
            Color = Color.Warning
        },
        new()
        {
            Title = "Resolved Tickets",
            Value = stats.ResolvedAssignedTicketCount,
            Icon = Icons.Material.Filled.CheckCircle,
            Color = Color.Success
        }
    ];
    }

    private static IReadOnlyList<DashboardStatItem> BuildSubmitterStats(DashboardDTO dashboard)
    {
        var stats = dashboard.SubmitterStats;

        return[
            new()
        {
            Title = "Submitted Tickets",
            Value = stats.SubmittedTicketCount,
            Icon = Icons.Material.Filled.Outbox,
            Color = Color.Primary
        },
        new()
        {
            Title = "Open Tickets",
            Value = stats.OpenSubmittedTicketCount,
            Icon = Icons.Material.Filled.PendingActions,
            Color = Color.Warning
        },
        new()
        {
            Title = "Resolved Tickets",
            Value = stats.ResolvedSubmittedTicketCount,
            Icon = Icons.Material.Filled.CheckCircle,
            Color = Color.Success
        }
    ]; }

    private static IReadOnlyList<DashboardStatItem> BuildDefaultStats(DashboardDTO dashboard)
    {
        var stats = dashboard.CompanyStats;
        return
        [
            new()
        {
            Title = "Total Tickets",
            Value = stats.TotalTicketCount,
            Icon = Icons.Material.Filled.QueryStats,
            Color = Color.Primary
        }
    ]; }
}

    
