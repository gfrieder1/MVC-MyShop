using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.DataAccess.InMemory;

namespace MyShop.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductManagerController : Controller
    {
        IRepository<Product> context;
        IRepository<ProductCategory> productCategories;

        public ProductManagerController(IRepository<Product> productContext, IRepository<ProductCategory> productCategoryContext)
        {
            context = productContext;
            productCategories = productCategoryContext;
        }

        // GET: ProductManager
        public ActionResult Index()
        {
            List<Product> products = context.Collection().ToList();
            return View(products);
        }

        public ActionResult Create()
        {
            ProductManagerViewModel viewModel = new ProductManagerViewModel();

            viewModel.Product = new Product();
            viewModel.ProductCategories = productCategories.Collection();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(Product product, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (file != null)
            {
                product.Image = product.ID + Path.GetExtension(file.FileName);
                file.SaveAs(Server.MapPath("//Content//ProductImages//") + product.Image);
            }

            context.Insert(product);
            context.Commit();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(string ID)
        {
            Product product = context.Find(ID);

            if (product == null)
            {
                return HttpNotFound();
            }

            ProductManagerViewModel viewModel = new ProductManagerViewModel();

            viewModel.Product = product;
            viewModel.ProductCategories = productCategories.Collection();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(Product product, string ID, HttpPostedFileBase file)
        {
            Product productToEdit = context.Find(ID);

            if (productToEdit == null)
            {
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (file != null)
            {
                productToEdit.Image = product.ID + Path.GetExtension(file.FileName);
                file.SaveAs(Server.MapPath("//Content//ProductImages//") + productToEdit.Image);
            }

            productToEdit.Category = product.Category;
            productToEdit.Description = product.Description;
            productToEdit.Name = product.Name;
            productToEdit.Price = product.Price;

            context.Commit();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(string ID)
        {
            Product productToDelete = context.Find(ID);

            if (productToDelete == null)
            {
                return HttpNotFound();
            }

            return View(productToDelete);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string ID)
        {
            Product productToDelete = context.Find(ID);

            if (productToDelete == null)
            {
                return HttpNotFound();
            }

            context.Delete(ID);
            context.Commit();

            return RedirectToAction("Index");
        }
    }
}