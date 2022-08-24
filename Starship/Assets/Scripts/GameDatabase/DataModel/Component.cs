﻿using System.Linq;
using GameDatabase.Enums;
using GameDatabase.Model;
using GameDatabase.Serializable;

namespace GameDatabase.DataModel
{
    public partial class Component
    {
        public static Component Empty = new Component();

        partial void OnDataDeserialized(ComponentSerializable serializable, Database.Loader loader)
        {
            CellType = string.IsNullOrEmpty(_cellType) ? CellType.Empty : (CellType)_cellType.First();
            WeaponSlotType = string.IsNullOrEmpty(_weaponSlotType) ? WeaponSlotType.Default : (WeaponSlotType)_weaponSlotType.First();
        }

        private Component() { Id = ItemId<Component>.Empty; }

        public CellType CellType { get; private set; }
        public WeaponSlotType WeaponSlotType { get; private set; }
    }
}
