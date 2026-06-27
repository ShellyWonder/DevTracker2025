using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Globalization;
using WonderDevTracker.Client.Models.Enums;
using WonderDevTracker.Models;

namespace WonderDevTracker.Data
{
    public static class DataUtility
    {
        private static int _company1Id;
        private static int _company2Id;
        private static int _demoCompanyId;

        //project Ids for Company1 and Company2 
        //(Demo Projects are seeded in DemoSeedData.cs)
        private static int _portfolioId;
        private static int _blogId;
        private static int _bugtrackerId;
        private static int _movieId;
        private static int _addressbookId;

        private static Faker faker = new();


        public static string? GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DbConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        public static string BuildConnectionString(string databaseUrl)
        {
            //Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            var database = Environment.GetEnvironmentVariable("RAILWAY_SERVICE_NAME")
                ?? typeof(DataUtility).Assembly.GetName().Name;

            //Provides a simple way to create and manage the contents of connection strings used by the NpgsqlConnection class.
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = database,
                SslMode = SslMode.Prefer,
            };

            return builder.ToString();
        }

        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {
            //Service: An instance of RoleManager
            await using var dbContextSvc = svcProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext();
            //Service: An instance of RoleManager
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //Service: An instance of the UserManager
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<ApplicationUser>>();
            //Service: An IConfiguration instance to get appsettings/secrets/environment variables
            var configurationSvc = svcProvider.GetRequiredService<IConfiguration>();
            //Migration: This is the programmatic equivalent to Update-Database
            await dbContextSvc.Database.MigrateAsync();

            string defaultPassword = configurationSvc["DefaultPassword"] ??
                throw new ApplicationException("Error seeding data - no DefaultPassword configured!");

            await SeedRolesAsync(roleManagerSvc);
            await SeedDefaultCompaniesAsync(dbContextSvc);
            await SeedDefaultUsersAsync(userManagerSvc, defaultPassword);
            await SeedDemoUsersAsync(userManagerSvc, defaultPassword);

            await SeedDefaultProjectsAsync(dbContextSvc);
            await SeedDemoProjectsAsync(dbContextSvc);

            await SeedDefaultTicketsAsync(dbContextSvc, userManagerSvc);
            await SeedDemoTicketsAsync(dbContextSvc, userManagerSvc);

            await dbContextSvc.DisposeAsync();
        }


        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            //Seed Role
            foreach (string roleName in Enum.GetNames<Role>())
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public static async Task SeedDefaultCompaniesAsync(ApplicationDbContext context)
        {
            try
            {
                IList<Company> defaultcompanies = [
                    new() { Name = "Company1", Description="This is default Company 1" },
                    new() { Name = "Company2", Description="This is default Company 2" },
                    new() { Name = "DemoCompany", Description="This is our Demo Company" }
    ];

                var dbCompanies = context.Companies.Select(c => c.Name).ToList();
                await context.Companies.AddRangeAsync(defaultcompanies.Where(c => !dbCompanies.Contains(c.Name)));
                await context.SaveChangesAsync();

                //Get company Ids
                _company1Id = context.Companies.FirstOrDefault(p => p.Name == "Company1")!.Id;
                _company2Id = context.Companies.FirstOrDefault(p => p.Name == "Company2")!.Id;
                _demoCompanyId = context.Companies.FirstOrDefault(p => p.Name == "DemoCompany")!.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Companies.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

        }

        public static async Task SeedDefaultUsersAsync(UserManager<ApplicationUser> userManager, string defaultPassword)
        {
            //Seed Default Admin User
            var defaultUser = new ApplicationUser
            {
                UserName = "btadmin1@devtracker.com",
                Email = "btadmin1@devtracker.com",
                FirstName = faker.Name.FirstName(),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.Admin));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Admin User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Admin User
            defaultUser = new ApplicationUser
            {
                UserName = "btadmin2@devtracker.com",
                Email = "btadmin2@devtracker.com",
                FirstName = faker.Name.FirstName(),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.Admin));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Admin User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default ProjectManager1 User
            defaultUser = new ApplicationUser
            {
                UserName = "ProjectManager1@devtracker.com",
                Email = "ProjectManager1@devtracker.com",
                FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.ProjectManager));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default ProjectManager1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default ProjectManager2 User
            defaultUser = new ApplicationUser
            {
                UserName = "ProjectManager2@devtracker.com",
                Email = "ProjectManager2@devtracker.com",
                FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.ProjectManager));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default ProjectManager2 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer1 User
            defaultUser = new ApplicationUser
            {
                UserName = "Developer1@devtracker.com",
                Email = "Developer1@devtracker.com",
                FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.Developer));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer2 User
            defaultUser = new ApplicationUser
            {
                UserName = "Developer2@devtracker.com",
                Email = "Developer2@devtracker.com",
                FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.Developer));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer2 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer3 User
            defaultUser = new ApplicationUser
            {
                UserName = "Developer3@devtracker.com",
                Email = "Developer3@devtracker.com",
                FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.Developer));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer3 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer4 User
            defaultUser = new ApplicationUser
            {
                UserName = "Developer4@devtracker.com",
                Email = "Developer4@devtracker.com",
                FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.Developer));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer4 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer5 User
            defaultUser = new ApplicationUser
            {
                UserName = "Developer5@devtracker.com",
                Email = "Developer5@devtracker.com",
                FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.Developer));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer5 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Developer6 User
            defaultUser = new ApplicationUser
            {
                UserName = "Developer6@devtracker.com",
                Email = "Developer6@devtracker.com",
                FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.Developer));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer5 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Submitter1 User
            defaultUser = new ApplicationUser
            {
                UserName = "Submitter1@devtracker.com",
                Email = "Submitter1@devtracker.com",
                FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.Submitter));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Submitter1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Submitter2 User
            defaultUser = new ApplicationUser
            {
                UserName = "Submitter2@devtracker.com",
                Email = "Submitter2@devtracker.com",
                FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.Submitter));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Submitter2 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

        }

        public static async Task SeedDemoUsersAsync(UserManager<ApplicationUser> userManager, string defaultPassword)
        {
            #region Seed Demo Admin User
            var defaultUser = new ApplicationUser
            {
                UserName = "demoadmin@devtracker.com",
                Email = "demoadmin@devtracker.com",
                FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _demoCompanyId
            };
            try
            {
                //Test database to see if user already exists
                var user = await userManager.FindByEmailAsync(defaultUser.Email);

                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.Admin));
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.DemoUser));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo Admin User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
            #endregion

            #region Seed Demo ProjectManager User
            defaultUser = new ApplicationUser
            {
                UserName = "demopm@devtracker.com",
                Email = "demopm@devtracker.com",
                FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _demoCompanyId
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.ProjectManager));
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.DemoUser));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo ProjectManager1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
            #endregion

            #region Seed Demo Developer User
            defaultUser = new ApplicationUser
            {
                UserName = "demodev@devtracker.com",
                Email = "demodev@devtracker.com",
                FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _demoCompanyId
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.Developer));
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.DemoUser));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo Developer1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
            #endregion

            #region Seed Demo Submitter User
            defaultUser = new ApplicationUser
            {
                UserName = "demosub@devtracker.com",
                Email = "demosub@devtracker.com",
                FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _demoCompanyId
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.Submitter));
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.DemoUser));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo Submitter User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
            #endregion

            #region Seed Demo New User
            defaultUser = new ApplicationUser
            {
                UserName = "demonew@devtracker.com",
                Email = "demonew@devtracker.com",
                FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female),
                LastName = faker.Name.LastName(),
                EmailConfirmed = true,
                CompanyId = _demoCompanyId
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, defaultPassword);
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.Submitter));
                    await userManager.AddToRoleAsync(defaultUser, nameof(Role.DemoUser));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo New User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
            #endregion
        }

        public static async Task SeedDefaultProjectsAsync(ApplicationDbContext context)
        {
            try
            {
                IList<Project> projects = [
                 new()
                 {
                     CompanyId = _company1Id,
                     Name = "Build a Personal Portfolio",
                     Description="Single page HTML, CSS & JavaScript page. Serves as a landing page for candidates and contains a bio and links to all applications and challenges.",
                     Created = DateTimeOffset.Now - TimeSpan.FromDays(7 * 10),
                     StartDate = DateTimeOffset.Now - TimeSpan.FromDays(7 * 9),
                     EndDate = DateTimeOffset.Now + TimeSpan.FromDays(7 * 8),
                     Priority = ProjectPriority.Low
                 },
                 new()
                 {
                     CompanyId = _company2Id,
                     Name = "Build Personal Blogging Platform",
                     Description= "A custom built web application using .NET with Blazor, a Postgres database, and deployed in a Railway container. The app is designed to create, update and maintain a live blog site.",
                     Created = DateTimeOffset.Now - TimeSpan.FromDays(7 * 5),
                     StartDate = DateTimeOffset.Now - TimeSpan.FromDays(7 * 3),
                     EndDate = DateTimeOffset.Now + TimeSpan.FromDays(7 * 2),
                     Priority = ProjectPriority.Medium
                 },
                 new()
                 {
                     CompanyId = _company1Id,
                     Name = "Build an Issue Tracking Web Application",
                     Description="A custom designed .NET application with Postgres database. The application is a multi-tenant application designed to track projects and their progress using a ticket system. Implemented with identity and user roles, tickets are maintained in projects which are maintained by users in the role of Project Manager. Each project has a team and team members.",
                     Created = DateTimeOffset.Now - TimeSpan.FromDays(7),
                     StartDate = DateTimeOffset.Now - TimeSpan.FromDays(7),
                     EndDate = DateTimeOffset.Now + TimeSpan.FromDays(7 * 4),
                     Priority = ProjectPriority.High
                 },
                 new()
                 {
                     CompanyId = _company2Id,
                     Name = "Build an Address Book Web Application",
                     Description="A custom designed .NET application with Postgres database.  This is an application to serve as a rolodex of contacts for a given user and allows users to send emails to individual contacts or entire categories of contacts.",
                     Created = DateTimeOffset.Now - TimeSpan.FromDays(7 * 5),
                     StartDate = DateTimeOffset.Now - TimeSpan.FromDays(7 * 4),
                     EndDate = DateTimeOffset.Now + TimeSpan.FromDays(7),
                     Priority = ProjectPriority.Low
                 },
                new()
                 {
                     CompanyId = _company1Id,
                     Name = "Build a Movie Information Web Application",
                     Description="A custom designed .NET application with Blazor. An API based application allows users to browse popular movies and retrieve detailed information.",
                     Created = DateTimeOffset.Now - TimeSpan.FromDays(7 * 6),
                     StartDate = DateTimeOffset.Now - TimeSpan.FromDays(7 * 5),
                     EndDate = DateTimeOffset.Now - TimeSpan.FromDays(7 * 4),
                     Priority = ProjectPriority.High
                 }
                 ];

                var dbProjects = context.Projects.Select(c => c.Name).ToList();
                await context.Projects.AddRangeAsync(projects.Where(c => !dbProjects.Contains(c.Name)));
                await context.SaveChangesAsync();


                //Get project Ids
                _portfolioId = context.Projects.FirstOrDefault(p => p.Name == "Build a Personal Portfolio")!.Id;

                _blogId = context.Projects.FirstOrDefault(p => p.Name == "Build Personal Blogging Platform")!.Id;
                _bugtrackerId = context.Projects.FirstOrDefault(p => p.Name == "Build an Issue Tracking Web Application")!.Id;
                _movieId = context.Projects.FirstOrDefault(p => p.Name == "Build a Movie Information Web Application")!.Id;
                _addressbookId = context.Projects.FirstOrDefault(p => p.Name == "Build an Address Book Web Application")!.Id;

            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Projects.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDemoProjectsAsync(ApplicationDbContext context)
        {
            try
            {
                bool demoProjectsAlreadySeeded = await context.Projects
                                .AnyAsync(p => p.CompanyId == _demoCompanyId);

                if (demoProjectsAlreadySeeded) return;

                List<Project> demoProjects = DemoSeedData.CreateDemoProjects(_demoCompanyId);

                    List<string?> existingDemoProjectNames = await context.Projects
                        .Where(p => p.CompanyId == _demoCompanyId)
                        .Select(p => p.Name)
                        .ToListAsync();

                    await context.Projects.AddRangeAsync(
                        demoProjects.Where(p => !existingDemoProjectNames.Contains(p.Name)));

                    await context.SaveChangesAsync();
                }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo Projects.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultTicketsAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            //Get admin Ids
            string company1AdminId = (await userManager.FindByEmailAsync("btadmin1@devtracker.com"))!.Id;
            string company2AdminId = (await userManager.FindByEmailAsync("btadmin2@devtracker.com"))!.Id;

            var portfolio = (await context.Projects.FindAsync(_portfolioId))!;
            var blog = (await context.Projects.FindAsync(_blogId))!;
            var tracker = (await context.Projects.FindAsync(_bugtrackerId))!;
            var movie = (await context.Projects.FindAsync(_movieId))!;
            var addressBook = (await context.Projects.FindAsync(_addressbookId))!;

            var company1Users = await context.Users.Where(u => u.CompanyId == _company1Id && !u.Email!.Contains("demo")).Select(u => u.Id).ToListAsync();
            var company2Users = await context.Users.Where(u => u.CompanyId == _company2Id && !u.Email!.Contains("demo")).Select(u => u.Id).ToListAsync();

            try
            {
                List<Project> projects = [portfolio, blog, tracker, movie, addressBook];

                foreach (var project in projects)
                {
                    if (context.Tickets.Any(t => t.ProjectId == project.Id)) continue;

                    var tickets = faker.Make(
                        count: faker.Random.Number(10, 30),
                        action: () =>
                        {
                            var submitterId = faker.PickRandom(project.CompanyId == _company1Id ? company1Users : company2Users);
                            var created = faker.Date.BetweenOffset(project.StartDate, DateTimeOffset.Now);

                            return new Ticket
                            {
                                SubmitterUserId = submitterId,
                                Title = faker.Company.Bs().Titleize(),
                                Description = faker.Lorem.Paragraph(),
                                Created = created,
                                ProjectId = project.Id,
                                Priority = faker.PickRandom<TicketPriority>(),
                                Status = faker.PickRandom<TicketStatus>(),
                                Type = faker.PickRandom<TicketType>(),
                                History = [
                                    new TicketHistory
                            {
                                Created = created,
                                UserId = submitterId,
                                Description = $"Ticket submitted"
                            }
                                ]
                            };
                        });

                    context.Tickets.AddRange(tickets);
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Tickets.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDemoTicketsAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            try
            {
                ApplicationUser demoSubmitter = await userManager.FindByEmailAsync("demosub@devtracker.com")
                    ?? throw new ApplicationException("Demo submitter user was not found.");

                List<string?> demoProjectNames = DemoSeedData
                    .CreateDemoProjects(_demoCompanyId)
                    .Select(p => p.Name)
                    .ToList();

                List<Project> demoProjects = await context.Projects
                    .Where(p =>
                        p.CompanyId == _demoCompanyId &&
                        demoProjectNames.Contains(p.Name))
                    .ToListAsync();

                foreach (Project project in demoProjects)
                {
                    bool projectAlreadyHasTickets = await context.Tickets
                        .AnyAsync(t => t.ProjectId == project.Id);

                    if (projectAlreadyHasTickets) continue;

                    List<Ticket> tickets = DemoSeedData.CreateDemoTickets(
                        [project],
                        demoSubmitter.Id);

                    context.Tickets.AddRange(tickets);
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Demo Tickets.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        private static readonly TextInfo _textInfo = new CultureInfo("en-US").TextInfo;
        private static string Titleize(this string input) => _textInfo.ToTitleCase(input);
    }
}
