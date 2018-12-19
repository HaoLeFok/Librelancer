﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using LibreLancer.Ini;
namespace LibreLancer.Compatibility.GameData.Equipment
{
    public class Gun : AbstractEquipment
    {
        [Entry("da_archetype")]
        public string DaArchetype;
        [Entry("material_library")]
        public string MaterialLibrary;
        [Entry("ids_name")]
        public int IdsName;
        [Entry("ids_info")]
        public int IdsInfo;
        [Entry("hit_pts")]
        public int Hitpoints;
        [Entry("turn_rate")]
        public float TurnRate;
    }
}
