using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services
{
    public class BasketService
    {
        IRepository<Product> productContext;
        IRepository<Basket> basketContext;

        public const string BasketSessionName = "eCommerceBasket";

        public BasketService(IRepository<Product> ProductContext, IRepository<Basket> BasketContext)
        {
            this.productContext = ProductContext;
            this.basketContext = BasketContext;
        }

        private Basket GetBasket(HttpContextBase httpContext, bool createIfNull)
        {
            HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);

            Basket basket = new Basket();

            if (cookie != null)
            {
                string basketID = cookie.Value;

                if (!string.IsNullOrEmpty(basketID))
                {
                    // load existing basket
                    basket = basketContext.Find(basketID);
                }
                else
                {
                    if (createIfNull)
                    {
                        // create new basket for user
                        basket = CreateNewBasket(httpContext);
                    }
                }
            }
            else
            {
                if (createIfNull)
                {
                    // create new basket for user
                    basket = CreateNewBasket(httpContext);
                }
            }

            return basket;
        }

        private Basket CreateNewBasket(HttpContextBase httpContext)
        {
            Basket basket = new Basket();
            basketContext.Insert(basket); // insert into the database
            basketContext.Commit();

            HttpCookie cookie = new HttpCookie(BasketSessionName);
            cookie.Value = basket.ID;
            cookie.Expires = DateTime.Now.AddDays(1); // cookie will expire after 1 day
            httpContext.Response.Cookies.Add(cookie); // add cookie to the user's context

            return basket;
        }

        public void AddToBasket(HttpContextBase httpContext, string productID)
        {
            Basket basket = GetBasket(httpContext, true); // make sure to create a basket for the user! (hence, true)
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ID == productID);

            if (item == null)
            {
                // item is not yet in user's basket
                item = new BasketItem() { BasketID = basket.ID, ProductID = productID, Quantity = 1 };

                basket.BasketItems.Add(item);
            }
            else
            {
                ++item.Quantity;
            }

            basketContext.Commit();
        }

        public void RemoveFromBasket(HttpContextBase httpContext, string itemID)
        {
            Basket basket = GetBasket(httpContext, true); // make sure to create a basket for the user! (hence, true)
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ID == itemID);

            if (item != null)
            {
                basket.BasketItems.Remove(item);
                basketContext.Commit();
            }
        }
    }
}
