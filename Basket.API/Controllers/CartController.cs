using Basket.API.Model;
using Basket.Service.Exception.Base;
using Basket.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        #region Public Fields

        public ICartService _CartService;
        public IProductService _ProductService;

        #endregion Public Fields

        #region Public Constructors

        public CartController(ICartService cartService, IProductService productService)
        {
            _CartService = cartService;
            _ProductService = productService;
        }

        #endregion Public Constructors

        #region Public Methods

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddToCartModel model)
        {
            if (model == null) return BadRequest("MODEL_IS_NULL");

            if (model.ProductId == null) return BadRequest("MODEL_PRODUCT_ID_IS_NULL");

            try
            {
                await _CartService.AddProductToCart(model.CartId, model.ProductId);
                return new JsonResult(new HttpServiceResponseBase<string> { Data = "Successfully Added", Error = null });
            }
            catch (Exception e)
            {
                var exception = e as ServiceException ?? e.InnerException as ServiceException;
                if (exception != null)
                    return new JsonResult(new HttpServiceResponseBase<string>
                    {
                        Data = null,
                        Error = new ErrorModel { Exception = exception.Message, StatusCode = exception.StatusCode, ErrorCode = exception.Name }
                    });

                return new JsonResult(new HttpServiceResponseBase<string>
                { Data = null, Error = new ErrorModel { Exception = e.Message, StatusCode = "503", } });
            }
        }

        #endregion Public Methods
    }
}