// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using LibreLancer.GameData;
using LibreLancer.Net;
using MoonSharp.Interpreter;

namespace LibreLancer.Interface
{
    [MoonSharpUserData]

    public class UISoldShip
    {
        public int IdsName;
        public int IdsInfo;
        public int ShipClass;
        public string Icon;
        public string Model;
        public double Price;
        [MoonSharpHidden] 
        public NetSoldShip Server;
        [MoonSharpHidden]
        public Ship Ship;
    }
}