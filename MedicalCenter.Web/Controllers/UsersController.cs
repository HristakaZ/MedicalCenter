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

        [AuthenticateAuthorize("Administrator")]
        public async Task<IActionResult> Index(string email,
                                               string name,
                                               string surname,
                                               int pageNumber = 1,
                                               int pageSize = 10)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .AsQueryable();

            // Filter by Email
            if (!string.IsNullOrWhiteSpace(email))
            {
                string e = email.Trim().ToLower();
                query = query.Where(u => u.Email.ToLower().Contains(e));
            }

            // Filter by Name
            if (!string.IsNullOrWhiteSpace(name))
            {
                string n = name.Trim().ToLower();
                query = query.Where(u => u.Name.ToLower().Contains(n));
            }

            // Filter by Surname
            if (!string.IsNullOrWhiteSpace(surname))
            {
                string s = surname.Trim().ToLower();
                query = query.Where(u => u.Surname.ToLower().Contains(s));
            }

            // Pagination
            int totalItems = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.Email)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map to DTO
            var dtos = users.Select(user => new GetUserDto
            {
                ID = user.ID,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Role = user.Role
            }).ToList();

            // Pass filter values back to view
            ViewBag.Email = email;
            ViewBag.Name = name;
            ViewBag.Surname = surname;

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View(dtos);
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
        public async Task<IActionResult> Register()
        {
            RegisterPatientUserDto registerPatientUserDto = new RegisterPatientUserDto()
            {
                Doctors = await _context.Doctors.Select(r => new SelectListItem()
                {
                    Value = r.ID.ToString(),
                    Text = $"{r.Name} {r.Surname}"
                }).ToListAsync()
            };

            return View(registerPatientUserDto);
        }

        // POST: Users/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterPatientUserDto registerPatientUserDto)
        {
            if (ModelState.IsValid)
            {
                if (!int.TryParse(registerPatientUserDto.SelectedDoctor, out int doctorId))
                {
                    return NotFound();
                }

                Patient patient = new Patient()
                {
                    Email = registerPatientUserDto.Email,
                    Name = registerPatientUserDto.Name,
                    Surname = registerPatientUserDto.Surname,
                    Password = HashPassword(registerPatientUserDto.Password),
                    // assigning default role as 'Patient' with ID = 1
                    Role = await _context.Roles.FirstOrDefaultAsync(role => role.ID == RoleConstants.PatientRoleId),
                    SSN = registerPatientUserDto.SSN,
                    PhoneNumber = registerPatientUserDto.PhoneNumber,
                    Doctor = await _context.Doctors.FindAsync(doctorId),
                    DoctorID = doctorId
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Login));
            }

            registerPatientUserDto.Doctors = await _context.Doctors.Select(r => new SelectListItem()
            {
                Value = r.ID.ToString(),
                Text = $"{r.Name} {r.Surname}"
            }).ToListAsync();

            return View(registerPatientUserDto);
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

            string error = "Грешни имейл или парола.";
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

            // Ensure that users can only view his own details(excluding administrators)
            if ((user.ID != HttpContext.Session.GetInt32("UserID")) && !isAdministrator)
            {
                return RedirectToAction("Logout");
            }

            List<Role> roles = await _context.Roles.ToListAsync();
            EditUserDto userDto = new EditUserDto
            {
                ID = user.ID,
                Name = user.Name,
                Surname = user.Surname
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

        [HttpGet]
        [AuthenticateAuthorize("Doctor", "Administrator")]
        public async Task<IActionResult> EditDoctor(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            var doctor = await _context.Doctors.FindAsync(id);
            if (user == null || doctor == null)
            {
                return NotFound();
            }
            // Ensure that users can only view his own details(excluding administrators)
            if ((user.ID != HttpContext.Session.GetInt32("UserID")) && !isAdministrator)
            {
                return RedirectToAction("Logout");
            }

            EditDoctorDto editDoctorDto = new EditDoctorDto
            {
                ID = user.ID,
                Name = user.Name,
                Surname = user.Surname,
                Roles = await _context.Roles.Select(r => new SelectListItem
                {
                    Value = r.ID.ToString(),
                    Text = r.Description,
                    Selected = r.ID == user.RoleID
                }).ToListAsync(),
                Room = (await _context.Doctors.FirstOrDefaultAsync(d => d.ID == user.ID))!.Room,
                SelectedRole = user.Role.ID.ToString(),
                SelectedSpecialty = doctor.SpecialtyID.ToString(),
                Specialties = await _context.Specialties.Select(s => new SelectListItem
                {
                    Value = s.ID.ToString(),
                    Text = s.Description,
                    Selected = s.ID == doctor.SpecialtyID
                }).ToListAsync(),
            };

            return View(editDoctorDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize("Doctor", "Administrator")]
        public async Task<IActionResult> EditDoctor(EditDoctorDto editDoctorDto)
        {
            if (!ModelState.IsValid)
            {
                editDoctorDto.Roles = await _context.Roles
                    .Select(r => new SelectListItem
                    {
                        Value = r.ID.ToString(),
                        Text = r.Description,
                        Selected = r.ID.ToString() == editDoctorDto.SelectedRole
                    }).ToListAsync();

                editDoctorDto.Specialties = await _context.Specialties
                    .Select(s => new SelectListItem
                    {
                        Value = s.ID.ToString(),
                        Text = s.Description,
                        Selected = s.ID.ToString() == editDoctorDto.SelectedSpecialty
                    }).ToListAsync();

                return View(editDoctorDto);
            }

            // Load the existing entities
            var user = await _context.Users.FindAsync(editDoctorDto.ID);
            var doctor = await _context.Doctors.FindAsync(editDoctorDto.ID);

            if (user == null || doctor == null)
            {
                return NotFound();
            }

            // Authorization: only the doctor himself or an admin can edit
            var currentUserId = HttpContext.Session.GetInt32("UserID");
            bool isSameUser = user.ID == currentUserId;
            bool isAdministrator = HttpContext.Session.GetString("UserRole") == "Administrator";

            if (!isSameUser && !isAdministrator)
            {
                return RedirectToAction("Logout");
            }


            user.Name = editDoctorDto.Name;
            user.Surname = editDoctorDto.Surname;
            doctor.Room = editDoctorDto.Room;
            if (!int.TryParse(editDoctorDto.SelectedSpecialty, out int specialtyId))
            {
                return NotFound();
            }
            doctor.SpecialtyID = specialtyId;
            _context.Users.Update(user);
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();

            // Only the administrator is able to change the role of the doctor
            if (isAdministrator)
            {
                if (!int.TryParse(editDoctorDto.SelectedRole, out int roleId))
                {
                    return NotFound();
                }
                if (roleId != user.RoleID)
                {
                    user.RoleID = roleId;
                    user.Role = await _context.Roles.FindAsync(user.RoleID);

                    return await ChangeRoleOfDoctor(roleId, doctor, user);
                }

                return RedirectToAction("Index");
            }

            return RedirectToAction("MyExaminations", "MedicalExaminations");
        }

        [HttpGet]
        [AuthenticateAuthorize("Patient", "Administrator")]
        public async Task<IActionResult> EditPatient(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            var patient = await _context.Patients.FindAsync(id);
            if (user == null || patient == null)
            {
                return NotFound();
            }
            // Ensure that users can only view his own details(excluding administrators)
            if ((user.ID != HttpContext.Session.GetInt32("UserID")) && !isAdministrator)
            {
                return RedirectToAction("Logout");
            }

            EditPatientDto editPatientDto = new EditPatientDto
            {
                ID = user.ID,
                Name = user.Name,
                Surname = user.Surname,
                SSN = patient.SSN,
                PhoneNumber = patient.PhoneNumber,
                SelectedDoctor = patient.DoctorID.ToString(),
                Doctors = await _context.Doctors.Select(d => new SelectListItem
                {
                    Value = d.ID.ToString(),
                    Text = $"{d.Name} {d.Surname}",
                    Selected = d.ID == patient.DoctorID
                }).ToListAsync(),
                SelectedRole = user.RoleID.ToString(),
                Roles = await _context.Roles.Select(r => new SelectListItem()
                {
                    Value = r.ID.ToString(),
                    Text = r.Description,
                    Selected = r.ID == user.RoleID
                }).ToListAsync()
            };

            return View(editPatientDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize("Patient", "Administrator")]
        public async Task<IActionResult> EditPatient(EditPatientDto editPatientDto)
        {
            if (!ModelState.IsValid)
            {
                editPatientDto.Doctors = await _context.Doctors
                    .Select(d => new SelectListItem
                    {
                        Value = d.ID.ToString(),
                        Text = $"{d.Name} {d.Surname}",
                        Selected = d.ID.ToString() == editPatientDto.SelectedDoctor
                    }).ToListAsync();
                editPatientDto.Roles = await _context.Roles
                    .Select(r => new SelectListItem()
                    {
                        Value = r.ID.ToString(),
                        Text = r.Description,
                        Selected = r.ID.ToString() == editPatientDto.SelectedRole
                    }).ToListAsync();
                return View(editPatientDto);
            }
            // Load the existing entities
            var user = await _context.Users.FindAsync(editPatientDto.ID);
            var patient = await _context.Patients.FindAsync(editPatientDto.ID);
            if (user == null || patient == null)
            {
                return NotFound();
            }
            // Authorization: only the patient himself or an admin can edit
            var currentUserId = HttpContext.Session.GetInt32("UserID");
            bool isSameUser = user.ID == currentUserId;
            bool isAdministrator = HttpContext.Session.GetString("UserRole") == "Administrator";
            if (!isSameUser && !isAdministrator)
            {
                return RedirectToAction("Logout");
            }

            user.Name = editPatientDto.Name;
            user.Surname = editPatientDto.Surname;
            patient.SSN = editPatientDto.SSN;
            patient.PhoneNumber = editPatientDto.PhoneNumber;
            if (!int.TryParse(editPatientDto.SelectedDoctor, out int doctorId))
            {
                return NotFound();
            }
            patient.DoctorID = doctorId;
            _context.Users.Update(user);
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            // Only the administrator can change the role of the patient
            if (isAdministrator)
            {
                if (!int.TryParse(editPatientDto.SelectedRole, out int roleId))
                {
                    return NotFound();
                }
                if (roleId != user.RoleID)
                {
                    user.Role = await _context.Roles.FindAsync(roleId);
                    user.RoleID = roleId;

                    return await ChangeRoleOfPatient(roleId, patient, user);
                }

                return RedirectToAction("Index");
            }

            return RedirectToAction("MyExaminations", "MedicalExaminations");
        }

        [HttpGet]
        [AuthenticateAuthorize]
        public async Task<IActionResult> ChangePassword(int? id)
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
            // Ensure that users can only view his own details(excluding administrators)
            if ((user.ID != HttpContext.Session.GetInt32("UserID")) && !isAdministrator)
            {
                return RedirectToAction("Logout");
            }
            ChangePasswordDto changePasswordDto = new ChangePasswordDto
            {
                UserID = user.ID
            };

            return View(changePasswordDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthenticateAuthorize]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordDto changePasswordDto)
        {
            if (!UserExists(id) || id != changePasswordDto.UserID)
            {
                return NotFound();
            }
            User user = await _context.Users.FindAsync(id);
            // Ensure that users can only change their own password(doesn't apply to administrators)
            if ((user!.ID != HttpContext.Session.GetInt32("UserID")) && !isAdministrator)
            {
                return RedirectToAction("Logout");
            }

            bool arePasswordsMatching = user!.Password == HashPassword(changePasswordDto.CurrentPassword);
            if (!arePasswordsMatching)
            {
                ModelState.AddModelError(nameof(changePasswordDto.CurrentPassword), "Грешна текуща парола.");
                return View(changePasswordDto);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Password = HashPassword(changePasswordDto.NewPassword);
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

            return View(changePasswordDto);
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
                // Ensure that users can only view their own details if he is not an administrator
                if ((user.ID != HttpContext.Session.GetInt32("UserID")) && !isAdministrator)
                {
                    return RedirectToAction("Logout");
                }

                // Ensure that the administrator cannot delete himself
                if (user.ID == HttpContext.Session.GetInt32("UserID") && isAdministrator)
                {
                    return RedirectToAction("Logout");
                }

                if ((user is Patient || user is Doctor) && (_context.MedicalExaminations.Any(me => me.PatientID == user.ID || me.DoctorID == user.ID)))
                {
                    var userDto = new GetUserDto()
                    {
                        ID = user.ID,
                        Email = user.Email,
                        Name = user.Name,
                        Surname = user.Surname,
                        Role = user.Role
                    };
                    ModelState.AddModelError("CannotDeleteUser", "Потребителят участва в медицински прегледи и не може да бъде изтрит.");
                    return View(userDto);
                }
                else
                {
                    _context.Users.Remove(user);
                }
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

        private async Task<IActionResult> ChangeRoleOfPatient(int roleId, Patient patient, User user)
        {
            switch (roleId)
            {
                case RoleConstants.PatientRoleId:
                    // No action needed, user remains a patient
                    break;
                case RoleConstants.DoctorRoleId:
                    // If changing to Doctor, remove from Patients table
                    _context.Patients.Remove(patient);
                    // Just adding the record in the doctors table, will redirect to EditDoctor to fill in the rest of the details
                    _context.Doctors.Add(new Doctor()
                    {
                        ID = user.ID,
                        Email = user.Email,
                        Password = user.Password,
                        Name = user.Name,
                        Surname = user.Surname,
                        RoleID = (await _context.Roles.FindAsync(RoleConstants.DoctorRoleId)).ID,
                        Room = default,
                        SpecialtyID = _context.Specialties.First().ID
                    });
                    await _context.SaveChangesAsync();
                    return RedirectToAction("EditDoctor", new { id = user.ID });
                case RoleConstants.AdminRoleId:
                    // If changing to Administrator, remove from Patients table
                    _context.Patients.Remove(patient);
                    _context.Users.Add(new User()
                    {
                        ID = user.ID,
                        Email = user.Email,
                        Password = user.Password,
                        Name = user.Name,
                        Surname = user.Surname,
                        RoleID = (await _context.Roles.FindAsync(RoleConstants.AdminRoleId)).ID,
                    });
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Edit", new { id = user.ID });
            }

            return RedirectToAction("Index");
        }

        private async Task<IActionResult> ChangeRoleOfDoctor(int roleId, Doctor doctor, User user)
        {
            switch (roleId)
            {
                case RoleConstants.DoctorRoleId:
                    // No action needed, user remains a doctor
                    await _context.SaveChangesAsync();
                    break;
                case RoleConstants.PatientRoleId:
                    // If changing to Doctor, remove from Doctor table
                    _context.Doctors.Remove(doctor);
                    // Just adding the record in the patients table, will redirect to EditPatient to fill in the rest of the details
                    _context.Patients.Add(new Patient()
                    {
                        ID = user.ID,
                        Email = user.Email,
                        Password = user.Password,
                        Name = user.Name,
                        Surname = user.Surname,
                        RoleID = (await _context.Roles.FindAsync(RoleConstants.PatientRoleId)).ID,
                        SSN = 0,
                        PhoneNumber = "",
                        DoctorID = _context.Doctors.First().ID,
                    });
                    await _context.SaveChangesAsync();
                    return RedirectToAction("EditPatient", new { id = user.ID });
                case RoleConstants.AdminRoleId:
                    // If changing to Administrator, remove from Doctor table
                    _context.Doctors.Remove(doctor);
                    _context.Users.Add(new User()
                    {
                        ID = user.ID,
                        Email = user.Email,
                        Password = user.Password,
                        Name = user.Name,
                        Surname = user.Surname,
                        RoleID = (await _context.Roles.FindAsync(RoleConstants.AdminRoleId)).ID,
                    });
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Edit", new { id = user.ID });
            }

            return RedirectToAction("Index");
        }
    }
}
