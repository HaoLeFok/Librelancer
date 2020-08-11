﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using LibreLancer.Thorn;
namespace LibreLancer
{
    [ThnEventRunner(EventTypes.StartPSysPropAnim)]
    public class StartPSysPropAnimRunner : IThnEventRunner
    {
        public void Process(ThnEvent ev, Cutscene cs)
        {
            if (!cs.Objects.ContainsKey((string) ev.Targets[0]))
            {
                FLLog.Error("Thn", $"Entity {ev.Targets[0]} does not exist");
                return;
            }
            var obj = cs.Objects[(string)ev.Targets[0]];
            var ren = ((ParticleEffectRenderer)obj.Object.RenderComponent);
            var props = (LuaTable)ev.Properties["psysprops"];
            if (props.Capacity == 0) return;
            var targetSparam = (float)props["sparam"];
            if (ev.Duration == 0)
            {
                ren.SParam = targetSparam;
            }
            else
            {
                cs.Coroutines.Add(new SParamAnimation()
                {
                    Renderer = ren,
                    StartSParam = ren.SParam,
                    EndSParam = targetSparam,
                    Duration = ev.Duration,
                    ParamCurve = ev.ParamCurve
                });
            }
        }

        class SParamAnimation : IThnRoutine
        {
            public ParticleEffectRenderer Renderer;
            public ParameterCurve ParamCurve;
            public float StartSParam;
            public float EndSParam;
            public double Duration;
            double time;
            public bool Run(Cutscene cs, double delta)
            {
                time += delta;
                if (time >= Duration)
                {
                    Renderer.SParam = EndSParam;
                    return false;
                }
                var pct = (float)(time / Duration);
                if (ParamCurve != null) pct = ParamCurve.GetValue((float) time, (float) Duration);
                Renderer.SParam = MathHelper.Lerp(StartSParam, EndSParam, pct);
                return true;
            }
        }
    }
}
