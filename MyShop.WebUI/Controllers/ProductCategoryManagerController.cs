using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.DataAccess.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductCategoryManagerController : Controller
    {
        IRepository<ProductCategory> context;

        public ProductCategoryManagerController(IRepository<ProductCategory> productCategoryContext)
        {
            context = productCategoryContext;
        }

        // GET: ProductManager
        public ActionResult Index()
        {
            List<ProductCategory> productCategories = context.Collection().ToList();
            return View(productCategories);
        }

        public ActionResult Create()
        {
            ProductCategory productCategory = new ProductCategory();
            return View(productCategory);
        }

        [HttpPost]
        public ActionResult Create(ProductCategory productCategory)
        {
            if (!ModelState.IsValid)
            {
                return View(productCategory);
            }

            context.Insert(productCategory);
            context.Commit();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(string ID)
        {
            ProductCategory productCategory = context.Find(ID);

            if (productCategory == null)
            {
                return HttpNotFound();
            }

            return View(productCategory);
        }

        [HttpPost]
        public ActionResult Edit(ProductCategory productCategory, string ID)
        {
            ProductCategory productCategoryToEdit = context.Find(ID);

            if (productCategoryToEdit == null)
            {
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(productCategory);
            }

            productCategoryToEdit.Category = productCategory.Category;
            //productCategoryToEdit.Description = productCategory.Description;
            //productCategoryToEdit.Image = productCategory.Image;
            //productCategoryToEdit.Name = productCategory.Name;
            //productCategoryToEdit.Price = productCategory.Price;

            context.Commit();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(string ID)
        {
            ProductCategory productCategoryToDelete = context.Find(ID);

            if (productCategoryToDelete == null)
            {
                return HttpNotFound();
            }

            return View(productCategoryToDelete);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string ID)
        {
            ProductCategory productCategoryToDelete = context.Find(ID);

            if (productCategoryToDelete == null)
            {
                return HttpNotFound();
            }

            context.Delete(ID);
            context.Commit();

            return RedirectToAction("Index");
        }
    }
}