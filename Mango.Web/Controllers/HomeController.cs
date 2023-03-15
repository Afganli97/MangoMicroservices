using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mango.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Mango.Web.Services.IServices;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly ICartService _cartService;

    public HomeController(ILogger<HomeController> logger, IProductService productService = null, ICartService cartService = null)
    {
        _logger = logger;
        _productService = productService;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        List<ProductDto> list = new();
        var response = await _productService.GetAllProductsAsync<ResponseDto>("");

        if (response != null && response.IsSuccess)
        {   
            list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
        }

        return View(list);
    }

    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        ProductDto product = new();
        var response = await _productService.GetProductByIdAsync<ResponseDto>(id, "");

        if (response != null && response.IsSuccess)
        {   
            product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
        }

        return View(product);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Details(ProductDto productDto)
    {
        CartDto cartDto = new()
        {
            CartHeaderDto = new()
            {
                UserId = User.Claims.Where(x => x.Type == "sub")?.FirstOrDefault()?.Value
            }
        };
        CartDetailDto cartDetailDto = new()
        {
            Count = productDto.Count,
            ProductId = productDto.Id 
        };

        var response = await _productService.GetProductByIdAsync<ResponseDto>(productDto.Id, "");

        if (response != null && response.IsSuccess)
        {
            cartDetailDto.ProductDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
        }

        List<CartDetailDto> cartDetailDtos = new();
        cartDetailDtos.Add(cartDetailDto);
        cartDto.CartDetailDtos = cartDetailDtos;

        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var addToCartResp = await _cartService.AddToCartAsync<ResponseDto>(cartDto, accessToken);

        if (addToCartResp != null && addToCartResp.IsSuccess)
        {
            return RedirectToAction(nameof(Index)) ;
        }

        return View(productDto);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Authorize]
    public async Task<IActionResult> Login()
    {
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Logout()
    {
        return SignOut("Cookies", "oidc");
    }
}

