
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork context, IWebHostEnvironment webHostEnvironment)
        {
            _unitWork = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> products =_unitWork.Product.GetAll(includeProperties:"Category").ToList();
           
            return View(products);
        }
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> CategoryList = 
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CategoryList"] = CategoryList;

            ProductVM productVM = new()
            {
                CategoryList= _unitWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product =new Product()
            };
            if( id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
            
            
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM pro ,IFormFile? file)
        {
            //This is Server Side Validation Application.
            if (ModelState.IsValid)
            {
                string wwwRootPath=_webHostEnvironment.WebRootPath;
                if (file != null){
                    string fileName=Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    if (!string.IsNullOrEmpty(pro.Product.ImageUrl))
                    {
                        ///Here We Have to Delete The Old Image
                        var oldImage=Path.Combine(wwwRootPath,pro.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);    
                        }
                    }
                    using(var fileStream =new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    pro.Product.ImageUrl =@"\images\product\"+ fileName;
                }
                if(pro.Product.Id == 0)
                {
                    _unitWork.Product.Add(pro.Product);
                }
                else
                {
                    _unitWork.Product.Update(pro.Product); 
                }
                
                _unitWork.Save();
                TempData["success"] = "Product Created Succesfully";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                ProductVM productVM = new()
                {
                    CategoryList = _unitWork.Category.GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                    Product = new Product()
                };
                return View(productVM);
            }
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? product = _unitWork.Product.Get(c=>c.Id == id);
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
                _unitWork.Product.Update(obj);
                _unitWork.Save();
                TempData["warning"] = "Product Updated Succesfully";
                return RedirectToAction("Index", "product");
            }
            return View();

        }
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? product = _unitWork.Product.Get(c => c.Id == id);
        //    //Category? category1 = _context.Categories.FirstOrDefault(c => c.Id == id);
        //    //Category? category2 = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}
        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{
        //    Product? obj = _unitWork.Product.Get(c => c.Id == id);
        //    _unitWork.Product.Remove(obj);
        //    _unitWork.Save();
        //    TempData["error"] = "Category Deleted Succesfully";
        //    return RedirectToAction("Index", "Product");


        //}
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll() 
        {
            List<Product> products = _unitWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data=products});
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted= _unitWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new {success=false,message="Error While deleting"});
            }
            var oldImage = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImage))
            {
                System.IO.File.Delete(oldImage);
            }
            _unitWork.Product.Remove(productToBeDeleted);
            _unitWork.Save();
            return Json(new { success = true, message = "Product Deleted Successfully" });
        }
        #endregion
    }
}
