// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System.Linq;

namespace LibreLancer
{
    public class SHealthComponent : GameComponent
    {
        public float MaxHealth { get; set; }
        public float CurrentHealth { get; set; }
        
        public float ShieldHealth { get; set; }
        
        public bool Invulnerable { get; set; }
        
        public SHealthComponent(GameObject parent) : base(parent) { }

        public override void FixedUpdate(double time)
        {
            var shield = Parent.GetChildComponents<SShieldComponent>().FirstOrDefault();
            if (shield != null)
            {
                if (shield.Health <= 0)
                    ShieldHealth = shield.Health;
                else
                    ShieldHealth = shield.Health / shield.Equip.Def.MaxCapacity;
            } else {
                ShieldHealth = -1;
            }
        }

        public void Damage(float amount)
        {
            var shield = Parent.GetChildComponents<SShieldComponent>().FirstOrDefault();
            if (shield == null || !shield.Damage(amount))
            {
                CurrentHealth -= amount;
                if (Invulnerable && CurrentHealth < (MaxHealth * 0.09f)) {
                    CurrentHealth = MaxHealth * 0.09f;
                }
                if (CurrentHealth <= 0) {
                    CurrentHealth = 0;
                    if (Parent.TryGetComponent<SNPCComponent>(out var npc)) {
                        npc.Killed(); 
                    }
                    if (Parent.TryGetComponent<SPlayerComponent>(out var player)) {
                        player.Killed();
                    }
                }
            }
        }
    }
}