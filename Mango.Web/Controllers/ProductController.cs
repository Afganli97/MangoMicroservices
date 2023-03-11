using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> list = new();
            var response = await _productService.GetAllProductsAsync<ResponseDto>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));   
            } 
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProductAsync<ResponseDto>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                } 
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _productService.GetProductByIdAsync<ResponseDto>(id);
            if (response != null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result)); 
                return View(model);
            } 
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.UpdateProductAsync<ResponseDto>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                } 
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _productService.GetProductByIdAsync<ResponseDto>(id);
            if (response != null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result)); 
                return View(model);
            } 
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.DeleteProductAsync<ResponseDto>(model.Id);
                if (response.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                } 
            }
            return View(model);
        }
    }
}