// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package
    
using System;
using System.Collections.Generic;
using LibreLancer.Ini;

namespace LibreLancer.Data.Pilots
{
    public class BuzzPassByBlock : PilotBlock, ICustomEntryHandler
    {
        [Entry("buzz_distance_to_pass_by")] public float DistanceToPassBy;
        [Entry("buzz_pass_by_time")] public float PassByTime;

        [Entry("buzz_break_direction_cone_angle")]
        public float BreakDirectionConeAngle;

        [Entry("buzz_break_turn_throttle")] public float BreakTurnThrottle;
        [Entry("buzz_pass_by_roll_throttle")] public float PassByRollThrottle;
        [Entry("buzz_drop_bomb_on_pass_by")] public bool DropBombOnPassBy;

        public List<DirectionWeight> BreakDirectionWeights = new List<DirectionWeight>();
        public List<BuzzPassByStyle> PassByStyleWeights = new List<BuzzPassByStyle>();
        
        private static readonly CustomEntry[] _custom = new CustomEntry[]
        {
            new(
                "buzz_pass_by_style_weight",
                (s, e) =>
                    ((BuzzPassByBlock) s).PassByStyleWeights.Add(new BuzzPassByStyle(e))),
            new("buzz_break_direction_weight",
                (s, e) => ((BuzzPassByBlock) s).BreakDirectionWeights.Add(new DirectionWeight(e)))

        };

        IEnumerable<CustomEntry> ICustomEntryHandler.CustomEntries => _custom;
    }

    public class BuzzPassByStyle
    {
        public string Style;
        public float Weight;

        public BuzzPassByStyle()
        {
        }

        public BuzzPassByStyle(Entry e)
        {
            Style = e[0].ToString();
            Weight = e[1].ToSingle();
        }
    }
}