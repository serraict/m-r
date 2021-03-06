﻿using System;
using System.ComponentModel;

namespace SimpleCQRS.Commands
{
    public class InventoryCommandHandlers
    {
        private readonly IRepository<InventoryItem> _repository;

        public InventoryCommandHandlers(IRepository<InventoryItem> repository)
        {
            _repository = repository;
        }

        public void HandleCommand(Command command)
        {
            this.AsDynamic().Handle(command);
        }

        private void Handle(CreateInventoryItem message)
        {
            var item = new InventoryItem(message.InventoryItemId, message.Name);
            _repository.Save(item, -1);
        }
        private void Handle(DeactivateInventoryItem message)
        {
            var item = _repository.GetById(message.InventoryItemId);
            item.Deactivate();
            _repository.Save(item, message.OriginalVersion);
        }
        private void Handle(RemoveItemsFromInventory message)
        {
            var item = _repository.GetById(message.InventoryItemId);
            item.Remove(message.Count);
            _repository.Save(item, message.OriginalVersion);
        }
        private void Handle(CheckInItemsToInventory message)
        {
            var item = _repository.GetById(message.InventoryItemId);
            item.CheckIn(message.Count);
            _repository.Save(item, message.OriginalVersion);
        }
        private void Handle(RenameInventoryItem message)
        {
            var item = _repository.GetById(message.InventoryItemId);
            item.ChangeName(message.NewName);
            _repository.Save(item, message.OriginalVersion);
        }
    }

}
