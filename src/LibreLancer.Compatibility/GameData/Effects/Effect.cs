﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using LibreLancer.Ini;
namespace LibreLancer.Compatibility.GameData.Effects
{
	public class Effect
	{
        [Entry("nickname")]
		public string Nickname;
        [Entry("vis_effect")]
		public string VisEffect;
        [Entry("snd_effect")]
        public string SndEffect;
        [Entry("type")]
        public string Type;
	}
}
