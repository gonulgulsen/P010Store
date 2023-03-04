using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using P010Store.Entities;
using P010Store.Service.Abstract;
using P010Store.WebUI.Utils;

namespace P010Store.WebUI.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Policy = "AdminPolicy")]
    public class ProductsController : Controller
    {
        private readonly IService<Product> _service;
        private readonly IService<Category> _serviceCategory;
        private readonly IService<Brand> _serviceBrand;
        private readonly IProductService _productService;
        public ProductsController(IService<Product> service, IService<Category> serviceCategory, IService<Brand> serviceBrand, IProductService productService)
        {
            _service = service;
            _serviceCategory = serviceCategory;
            _serviceBrand = serviceBrand;
            _productService = productService; //InvalidOperationException: Unable to resolve service for type 'P010Store.Service.Abstract.IProductService' while attempting to activate 'P010Store.WebUI.Areas.Admin.Controllers.ProductsController'.  bu servisi kullandığımızda bu hatayı alırız, bu sorunu çözmek için servisi program.cs de tanımlamamız gerekir.

        }
        // GET: ProductsController
        public async Task<ActionResult> IndexAsync()
        {
            var model = await _productService.GetAllProductsByCategoriesBrandsAsync();//eskisi _service.GetAll();
            return View(model);
        }

        // GET: ProductsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductsController/Create
        public async Task<ActionResult> CreateAsync()
        {
            ViewBag.CategoryId = new SelectList(await _serviceCategory.GetAllAsync(), "Id","Name");
            ViewBag.BrandId = new SelectList(await _serviceBrand.GetAllAsync(), "Id", "Name");
            return View();
        }

        // POST: ProductsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(Product product, IFormFile? Image)
        {
            try
            {
                if (Image is not null) product.Image = await FileHelper.FileLoaderAsync(Image, filePath: "/wwwroot/Img/Products/");
                await _service.AddAsync(product);
                await _service.SaveChangesAsync();
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                ModelState.AddModelError("", "Hata Oluştu!");
            }

            ViewBag.CategoryId = new SelectList(await _serviceCategory.GetAllAsync(), "Id", "Name"); //burada ürün ekleme esnasında ekleme başarısız olursa ekrandaki select elementlerine verileri tekrar gönderiyoruz aksi taktirde null reference hatası alırız.
            ViewBag.BrandId = new SelectList(await _serviceBrand.GetAllAsync(), "Id", "Name");
            
            return View(product);
    }

        // GET: ProductsController/Edit/5
        public async Task<ActionResult> EditAsync(int id)
        {
            var model = await _service.FindAsync(id);
            return View(model);
        }

        // POST: ProductsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(int id, Product product, IFormFile? Image, bool? resmisil)
        {
            try
            {
                if (resmisil == true)
                {
                    FileHelper.FileRemover(product.Image);
                    product.Image = string.Empty;
                }
                if (Image is not null) product.Image = await FileHelper.FileLoaderAsync(Image, filePath: "/wwwroot/Img/Products/");
                _service.Update(product);
                await _service.SaveChangesAsync();
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                ModelState.AddModelError("", "Hata Oluştu!");
            }

            ViewBag.CategoryId = new SelectList(await _serviceCategory.GetAllAsync(), "Id", "Name");
            ViewBag.BrandId = new SelectList(await _serviceBrand.GetAllAsync(), "Id", "Name");

            return View(product);
        }

        // GET: ProductsController/Delete/5
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var model = await _service.FindAsync(id);
            return View(model);
        }

        // POST: ProductsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id,Product product)
        {
            try
            {
                FileHelper.FileRemover(product.Image);
                _service.Delete(product);
                _service.SaveChanges();
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }
    }
}
