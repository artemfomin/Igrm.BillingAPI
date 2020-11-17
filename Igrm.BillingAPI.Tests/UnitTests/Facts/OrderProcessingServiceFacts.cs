using Igrm.BillingAPI.Exceptions;
using Igrm.BillingAPI.Models.Business.Values;
using Igrm.BillingAPI.Tests.UnitTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Igrm.BillingAPI.Tests.UnitTests.Facts
{
    public class OrderProcessingServiceFacts
    {
        public class PlaceOrderAsyncMethod : IClassFixture<ServiceFixture>
        {
            private readonly ServiceFixture _fixture;

            public PlaceOrderAsyncMethod(ServiceFixture fixture)
            {
                _fixture = fixture;
            }

            [Theory]
            [InlineData("123",1,10,Gateway.SEPA,"","https://www.google.com/")]
            [InlineData("--rffdgfdgjpfdjgkdfjgkdfjglkdjglkdf$$@@_%^-", -100, 0.000001, Gateway.Bitcoin, null, null)]
            public async void WhenRegularParameterValuesProvided_ExpectOrderIdReturnedAsync(string? orderNumber, int userId, decimal orderAmount, Gateway gateway, string? description, string? callback)
            {
                var orderId = await _fixture.OrderProcessingService.PlaceOrderAsync(orderNumber, userId, orderAmount, gateway, description, callback);
                Assert.InRange<int>(orderId, 1, int.MaxValue);
            }

            [Theory]
            [InlineData(null, 1, 10, Gateway.SEPA, "", "https://www.google.com/")]
            [InlineData("123", 1, -10, Gateway.SEPA, "", "https://www.google.com/")]
            [InlineData("123", 1, 10, (Gateway)11, "", "https://www.google.com/")]
            [InlineData("123", 1, 10, Gateway.SEPA, "", "abc")]
            public async void WhenParametersAreWrong_ExpectValidationErrorThrown_Async(string? orderNumber, int userId, decimal orderAmount, Gateway gateway, string? description, string? callback)
            {
                ValidationException? validationException = null;
                try 
                {
                    var orderId = await _fixture.OrderProcessingService.PlaceOrderAsync(orderNumber, userId, orderAmount, gateway, description, callback); 
                }
                catch(ValidationException ex)
                {
                    validationException = ex;
                }

                Assert.NotNull(validationException);

            }
        }


        public class GetOrderAsyncMethod : IClassFixture<ServiceFixture>
        {
            private readonly ServiceFixture _fixture;

            public GetOrderAsyncMethod(ServiceFixture fixture)
            {
                _fixture = fixture;
            }

            [Fact]
            public async void WhenExistingOrderNumberProvided_ExpectOrderIsReturned_Async()
            {
                await _fixture.OrderProcessingService.PlaceOrderAsync("123", 1, 10, Gateway.SEPA, "", "https://www.google.com/");
                var order = _fixture.OrderProcessingService.GetOrderAsync("123");
                Assert.NotNull(order);
            }

            [Fact]
            public async void WhenNonExistingOrderNumberProvided_ExpectErrorThrown_Async()
            {
                await Assert.ThrowsAsync<OrderNotFoundException>(() => { return  _fixture.OrderProcessingService.GetOrderAsync("1234"); });
            }
        }

        public class GetReceiptAsyncMethod : IClassFixture<ServiceFixture>
        {
            private readonly ServiceFixture _fixture;
            public GetReceiptAsyncMethod(ServiceFixture fixture)
            {
                _fixture = fixture;
            }

            [Fact]
            public async void WhenExistingPaidOrderNumberProvided_ExpectReceiptIsReturned_Async()
            {
                await _fixture.OrderProcessingService.PlaceOrderAsync("123", 1, 10, Gateway.SEPA, "", "https://www.google.com/");
                var order = await _fixture.OrderProcessingService.GetOrderAsync("123");
                await _fixture.OrderProcessingService.AllocateSettlementsAsync(order);
                var receipt = await _fixture.OrderProcessingService.GetReceiptAsync(order);
                Assert.NotNull(receipt);
            }

            [Fact]
            public async void WhenExistingNotPaidOrderNumberProvided_ExpectErrorThrown_Async()
            {
                await _fixture.OrderProcessingService.PlaceOrderAsync("1234", 1, 10, Gateway.SEPA, "", "https://www.google.com/");
                var order = await _fixture.OrderProcessingService.GetOrderAsync("1234");
                await Assert.ThrowsAsync<OrderNotProcessedException>(() => { return _fixture.OrderProcessingService.GetReceiptAsync(order); });
            }
        }

        public class GetOrdersToProcessAsyncMethod : IClassFixture<ServiceFixture>
        {
            private ServiceFixture _fixture;

            public GetOrdersToProcessAsyncMethod(ServiceFixture fixture)
            {
                _fixture = fixture;
            }

            [Fact]
            public async void WhenSomeOrdersArePlaced_ExpectCollectionReturned_Async()
            {
                await _fixture.OrderProcessingService.PlaceOrderAsync("1-1", 1, 10, Gateway.SEPA, "", "https://www.google.com/");
                await _fixture.OrderProcessingService.PlaceOrderAsync("2-2", 1, 10, Gateway.SEPA, "", "https://www.google.com/");

                var orders = await _fixture.OrderProcessingService.GetOrdersToProcessAsync();
                Assert.Equal(2, orders.Count);
            }
        }

        public class CancelOrderAsyncMethod : IClassFixture<ServiceFixture>
        {
            private ServiceFixture _fixture;

            public CancelOrderAsyncMethod(ServiceFixture fixture)
            {
                _fixture = fixture;
            }

            [Fact]
            public async void WhenCancelingPlacedOrder_ExpectSuccessfulCancelation_Async()
            {
                await _fixture.OrderProcessingService.PlaceOrderAsync("123", 1, 10, Gateway.SEPA, "", "https://www.google.com/");
                var order = await _fixture.OrderProcessingService.GetOrderAsync("123");
                await _fixture.OrderProcessingService.CancelOrderAsync(order);
                Assert.Equal(OrderStatus.Canceled, order.OrderStatus);
            }

            [Fact]
            public async void WhenCancelingPaidOrder_ExpectErrorThrown_Async()
            {
                await _fixture.OrderProcessingService.PlaceOrderAsync("1234", 1, 10, Gateway.SEPA, "", "https://www.google.com/");
                var order = await _fixture.OrderProcessingService.GetOrderAsync("1234");
                await _fixture.OrderProcessingService.AllocateSettlementsAsync(order);
                await Assert.ThrowsAsync<CanNotCancelOrderException>(() => { return _fixture.OrderProcessingService.CancelOrderAsync(order); });
            }

        }
    }
}
