
using Bulky.DataAccess.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {       
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
                    _context = context;
        }
        public IActionResult Index()
        {
            List<Category> category=_context.Categories.ToList();
            return View(category);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]  
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display Order Cannot Exaclty match the Name");
            }
            //This is Server Side Validation Application.
            if (ModelState.IsValid) { 
                _context.Categories.Add(obj);
                _context.SaveChanges();
                TempData["success"] = "Category Created Succesfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
            
        }

        public IActionResult Edit(int? id) {
            if (id == null || id == 0) { 
                return NotFound();
            }
            Category? category=_context.Categories.Find(id);
            Category? category1=_context.Categories.FirstOrDefault(c => c.Id == id);
            Category? category2=_context.Categories.Where(c=>c.Id==id).FirstOrDefault();
            if (category == null) {
                return NotFound();
            }
            //var ed = 2;
            //var category = _context.Categories.FirstOrDefault(c => c.Id == ed);
            return View(category); 
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display Order Cannot Exaclty match the Name");
            }
            //This is Server Side Validation Application.
            if (ModelState.IsValid)
            {
                _context.Categories.Update(obj);
                _context.SaveChanges();
                TempData["warning"] = "Category Updated Succesfully";
                return RedirectToAction("Index", "Category");
            }
            return View();

        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _context.Categories.Find(id);
            //Category? category1 = _context.Categories.FirstOrDefault(c => c.Id == id);
            //Category? category2 = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? obj = _context.Categories.FirstOrDefault(c => c.Id == id);
                _context.Categories.Remove(obj);
                _context.SaveChanges();
            TempData["error"] = "Category Deleted Succesfully";
            return RedirectToAction("Index", "Category");
 

        }
    } 
}
