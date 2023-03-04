using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P010Store.Entities;
using P010Store.WebAPIUsing.Utils;
using System.Net.Http.Json;

namespace P010Store.WebAPIUsing.Areas.Admin.Controllers
{
	[Area("Admin"), Authorize(Policy = "AdminPolicy")]
	public class BrandsController : Controller
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiAdres = "https://localhost:7015/api/Brands";


		public BrandsController(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		// GET: BrandsController
		public async Task<ActionResult> Index()
		{
			var model = await _httpClient.GetFromJsonAsync<List<Brand>>(_apiAdres); //_httpClient nesnesi içerisindeki GetfromJsoonAsync metodu _apiAdres içerisine yazdığımız api ye get isteği atıp oradan dönen Json listesini bizim belirttiğimiz <List<Brand>> marka listesine dönüştürür.
			return View(model); //json olarak çekip marka listeesine dönüştürdüğümüz listeyi model olarak ekrana yolladık.
		}

		// GET: BrandsController/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: BrandsController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: BrandsController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> CreateAsync(Brand brand, IFormFile? Logo)
		{
			if (ModelState.IsValid) //Model class ımız olan brand nesnesinin validayon için koyduğumuz kurallarını(örneğin marka adı required- boş geçilemez gibi)uyulmuşsa
			{
				try
				{
					brand.Logo = await FileHelper.FileLoaderAsync(Logo);
					var response = await _httpClient.PostAsJsonAsync(_apiAdres, brand);//_httpClient nesnesi içerisindeki PostAsJsonAsync metoduna parametre olarak Post isteği atacağımız adresi (_apiAdres) ve eklemek istedğimiz nesneyi(brand) verdiğimizde brand i json a çevirerek api ye eklenmek üzere yolluyor.Eğer bir eksik yoksa api brand i ekler ve geriye response nesnesine bir cevap ekler.
					if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));//eğer response değişkeni api den IssuccessStatusCode yani başarılı bir durum konu dönmüşse sayfayı ondex e yönlendir.
					
				}
				catch
				{
					ModelState.AddModelError("", "Hata Oluştu!");
				}
			}

			return View(brand);
		}
		// GET: BrandsController/Edit/5
		public async Task<ActionResult> EditAsync(int id)
		{
			var model = await _httpClient.GetFromJsonAsync<Brand>(_apiAdres + "/" + id);
			return View(model);
		}

		// POST: BrandsController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> EditAsync(int id, Brand brand, IFormFile? Logo)
		{
			if (ModelState.IsValid)
			{
				try
				{
					if (Logo is not null) brand.Logo = await FileHelper.FileLoaderAsync(Logo);
					var cevap = await _httpClient.PutAsJsonAsync(_apiAdres + "/" + id, brand);
					if(cevap.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
				}
				catch
				{
					ModelState.AddModelError("", "Hata Oluştu!");
				}
			}
			return View(brand);

		}


		// GET: BrandsController/Delete/5
		public async Task<ActionResult> DeleteAsync(int id)
		{
			var model = await _httpClient.GetFromJsonAsync<Brand>(_apiAdres + "/" + id);
			return View(model);
		}

		// POST: BrandsController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteAsync(int id,Brand collection)
		{
			try
			{
				FileHelper.FileRemover(collection.Logo);
				var sonuc = await _httpClient.DeleteAsync(_apiAdres + "/" + id);
				if(sonuc.IsSuccessStatusCode ) return RedirectToAction(nameof(Index));
			}
			catch
			{
				ModelState.AddModelError("", "Hata Oluştu!");
			}
			return View(collection);
		}
	}
}