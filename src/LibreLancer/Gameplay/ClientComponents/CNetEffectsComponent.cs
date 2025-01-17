// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System.Collections.Generic;

namespace LibreLancer
{
    public class CNetEffectsComponent : GameComponent
    {
        private SpawnedEffect[] effects = new SpawnedEffect[0];
        
        public CNetEffectsComponent(GameObject parent) : base(parent)
        {
        }

        private int renIndex = 0;

        private List<AttachedEffect> spawned = new List<AttachedEffect>();
        
        void Spawn(SpawnedEffect effect)
        {
            var fx = Parent.World.Renderer.Game.GetService<GameDataManager>().GetEffect(effect.Effect);
            if (fx == null) return;
            var pfx = fx.GetEffect(Parent.World.Renderer.ResourceManager);
            if (pfx == null) return;
            foreach (var fxhp in effect.Hardpoints)
            {
                var hp = Parent.GetHardpoint(fxhp);
                var fxobj = new AttachedEffect(hp,
                    new ParticleEffectRenderer(pfx) {Index = renIndex++});
                Parent.ExtraRenderers.Add(fxobj.Effect);
                spawned.Add(fxobj);
            }
        }

        public override void Update(double time)
        {
            foreach(var fx in spawned)
                fx.Update(Parent, time, 0);
        }

        public void UpdateEffects(SpawnedEffect[] fx)
        {
            foreach (var f in fx)
            {
                bool found = false;
                foreach (var f2 in effects) {
                    if (f2.ID == f.ID)
                    {
                        found = true;
                        break;
                    }
                }
                if(!found) Spawn(f);
            }
            effects = fx;
        }
    }
}