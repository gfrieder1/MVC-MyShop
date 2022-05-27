using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.Services;
using MyShop.WebUI.Controllers;
using MyShop.WebUI.Tests.Mocks;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MyShop.WebUI.Tests.Controllers
{
    [TestClass]
    public class BasketControllerTests
    {
        [TestMethod]
        public void CanAddBasketItem()
        {
            // Arrange
            IRepository<Basket> baskets = new MockContext<Basket>();
            IRepository<Product> products = new MockContext<Product>();
            IRepository<Order> orders = new MockContext<Order>();

            var httpContext = new MockHttpContext();

            IBasketService basketService = new BasketService(products, baskets);
            IOrderService orderService = new OrderService(orders);
            var controller = new BasketController(basketService, orderService);
            // inject httpcontext to controller
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            // Act
            controller.AddToBasket("1");
            Basket basket = baskets.Collection().FirstOrDefault();

            // Assert
            Assert.IsNotNull(basket); // Basket was created
            Assert.AreEqual(basket.BasketItems.Count(), 1); // Basket has 1 item
            Assert.AreEqual(basket.BasketItems.ToList().FirstOrDefault().ProductID, "1"); // Product in basket has id "1"
        }

        [TestMethod]
        public void CanGetSummaryViewModel()
        {
            // Arrange
            IRepository<Basket> baskets = new MockContext<Basket>();
            IRepository<Product> products = new MockContext<Product>();
            IRepository<Order> orders = new MockContext<Order>();


            products.Insert(new Product() { ID = "1", Price = 10.00m });
            products.Insert(new Product() { ID = "2", Price = 5.00m });

            Basket basket = new Basket();

            basket.BasketItems.Add(new BasketItem() { ProductID = "1", Quantity = 2 });
            basket.BasketItems.Add(new BasketItem() { ProductID = "2", Quantity = 1 });
            baskets.Insert(basket);

            IBasketService basketService = new BasketService(products, baskets);
            IOrderService orderService = new OrderService(orders);
            var controller = new BasketController(basketService, orderService);
            var httpContext = new MockHttpContext();
            httpContext.Request.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket") { Value = basket.ID });

            controller.ControllerContext = new ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            // Act
            var result = controller.BasketSummary() as PartialViewResult;
            var basketSummary = (BasketSummaryViewModel) result.ViewData.Model;

            // Assert
            Assert.AreEqual(basketSummary.BasketCount, 3);
            Assert.AreEqual(basketSummary.BasketValue, 25.00m);
        }

        [TestMethod]
        public void CanCheckoutAndCreateOrder ()
        {
            // Arrange
            IRepository<Product> products = new MockContext<Product>();
            products.Insert(new Product() { ID = "1", Price = 10.00m });
            products.Insert(new Product() { ID = "2", Price = 5.00m });

            IRepository<Basket> baskets = new MockContext<Basket>();
            Basket basket = new Basket();
            basket.BasketItems.Add(new BasketItem() { ProductID = "1", Quantity = 2, BasketID = basket.ID });
            basket.BasketItems.Add(new BasketItem() { ProductID = "2", Quantity = 1, BasketID = basket.ID });

            baskets.Insert(basket);

            IBasketService basketService = new BasketService(products, baskets);

            IRepository<Order> orders = new MockContext<Order>();
            IOrderService ordersService = new OrderService(orders);

            var controller = new BasketController(basketService, ordersService);
            var httpContext = new MockHttpContext();
            httpContext.Request.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket")
            {
                Value = basket.ID
            });

            controller.ControllerContext = new ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            // Act
            Order order = new Order();
            controller.Checkout(order);
            Order orderInRep = orders.Find(order.ID);

            // Assert
            Assert.AreEqual(order.OrderItems.Count, 2);
            Assert.AreEqual(basket.BasketItems.Count, 0);
            Assert.AreEqual(orderInRep.OrderItems.Count, 2);
        }
    }
}
