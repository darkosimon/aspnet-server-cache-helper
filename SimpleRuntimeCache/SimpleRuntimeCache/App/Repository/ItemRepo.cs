using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimpleRuntimeCache.App.CacheHelper;
using SimpleRuntimeCache.Models;

namespace SimpleRuntimeCache.App.Repository
{
    public class ItemRepo : IItemRepo
    {
        private readonly IList<Item> _items;
        private readonly string cacheKey = "itemsKey";

        public ItemRepo(IList<Item> items)
        {
            _items = items;
        }

        public bool Add(Item item)
        {
            try
            {
                CacheWrapper.Invalidate(() =>
                {
                    _items.Add(item);
                }, cacheKey);
            }
            catch (Exception e)
            {
                //log e
                return false;
            }

            return true;
        }

        public bool Delete(int id)
        {
            try
            {
                CacheWrapper.Invalidate(() =>
                {
                    Item found = _items.FirstOrDefault(i => i.Id.Equals(id));
                    if (null != found)
                    {
                        _items.Remove(found);
                    }
                }, cacheKey);
            }
            catch (Exception e)
            {
                //log e
                return false;
            }

            return true;
        }

        public IList<Item> GetAll()
        {
            IList<Item> retVal;

            if (CacheWrapper.Exists(cacheKey))
            {
                CacheWrapper.Get(cacheKey, out retVal);
            }
            else
            {
                retVal = _items;
                CacheWrapper.Add(retVal, cacheKey, DateTimeOffset.UtcNow.AddMinutes(1));
            }

            return retVal;
        }

        public Item GetById(int id)
        {
            IList<Item> items;

            if (CacheWrapper.Exists(cacheKey))
            {
                CacheWrapper.Get(cacheKey, out items);
            }
            else
            {
                items = _items;
            }

            return items.FirstOrDefault(i => i.Id.Equals(id));
        }

        public void Update(Item item)
        {
            CacheWrapper.Invalidate(() =>
            {
                Item found = _items.FirstOrDefault(i => i.Id.Equals(item.Id));
                if (null != found)
                {
                    found.ItemName = item.ItemName;
                    found.Quantity = item.Quantity;
                }

            }, cacheKey);
        }
    }
}