﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using System.Numerics;
using BulletSharp;
using BM = BulletSharp.Math;

namespace LibreLancer.Physics
{
    public delegate void FixedUpdateHandler(double elapsed);

    /// <summary>
    /// Creates a bullet physics world. Any object created here will be invalid upon Dispose()
    /// </summary>
    public class PhysicsWorld : IDisposable
    {
        public IReadOnlyList<PhysicsObject> Objects
        {
            get { return objects; }
        }

        public event FixedUpdateHandler FixedUpdate;

        DiscreteDynamicsWorld btWorld;
        CollisionDispatcher btDispatcher;
        DbvtBroadphase broadphase;
        CollisionConfiguration collisionConf;
        List<PhysicsObject> objects = new List<PhysicsObject>();
        List<PhysicsObject> dynamicObjects = new List<PhysicsObject>();
        bool disposed = false;
        
        private CollisionObject pointObj;

        public PhysicsWorld()
        {
            collisionConf = new DefaultCollisionConfiguration();

            btDispatcher = new CollisionDispatcher(collisionConf);
            broadphase = new DbvtBroadphase();
            btWorld = new DiscreteDynamicsWorld(btDispatcher, broadphase, null, collisionConf);
            btWorld.Gravity = BM.Vector3.Zero;

            pointObj = new CollisionObject();
            pointObj.CollisionShape = new SphereShape(1);
        }


        DebugDrawWrapper wrap;

        public void EnableWireframes(IDebugRenderer renderer)
        {
            wrap = new DebugDrawWrapper(renderer);
            btWorld.DebugDrawer = wrap;
        }

        public void DisableWireframes()
        {
            btWorld.DebugDrawer = null;
            wrap = null;
        }

        public void DrawWorld()
        {
            btWorld.DebugDrawWorld();
        }

        public PhysicsObject AddStaticObject(Matrix4x4 transform, Collider col)
        {
            using (var rbInfo = new RigidBodyConstructionInfo(0,
                new DefaultMotionState(transform.Cast()),
                col.BtShape, BM.Vector3.Zero))
            {
                var body = new RigidBody(rbInfo);
                body.Restitution = 1;
                var phys = new PhysicsObject(body, col) {Static = true};
                phys.UpdateProperties();
                body.UserObject = phys;
                btWorld.AddRigidBody(body);
                objects.Add(phys);
                return phys;
            }
        }

        public bool PointRaycast(PhysicsObject me, Vector3 origin, Vector3 direction, float maxDist, out Vector3 contactPoint, out PhysicsObject didHit)
        {
            contactPoint = Vector3.Zero;
            didHit = null;
            ClosestRayResultCallback cb;
            var from = origin.Cast();
            var to = (origin + direction * maxDist).Cast();
            if (me != null) {
                cb = new KinematicClosestNotMeRayResultCallback(me.RigidBody);
                cb.RayFromWorld = from;
                cb.RayToWorld = to;
            }
            else {
                cb = new ClosestRayResultCallback(ref from,  ref to);
            }
            using (cb)
            {
                btWorld.RayTestRef(ref from, ref to, cb);
                if (cb.HasHit)
                {
                    didHit = cb.CollisionObject.UserObject as PhysicsObject;
                    contactPoint = cb.HitPointWorld.Cast();
                    return true;
                }
                return false;
            }
        }

        public PhysicsObject AddDynamicObject(float mass, Matrix4x4 transform, Collider col, Vector3? inertia = null) {
            if(mass < float.Epsilon) {
                throw new Exception("Mass must be non-zero");
            }
            BM.Vector3 localInertia;
            if (inertia != null) {
                localInertia = inertia.Value.Cast();
            }
            else {
                col.BtShape.CalculateLocalInertia(mass, out localInertia);
            }
            using (var rbInfo = new RigidBodyConstructionInfo(mass,
                                                             new DefaultMotionState(transform.Cast()),
                                                             col.BtShape, localInertia))
            {
                var body = new RigidBody(rbInfo);
                body.SetDamping(0, 0);
                body.Restitution = 0.8f;
                var phys = new PhysicsObject(body, col) { Static = false };
                phys.UpdateProperties();
                body.UserObject = phys;
                btWorld.AddRigidBody(body);
                objects.Add(phys);
                dynamicObjects.Add(phys);
                return phys;
            }
        }
        double accumulatedTime = 0;
        private const float TIMESTEP = 1 / 60.0f;
        public void Step(double elapsed)
        {
            if (disposed) throw new ObjectDisposedException("PhysicsWorld");
            accumulatedTime += elapsed;
            while(accumulatedTime >= TIMESTEP) {
                FixedUpdate?.Invoke(TIMESTEP);
                if (disposed) return; //Allow delete within FixedUpdate. Hacky but works
                btWorld.StepSimulation(TIMESTEP, 0, TIMESTEP);
                accumulatedTime -= TIMESTEP;
                //Update C#-side properties after each step. Creates stuttering otherwise
                foreach (var obj in dynamicObjects) {
                    obj.UpdateProperties();
                    obj.RigidBody.Activate(true);
                }
            }
           
        }

        /// <summary>
        /// Removes and destroys (!) the object
        /// </summary>
        /// <param name="obj">Physics Object to remove.</param>
        public void RemoveObject(PhysicsObject obj)
        {
            if (obj.RigidBody.MotionState != null)
                obj.RigidBody.MotionState.Dispose();
            btWorld.RemoveCollisionObject(obj.RigidBody);
            obj.RigidBody.Dispose();
            objects.Remove(obj);
            if (!obj.Static) dynamicObjects.Remove(obj);
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            //Delete all RigidBody objects
            for (int i = btWorld.NumCollisionObjects - 1; i >= 0; i--)
            {
                CollisionObject obj = btWorld.CollisionObjectArray[i];
                RigidBody body = obj as RigidBody;
                if (body != null && body.MotionState != null)
                {
                    body.MotionState.Dispose();
                }
                btWorld.RemoveCollisionObject(obj);
                obj.Dispose();
            }
            pointObj.CollisionShape.Dispose();
            pointObj.Dispose();
            btWorld.Dispose();
            broadphase.Dispose();
            btDispatcher.Dispose();
            collisionConf.Dispose();
        }
    }
}
