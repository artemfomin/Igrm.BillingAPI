using Igrm.BillingAPI.Exceptions;
using Igrm.BillingAPI.Models.API;
using Igrm.BillingAPI.Models.DTO;
using Igrm.BillingAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderProcessingService _orderProcessingService;

        public OrderController(ILogger<OrderController> logger, IOrderProcessingService orderProcessingService)
        {
            _logger = logger;
            _orderProcessingService = orderProcessingService;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PlaceOrderResponse>> Place([FromBody] PlaceOrderRequest request)
        {
            _logger.LogInformation($"Place order service call for {request.OrderNumber}...");
            var response = new PlaceOrderResponse();
            try
            {
                await _orderProcessingService.PlaceOrderAsync(request.OrderNumber, request.UserId, request.OrderAmount, request.Gateway, request.Description, request.Callback);
                var order = await _orderProcessingService.GetOrderAsync(request.OrderNumber);
                response.Bills.AddRange(order.Bills.Select(x => (BillingInfoDTO)x).ToList());
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning($"Validations failures: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured while placing order: {ex.Message} {ex.StackTrace}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return Ok(response);
        }

        [HttpGet("{OrderNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetOrderStatusResponse>> Status([FromRoute] GetOrderStatusRequest request)
        {
            _logger.LogInformation($"Get order status service call for {request.OrderNumber}...");
            var response = new GetOrderStatusResponse();
            try
            {
                response.OrderStatus = (await _orderProcessingService.GetOrderAsync(request.OrderNumber)).OrderStatus;
            }
            catch (OrderNotFoundException ex)
            {
                _logger.LogWarning($"{ex.Message}");
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning($"Validations failures: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured while getting order status: {ex.Message} {ex.StackTrace}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return Ok(response);

        }

        [HttpGet("{OrderNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetReceiptResponse>> Receipt([FromRoute] GetReceiptRequest request)
        {
            _logger.LogInformation($"Get receipt service call for {request.OrderNumber}...");
            var response = new GetReceiptResponse();
            try
            {
                var order = await _orderProcessingService.GetOrderAsync(request.OrderNumber);
                response.Receipt = await _orderProcessingService.GetReceiptAsync(order);

            }
            catch (OrderNotFoundException ex)
            {
                _logger.LogWarning($"{ex.Message}");
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning($"Validations failures: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured while getting receipt: {ex.Message} {ex.StackTrace}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return Ok(response);
        }


        [HttpPut("{OrderNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CancelOrderResponse>> Cancel([FromRoute] CancelOrderRequest request)
        {
            _logger.LogInformation($"Cancel order service call for {request.OrderNumber}...");
            var response = new CancelOrderResponse();
            try
            {
                var order = await _orderProcessingService.GetOrderAsync(request.OrderNumber);
                await _orderProcessingService.CancelOrderAsync(order);
                response.OperationSucceded = true;
            }
            catch (OrderNotFoundException ex)
            {
                _logger.LogWarning($"{ex.Message}");
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning($"Validations failures: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured while canceling order: {ex.Message} {ex.StackTrace}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return Ok(response);
        }

    }
}
