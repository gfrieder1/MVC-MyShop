using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using MyShop.Core;
using MyShop.Core.Models;

namespace MyShop.DataAccess.InMemory
{
    public class ProductRepository
    {
        // Fields
        ObjectCache cache = MemoryCache.Default;
        List<Product> products;

        // Constructors
        public ProductRepository()
        {
            products = cache["Products"] as List<Product>;
            if (products == null)
            {
                products = new List<Product>();
            }
        }

        // When people add product to the repository, we don't want to save them straight away. Commit() saves the products list into the cache
        public void Commit()
        {
            cache["Products"] = products;
        }

        // Add a product to the local repository
        public void Insert(Product p)
        {
            products.Add(p);
        }

        // Change data about a Product
        // in1: Updated copy of an existing product
        // alg: Existing copy of product is overwritten
        public void Update(Product product)
        {
            Product productToUpdate = products.Find(p => p.ID == product.ID);

            // Product found
            if (productToUpdate != null)
            {
                productToUpdate = product;
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        // Search for and return a product given an ID
        public Product Find(string ID)
        {
            Product product = products.Find(p => p.ID == ID);

            // Product found
            if (product != null)
            {
                return product;
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        // Removes a product from the repository given an ID
        public void Delete(string ID)
        {
            Product productToDelete = products.Find(p => p.ID == ID);

            // Product found
            if (productToDelete != null)
            {
                products.Remove(productToDelete);
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        public IQueryable<Product> Collection()
        {
            return products.AsQueryable();
        }
    }
}
