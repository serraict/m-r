﻿using System;
using System.Dynamic;
using System.Web.Mvc;
using CQRSGui.Infra;
using SimpleCQRS;

namespace CQRSGui.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private FakeBus _bus;
        private IReadModelFacade _readmodel;

        public HomeController()
        {
            _bus = ServiceLocator.Bus;
            _readmodel = new MongoReadModelFacade();
        }

        public ActionResult Index()
        {
            ViewData.Model = _readmodel.GetInventoryItems();

            return View();
        }

        public ActionResult Details(Guid id)
        {
            ViewData.Model = _readmodel.GetInventoryItemDetails(id);
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string name)
        {
            _bus.Send(new CreateInventoryItem(Guid.NewGuid(), name));

            return RedirectToAction("Index");
        }

        public ActionResult ChangeName(Guid id)
        {
            ViewData.Model = _readmodel.GetInventoryItemDetails(id);
            return View();
        }

        [HttpPost]
        public ActionResult ChangeName(Guid id, string name, int version)
        {
            var command = new RenameInventoryItem(id, name, version);
            _bus.Send(command);

            return RedirectToAction("Index");
        }

        public ActionResult Deactivate(Guid id, int version)
        {
            _bus.Send(new DeactivateInventoryItem(id, version));
            return RedirectToAction("Index");
        }

        public ActionResult CheckIn(Guid id)
        {
            ViewData.Model = _readmodel.GetInventoryItemDetails(id);
            return View();
        }

        [HttpPost]
        public ActionResult CheckIn(Guid id, int number, int version)
        {
            var model = _readmodel.GetInventoryItemDetails(id);
            ValidateForCheckIn(model, number);

            if (!ModelState.IsValid)
            {
                ViewData.Model = model;
                return View();
            }

            _bus.Send(new CheckInItemsToInventory(id, number, version));
            return RedirectToAction("Index");
        }

        public ActionResult Remove(Guid id)
        {
            ViewData.Model = _readmodel.GetInventoryItemDetails(id);
            return View();
        }

        [HttpPost]
        public ActionResult Remove(Guid id, int number, int version)
        {
            var model = _readmodel.GetInventoryItemDetails(id);
            ValidateForRemoval(model, number);

            if(!ModelState.IsValid)
            {
                ViewData.Model = model;
                return View();
            }

            _bus.Send(new RemoveItemsFromInventory(id, number, version));
            return RedirectToAction("Index");
        }

        private void ValidateForRemoval(InventoryItemDetailsDto item, int numberToRemove)
        {
            if(numberToRemove <= 0 )
                ModelState.AddModelError("Number", "Number should be greater than 0.");
            if(numberToRemove > item.CurrentCount)
                ModelState.AddModelError("Number", "You cannot check out more items than currently are in stock.");
        }

        private void ValidateForCheckIn(InventoryItemDetailsDto model, int numberToCheckIn)
        {
            if (numberToCheckIn <= 0)
                ModelState.AddModelError("Number", "Number should be greater than 0.");
        }

    }

    public class RebuildReadModelModel
    {
        public int ToVersion { get; set; }
    }

}
