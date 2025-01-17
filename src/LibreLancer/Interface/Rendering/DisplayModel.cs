using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MoonSharp.Interpreter;

namespace LibreLancer.Interface
{
    public class ModifiedMaterial
    {
        public BasicMaterial Mat;
        public Color4 Dc;
        public string Dt;
    }
    public class MaterialModification
    {
        public static List<ModifiedMaterial> Setup(RigidModel model, ResourceManager res)
        {
            var mats = new List<ModifiedMaterial>();
            foreach (var p in model.AllParts)
            {
                if (p.Mesh == null) continue;
                foreach (var l in p.Mesh.Levels)
                {
                    if (l == null) continue;
                    foreach (var dc in l)
                    {
                        var mat = dc.GetMaterial(res)?.Render;
                        if (mat is BasicMaterial bm)
                        {
                            if (mats.Any(x => x.Mat == bm)) continue;
                            mats.Add(new ModifiedMaterial() {Mat = bm, Dc = bm.Dc, Dt = bm.DtSampler});
                        }
                    }
                }
            }
            return mats;
        }
    }
    
    [UiLoadable]
    [MoonSharpUserData]
    public class DisplayModel : DisplayElement
    {
        public InterfaceModel Model { get; set; }
        public InterfaceColor Tint { get; set; }

        public Vector3 Rotate { get; set; }
        public Vector3 RotateAnimation { get; set; }
        
        public float BaseRadius { get; set; }
        
        public bool Clip { get; set; }
        
        private RigidModel model;
        private bool loadable = true;
        private List<ModifiedMaterial> mats;
        
        public static Matrix4x4 CreateTransform(int gWidth, int gHeight, Rectangle r)
        {
            float gX = (float)gWidth / 2;
            float gY = (float)gHeight / 2;
            var tX = (r.X + (r.Width / 2) - gX) / gX;
            var tY = (gY - r.Y - (r.Height / 2)) / gY;
            var sX = r.Width / (float)(gWidth);
            var sY = r.Height / (float)(gHeight);
            return Matrix4x4.CreateScale(sX, sY, 1) * Matrix4x4.CreateTranslation(tX, tY, 0);
        }
        
        public override void Render(UiContext context, RectangleF clientRectangle)
        {
            if (Model == null) return;
            if (!CanRender(context)) return;
            var rect = context.PointsToPixels(clientRectangle);
            if (Clip) {
                context.RenderContext.ScissorEnabled = true;
                context.RenderContext.ScissorRectangle = rect;
            }
            Matrix4x4 rotationMatrix = Matrix4x4.Identity;
            var rot = Rotate + (RotateAnimation * (float)context.GlobalTime);
            if (rot != Vector3.Zero) {
                rotationMatrix = Matrix4x4.CreateRotationX(rot.X) *
                      Matrix4x4.CreateRotationY(rot.Y) *
                      Matrix4x4.CreateRotationZ(rot.Z);
            }

            float scaleMult = 1;
            if (BaseRadius > 0)
            {
                scaleMult = BaseRadius / model.GetRadius();
            }
            var transform = Matrix4x4.CreateScale(Model.XScale * scaleMult, Model.YScale * scaleMult, 1) *
                            rotationMatrix *
                            Matrix4x4.CreateTranslation(Model.X, Model.Y, 0);
            transform *= CreateTransform((int) context.ViewportWidth, (int) context.ViewportHeight, rect);
            context.RenderContext.Cull = false;
            model.UpdateTransform();
            model.Update(context.MatrixCam, context.GlobalTime, context.Data.ResourceManager);
            if (Tint != null)
            {
                var color = Tint.GetColor(context.GlobalTime);
                for (int i = 0; i < mats.Count; i++)
                    mats[i].Mat.Dc = color;
            }
            model.DrawImmediate(context.RenderContext, context.Data.ResourceManager, transform, ref Lighting.Empty);
            if (Tint != null)
            {
                for (int i = 0; i < mats.Count; i++)
                {
                    mats[i].Mat.Dc = mats[i].Dc;
                }
            }           
            context.RenderContext.ScissorEnabled = false;
            context.RenderContext.Cull = true;
        }
        
        bool CanRender(UiContext context)
        {
            if (!loadable) return false;
            if (model == null)
            {
                model = context.Data.GetModel(Model.Path);
                if (model == null)
                {
                    loadable = false;
                    return false;
                }
                if (Tint != null)
                    mats = MaterialModification.Setup(model, context.Data.ResourceManager);
            }
            return true;
        }
        
        
       
    }
}