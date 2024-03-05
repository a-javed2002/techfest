using fest.Data;
using fest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fest.Controllers
{
    public class CategoryController : Controller
    {
        public readonly festdbContext db;

        public CategoryController(festdbContext context)
        {
            db = context;
        }
        // GET: CategoryController
        public async Task<ActionResult> Index()
        {
            var categories = await db.Categories.ToListAsync();

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
        public async Task<IActionResult> Create(Category c)
        {
            try
            {
                db.Categories.Add(c);
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
            var edt = await db.Categories.FindAsync(id);
            return View(edt);
        }


        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category c)
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
                    if (db.Categories.Any(x => x.CategoryId != c.CategoryId))
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
                var a = db.Categories.Find(id);
                db.Categories.Remove(a);
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
