using fest.Data;
using fest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Web;

namespace fest.Controllers
{

    public class UserController : Controller
    {
        public readonly festdbContext db;
        public UserController(festdbContext context)
        {
            db = context;
        }
        public  async Task<IActionResult> Index()
        {
            var a = await db.Users.Include(u => u.RoleIdfkNavigation).ToListAsync();
            return View(a);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.roleid = new SelectList(db.Roles, "Id", "RoleName");
          
            return View();
        }
        [HttpPost]
      [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Create(User u)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Users.Add(u);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log or handle the exception appropriately
                    throw;
                }
            }
            else
            {
                // If ModelState is not valid, return to the view with validation errors
                return View(u);
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var edt = await db.Users.FindAsync(id);

                if (edt == null)
                {
                    return NotFound(); // Return a 404 Not Found if the user is not found
                }

                ViewBag.roleid = new SelectList(db.Roles, "Id", "RoleName", edt.RoleIdfk);

                return View(edt);
            }
            catch (Exception ex)
            {
                // Log the exception (you might want to use a proper logging framework)
                Console.WriteLine($"Error in Edit action: {ex.Message}");

                // Redirect to an error page
                return View("Error");
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User u)
        {
            if (id == null)
            {
                return NotFound();
            }
            else { 


            try
            {
                db.Update(u);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (db.Users.Any(u => u.Id != id)) {

                }
                else {
                    throw;
                }

            }
        }
            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var a = db.Users.Include(u => u.RoleIdfkNavigation).FirstOrDefault(u => u.Id == id);
                db.Users.Remove(a);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                throw;
            }
            }

        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public  IActionResult  Login()
        {
      
            return View();
        }
        [HttpPost]
        public IActionResult Login(User u)
        {
            try
            {
                ClaimsIdentity identity = null;
                bool isAuthenticate = false;

                var res = db.Users.Where(x =>
                    x.Email == u.Email &&
                    x.Passwords == u.Passwords).FirstOrDefault();

                // Log the values for debugging
                Console.WriteLine($"u.Email: {u.Email}, u.Passwords: {u.Passwords}");

                if (res != null)
                {
                    // Check whether res.Email and res.Passwords are not null before accessing them
                    Console.WriteLine($"Found user: {res.UserName}, RoleIdfk: {res.RoleIdfk}, Email: {res.Email}, Passwords: {res.Passwords}");

                    if (res.RoleIdfk == 1)
                    {
                        identity = new ClaimsIdentity(new[]
                        {
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim(ClaimTypes.Name, res.UserName),
                       new Claim("UserId", res.Id.ToString()),
                }, CookieAuthenticationDefaults.AuthenticationScheme);
                        isAuthenticate = true;
                    }
                    else if (res.RoleIdfk == 2)
                    {
                        identity = new ClaimsIdentity(new[]
                        {
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim(ClaimTypes.Name, res.UserName),
                                 new Claim("UserId", res.Id.ToString()),
                }, CookieAuthenticationDefaults.AuthenticationScheme);
                        isAuthenticate = true;
                    }

                    if (isAuthenticate)
                    {
                        var principal = new ClaimsPrincipal(identity);
                        var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    Console.WriteLine("User not found in the database or invalid password.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }

            return View();
        }


    }
}
