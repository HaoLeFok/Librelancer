// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using LibreLancer.Ini;

namespace LibreLancer.Data.Pilots
{
    public class EvadeDodgeBlock : PilotBlock, ICustomEntryHandler
    {
        [Entry("evade_dodge_cone_angle")] public float DodgeConeAngle;
        [Entry("evade_dodge_interval_time")] public float DodgeIntervalTime;
        [Entry("evade_dodge_time")] public float DodgeTime;
        [Entry("evade_dodge_distance")] public float DodgeDistance;
        [Entry("evade_activate_range")] public float ActivateRange;
        [Entry("evade_dodge_roll_angle")] public float RollAngle;
        [Entry("evade_dodge_waggle_axis_cone_angle")]
        public float DodgeWaggleAxisConeAngle;
        [Entry("evade_dodge_slide_throttle")]
        public float DodgeSlideThrottle;
        [Entry("evade_dodge_turn_throttle")] public float DodgeTurnThrottle;
        [Entry("evade_dodge_corkscrew_turn_throttle")]
        public float DodgeCorkscrewTurnThrottle;
        [Entry("evade_dodge_corkscrew_roll_throttle")]
        public float DodgeCorkscrewRollThrottle;
        [Entry("evade_dodge_corkscrew_roll_flip_direction")]
        public float DodgeCorkscrewRollFlipDirection;
        [Entry("evade_dodge_interval_time_variance_percent")]
        public float DodgeIntervalTimeVariancePercent;
        [Entry("evade_dodge_cone_angle_variance_percent")]
        public float DodgeConeAngleVariancePercent;
        public List<DodgeStyle> DodgeStyleWeights = new List<DodgeStyle>();
        public List<DirectionWeight> DodgeDirectionWeights = new List<DirectionWeight>();

        private static readonly CustomEntry[] _custom = new CustomEntry[]
        {
            new(
                "evade_dodge_style_weight",
                (s, e) =>
                    ((EvadeDodgeBlock) s).DodgeStyleWeights.Add(new DodgeStyle(e))),
            new("evade_dodge_direction_weight",
                (s, e) => ((EvadeDodgeBlock) s).DodgeDirectionWeights.Add(new DirectionWeight(e)))

        };

        IEnumerable<CustomEntry> ICustomEntryHandler.CustomEntries => _custom;

    }

    public class DodgeStyle
    {
        public string Style;
        public float Weight;

        public DodgeStyle()
        {
        }

        public DodgeStyle(Entry e)
        {
            Style = e[0].ToString();
            Weight = e[1].ToSingle();
        }
    }
}