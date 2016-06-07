using System.Collections.Generic;
using SimpleRuntimeCache.Models;

namespace SimpleRuntimeCache.App.Repository
{
    public interface IItemRepo
    {
        IList<Item> GetAll();

        Item GetById(int id);

        bool Add(Item item);

        void Update(Item item);

        bool Delete(int id);
    }
}
