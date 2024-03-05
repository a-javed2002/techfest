using fest.Data;
using fest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;




namespace fest.Controllers
{
    public class ProductController : Controller
    {

        public readonly festdbContext db;
        public ProductController(festdbContext context)

        {
            db = context;
        }

        // GET: ProductController
        public async Task<IActionResult> Index()
        {

            var a = await db.Products.Include(U => U.Category).ToListAsync();

            return View(a);
        }

        // GET: ProductController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductController/Create
        public async Task<IActionResult> Create()
        {

            ViewBag.catid = new SelectList(db.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Create(Product p, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                {
                    var filename = Path.GetFileName(file.FileName);
                    string imagesfolder = Path.Combine(HttpContext.Request.PathBase.Value, "wwwroot/carimage");
                    if (!Directory.Exists(imagesfolder))
                    {
                        Directory.CreateDirectory(imagesfolder);
                    }
                    string filepath = Path.Combine(imagesfolder, filename);
                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var dbaddress = Path.Combine("carimage", filename);
                 p.ProductImage = dbaddress;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Only PNG and JPG image files are allowed.");
                }
            }

            db.Products.Add(p);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

            return View(p);
        }
        // GET: ProductController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {


            var edt = await db.Products.FindAsync(id);
            // Set the ViewBag.catid with the SelectList
            ViewBag.catid = new SelectList(db.Categories, "CategoryId", "CategoryName", edt.CategoryId);
            return View();


        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product p, IFormFile file)
        {
            if (id == 0)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    var prd = await db.Products.FindAsync(id);
                    if (prd == null)
                    {
                        return NotFound();
                    }
                    var dbaddress= prd.ProductImage;
                    if (file != null && file.Length > 0)
                    {
                        var extension = Path.GetExtension(file.FileName).ToLower();
                        if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                        {
                            var filename = Path.GetFileName(file.FileName);
                            string imagesfolder = Path.Combine(HttpContext.Request.PathBase.Value, "wwwroot/carimage");
                            if (!Directory.Exists(imagesfolder))
                            {
                                Directory.CreateDirectory(imagesfolder);
                            }
                            string filepath = Path.Combine(imagesfolder, filename);
                            using (var stream = new FileStream(filepath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                             dbaddress = Path.Combine("carimage", filename);
                            p.ProductImage = dbaddress;
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Only PNG and JPG image files are allowed.");
                        }

                    }
                    prd.ProductName = p.ProductName;
       
                    prd.Price =p.Price;
                    prd.ProductImage = dbaddress;
                    prd.CategoryId = p.CategoryId;
                    db.Update(prd);
                    await db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (db.Products.Any(x => x.ProductId != p.ProductId))
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

            ViewBag.catid = new SelectList(db.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // GET: ProductController/Delete/5
        // GET: ProductController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var del = await db.Products.FirstOrDefaultAsync(c => c.ProductId == id);

            if (del == null)
            {
                return NotFound();
            }

            db.Products.Remove(del);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public IActionResult AddCart(int? Id)
        {
            List<Cart> cartItems = HttpContext.Session.GetObject<List<Cart>>("Sess_Name") ?? new List<Cart>();
            Cart exisitng_item = cartItems.Find(item => item.ProductId == Id);
            if (exisitng_item != null)
            {
                exisitng_item.Quantity += 1;
            }
            else
            {
                var mydata = db.Products.Find(Id);
                cartItems.Add(
                new Cart
                {
                    ProductId = mydata.ProductId,
                    ProductName = mydata.ProductName,
                    ProductImage = mydata.ProductImage,
                    Quantity = 1,
                    Price = (int)mydata.Price
                }
                    );
            }
            HttpContext.Session.SetObject<List<Cart>>("Sess_Name", cartItems);
            ViewBag.mycart = HttpContext.Session.GetObject<List<Cart>>("Sess_Name");
            return View("AddCart");
        }


        public IActionResult minus(int? Id)
        {
            List<Cart> cartItems = HttpContext.Session.GetObject<List<Cart>>("Sess_Name") ?? new List<Cart>();
            Cart exisitng_item = cartItems.Find(item => item.ProductId == Id);
            if (exisitng_item != null)
            {
                if (exisitng_item.Quantity > 1)
                {
                    exisitng_item.Quantity -= 1;
                }
                else if (exisitng_item.Quantity == 1)
                {
                    cartItems.Remove(exisitng_item);

                }
            }
            HttpContext.Session.SetObject<List<Cart>>("Sess_Name", cartItems);
            ViewBag.mycart = HttpContext.Session.GetObject<List<Cart>>("Sess_Name");
            return View("AddCart");
        }

        public IActionResult Remove(int? Id)
        {
            List<Cart> cartItems = HttpContext.Session.GetObject<List<Cart>>("Sess_Name") ?? new List<Cart>();
            Cart exisitng_item = cartItems.Find(item => item.ProductId == Id);
            if (exisitng_item != null)
            {

                cartItems.Remove(exisitng_item);


            }
            HttpContext.Session.SetObject<List<Cart>>("Sess_Name", cartItems);
            ViewBag.mycart = HttpContext.Session.GetObject<List<Cart>>("Sess_Name");
            return View("AddCart");
        }
        public IActionResult plus(int? Id)
        {
            List<Cart> cartItems = HttpContext.Session.GetObject<List<Cart>>("Sess_Name") ?? new List<Cart>();
            Cart existingItem = cartItems.Find(item => item.ProductId == Id);

            if (existingItem != null)
            {
                existingItem.Quantity += 1;
            }

            HttpContext.Session.SetObject<List<Cart>>("Sess_Name", cartItems);
            ViewBag.mycart = HttpContext.Session.GetObject<List<Cart>>("Sess_Name");
            return View("AddCart");
        }
        public IActionResult RemoveAll()
        {
            HttpContext.Session.Remove("Sess_Name");
            return View("AddCart");
        }


        public IActionResult checkout(int total)
        {
            TempData["p"] = total;
            return View();
        }

        [HttpPost]
        public IActionResult checkout(IFormCollection f)
        {
            List<Cart> cartItems = HttpContext.Session.GetObject<List<Cart>>("Sess_Name") ?? new List<Cart>();

            //insertion in order table
            Order o = new Order();
            o.OrderDate = DateTime.Now;
            o.TotalAmount = int.Parse(TempData["p"].ToString());

            var userIdClaim = User.FindFirst("UserId")?.Value;

            o.UserId = int.Parse(userIdClaim);

            db.Add(o);
            db.SaveChanges();

            foreach (Cart itms in cartItems)
            {
                //insertion in item table
                OrderItem itemsTable = new OrderItem();
                itemsTable.ProductId = itms.ProductId;
                itemsTable.Quantity = itms.Quantity;
                itemsTable.OrderId = o.OrderId;
                db.Add(itemsTable);
                db.SaveChanges();

            }

            HttpContext.Session.SetObject("Sess_Name", "");
            return RedirectToAction(nameof(Index));


        }

        /// Session CLASSSSSS


    }

    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            string serializedValue = JsonConvert.SerializeObject(value);
            session.SetString(key, serializedValue);
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            string serializedValue = session.GetString(key);
            if (serializedValue != null)
            {
                return JsonConvert.DeserializeObject<T>(serializedValue);
            }
            return default(T);
        }
    }
}