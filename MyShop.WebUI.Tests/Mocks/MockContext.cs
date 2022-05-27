using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.WebUI.Tests.Mocks
{
    public class MockContext<T> : IRepository<T> where T : BaseEntity
    {
        List<T> items;
        String className;

        public MockContext()
        {
            items = new List<T>();
        }

        public void Commit()
        {
            return;
        }

        public void Insert(T t)
        {
            items.Add(t);
        }

        public void Update(T t)
        {
            T tToUpdate = items.Find(i => i.ID == t.ID);

            if (tToUpdate != null)
            {
                tToUpdate = t;
            }
            else
            {
                throw new Exception(className + " not found");
            }
        }

        public T Find(string ID)
        {
            T t = items.Find(i => i.ID == ID);

            if (t != null)
            {
                return t;
            }

            throw new Exception(className + " not found");
        }

        public void Delete(string ID)
        {
            T tToDelete = items.Find(i => i.ID == ID);

            if (tToDelete != null)
            {
                items.Remove(tToDelete);
            }

            throw new Exception(className + " not found");
        }

        public IQueryable<T> Collection()
        {
            return items.AsQueryable();
        }
    }
}