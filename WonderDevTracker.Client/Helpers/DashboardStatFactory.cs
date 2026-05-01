using MudBlazor;
using WonderDevTracker.Client.Models.DTOs;
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
            Role.Admin => BuildAdminStats(dashboard),
            Role.ProjectManager => BuildProjectManagerStats(dashboard),
            Role.Developer => BuildDeveloperStats(dashboard),
            Role.Submitter => BuildSubmitterStats(dashboard),
            _ => BuildDefaultStats(dashboard)
        };
    }

    private static IReadOnlyList<DashboardStatItem> BuildAdminStats(DashboardDTO dashboard) =>
    [
        new()
        {
            Title = "Total Projects",
            Value = dashboard.TotalProjectCount,
            Icon = Icons.Material.Filled.Folder,
            Color = Color.Primary
        },
        new()
        {
            Title = "Total Tickets",
            Value = dashboard.TotalTicketCount,
            Icon = Icons.Material.Filled.ConfirmationNumber,
            Color = Color.Secondary
        },
        new()
        {
            Title = "Open Tickets",
            Value = dashboard.OpenTicketCount,
            Icon = Icons.Material.Filled.PendingActions,
            Color = Color.Warning
        },
        new()
        {
            Title = "Resolved Tickets",
            Value = dashboard.ResolvedTicketCount,
            Icon = Icons.Material.Filled.CheckCircle,
            Color = Color.Success
        }
    ];

    private static IReadOnlyList<DashboardStatItem> BuildProjectManagerStats(DashboardDTO dashboard) =>
    [
        new()
        {
            Title = "Managed Projects",
            Value = dashboard.TotalProjectCount,
            Icon = Icons.Material.Filled.Folder,
            Color = Color.Primary
        },
        new()
        {
            Title = "Project Tickets",
            Value = dashboard.TotalTicketCount,
            Icon = Icons.Material.Filled.ConfirmationNumber,
            Color = Color.Secondary
        },
        new()
        {
            Title = "Open Tickets",
            Value = dashboard.OpenTicketCount,
            Icon = Icons.Material.Filled.PendingActions,
            Color = Color.Warning
        },
        new()
        {
            Title = "Resolved Tickets",
            Value = dashboard.ResolvedTicketCount,
            Icon = Icons.Material.Filled.CheckCircle,
            Color = Color.Success
        }
    ];

    private static IReadOnlyList<DashboardStatItem> BuildDeveloperStats(DashboardDTO dashboard) =>
    [
        new()
        {
            Title = "Assigned Tickets",
            Value = dashboard.TotalTicketCount,
            Icon = Icons.Material.Filled.AssignmentInd,
            Color = Color.Primary
        },
        new()
        {
            Title = "Open Tickets",
            Value = dashboard.OpenTicketCount,
            Icon = Icons.Material.Filled.PendingActions,
            Color = Color.Warning
        },
        new()
        {
            Title = "Resolved Tickets",
            Value = dashboard.ResolvedTicketCount,
            Icon = Icons.Material.Filled.CheckCircle,
            Color = Color.Success
        }
    ];

    private static IReadOnlyList<DashboardStatItem> BuildSubmitterStats(DashboardDTO dashboard) =>
    [
        new()
        {
            Title = "Submitted Tickets",
            Value = dashboard.TotalTicketCount,
            Icon = Icons.Material.Filled.Outbox,
            Color = Color.Primary
        },
        new()
        {
            Title = "Open Tickets",
            Value = dashboard.OpenTicketCount,
            Icon = Icons.Material.Filled.PendingActions,
            Color = Color.Warning
        },
        new()
        {
            Title = "Resolved Tickets",
            Value = dashboard.ResolvedTicketCount,
            Icon = Icons.Material.Filled.CheckCircle,
            Color = Color.Success
        }
    ];

    private static IReadOnlyList<DashboardStatItem> BuildDefaultStats(DashboardDTO dashboard) =>
    [
        new()
        {
            Title = "Total Tickets",
            Value = dashboard.TotalTicketCount,
            Icon = Icons.Material.Filled.QueryStats,
            Color = Color.Primary
        }
    ];
}

    
