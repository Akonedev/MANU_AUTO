using MANU_AUTO.Data;
using MANU_AUTO.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Dynamic;
using System.Security.Claims;

namespace MANU_AUTO.Controllers
{
    //[Authorize]
    public class UserController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<IdentityUser> _userManager;
        private readonly LAMANU_AUTOContext _context;


        public UserController(LAMANU_AUTOContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;

        }

        //[Authorize(Roles ="Admin")]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var roles = _roleManager.Roles.ToList();

            dynamic UsersRolesModel = new ExpandoObject();
            UsersRolesModel.users = users;
            UsersRolesModel.roles = roles;
            return View(UsersRolesModel);
        }


        // [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            CreateViewModel createViewModel = new CreateViewModel();

            var allRoles = _roleManager.Roles;
            ViewBag.allRoles = new SelectList(allRoles);
            //return View(new IdentityUser());
            return View(createViewModel);
        }


        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole? role, [Bind("Email, PhoneNumber")] IdentityUser user, IFormCollection? Form)
        {
            var selectedRoleToAdd = Form["RolesToAdd"];
            var password = Form["Password"];

            user.NormalizedEmail = user.Email.ToUpper();
            user.UserName = user.Email;
            user.NormalizedUserName = user.Email.ToUpper();

            await _userManager.CreateAsync(user, password);

            if (selectedRoleToAdd != "" && selectedRoleToAdd.Any())
            {
                IdentityResult roleresult = await _userManager.AddToRoleAsync(user, selectedRoleToAdd);
            }
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(string? id, IdentityUser? user)
        {

            if (id == null || _userManager.GetUserId == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            //var userEmail = User.FindFirstValue(ClaimTypes.email);
            //var user = await _userManager.FindByIdAsync(id);

            //var userId = ((ClaimsIdentity)User.Identity).FindFirst("Id").Value;
            var userDetail = await _userManager.FindByIdAsync(id);
            // var user2 = _userManager.GetUserAsync(userDetail);
            // var users = _userManager.GetUserAsync(userDetail).Include(a => _userManager.GetRolesAsync(a)).ToList();

            var roles = await _userManager.GetRolesAsync(userDetail);
            ViewBag.Roles = roles;


            dynamic UserRolesModel = new ExpandoObject();
            UserRolesModel.userDetail = userDetail;
            UserRolesModel.userRoles = roles;

            //var user = await _userManager.FindByNameAsync(m => m. == id);
            //var user = await _userManager.FindByNameAsync(id);
            if (userDetail == null)
            {
                return NotFound();
            }

            return View(UserRolesModel);
        }

        // [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(string? id, IdentityUser? user)
        {
            if (id == null || user == null)
            {
                return NotFound();
            }
            var userEdit = await _userManager.FindByIdAsync(id);
            var userRoles = await _userManager.GetRolesAsync(userEdit);

            var allRoles = _roleManager.Roles;
            ViewBag.userEdit = userEdit;
            ViewBag.allRoles = new SelectList(allRoles);
            ViewBag.userRoles = userRoles;



            if (userEdit == null)
            {
                return NotFound();
            }
            return View(userEdit);
        }


        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, IdentityRole? roles, [Bind("Id,UserName, Email, PhoneNumber")] IdentityUser user, IFormCollection? Form)
        {

            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var newuser = await _userManager.FindByIdAsync(user.Id);

                newuser.UserName = user.UserName;
                newuser.Email = user.Email;
                newuser.PhoneNumber = user.PhoneNumber;

                var selectedRoleToAdd = Form["RolesToAdd"];
                var selectedRoleToDelete = Form["RoleToRemove"];


                // Console.WriteLine("selectedRole  = " + selectedRole);

                await _userManager.UpdateAsync(newuser);

                if (selectedRoleToAdd != "" && selectedRoleToAdd.Any())
                {
                    IdentityResult roleresult = await _userManager.AddToRoleAsync(newuser, selectedRoleToAdd);
                }

                if (selectedRoleToDelete != "" && selectedRoleToDelete.Any())
                {
                    IdentityResult roleresult = await _userManager.RemoveFromRoleAsync(newuser, selectedRoleToDelete);
                }
                return RedirectToAction(nameof(Index));
            }
            return View();

        }



        // [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(string? id, IdentityUser user)
        {

            if (id == null || user == null)
            {
                return NotFound();
            }

            // var currentUser = await _userManager.GetUserAsync(User);
            var newuser = await _userManager.FindByIdAsync(user.Id);
            var userDelete = await _userManager.FindByIdAsync(user.Id);
            if (id != userDelete.Id)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(userDelete);

            return RedirectToAction(nameof(Index));
        }



        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(IdentityRole role, IdentityUser user)
        {
            await _userManager.DeleteAsync(user);
            return View();
        }


        public async Task<IActionResult> GetMyRoles()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByNameAsync(userEmail);
            var roles = await _userManager.GetRolesAsync(user);
            return View(roles);
        }
    }
}
