using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Vision.Data;
using Vision.DataTables;
using System.Linq.Dynamic.Core;


namespace Vision.Areas.CRM.Pages.Configurations.Users
{
    public class Index1Model : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        private readonly UserManager<ApplicationUser> _userManager;

        public string url { get; set; }

        private readonly RoleManager<IdentityRole> _roleManager;

        public Index1Model(CRMDBContext context, IWebHostEnvironment hostEnvironment, RoleManager<IdentityRole> roleManager,
                                         UserManager<ApplicationUser> userManager, IToastNotification toastNotification)
        {
            _roleManager = roleManager;

            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            _userManager = userManager;

        }

        [BindProperty]
        public DataTablesRequest DataTablesRequest { get; set; }
        public async Task<JsonResult> OnPostAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("Admin");
            var recordsTotal = users.Count();

            var customersQuery = users.Select(e => new
            {
                Id=   e.Id,
                FullName= e.FullName,
                UserName= e.UserName,
                Email = e.Email
            }).AsQueryable();

            var searchText = DataTablesRequest.Search.Value?.ToUpper();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                customersQuery = customersQuery.Where(s =>
                    s.Email.ToUpper().Contains(searchText) ||
                    s.FullName.ToUpper().Contains(searchText) ||
                    s.UserName.ToUpper().Contains(searchText)

                );
            }

            var recordsFiltered = customersQuery.Count();

            var sortColumnName = DataTablesRequest.Columns.ElementAt(DataTablesRequest.Order.ElementAt(0).Column).Name;
            var sortDirection = DataTablesRequest.Order.ElementAt(0).Dir.ToLower();

            // using System.Linq.Dynamic.Core
            if (sortColumnName != null && sortDirection != null)
            {
                customersQuery = customersQuery.OrderBy($"{sortColumnName} {sortDirection}");

            }


            var skip = DataTablesRequest.Start;
            var take = DataTablesRequest.Length;
            var data = customersQuery
                .Skip(skip)
                .Take(take)
                .ToList();

            return new JsonResult(new
            {
                draw = DataTablesRequest.Draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            });
        
        }

        //public async Task<JsonResult> OnPostAsync()
        //{


        //    var adminRole = "Admin"; // The role name you want to exclude

        //    var usersWithAdminRole = await _userManager.GetUsersInRoleAsync("Admin");

        //    var allUsers = _userManager.Users.ToList();

        //    var usersWithoutAdminRole = allUsers.Except(usersWithAdminRole).Select(e => new
        //    {
        //        Id=  e.Id,
        //        FullName= e.FullName,
        //        PhoneNumber = e.PhoneNumber,
        //        Email= e.Email
        //    }).ToList();
        //    //var users = await _userManager.GetUsersInRoleAsync("User");

        //    var recordsTotal = usersWithoutAdminRole.Count();

        //    var customersQuery = usersWithoutAdminRole.AsQueryable();
        //    var searchText = DataTablesRequest.Search?.Value?.ToUpper();

        //    if (!string.IsNullOrWhiteSpace(searchText))
        //    {
        //        customersQuery = customersQuery.Where(s =>
        //            s.FullName.ToUpper().Contains(searchText) ||
        //            s.PhoneNumber.ToUpper().Contains(searchText) ||
        //        s.Email.ToUpper().Contains(searchText)
        //        );
        //    }

        //    var recordsFiltered = customersQuery.Count();

        //    //var sortColumnName = DataTablesRequest.Columns.ElementAt(DataTablesRequest.Order.ElementAt(0).Column).Name;
        //    //var sortDirection = DataTablesRequest.Order.ElementAt(0).Dir.ToLower();

        //    //// using System.Linq.Dynamic.Core
        //    //customersQuery = customersQuery.OrderBy($"{sortColumnName} {sortDirection}");

        //    //var skip = DataTablesRequest.Start;
        //    //var take = DataTablesRequest.Length;
        //    var data =  usersWithoutAdminRole/*.Where(e => e. == true)*/
        //        //.Skip(skip)
        //        //.Take(take)
        //       ;

        //    return new JsonResult(new
        //    {
        //        draw = DataTablesRequest.Draw,
        //        recordsTotal = recordsTotal,
        //        recordsFiltered = recordsFiltered,
        //        data = data
        //    });
        //}
        public void OnGet()
        {
        }
    }
}
