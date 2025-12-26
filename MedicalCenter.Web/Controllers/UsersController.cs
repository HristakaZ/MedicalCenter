using MedicalCenter.Web.Attributes;
using MedicalCenter.Web.Constants;
using MedicalCenter.Web.Dtos.User;
using MedicalCenter.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace MedicalCenter.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly MedicalCenterDbContext _context;
        private readonly bool isAdministrator;
        public UsersController(MedicalCenterDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            isAdministrator = httpContextAccessor.HttpContext.Session.GetString("UserRole") == "Administrator";
        }

        // GET: Users
        [AuthenticateAuthorize("Administrator")]
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            List<GetUserDto> userDtos = new List<GetUserDto>();
            foreach (var user in users)
            {
                userDtos.Add(new GetUserDto()
                {
                    ID = user.ID,
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    Role = user.Role
                });
            }

            return View(userDtos);
        }

        // GET: Users/Details/5
        [AuthenticateAuthorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            // Ensure that users can only view their own details if he is not an administrator
            if ((user.ID != HttpContext.Session.GetInt32("UserID")) && !isAdministrator)
            {
                return RedirectToAction("Register");
            }

            GetUserDto userDto = new GetUserDto()
            {
                ID = user.ID,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Role = user.Role
            };

            return View(userDto);
        }

        // GET: Users/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    Email = registerUserDto.Email,
                    Name = registerUserDto.Name,
                    Surname = registerUserDto.Surname,
                    Password = HashPassword(registerUserDto.Password),
                    // assigning default role as 'Administrator' with ID = 3
                    Role = await _context.Roles.FirstOrDefaultAsync(role => role.ID == RoleConstants.AdminRoleId)
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(registerUserDto);
        }

        // GET: Users/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Users/Login
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(x => x.Email == loginUserDto.Email && x.Password == HashPassword(loginUserDto.Password));
                if (user != null)
                {
                    HttpContext.Session.SetInt32("UserID", user.ID);
                    HttpContext.Session.SetString("UserRole", user.Role.Description);
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    if (user.RoleID == RoleConstants.AdminRoleId)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction("MyExaminations", "MedicalExaminations");
                    }
                }
            }

            string error = "Invalid email or password.";
            ModelState.AddModelError(nameof(loginUserDto.Email), error);
            ModelState.AddModelError(nameof(loginUserDto.Password), error);

            return View(loginUserDto);
        }

        // GET: Users/Edit/5
        [AuthenticateAuthorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Ensure that users can only view their own details if he is not an administrator
            if ((user.ID != HttpContext.Session.GetInt32("UserID")) && !isAdministrator)
            {
                return RedirectToAction("Logout");
            }

            List<Role> roles = await _context.Roles.ToListAsync();
            EditUserDto userDto = new EditUserDto
            {
                ID = user.ID,
                Name = user.Name,
                Surname = user.Surname,
                SelectedRole = user.Role.ID.ToString(),
                Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.ID.ToString(),
                    Text = r.Description,
                    Selected = r.ID == user.Role.ID
                }).ToList()
            };

            return View(userDto);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize]
        public async Task<IActionResult> Edit(int id, EditUserDto editUserDto)
        {
            if (!UserExists(id) || id != editUserDto.ID)
            {
                return NotFound();
            }

            Role selectedRole = null;
            if (isAdministrator)
            {
                if (!int.TryParse(editUserDto.SelectedRole, out int selectedRoleId))
                {
                    return NotFound();
                }

                selectedRole = await _context.Roles.FirstOrDefaultAsync(x => x.ID == selectedRoleId);

                if (selectedRole == null)
                {
                    return NotFound();
                }
            }

            User user = await _context.Users.FindAsync(id);

            // Ensure that users can only view their own details(doesn't apply to administrators)
            if ((user!.ID != HttpContext.Session.GetInt32("UserID")) && !isAdministrator)
            {
                return RedirectToAction("Logout");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Name = editUserDto.Name;
                    user.Surname = editUserDto.Surname;
                    if (isAdministrator)
                    {
                        user.Role = selectedRole!;
                    }
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(editUserDto);
        }

        // GET: Users/Delete/5
        [AuthenticateAuthorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            // Ensure that users can only view their own details if he is not an administrator
            if ((user.ID != HttpContext.Session.GetInt32("UserID")) && !isAdministrator)
            {
                return RedirectToAction("Logout");
            }

            GetUserDto userDto = new GetUserDto()
            {
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Role = user.Role
            };

            return View(userDto);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [AuthenticateAuthorize]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }

        private string HashPassword(string password)
        {
            byte[] hashBytes;
            using (SHA512 algorithm = SHA512.Create())
            {
                hashBytes = algorithm.ComputeHash(new UTF8Encoding().GetBytes(password));
            }

            return Convert.ToBase64String(hashBytes);
        }
    }
}
