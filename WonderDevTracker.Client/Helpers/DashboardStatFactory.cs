using MudBlazor;
using WonderDevTracker.Client.Models.DTOs.DashboardDTO;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Client.Models.ViewModels;

namespace WonderDevTracker.Client.Helpers;

public static class DashboardStatFactory
{
    public static IReadOnlyList<DashboardStatItemViewModel> BuildStats(
        DashboardDTO dashboard,
        Role role)
    {
        return role switch
        {
            Role.Admin => BuildCompanyStats(dashboard.AdminDashboard),
            Role.ProjectManager => BuildPMStats(dashboard.PMDashboard),
            Role.Developer => BuildDevStats(dashboard.DevDashboard),
            Role.Submitter => BuildSubmitterStats(dashboard.SubmitterDashboard),
            _ => BuildDefaultStats(dashboard.AdminDashboard)
        };
    }

    public static IReadOnlyList<DashboardStatItemViewModel> BuildStats(AdminDashboardDTO dashboard)
    {
        return BuildCompanyStats(dashboard);
    }
    public static IReadOnlyList<DashboardStatItemViewModel> BuildStats(
                                                           PMDashboardDTO dashboard)
    {
        return BuildPMStats(dashboard);
    }

    public static IReadOnlyList<DashboardStatItemViewModel> BuildStats(
                                                        DeveloperDashboardDTO dashboard)
    {
        return BuildDevStats(dashboard);
    }
    public static IReadOnlyList<DashboardStatItemViewModel> BuildStats(
                                                        SubmitterDashboardDTO dashboard)
    {
        return BuildSubmitterStats(dashboard);
    }

    private static IReadOnlyList<DashboardStatItemViewModel> BuildCompanyStats(AdminDashboardDTO dashboard)
    {
        var stats = dashboard.CompanyStats;

        return
        [
            new DashboardStatItemViewModel
        {
            Title = "Total Projects",
            Value = stats.TotalProjectCount,
            Icon = Icons.Material.Filled.Folder,
            Color = Color.Primary,
            DetailsHref = "/projects"
        },
        
        new()
        {
            Title = "Open Tickets",
            Value = stats.OpenTicketCount,
            Icon = Icons.Material.Filled.PendingActions,
            Color = Color.Warning,
            DetailsHref = "/tickets/open"
        },
        new()
        {
            Title = "Resolved Tickets",
            Value = stats.ResolvedTicketCount,
            Icon = Icons.Material.Filled.CheckCircle,
            Color = Color.Success,
            DetailsHref = "/tickets/resolved"
        }
    ];
    }

    private static IReadOnlyList<DashboardStatItemViewModel> BuildPMStats(PMDashboardDTO dashboard)
    {
        var stats = dashboard.PMStats;
        return
        [
            new()
        {
            Title = "Managed Projects",
            Value = stats.ManagedProjectCount,
            Icon = Icons.Material.Filled.Folder,
            Color = Color.Primary,
            DetailsHref = "/projects/assigned"
        },
        new()
        {
            Title = "Project Tickets",
            Value = stats.ActiveManagedTicketCount,
            Icon = Icons.Material.Filled.ConfirmationNumber,
            Color = Color.Secondary,
            DetailsHref = "/tickets/assigned"
        },
        new()
        {
            Title = "Open Tickets",
            Value = stats.OpenManagedTicketCount,
            Icon = Icons.Material.Filled.PendingActions,
            Color = Color.Warning,
            DetailsHref = "/tickets/open"
        },
        new()
        {
            Title = "Resolved Tickets",
            Value = stats.ResolvedManagedTicketCount,
            Icon = Icons.Material.Filled.CheckCircle,
            Color = Color.Success,
            DetailsHref = "/tickets/resolved"
        }
    ];
    }

    private static IReadOnlyList<DashboardStatItemViewModel> BuildDevStats(DeveloperDashboardDTO dashboard)
    {
        var stats = dashboard.DevStats;

        return
        [
            new()
        {
            Title = "Assigned Tickets",
            Value = stats.AssignedTicketCount,
            Icon = Icons.Material.Filled.AssignmentInd,
            Color = Color.Primary,
            DetailsHref = "/"
        },
        new()
        {
            Title = "Open Tickets",
            Value = stats.OpenAssignedTicketCount,
            Icon = Icons.Material.Filled.PendingActions,
            Color = Color.Warning,
            DetailsHref = "/"
        },
        new()
        {
            Title = "In Progress",
            Value = stats.InProgressCount,
            Icon = Icons.Material.Filled.Build,
            Color = Color.Info,
            DetailsHref = "/"
        },
        new()
        {
            Title = "Resolved Tickets",
            Value = stats.ResolvedCount,
            Icon = Icons.Material.Filled.CheckCircle,
            Color = Color.Success,
            DetailsHref = "/"
        }
    ];
    }

    private static IReadOnlyList<DashboardStatItemViewModel> BuildSubmitterStats(SubmitterDashboardDTO dashboard)
    {
        var stats = dashboard.SubmitterStats;

        return [
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
    ];
    }

    private static IReadOnlyList<DashboardStatItemViewModel> BuildDefaultStats(AdminDashboardDTO dashboard)
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
    ];
    }
}


