using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleRuntimeCache.App.Repository;
using SimpleRuntimeCache.Models;

namespace SimpleRuntimeCache.Controllers
{
    public class HomeController : Controller
    {
        private readonly IItemRepo _itemRepo;

        public static IList<Item> Items { get; } = new List<Item>()
        {
            new Item() {Id = 1, ItemName = "Orange", Quantity = 3},
            new Item() {Id = 2, ItemName = "Lemon", Quantity = 56},
            new Item() {Id = 3, ItemName = "Banana", Quantity = 8},
            new Item() {Id = 4, ItemName = "Mango", Quantity = 19},
            new Item() {Id = 5, ItemName = "Watermelon", Quantity = 21},
        };

        public HomeController()
        {
            _itemRepo = new ItemRepo(Items);
        }

        public ActionResult Index()
        {
            IList<Item> items = _itemRepo.GetAll();

            return View(items);
        }

        public ActionResult Edit(int id)
        {
            Item item = _itemRepo.GetById(id);

            if (null != item)
            {
                item.ItemName += " (u)";
                item.Quantity += 1;
                _itemRepo.Update(item);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            bool success =_itemRepo.Delete(id);
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            int newId = _itemRepo.GetAll().Max(i=> i.Id) + 1;

            Item newItem = new Item();
            newItem.Id = newId;
            newItem.ItemName = " New:" + newId;
            newItem.Quantity = 1;

            _itemRepo.Add(newItem);

            return RedirectToAction("Index");
        }
    }
}