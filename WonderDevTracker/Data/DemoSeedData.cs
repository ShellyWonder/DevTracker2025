using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Models;

namespace WonderDevTracker.Data
{
    internal static class DemoSeedData
    {
        public static List<Project> CreateDemoProjects(int demoCompanyId)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            return
            [
                new Project()
                {
                    CompanyId = demoCompanyId,
                    Name = "Sixth Grade American History Quiz Generator",
                    Description = "Custom quiz generator for a teacher's 6th grade American History class. Allows the teacher to create question banks, generate quizzes by unit, and print answer keys.",
                    Created = now - TimeSpan.FromDays(7 * 10),
                    StartDate = now - TimeSpan.FromDays(7 * 9),
                    EndDate = now + TimeSpan.FromDays(7 * 8),
                    Priority = ProjectPriority.Medium
                },
                new Project()
                {
                    CompanyId = demoCompanyId,
                    Name = "Plumbing Invoice Generator",
                    Description = "Invoice generator for a plumbing business that parses worker time cards and parts orders by job, then produces customer-ready invoices.",
                    Created = now - TimeSpan.FromDays(7 * 9),
                    StartDate = now - TimeSpan.FromDays(7 * 8),
                    EndDate = now + TimeSpan.FromDays(7 * 7),
                    Priority = ProjectPriority.High
                },

                new Project()
                {
                    CompanyId = demoCompanyId,
                    Name = "Writer Content Scheduler",
                    Description = "Scheduling and planning application for a writer to organize blog posts, social media content, publishing dates, reminders, and content status.",
                    Created = now - TimeSpan.FromDays(7 * 8),
                    StartDate = now - TimeSpan.FromDays(7 * 7),
                    EndDate = now + TimeSpan.FromDays(7 * 6),
                    Priority = ProjectPriority.Medium
                },

                new Project()
                {
                    CompanyId = demoCompanyId,
                    Name = "Homeschool Compliance Planner",
                    Description = "Home education scheduling app that helps a parent plan lessons, track instructional hours, document attendance, and monitor state compliance requirements.",
                    Created = now - TimeSpan.FromDays(7 * 7),
                    StartDate = now - TimeSpan.FromDays(7 * 6),
                    EndDate = now + TimeSpan.FromDays(7 * 9),
                    Priority = ProjectPriority.High
                },
                new Project()
                {
                    CompanyId = demoCompanyId,
                    Name = "Realtor Client Match Database",
                    Description = "Client and listing database for a realtor to track buyer interviews, showing feedback, home preferences, and match customers with current listings.",
                    Created = now - TimeSpan.FromDays(7 * 6),
                    StartDate = now - TimeSpan.FromDays(7 * 5),
                    EndDate = now + TimeSpan.FromDays(7 * 10),
                    Priority = ProjectPriority.Urgent
                }
            ];
        }

        public static List<Ticket> CreateDemoTickets(IEnumerable<Project> demoProjects, string submitterId)
        {
            List<Ticket> tickets = [];

            foreach (Project project in demoProjects)
            {
                tickets.AddRange(CreateTicketsForProject(project, submitterId));
            }

            return tickets;
        }
        private static List<Ticket> CreateTicketsForProject(Project project, string submitterId)
        {
            return project.Name switch
            {
                "Sixth Grade American History Quiz Generator" =>
                [
                    CreateTicket(project, submitterId, "Create teacher dashboard", "Build the main dashboard where the teacher can view recent quizzes, drafts, and quick actions.", TicketPriority.Medium, TicketStatus.New, TicketType.NewDevelopment, 62),
                    CreateTicket(project, submitterId, "Build quiz generator form", "Create a form that allows the teacher to choose unit, number of questions, question type, and difficulty level.", TicketPriority.High, TicketStatus.InDevelopment, TicketType.NewDevelopment, 58),
                    CreateTicket(project, submitterId, "Add American History question bank", "Create the data structure for storing questions by unit, topic, answer choices, and correct answers.", TicketPriority.High, TicketStatus.New, TicketType.NewDevelopment, 55),
                    CreateTicket(project, submitterId, "Support multiple choice questions", "Allow quizzes to include multiple choice questions with four answer options and one correct answer.", TicketPriority.Medium, TicketStatus.InTesting, TicketType.Enhancement, 50),
                    CreateTicket(project, submitterId, "Generate printable answer keys", "Add answer key generation so the teacher can print or download a teacher-facing answer sheet.", TicketPriority.Medium, TicketStatus.New, TicketType.NewDevelopment, 47),
                    CreateTicket(project, submitterId, "Add quiz export to PDF", "Allow completed quizzes to be exported as PDF files for classroom printing.", TicketPriority.High, TicketStatus.InDevelopment, TicketType.Enhancement, 42),
                    CreateTicket(project, submitterId, "Create unit filter for question selection", "Add filtering so quizzes can be generated from specific American History units.", TicketPriority.Low, TicketStatus.New, TicketType.WorkTask, 39),
                    CreateTicket(project, submitterId, "Save quiz drafts", "Allow the teacher to save incomplete quizzes and return to edit them later.", TicketPriority.Medium, TicketStatus.InTesting, TicketType.NewDevelopment, 34),
                    CreateTicket(project, submitterId, "Add grade-level review labels", "Add labels that help identify whether a question is appropriate for 6th grade reading level.", TicketPriority.Low, TicketStatus.New, TicketType.Enhancement, 29),
                    CreateTicket(project, submitterId, "Fix answer option ordering bug", "Correct the issue where answer options display in a different order between preview and print view.", TicketPriority.High, TicketStatus.New, TicketType.Defect, 25)
                ],
                "Plumbing Invoice Generator" =>
                [
                    CreateTicket(project, submitterId, "Import worker time cards", "Create an import workflow for worker time card records that include employee, date, job number, and hours worked.", TicketPriority.High, TicketStatus.InDevelopment, TicketType.NewDevelopment, 60),
                    CreateTicket(project, submitterId, "Parse parts orders by job", "Build logic to group parts order line items by job number for invoice preparation.", TicketPriority.High, TicketStatus.New, TicketType.NewDevelopment, 57),
                    CreateTicket(project, submitterId, "Create job cost summary", "Display labor, parts, markup, taxes, and total job cost before invoice generation.", TicketPriority.Medium, TicketStatus.New, TicketType.NewDevelopment, 53),
                    CreateTicket(project, submitterId, "Generate invoice PDF", "Create a customer-ready invoice PDF with business information, job details, labor, parts, and total due.", TicketPriority.High, TicketStatus.InTesting, TicketType.NewDevelopment, 49),
                    CreateTicket(project, submitterId, "Add labor rate settings", "Allow the plumber to configure default labor rates and override rates by worker or job type.", TicketPriority.Medium, TicketStatus.New, TicketType.Enhancement, 45),
                    CreateTicket(project, submitterId, "Add parts markup settings", "Add configurable markup percentages for parts and materials used on each job.", TicketPriority.Medium, TicketStatus.InDevelopment, TicketType.Enhancement, 41),
                    CreateTicket(project, submitterId, "Match unmatched time entries", "Create a review screen for time card entries that are missing valid job numbers.", TicketPriority.High, TicketStatus.New, TicketType.WorkTask, 36),
                    CreateTicket(project, submitterId, "Track invoice payment status", "Add invoice status values such as Draft, Sent, Paid, and Overdue.", TicketPriority.Medium, TicketStatus.New, TicketType.NewDevelopment, 32),
                    CreateTicket(project, submitterId, "Export invoice report to CSV", "Allow the business owner to export invoice records for bookkeeping and tax preparation.", TicketPriority.Low, TicketStatus.Resolved, TicketType.Enhancement, 28),
                    CreateTicket(project, submitterId, "Fix duplicate parts line items", "Resolve issue where repeated parts order imports can duplicate parts on the same invoice.", TicketPriority.Urgent, TicketStatus.New, TicketType.Defect, 23)
                ],

                "Writer Content Scheduler" =>
              [
                    CreateTicket(project, submitterId, "Create content calendar view", "Build a calendar view that shows scheduled blog posts, social media posts, and publishing deadlines.", TicketPriority.High, TicketStatus.InDevelopment, TicketType.NewDevelopment, 59),
                    CreateTicket(project, submitterId, "Add blog post draft queue", "Create a draft queue where the writer can track ideas, outlines, drafts, edits, and ready-to-publish posts.", TicketPriority.Medium, TicketStatus.New, TicketType.NewDevelopment, 56),
                    CreateTicket(project, submitterId, "Create social platform selector", "Allow the writer to tag posts for platforms such as Substack, Instagram, Facebook, LinkedIn, and Pinterest.", TicketPriority.Medium, TicketStatus.InTesting, TicketType.Enhancement, 52),
                    CreateTicket(project, submitterId, "Add publishing status workflow", "Add content statuses including Idea, Drafting, Editing, Scheduled, Published, and Archived.", TicketPriority.High, TicketStatus.New, TicketType.NewDevelopment, 48),
                    CreateTicket(project, submitterId, "Add reminder notifications", "Notify the writer when a scheduled post date is approaching or a draft has not been updated.", TicketPriority.Medium, TicketStatus.New, TicketType.NewDevelopment, 44),
                    CreateTicket(project, submitterId, "Create recurring post templates", "Allow reusable templates for recurring content such as weekly updates, newsletters, and promotional posts.", TicketPriority.Low, TicketStatus.New, TicketType.Enhancement, 40),
                    CreateTicket(project, submitterId, "Add asset and link fields", "Add fields for post images, reference links, hashtags, and supporting notes.", TicketPriority.Low, TicketStatus.Resolved, TicketType.WorkTask, 35),
                    CreateTicket(project, submitterId, "Build weekly planning board", "Create a board-style planning screen grouped by week and publishing status.", TicketPriority.Medium, TicketStatus.InDevelopment, TicketType.NewDevelopment, 31),
                    CreateTicket(project, submitterId, "Add publishing history table", "Create a history table that shows what was published, where it was posted, and when it went live.", TicketPriority.Low, TicketStatus.New, TicketType.Enhancement, 27),
                    CreateTicket(project, submitterId, "Fix calendar date rollover issue", "Correct issue where posts scheduled late at night display on the wrong date in the calendar.", TicketPriority.High, TicketStatus.New, TicketType.Defect, 22)
              ],

                "Homeschool Compliance Planner" =>
              [
                    CreateTicket(project, submitterId, "Create state requirement profile", "Store state-specific homeschool requirements including subjects, attendance days, instructional hours, and reporting notes.", TicketPriority.High, TicketStatus.New, TicketType.NewDevelopment, 61),
                    CreateTicket(project, submitterId, "Build weekly lesson planner", "Allow the parent to schedule subjects, lesson topics, resources, and assignments by week.", TicketPriority.High, TicketStatus.InDevelopment, TicketType.NewDevelopment, 57),
                    CreateTicket(project, submitterId, "Track instructional hours", "Add hour tracking by subject and date to help demonstrate compliance with required instruction time.", TicketPriority.High, TicketStatus.New, TicketType.NewDevelopment, 54),
                    CreateTicket(project, submitterId, "Record attendance days", "Create a simple attendance tracker for school days, holidays, sick days, and makeup days.", TicketPriority.Medium, TicketStatus.InTesting, TicketType.NewDevelopment, 49),
                    CreateTicket(project, submitterId, "Create portfolio checklist", "Add a checklist for required portfolio items such as work samples, reading logs, assessments, and progress notes.", TicketPriority.Medium, TicketStatus.New, TicketType.WorkTask, 46),
                    CreateTicket(project, submitterId, "Build compliance summary view", "Show progress toward state requirements with warnings for missing subjects, hours, or attendance days.", TicketPriority.High, TicketStatus.New,TicketType.NewDevelopment, 42),
                    CreateTicket(project, submitterId, "Add yearly goal tracking", "Allow the parent to define academic goals and connect lesson plans to those goals.", TicketPriority.Low, TicketStatus.Resolved, TicketType.Enhancement, 37),
                    CreateTicket(project, submitterId, "Flag requirement gaps", "Create alerts when planned lessons do not satisfy required subject coverage or minimum time expectations.", TicketPriority.High, TicketStatus.InDevelopment, TicketType.Enhancement, 33),
                    CreateTicket(project, submitterId, "Export compliance report", "Generate a printable report for records, reviews, or annual documentation.", TicketPriority.Medium, TicketStatus.New, TicketType.NewDevelopment, 28),
                    CreateTicket(project, submitterId, "Fix duplicate lesson entries", "Resolve issue where copying a weekly plan creates duplicate lessons in the compliance totals.", TicketPriority.Medium, TicketStatus.New, TicketType.Defect, 24)
                ],

                "Realtor Client Match Database" =>
              [
                   CreateTicket(project, submitterId, "Create client interview form", "Build a structured intake form for capturing buyer goals, budget, timeline, must-haves, and deal breakers.", TicketPriority.High, TicketStatus.InDevelopment, TicketType.NewDevelopment, 58),
                   CreateTicket(project, submitterId, "Store buyer preference profile", "Create a client profile that stores preferred locations, home styles, bedroom count, price range, and school preferences.", TicketPriority.High, TicketStatus.New, TicketType.NewDevelopment, 55),
                   CreateTicket(project, submitterId, "Record showing feedback", "Allow the realtor to record client reactions, likes, dislikes, and follow-up notes after each showing.", TicketPriority.High, TicketStatus.InTesting, TicketType.NewDevelopment, 51),
                   CreateTicket(project, submitterId, "Create listing match score", "Build a matching score that compares client preferences against available listing details.", TicketPriority.Urgent, TicketStatus.New, TicketType.NewDevelopment, 47),
                   CreateTicket(project, submitterId, "Add neighborhood preference tags", "Allow clients and listings to be tagged with neighborhood features such as walkable, quiet, schools, commute, and shopping.", TicketPriority.Medium, TicketStatus.New, TicketType.Enhancement, 43),
                   CreateTicket(project, submitterId, "Track financing readiness", "Add fields for pre-approval status, lender contact, budget confidence, and cash-to-close notes.", TicketPriority.Medium, TicketStatus.New, TicketType.WorkTask, 39),
                   CreateTicket(project, submitterId, "Build follow-up reminder queue", "Create reminders for client follow-ups, listing updates, offer deadlines, and showing feedback requests.", TicketPriority.Medium, TicketStatus.InDevelopment, TicketType.NewDevelopment, 34),
                   CreateTicket(project, submitterId, "Create client activity timeline", "Show a timeline of interviews, showings, notes, listing matches, offers, and follow-up actions.", TicketPriority.Low, TicketStatus.New, TicketType.Enhancement, 30),
                   CreateTicket(project, submitterId, "Export client summary packet", "Generate a client-facing summary of preferences, top listings, showing notes, and next steps.", TicketPriority.Low, TicketStatus.Resolved, TicketType.Enhancement, 26),
                   CreateTicket(project, submitterId, "Fix listing match filter bug", "Correct issue where listings outside the buyer's maximum budget are included in match results.", TicketPriority.High, TicketStatus.New, TicketType.Defect, 21)
                ],

                _ => []
            };
        }

        private static Ticket CreateTicket(
            Project project,
            string submitterId,
            string title,
            string description,
            TicketPriority priority,
            TicketStatus status,
            TicketType type,
            int daysAgo)
        {
            DateTimeOffset created = DateTimeOffset.UtcNow - TimeSpan.FromDays(daysAgo);
            return new Ticket
            {
                SubmitterUserId = submitterId,
                Title = title,
                Description = description,
                Created = created,
                ProjectId = project.Id,
                Priority = priority,
                Status = status,
                Type = type,
                History =
                [
                    new TicketHistory
                    {
                        Created = created,
                        UserId = submitterId,
                        Description = "Ticket submitted"
                    }
                ]
            };
        }
    }
}

