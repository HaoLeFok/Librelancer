// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using LibreLancer;
using LibreLancer.Interface;
using System.Collections.Generic;
using System.Numerics;
using LibreLancer.Infocards;
using LibreLancer.Net;
using MoonSharp.Interpreter;

namespace InterfaceEdit
{
    public class TestServerList : ITableData
    {
        public int Count => 5;
        public int Selected { get; set; } = -1;
        public string GetContentString(int row, string column)
        {
            return "A";
        }

        public string CurrentDescription()
        {
            if (Selected < 0) return "";
            return "Server Description";
        }

        public bool ValidSelection()
        {
            return Selected > -1;
        }
    }

    public class TestSaveGameList : ITableData
    {
        public int Count => 5;
        public int Selected { get; set; } = -1;

        static string FlTime(DateTime time)
        {
            return $"{time.ToShortDateString()} {time:HH:mm}";
        }
        public string GetContentString(int row, string column)
        {
            if (column == "name")
                return ((char) ('a' + row)).ToString();
            else if (column == "date")
                return FlTime(new DateTime(2019, 7, 3 + row, 12, 05, 10 + row));
            else
                return "n/a";
        }

        public string CurrentDescription()
        {
            if (Selected < 0) return "";
            return ((char) ('a' + Selected)).ToString();
        }

        public bool ValidSelection()
        {
            return Selected > -1;
        }
    }
    public class TestCharacterList : ITableData
    {
        public int Count => 5;
        public int Selected { get; set; } = -1;
        public string GetContentString(int row, string column)
        {
            return "A";
        }

        public string ServerName = "Server";
        public string ServerDescription = "DESCRIPTION";
        public string ServerNews = "Placeholder News";

        public bool ValidSelection()
        {
            return Selected > -1;
        }
    }

    public class TestingApi
    {
        static TestingApi()
        {
            LuaContext.RegisterType<TestingApi>();
            LuaContext.RegisterType<TestServerList>();
            LuaContext.RegisterType<TestCharacterList>();
            LuaContext.RegisterType<TestSaveGameList>();
            LuaContext.RegisterType<FakeShipDealer>();
        }

        static readonly NavbarButtonInfo cityscape = new NavbarButtonInfo("IDS_HOTSPOT_EXIT", "Cityscape");
        static readonly NavbarButtonInfo bar = new NavbarButtonInfo("IDS_HOTSPOT_BAR", "Bar");
        static readonly NavbarButtonInfo trader = new NavbarButtonInfo("IDS_HOTSPOT_COMMODITYTRADER_ROOM", "Trader");
        static readonly NavbarButtonInfo equip = new NavbarButtonInfo("IDS_HOTSPOT_EQUIPMENTDEALER_ROOM", "Equipment");
        static readonly NavbarButtonInfo shipDealer = new NavbarButtonInfo("IDS_HOTSPOT_SHIPDEALER_ROOM", "ShipDealer");

        public bool HasBar = true;
        public bool HasTrader = true;
        public bool HasEquip = true;
        public bool HasShipDealer = true;

        public bool HasLaunchAction = true;
        public bool HasRepairAction = false;
        public bool HasMissionVendor = false;
        public bool HasNewsAction = false;
        public bool HasCommodityTraderAction = false;
        public bool HasEquipmentDealerAction = false;
        public bool HasShipDealerAction = false;

        public int ActiveHotspotIndex = 0;

        TestServerList serverList = new TestServerList();
        public TestServerList ServerList() => serverList;

        private TestSaveGameList _testSaveGames = new TestSaveGameList();

        public TestSaveGameList SaveGames() => _testSaveGames;

        private GameSettings settings = new GameSettings();

        public GameSettings GetCurrentSettings() => settings.MakeCopy();

        public void ApplySettings(GameSettings settings)
        {
            this.settings = settings;
        }

        public void LoadSelectedGame()
        {
        }

        public void Resume()
        {
        }

        public void QuitToMenu()
        {
        }

        public void DeleteSelectedGame()
        {
        }

        public Infocard _Infocard;

        public Infocard CurrentInfocard() => _Infocard;
        public string CurrentInfoString() => "CURRENT INFORMATION";


        public void ConnectSelection()
        {
        }

        public void StartNetworking()
        {
        }

        public bool ConnectAddress(string address)
        {
            return false;
        }

        public void StopNetworking()
        {
        }

        public int CruiseCharge() => 25;

        public void PopulateNavmap(Navmap nav)
        {
        }

        public NavbarButtonInfo[] GetNavbarButtons()
        {
            var l = new List<NavbarButtonInfo>();
            l.Add(cityscape);
            if (HasBar) l.Add(bar);
            if (HasTrader) l.Add(trader);
            if (HasEquip) l.Add(equip);
            if (HasShipDealer) l.Add(shipDealer);
            return l.ToArray();
        }

        private NewsArticle[] articles = new[]
        {
            new NewsArticle()
                {Icon = "critical", Logo = "news_scene2", Category = 15001, Headline = 15001, Text = 15002},
            new NewsArticle()
                {Icon = "world", Logo = "news_schultzsky", Category = 15003, Headline = 15003, Text = 15004},
            new NewsArticle()
                {Icon = "world", Logo = "news_manhattan", Category = 15009, Headline = 15009, Text = 15010},
            new NewsArticle()
                {Icon = "system", Logo = "news_cambridge", Category = 56152, Headline = 56152, Text = 56153},
            new NewsArticle() {Icon = "world", Logo = "news_leeds", Category = 56162, Headline = 56162, Text = 56163},
            new NewsArticle() {Icon = "system", Logo = "news_leeds", Category = 56166, Headline = 56166, Text = 56167},
            new NewsArticle()
                {Icon = "world", Logo = "news_newtokyo", Category = 56180, Headline = 56180, Text = 56181},
        };

        public NewsArticle[] GetNewsArticles() => articles;

        public NavbarButtonInfo[] GetActionButtons()
        {
            var l = new List<NavbarButtonInfo>();
            if (HasLaunchAction) l.Add(new NavbarButtonInfo("Launch", "IDS_HOTSPOT_LAUNCH"));
            if (HasRepairAction) l.Add(new NavbarButtonInfo("Repair", "IDS_NN_REPAIR_YOUR_SHIP"));
            if (HasMissionVendor) l.Add(new NavbarButtonInfo("MissionVendor", "IDS_HOTSPOT_MISSIONVENDOR"));
            if (HasNewsAction) l.Add(new NavbarButtonInfo("NewsVendor", "IDS_HOTSPOT_NEWSVENDOR"));
            if (HasCommodityTraderAction) l.Add(new NavbarButtonInfo("CommodityTrader", "IDS_HOTSPOT_COMMODITYTRADER"));
            if (HasEquipmentDealerAction) l.Add(new NavbarButtonInfo("EquipmentDealer", "IDS_HOTSPOT_EQUIPMENTDEALER"));
            if (HasShipDealerAction) l.Add(new NavbarButtonInfo("ShipDealer", "IDS_HOTSPOT_SHIPDEALER"));
            return l.ToArray();
        }

        public Maneuver[] ManeuverData;
        public Maneuver[] GetManeuvers() => ManeuverData;
        public string GetActiveManeuver() => "FreeFlight";

        private ChatSource chats = new ChatSource();

        public ChatSource GetChats() => chats;

        public double GetCredits() => 10000;

        public LuaCompatibleDictionary GetManeuversEnabled()
        {
            var dict = new LuaCompatibleDictionary();
            dict.Set("FreeFlight", true);
            dict.Set("Goto", true);
            dict.Set("Dock", true);
            dict.Set("Formation", false);
            return dict;
        }

        public float GetPlayerHealth() => 0.75f;
        public float GetPlayerShield() => 0.8f;

        public float GetPlayerPower() => 1f;

        public class TraderFake
        {
            public static UIInventoryItem[] pitems = new[]
            {
                new UIInventoryItem()
                {
                    Hardpoint = "HpWeapon01",
                    IdsHardpoint = 1526,
                    IdsHardpointDescription = 907,
                    MountIcon = true,
                    Icon = @"equipment\models\commodities\nn_icons\EQUIPICON_gun.3db",
                    IdsName = 263175,
                    IdsInfo = 264175
                },
                new UIInventoryItem()
                {
                    Hardpoint = "HpWeapon02",
                    IdsHardpoint = 1527,
                    IdsHardpointDescription = 907
                },
                new UIInventoryItem()
                {
                    Icon = @"Equipment\models\commodities\nn_icons\COMMOD_chemicals.3db",
                    Price = 240,
                    PriceRank = "good",
                    IdsName = 261626,
                    IdsInfo = 65908,
                    Combinable = true,
                    Count = 32,
                },
                new UIInventoryItem()
                {
                    Icon = @"Equipment\models\commodities\nn_icons\COMMOD_metals.3db",
                    Price = 40,
                    PriceRank = "bad",
                    IdsName = 261627,
                    IdsInfo = 65908,
                    Combinable = true,
                    Count = 1
                },
                new UIInventoryItem()
                {
                    Icon = @"equipment\models\commodities\nn_icons\EQUIPICON_gun.3db",
                    Price = 1000,
                    IdsName = 263175,
                    IdsInfo = 264175,
                    Combinable = false,
                    Count = 1,
                    MountIcon = true,
                    CanMount = true
                },
                new UIInventoryItem()
                {
                    Icon = @"equipment\models\commodities\nn_icons\EQUIPICON_gun.3db",
                    Price = 2000,
                    IdsName = 263177,
                    IdsInfo = 264177,
                    Combinable = false,
                    Count = 1,
                    MountIcon = true,
                    CanMount = false
                },


            };

            public static UIInventoryItem[] titems = new[]
            {
                new UIInventoryItem()
                {
                    Icon = @"Equipment\models\commodities\nn_icons\COMMOD_chemicals.3db",
                    Price = 240,
                    PriceRank = "neutral",
                    IdsName = 261626, //mox
                    IdsInfo = 65908,
                    Combinable = true,
                    Count = 0,
                },
                new UIInventoryItem()
                {
                    Icon = @"Equipment\models\commodities\nn_icons\COMMOD_metals.3db",
                    Price = 20000,
                    PriceRank = "bad",
                    IdsName = 261627, //basic alloy
                    IdsInfo = 65885,
                    Combinable = true,
                    Count = 0
                }
            };

            public UIInventoryItem[] GetPlayerGoods(string filter) => pitems;
            public UIInventoryItem[] GetTraderGoods(string filter) => titems;

            public void Buy(string good, int count, Closure onSuccess)
            {
            }

            public void Sell(UIInventoryItem item, int count, Closure onSuccess)
            {
                onSuccess.Call();
            }

            public void OnUpdateInventory(Closure handler)
            {
            }

            public void ProcessMount(UIInventoryItem item, Closure onsuccess)
            {
                onsuccess.Call("mount");
            }
        }

       

        public TraderFake Trader = new TraderFake();
        public FakeShipDealer ShipDealer = new FakeShipDealer();

        public string SelectionName() => "Selected Object";
        public bool SelectionVisible() => true;

        public float SelectionHealth() => 0.5f;
        public float SelectionShield() => 0.75f;
        public Vector2 SelectionPosition() => new Vector2(300,300);

        public int ThrustPercent() => 111;
        public int Speed() => 67;
        public void HotspotPressed(string hotspot)
        {
            
        }

        public string ActiveNavbarButton()
        {
            var btns = GetNavbarButtons();
            if (ActiveHotspotIndex >= btns.Length) return btns[0].IDS;
            return btns[ActiveHotspotIndex].IDS;
        }

        public bool IsMultiplayer() => false;

        public void NewGame() { }

        public void Exit() { }

        private TestCharacterList clist = new TestCharacterList();
        public TestCharacterList CharacterList()
        {
            return clist;
        }

        private MainWindow win;
        public TestingApi(MainWindow win)
        {
            this.win = win;
            this.settings.RenderContext = win.RenderContext;
        }
        
        public void RequestNewCharacter()
        {
            win.UiEvent("OpenNewCharacter");
        }
        
        public void LoadCharacter(){}
        public void DeleteCharacter() { }

        public void NewCharacter(string name, int index) { }
    }

    [MoonSharpUserData]
    public class FakeShipDealer
    {
        public UISoldShip[] SoldShips() => new[]{
            new UISoldShip()
            {
                IdsName = 237051,
                IdsInfo = 66598,
                Price = 172000,
                Model = @"DATA\ships\rheinland\rh_elite\rh_elite.cmp",
                Icon = @"DATA\Equipment\models\commodities\nn_icons\rh_elite.3db",
                ShipClass = 1,
            },
            new UISoldShip()
            {
                IdsName = 237033,
                IdsInfo = 66567,
                Price = 10400,
                Model = @"DATA\ships\liberty\li_elite\li_elite.cmp",
                Icon = @"DATA\Equipment\models\commodities\nn_icons\li_elite.3db",
                ShipClass = 4,
            }
        };

        public UISoldShip PlayerShip() => new UISoldShip()
        {
            IdsName = 237015,
            IdsInfo = 66527,
            Price = 4800,
            Model = @"DATA\ships\civilian\cv_starflier\cv_starflier.cmp",
            Icon = @"DATA\Equipment\models\commodities\nn_icons\cv_starflier.3db",
        };

        public void StartPurchase(UISoldShip ship, Closure callback)
        {
            
        }

        public UIInventoryItem[] GetPlayerGoods(string filter) => TestingApi.TraderFake.pitems;
        public UIInventoryItem[] GetDealerGoods(string filter) => TestingApi.TraderFake.titems;
    }
}