﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Numerics;
using System.Collections.Generic;
using LibreLancer.GameData;
using LibreLancer.Physics;
namespace LibreLancer
{
    public class GameWorld : IDisposable
	{
		public PhysicsWorld Physics;
		public SystemRenderer Renderer;
        public ProjectileManager Projectiles;

        public ServerWorld Server;

		private List<GameObject> objects = new List<GameObject>();

        public IReadOnlyList<GameObject> Objects => objects;
        
		public delegate void RenderUpdateHandler(double delta);
		public event RenderUpdateHandler RenderUpdate;
		public delegate void PhysicsUpdateHandler(double delta);
		public event PhysicsUpdateHandler PhysicsUpdate;

        public SpatialLookup SpatialLookup = new SpatialLookup();

		public GameWorld(SystemRenderer render, bool initPhys = true)
		{
            if(initPhys)
            Physics = new PhysicsWorld();
            if (render != null)
            {
                Renderer = render;
                render.World = this;
                if (initPhys)
                {
                    Renderer.PhysicsHook = () => { Physics.DrawWorld(); };
                }
            }
            if(initPhys)
            Physics.FixedUpdate += FixedUpdate;
            Projectiles = new ProjectileManager(this);
		}

        public void LoadSystem(StarSystem sys, ResourceManager res, bool server, double timeOffset = 0)
		{
            foreach (var g in objects)
                g.Unregister(Physics);

            if(Renderer != null) Renderer.StarSystem = sys;
           
            objects = new List<GameObject>();
            if(Renderer != null) AddObject((new GameObject() { Nickname = "projectiles", RenderComponent = new ProjectileRenderer(Projectiles) }));

            foreach (var obj in sys.Objects)
            {
                var g = new GameObject(obj.Archetype, res, Renderer != null);
                g.Name = obj.DisplayName;
                g.Nickname = obj.Nickname;
                g.SetLocalTransform((obj.Rotation ?? Matrix4x4.Identity) * Matrix4x4.CreateTranslation(obj.Position));
                g.SetLoadout(obj.Loadout, obj.LoadoutNoHardpoint, timeOffset);
                g.World = this;
                g.CollisionGroups = obj.Archetype.CollisionGroups;
                if (g.RenderComponent != null)
                {
                    g.RenderComponent.LODRanges = obj.Archetype.LODRanges;
                    if (g.RenderComponent is ModelRenderer && obj.Spin != Vector3.Zero) {
                        g.RenderComponent.Spin = obj.Spin;
                    }
                }
                if (obj.Dock != null)
                {
                    if (obj.Archetype.DockSpheres.Count > 0) //Dock with no DockSphere?
                    {
                        if (server)
                        {
                            g.Components.Add(new SDockableComponent(g)
                            {
                                Action = obj.Dock,
                                DockSpheres = obj.Archetype.DockSpheres.ToArray()
                            });
                        }
                        g.Components.Add(new CDockComponent(g)
                        {
                            Action = obj.Dock,
                            DockAnimation = obj.Archetype.DockSpheres[0].Script,
                            DockHardpoint = obj.Archetype.DockSpheres[0].Hardpoint,
                            TriggerRadius = obj.Archetype.DockSpheres[0].Radius
                        });
                    }
                }
                g.Register(Physics);
                AddObject(g);
            }
            foreach (var field in sys.AsteroidFields)
            {
                var g = new GameObject();
                g.Resources = res;
                g.World = this;
                g.Components.Add(new CAsteroidFieldComponent(field, g));
                AddObject(g);
                g.Register(Physics);
            }
            GC.Collect();
        }
#if DEBUG
        public List<Vector3> DebugPoints = new List<Vector3>();
        public bool RenderDebugPoints = false;
        public void DrawDebug(Vector3 point)
        {
            if(RenderDebugPoints)
                DebugPoints.Add(point);
        }
        #else
        public void DrawDebug(Vector3 point) {}
#endif

        public void AddObject(GameObject obj)
        {
            objects.Add(obj);
            SpatialLookup.AddObject(obj, Vector3.Transform(Vector3.Zero, obj.WorldTransform));
        }

        public void RemoveObject(GameObject obj)
        {
            objects.Remove(obj);
            SpatialLookup.RemoveObject(obj);
        }

        public GameObject GetObject(uint crc)
        {
            if (crc == 0) return null;
            foreach (var obj in objects)
            {
                if (obj.NicknameCRC == crc) return obj;
            }
            return null;
        }
		public GameObject GetObject(string nickname)
		{
			if (nickname == null) return null;
			foreach (var obj in objects)
			{
				if (obj.Nickname == nickname) return obj;
			}
			return null;
		}

		public void RegisterAll()
		{
			foreach (var obj in objects)
				obj.Register(Physics);
		}

        void FixedUpdate(double time)
        {
            Projectiles.FixedUpdate(time);
            for (int i = 0; i < objects.Count; i++) {
                objects[i].FixedUpdate(time);
                SpatialLookup.UpdatePosition(objects[i], Vector3.Transform(Vector3.Zero, objects[i].WorldTransform));
            }

            if (PhysicsUpdate != null) PhysicsUpdate(time);
        }

        public void Update(double t)
		{
            if (Renderer != null)
            {
                #if DEBUG
                Renderer.UseDebugPoints(DebugPoints);
                #endif
                Renderer.Update(t);
            }
            Physics?.Step(t);
			for (int i = 0; i < objects.Count; i++)
				objects[i].Update(t);
            RenderUpdate?.Invoke(t);
        }

		public event Action<GameObject, GameMessageKind> MessageBroadcasted;

		public void BroadcastMessage(GameObject sender, GameMessageKind kind)
		{
			if (MessageBroadcasted != null)
				MessageBroadcasted(sender, kind);
		}

        public void Dispose()
        {
            Physics?.Dispose();
        }
	}
}

