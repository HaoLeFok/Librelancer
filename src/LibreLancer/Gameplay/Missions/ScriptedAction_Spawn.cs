// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Linq;
using System.Numerics;
using LibreLancer.AI;
using LibreLancer.Data.Missions;
using LibreLancer.Missions;

namespace LibreLancer.Gameplay.Missions
{
    public class Act_SpawnSolar : ScriptedAction
    {
        public string Solar;
        public Act_SpawnSolar(MissionAction act) : base(act)
        {
            Solar = act.Entry[0].ToString();
        }

        public override void Invoke(MissionRuntime runtime, MissionScript script)
        {
            var sol = script.Solars[Solar];
            var arch = sol.Archetype;
            runtime.Player.WorldAction(() => { runtime.Player.World.SpawnSolar(sol.Nickname, arch, sol.Loadout, sol.Position, sol.Orientation); });
        }
    }

    public abstract class ShipSpawnBase : ScriptedAction
    {
        protected ShipSpawnBase(MissionAction act) : base(act) { }

        protected void SpawnShip(string msnShip, Vector3? spawnpos, Quaternion? spawnorient, string objList, MissionScript script, MissionRuntime runtime)
        {
            var ship = script.Ships[msnShip];
            var npcDef = script.NPCs[ship.NPC];
            script.NpcShips.TryGetValue(npcDef.NpcShipArch, out var shipArch);
            foreach (var lbl in ship.Labels)
                runtime.LabelIncrement(lbl);
            if (shipArch == null)
            {
                shipArch = runtime.Player.Game.GameData.Ini.NPCShips.ShipArches.First(x =>
                    x.Nickname.Equals(npcDef.NpcShipArch, StringComparison.OrdinalIgnoreCase));
            }

            var pos = spawnpos ?? ship.Position;
            var orient = spawnorient ?? ship.Orientation;
            AiState state = null;
            if (!string.IsNullOrEmpty(objList))
            {
                state = script.ObjLists[objList].Construct();
            }
            
            runtime.Player.WorldAction(() =>
            {
                runtime.Player.World.Server.GameData.TryGetLoadout(shipArch.Loadout, out var ld);
                var pilot = runtime.Player.World.Server.GameData.GetPilot(shipArch.Pilot);
                var obj = runtime.Player.World.NPCs.DoSpawn(ship.Nickname, ld, pilot, pos, orient);
                var npcComp = obj.GetComponent<SNPCComponent>();
                npcComp.OnKilled = () => {
                    runtime.NpcKilled(msnShip);
                    foreach (var lbl in ship.Labels)
                        runtime.LabelKilled(lbl);
                };
                npcComp.SetState(state);
            });
        }
    }
    
    public class Act_SpawnFormation : ShipSpawnBase
    {
        public string Formation;
        public Vector3? Position;

        //TODO: implement formations
        private static Vector3[] formationOffsets = new Vector3[]
        {
            new Vector3(-60, 0, 0),
            new Vector3(60, 0, 0),
            new Vector3(0, -60, 0),
            new Vector3(0, 60, 0)
        };
        public Act_SpawnFormation(MissionAction act) : base(act)
        {
            Formation = act.Entry[0].ToString();
            if (act.Entry.Count > 1)
                Position = new Vector3(act.Entry[1].ToSingle(), act.Entry[2].ToSingle(),
                    act.Entry[3].ToSingle());
        }

        public override void Invoke(MissionRuntime runtime, MissionScript script)
        {
            var form = script.Formations[Formation];
            var fpos = Position ?? form.Position;
            SpawnShip(form.Ships[0], fpos, form.Orientation, null, script, runtime);
            var mat = Matrix4x4.CreateFromQuaternion(form.Orientation) *
                      Matrix4x4.CreateTranslation(fpos);
            int j = 0;
            for (int i = 1; i < form.Ships.Count; i++)
            {
                var pos = Vector3.Transform(formationOffsets[j++], mat);
                SpawnShip(form.Ships[i], pos, form.Orientation, null, script, runtime);
            }
        }
    }
    
    public class Act_SpawnShip : ShipSpawnBase
    {
        public string Ship;
        public string ObjList;
        public Vector3? Position;
        public Quaternion? Orientation;

        public Act_SpawnShip(MissionAction act) : base(act)
        {
            Ship = act.Entry[0].ToString();
            if (act.Entry.Count > 1)
            {
                ObjList = act.Entry[1].ToString();
            }
            if (act.Entry.Count > 2)
            {
                Position = new Vector3(act.Entry[2].ToSingle(), act.Entry[3].ToSingle(), act.Entry[4].ToSingle());
            }
            if (act.Entry.Count > 5)
            {
                Orientation = new Quaternion(act.Entry[6].ToSingle(), act.Entry[7].ToSingle(), act.Entry[8].ToSingle(),
                    act.Entry[5].ToSingle());
            }
        }
        public override void Invoke(MissionRuntime runtime, MissionScript script)
        {
            SpawnShip(Ship, Position, Orientation, ObjList, script, runtime);
        }
    }
    
    public class Act_Destroy : ScriptedAction
    {
        public string Target;

        public Act_Destroy(MissionAction act) : base(act)
        {
            Target = act.Entry[0].ToString();
        }

        public override void Invoke(MissionRuntime runtime, MissionScript script)
        {
            if (script.Ships.ContainsKey(Target))
            {
                var ship = script.Ships[Target];
                var npcDef = script.NPCs[ship.NPC];
                script.NpcShips.TryGetValue(npcDef.NpcShipArch, out var shipArch);
                foreach (var lbl in ship.Labels)
                    runtime.LabelDecrement(lbl);
                runtime.Player.WorldAction(() => { runtime.Player.World.NPCs.Despawn(runtime.Player.World.GameWorld.GetObject(Target)); });
            }
        }
    }
}