using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class BasketController : Controller
    {
        IBasketService basketService;
        IOrderService orderService;
        IRepository<Customer> customers;

        public BasketController(IBasketService basketService, IOrderService orderService, IRepository<Customer> customers)
        {
            this.basketService = basketService;
            this.orderService = orderService;
            this.customers = customers;
        }

        // GET: Basket
        public ActionResult Index()
        {
            var model = basketService.GetBasketItems(this.HttpContext);
            return View(model);
        }

        public ActionResult AddToBasket(string ID)
        {
            basketService.AddToBasket(this.HttpContext, ID);

            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromBasket(string ID)
        {
            basketService.RemoveFromBasket(this.HttpContext, ID);

            return RedirectToAction("Index");
        }

        public PartialViewResult BasketSummary()
        {
            var basketSummary = basketService.GetBasketSummary(this.HttpContext);

            return PartialView(basketSummary);
        }

        [Authorize]
        public ActionResult Checkout()
        {
            Customer customer = customers.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);

            if (customer != null)
            {
                Order order = new Order()
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    State = customer.State,
                    City = customer.City,
                    Street = customer.Street,
                    ZipCode = customer.ZipCode
                };

                return View(order);
            }

            return RedirectToAction("Error");
        }

        [HttpPost]
        [Authorize]
        public ActionResult Checkout(Order order)
        {
            var basketItem = basketService.GetBasketItems(this.HttpContext);
            order.OrderStatus = "Order Created"; // order statuses should be enum
            order.Email = User.Identity.Name;

            // process payment

            order.OrderStatus = "Payment Processed";
            orderService.CreateOrder(order, basketItem);
            basketService.ClearBasket(this.HttpContext);

            return RedirectToAction("ThankYou", new { OrderID = order.ID });
        }

        public ActionResult ThankYou(string OrderID)
        {
            ViewBag.OrderID = OrderID;
            return View();
        }
    }
}