﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package
using System;
using System.Collections.Generic;
using LibreLancer.Ini;
namespace LibreLancer.Data.Missions
{
    public class MissionDialog : ICustomEntryHandler
    {
        [Entry("nickname")]
        public string Nickname;
        [Entry("system")]
        public string System;

        public List<DialogLine> Lines = new List<DialogLine>();

        private static readonly CustomEntry[] _custom = new CustomEntry[]
        {
            new("line", (s,e) => ((MissionDialog)s).HandleLine(e)),
        };

        IEnumerable<CustomEntry> ICustomEntryHandler.CustomEntries => _custom;
        void HandleLine(Entry e) => Lines.Add(new DialogLine() { Source = e[0].ToString(), Target = e[1].ToString(), Line = e[2].ToString() });
    }
    public class DialogLine
    {
        public string Source;
        public string Target;
        public string Line;
    }
}
