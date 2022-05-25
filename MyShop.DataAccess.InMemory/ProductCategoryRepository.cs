using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.InMemory
{
    public class ProductCategoryRepository
    {
        // Fields
        ObjectCache cache = MemoryCache.Default;
        List<ProductCategory> productCategories;

        // Constructors
        public ProductCategoryRepository()
        {
            productCategories = cache["ProductCategories"] as List<ProductCategory>;
            if (productCategories == null)
            {
                productCategories = new List<ProductCategory>();
            }
        }

        // When people add product to the repository, we don't want to save them straight away. Commit() saves the products list into the cache
        public void Commit()
        {
            cache["ProductCategories"] = productCategories;
        }

        // Add a product to the local repository
        public void Insert(ProductCategory p)
        {
            productCategories.Add(p);
        }

        // Change data about a Product Category
        // in1: Updated copy of an existing product
        // alg: Existing copy of product category is overwritten
        public void Update(ProductCategory productCategory)
        {
            ProductCategory productCategoryToUpdate = productCategories.Find(p => p.ID == productCategory.ID);

            // Product found
            if (productCategoryToUpdate != null)
            {
                productCategoryToUpdate = productCategory;
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        // Search for and return a product given an ID
        public ProductCategory Find(string ID)
        {
            ProductCategory productCategory = productCategories.Find(p => p.ID == ID);

            // Product found
            if (productCategory != null)
            {
                return productCategory;
            }
            else
            {
                throw new Exception("Product Category not found");
            }
        }

        // Removes a product from the repository given an ID
        public void Delete(string ID)
        {
            ProductCategory productCategoryToDelete = productCategories.Find(p => p.ID == ID);

            // Product found
            if (productCategoryToDelete != null)
            {
                productCategories.Remove(productCategoryToDelete);
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        public IQueryable<ProductCategory> Collection()
        {
            return productCategories.AsQueryable();
        }
    }
}