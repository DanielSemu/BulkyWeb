
using Bulky.DataAccess.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Product> products = _context.Products.ToList();
            return View(products);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product) {
            //This is Server Side Validation Application.
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                TempData["success"] = "Product Created Succesfully";
                return RedirectToAction("Index", "Product");
            }

            return RedirectToPage("Index");
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? product = _context.Products.Find(id);
            //Category? category1 = _context.Categories.FirstOrDefault(c => c.Id == id);
            //Category? category2 = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }
            //var ed = 2;
            //var category = _context.Categories.FirstOrDefault(c => c.Id == ed);
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {
           
            if (ModelState.IsValid)
            {
                _context.Products.Update(obj);
                _context.SaveChanges();
                TempData["warning"] = "Product Updated Succesfully";
                return RedirectToAction("Index", "product");
            }
            return View();

        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? product = _context.Products.Find(id);
            //Category? category1 = _context.Categories.FirstOrDefault(c => c.Id == id);
            //Category? category2 = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? obj = _context.Products.FirstOrDefault(c => c.Id == id);
            _context.Products.Remove(obj);
            _context.SaveChanges();
            TempData["error"] = "Category Deleted Succesfully";
            return RedirectToAction("Index", "Product");


        }
    }
}
