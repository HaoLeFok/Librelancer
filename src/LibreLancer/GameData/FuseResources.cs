﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package
    
using System;
using System.Collections.Generic;
using LibreLancer.Fx;

namespace LibreLancer.GameData
{
    public class FuseResources
    {
        public Dictionary<string, ParticleEffect> Fx = new Dictionary<string, ParticleEffect>(StringComparer.OrdinalIgnoreCase);
        public Data.Fuses.Fuse Fuse;
    }
}
