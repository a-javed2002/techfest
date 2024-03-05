using fest.Data;
using fest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fest.Controllers
{
    public class RoleController : Controller
    {
        public readonly festdbContext db;

        public RoleController(festdbContext context)
        {
            db = context;
        }
        // GET: CategoryController
        public async Task<ActionResult> Index()
        {
            var categories = await db.Roles.ToListAsync();

            // Pass the list of categories to the view
            return View(categories);
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CategoryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role c)
        {
            try
            {
                db.Roles.Add(c);
               await  db.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                throw;
            }
        }

        // GET: CategoryController/Edit/5
        // Asynchronous version
        public async Task<ActionResult> Edit(int id)
        {
            var edt = await db.Roles.FindAsync(id);
            return View(edt);
        }


        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Role c)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                try


                {

                    db.Update(c);
                 await db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch(DbUpdateConcurrencyException)
                {
                    if (db.Roles.Any(x => x.Id != c.Id))
                    {
                        return NotFound();

                    }
                    else
                    {
                        throw;
                    }
                    return View();
                }
            }
        }

  
        // POST: CategoryController/Delete/5
     
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var a = db.Roles.Find(id);
                db.Roles.Remove(a);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
