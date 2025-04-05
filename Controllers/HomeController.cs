using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViagemCRUDProject.Models;

namespace ViagemCRUDProject.Controllers
{
    public class HomeController : Controller
    {

        private AppDbContext _context = new AppDbContext();

        // REGISTER
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Register(User user)
        {
            if(ModelState.IsValid == false || ValidateUser(user.Country, user.Gender) == false)
            {
                TempData["Register User Error"] = "Fill all values properly.";
            }
            else
            {
                _context.Table_Users.Add(user);
                await _context.SaveChangesAsync();
                TempData["Register User Success"] = "Account created successfully!";
            }
            return View();
        }

        private bool ValidateUser(string country, string gender)
        {
            if(string.IsNullOrEmpty(country) || string.IsNullOrEmpty(gender))
            {
                return false;
            }
            return true;
        }

        // LOGIN User
        [HttpGet]
        public ActionResult LoginUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginUser(string email, string password)
        {
            if(ValidateUserLogin(email, password) == false)
            {
                TempData["Login User Error"] = "Fill all values properly.";
            }
            else
            {
                var existingUser = _context.Table_Users.FirstOrDefault(user => user.Email == email);
                if(existingUser != null && existingUser.Password == password)
                {
                    Session["UserEmail"] = existingUser.Email;
                    Session["Role"] = "User";
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    TempData["Login User Error"] = "Incorrect Email/Password.";
                }
            }
            return View();
        }

        private bool ValidateUserLogin(string email, string password)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return false;
            }
            return true;
        }

        // LOGIN Admin
        [HttpGet]
        public ActionResult LoginAdmin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginAdmin(string username, string password)
        {
            if (ValidateAdminLogin(username, password) == false)
            {
                TempData["Login User Error"] = "Fill all values properly.";
            }
            else
            {
                var existingAdmin = _context.Table_Admins.FirstOrDefault(admin => admin.Username == username);
                if (existingAdmin != null && existingAdmin.Password == password)
                {
                    Session["AdminUniqueId"] = existingAdmin.Username;
                    Session["Role"] = "Admin";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Login Admin Error"] = "Incorrect Email/Password.";
                }
            }
            return View();
        }

        private bool ValidateAdminLogin(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }
            return true;
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        // UI Pages
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult About()
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> AdminOperations()
        {
            if (Session["Role"].ToString() != "Admin") return RedirectToAction("Index", "Home");

            var allUsers = await _context.Table_Users.ToListAsync();
            return View(allUsers);
        }

        [HttpGet]
        public async Task<ActionResult> UserDetails(int id)
        {
            if (Session["Role"].ToString() != "Admin") return RedirectToAction("Index", "Home");

            var user = await _context.Table_Users.FindAsync(id);
            if(user == null) return RedirectToAction("AdminOperations", "Home");
            return View(user);
        }

        [HttpGet]
        public async Task<ActionResult> UserEdit(int id)
        {
            if (Session["Role"].ToString() != "Admin") return RedirectToAction("Index", "Home");

            var user = await _context.Table_Users.FindAsync(id);
            if (user == null) return RedirectToAction("AdminOperations", "Home");
            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> UserEdit(User user)
        {
            if (Session["Role"].ToString() != "Admin") return RedirectToAction("Index", "Home");

            if (ModelState.IsValid == false || ValidateUser(user.Country, user.Gender) == false)
            {
                TempData["User Edit Error"] = "Fill all values properly.";
            }
            else
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminOperations", "Home");
            }
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> UserDelete(int? id)
        {
            if (Session["Role"].ToString() != "Admin") return RedirectToAction("Index", "Home");
            var user = await _context.Table_Users.FindAsync(id);
            if (user == null) return RedirectToAction("AdminOperations", "Home");
            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> UserDelete(int id)
        {
            if (Session["Role"].ToString() != "Admin") return RedirectToAction("Index", "Home");
            var user = await _context.Table_Users.FindAsync(id);
            _context.Table_Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("AdminOperations", "Home");
        }

    }
}