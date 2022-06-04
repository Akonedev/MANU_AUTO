using MANU_AUTO.Data;
using MANU_AUTO.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Dynamic;
using System.Security.Claims;

namespace MANU_AUTO.Controllers;

[Authorize]
public class UserController : Controller
{
    private RoleManager<IdentityRole> _roleManager;
    private UserManager<IdentityUser> _userManager;
    private readonly LAMANU_AUTOContext _context;
    private readonly IUserEmailStore<IdentityUser> _emailStore;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<CreateViewModel> _logger;


    public UserController(
        LAMANU_AUTOContext context, 
        RoleManager<IdentityRole> roleManager, 
        UserManager<IdentityUser> userManager,
         IUserStore<IdentityUser> userStore,
         IEmailSender emailSender,
         ILogger<CreateViewModel> logger
         )
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _emailSender = emailSender;
        _logger = logger;

    }

    [Authorize(Roles ="Admin")]
    public IActionResult Index()
    {
        var users = _userManager.Users.ToList();
        var roles = _roleManager.Roles.ToList();

        dynamic UsersRolesModel = new ExpandoObject();
        UsersRolesModel.users = users;
        UsersRolesModel.roles = roles;
        return View(UsersRolesModel);
    }


    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Create()
    {
        CreateViewModel createViewModel = new CreateViewModel();

        var allRoles = _roleManager.Roles;
        ViewBag.allRoles = new SelectList(allRoles);
        //return View(new IdentityUser());
        return View(createViewModel);
    }


    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(IdentityRole? role, IdentityUser newUser, IFormCollection? Form)
    {
        if (ModelState.IsValid)
        {

            string? selectedRoleToAdd = Form?["RolesToAdd"];
            string? password = Form?["Password"];

            var user = CreateUser();

            user.Email = newUser.Email;
            user.NormalizedEmail = newUser.Email.ToUpper();
            user.UserName = newUser.Email;
            user.NormalizedUserName = newUser.Email.ToUpper();

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                //When registering Add employee role if email contains "lamanu" otherwise add Default Role
                var defaultRole = _roleManager.FindByNameAsync("User").Result;
                var employeeRole = _roleManager.FindByNameAsync("Employee").Result;
                string emailValueToCheck = "lamanu";

                if (newUser.Email.Contains(emailValueToCheck) && employeeRole != null)
                {
                    IdentityResult roleresult = await _userManager.AddToRoleAsync(user, employeeRole.Name);
                }
                else if (defaultRole != null)
                {
                    IdentityResult roleresult = await _userManager.AddToRoleAsync(user, defaultRole.Name);
                }
                _logger.LogInformation("User created a new account with password.");

                //Add Role Manually
                if (selectedRoleToAdd! != "" && selectedRoleToAdd!.Any())
                {
                    IdentityResult roleresult = await _userManager.AddToRoleAsync(user, selectedRoleToAdd);
                }
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
       
        return View();
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

    [Authorize(Roles = "Admin")]
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


    [Authorize(Roles = "Admin")]
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

            string? selectedRoleToAdd = Form?["RolesToAdd"];
            string? selectedRoleToDelete = Form?["RoleToRemove"];

            await _userManager.UpdateAsync(newuser);

            if (selectedRoleToAdd! != "" && selectedRoleToAdd!.Any())
            {
                IdentityResult roleresult = await _userManager.AddToRoleAsync(newuser, selectedRoleToAdd);
            }

            if (selectedRoleToDelete! != "" && selectedRoleToDelete!.Any())
            {
                IdentityResult roleresult = await _userManager.RemoveFromRoleAsync(newuser, selectedRoleToDelete);
            }
            return RedirectToAction(nameof(Index));
        }
        return View();

    }



    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Delete(string? id, IdentityUser user)
    {

        if (id == null || user == null)
        {
            return NotFound();
        }

        var userToDelete = await _userManager.FindByIdAsync(user.Id);
        if (id != userToDelete.Id)
        {
            return NotFound();
        }

        return View(userToDelete);
    }



    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(string? id, IdentityUser user)
    {
        if (id == null || user == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Users'  is null.");
        }
        var userToDelete = await _context.Users.FindAsync(id);

        if (userToDelete != null)
        {
             await _userManager.DeleteAsync(userToDelete);
        }
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> GetMyRoles()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByNameAsync(userEmail);
        var roles = await _userManager.GetRolesAsync(user);
        return View(roles);
    }

    private IUserEmailStore<IdentityUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<IdentityUser>)_userStore;
    }

    private IdentityUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<IdentityUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }
}
