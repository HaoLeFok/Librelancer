﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using System.Text;
using ImGuiNET;
using LibreLancer;
using LibreLancer.ImUI;
using LibreLancer.ContentEdit;
using SimpleMesh;

namespace LancerEdit
{
    public class ImportModelTab : EditorTab
    {
        private Model model;
        List<OutModel> output = new List<OutModel>();
        class OutModel
        {
            public string Name;
            ModelNode def;
            public ModelNode Def {
                get {
                    if (LODs.Count > 0) return LODs[0];
                    else return def;
                } set {
                    def = value;
                }
            }
            public bool ParentTransform = false;
            public bool Transform = true;
            public List<ModelNode> LODs = new List<ModelNode>();
            public List<OutModel> Children = new List<OutModel>();
            public List<MaterialName> Materials = new List<MaterialName>();
        }
        List<TextBuffer> nameBuffers = new List<TextBuffer>();
        TextBuffer modelNameBuffer = new TextBuffer(72);
        string modelNameDefault;
        class MaterialName
        {
            public Geometry Geometry;
            public int Drawcall;
            public TextBuffer Name;
        }
        MainWindow win;
        public ImportModelTab(Model model, string fname, MainWindow win)
        {
            this.model = model;
            Autodetect();
            foreach (var obj in output)
                DoMats(obj);
            Title = string.Format("Model Importer ({0})", fname);
            modelNameDefault = Path.GetFileNameWithoutExtension(fname);
            modelNameBuffer.SetText(modelNameDefault);
            this.win = win;
        }
        void DoMats(OutModel mdl)
        {
            foreach(var lod in mdl.LODs) {
                for (int i = 0; i < lod.Geometry.Groups.Length; i++) {
                    var buf = new TextBuffer(256);
                    buf.SetText(lod.Geometry.Groups[i].Material.Name);
                    mdl.Materials.Add(new MaterialName()
                    {
                        Geometry = lod.Geometry,
                        Drawcall = i,
                        Name = buf
                    });
                    nameBuffers.Add(buf);
                }
            }
            foreach (var child in mdl.Children)
                DoMats(child);
        }
        void Autodetect()
        {
            Dictionary<string, ModelNode[]> autodetect = new Dictionary<string,ModelNode[]>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var obj in model.Roots)
                GetLods(obj, autodetect);
            foreach(var obj in model.Roots) {
                AutodetectTree(obj, output, autodetect);
            }
        }
        void AutodetectTree(ModelNode obj, List<OutModel> parent, Dictionary<string,ModelNode[]> autodetect)
        {
            var num = LodNumber(obj, out _);
            if (num != 0) return;
            var mdl = new OutModel();
            mdl.Name = obj.Name;
            if (obj.Name.EndsWith("_lod0", StringComparison.InvariantCultureIgnoreCase))
                mdl.Name = obj.Name.Remove(obj.Name.Length - 5, 5);
            var geometry = autodetect[mdl.Name];
            foreach (var g in geometry)
                if (g != null) mdl.LODs.Add(g);
            foreach(var child in obj.Children) {
                AutodetectTree(child, mdl.Children, autodetect);
            }
            parent.Add(mdl);
        }
        void GetLods(ModelNode obj, Dictionary<string,ModelNode[]> autodetect)
        {
            string objn;
            var num = LodNumber(obj, out objn);
            if(num != -1) {
               ModelNode[] lods;
                if(!autodetect.TryGetValue(objn, out lods)) {
                    lods = new ModelNode[10];
                    autodetect.Add(objn, lods);
                }
                lods[num] = obj;
            }
            foreach (var child in obj.Children)
                GetLods(child, autodetect);
        }
        //Autodetected LOD: object with geometry + suffix _lod[0-9]
        int LodNumber(ModelNode obj, out string name)
        {
            name = obj.Name;
            if (obj.Geometry == null) return -1;
            if (obj.Name.Length < 6) return 0;
            if (!char.IsDigit(obj.Name, obj.Name.Length - 1)) return 0;
            if (!CheckSuffix("_lodX", obj.Name, 4)) return 0;
            name = obj.Name.Substring(0, obj.Name.Length - "_lodX".Length);
            return int.Parse(obj.Name[obj.Name.Length - 1] + "");
        }
        bool CheckSuffix(string postfixfmt, string src, int count)
        {
            for (int i = 0; i < count; i++)
                if (src[src.Length - postfixfmt.Length + i] != postfixfmt[i]) return false;
            return true;
        }
        void ApplyMatNames(OutModel model)
        {
            foreach(var mat in model.Materials) {
                mat.Geometry.Groups[mat.Drawcall].Material.Name = mat.Name.GetText();
            }
            foreach(var child in model.Children) {
                ApplyMatNames(child);
            }
        }
        
        bool Finish(out EditableUtf result)
        {
            result = null;
            var utf = new EditableUtf();
            //Vanity
            var expv = new LUtfNode() { Name = "Exporter Version", Parent = utf.Root };
            expv.Data = System.Text.Encoding.UTF8.GetBytes("LancerEdit " + Platform.GetInformationalVersion<ImportModelTab>());
            utf.Root.Children.Add(expv);
            //Apply Material names
            foreach(var mdl in output) {
                ApplyMatNames(mdl);
            }
            //Actual stuff
            if (output.Count == 1)
            {
                var modelName = modelNameBuffer.GetText();
                if (string.IsNullOrEmpty(modelName)) modelName = modelNameDefault;
                if (output[0].Children.Count == 0)
                {
                    Export3DB(modelName, utf.Root, output[0]);
                } 
                else 
                {
                    var suffix = (new Random().Next()) + ".3db";
                    var vmslib = new LUtfNode() { Name = "VMeshLibrary", Parent = utf.Root, Children = new List<LUtfNode>() };
                    utf.Root.Children.Add(vmslib);
                    var cmpnd = new LUtfNode() { Name = "Cmpnd", Parent = utf.Root, Children = new List<LUtfNode>() };
                    utf.Root.Children.Add(cmpnd);
                    ExportModels(modelName, utf.Root, suffix,vmslib, output[0]);
                    int cmpndIndex = 1;
                    FixConstructor fix = new FixConstructor(); 
                    cmpnd.Children.Add(CmpndNode(cmpnd, "Root", output[0].Name + suffix, "Root", 0));
                    foreach(var child in output[0].Children) {
                        ProcessConstruct("Root", child, cmpnd,fix,suffix, ref cmpndIndex);
                    }
                    var cons = new LUtfNode() { Name = "Cons", Parent = cmpnd, Children = new List<LUtfNode>() };
                    var trs = new LUtfNode() { Name = "Fix", Parent = cons, Data = fix.GetData() };
                    cons.Children.Add(trs);
                    cmpnd.Children.Add(cons);
                }
                if(generateMaterials)
                {
                    List<SimpleMesh.Material> materials = new List<SimpleMesh.Material>();
                    foreach (var mdl in output)
                        IterateMaterials(materials, mdl);
                    var mats = new LUtfNode() { Name = "material library", Parent = utf.Root };
                    mats.Children = new List<LUtfNode>();
                    int i = 0;
                    foreach (var mat in materials)
                        mats.Children.Add(DefaultMaterialNode(mats,mat,i++));
                    var txms = new LUtfNode() { Name = "texture library", Parent = utf.Root };
                    txms.Children = new List<LUtfNode>();
                    foreach (var mat in materials)
                        txms.Children.Add(DefaultTextureNode(txms,mat.Name));
                    utf.Root.Children.Add(mats);
                    utf.Root.Children.Add(txms);
                }
                result = utf;
                return true;
            }
            else
                return false;
        }
        void ProcessConstruct(string parentName, OutModel mdl, LUtfNode cmpnd, FixConstructor fix, string suffix, ref int index)
        {
            cmpnd.Children.Add(CmpndNode(cmpnd, "PART_" + mdl.Name, mdl.Name + suffix, mdl.Name, index++));
            if(mdl.Transform == true) {
                fix.Add(parentName, mdl.Name, mdl.Def.Transform);
            } else {
                fix.Add(parentName, mdl.Name, Matrix4x4.Identity);
            }
            foreach (var child in mdl.Children)
                ProcessConstruct(mdl.Name, child, cmpnd, fix, suffix, ref index);

        }
        LUtfNode CmpndNode(LUtfNode cmpnd, string name, string filename, string objname, int index)
        {
            var node = new LUtfNode() { Parent = cmpnd, Name = name, Children = new List<LUtfNode>() };
            node.Children.Add(new LUtfNode()
            {
                Name = "File Name",
                Parent = node,
                Data = Encoding.ASCII.GetBytes(filename)
            });
            node.Children.Add(new LUtfNode()
            {
                Name = "Object Name",
                Parent = node,
                Data = Encoding.ASCII.GetBytes(objname)
            });
            node.Children.Add(new LUtfNode()
            {
                Name = "Index",
                Parent = node,
                Data = BitConverter.GetBytes(index)
            });
            return node;
        }
        void ExportModels(string mdlName, LUtfNode root, string suffix,LUtfNode vms, OutModel model)
        {
            var modelNode = new LUtfNode() { Parent = root, Name = model.Name + suffix };
            modelNode.Children = new List<LUtfNode>();
            root.Children.Add(modelNode);
            Export3DB(mdlName, modelNode, model, vms);
            foreach (var child in model.Children)
                ExportModels(mdlName, root, suffix, vms, child);
        }
        static LUtfNode DefaultMaterialNode(LUtfNode parent, SimpleMesh.Material mat, int i)
        {
            var matnode = new LUtfNode() { Name = mat.Name, Parent = parent };
            matnode.Children = new List<LUtfNode>();
            matnode.Children.Add(new LUtfNode() { Name = "Type", Parent = matnode, Data = Encoding.ASCII.GetBytes("DcDt") });
            var arr = new float[] {mat.DiffuseColor.X, mat.DiffuseColor.Y, mat.DiffuseColor.Z};
            matnode.Children.Add(new LUtfNode() { Name = "Dc", Parent = matnode, Data = UnsafeHelpers.CastArray(arr) });
            matnode.Children.Add(new LUtfNode() { Name = "Dt_name", Parent = matnode, Data = Encoding.ASCII.GetBytes(mat.Name + ".dds") });
            matnode.Children.Add(new LUtfNode() { Name = "Dt_flags", Parent = matnode, Data = BitConverter.GetBytes(64) });
            return matnode;
        }
        static LUtfNode DefaultTextureNode(LUtfNode parent, string name)
        {
            var texnode = new LUtfNode() { Name = name + ".dds", Parent = parent };
            texnode.Children = new List<LUtfNode>();
            var d = new byte[DefaultTexture.Data.Length];
            Buffer.BlockCopy(DefaultTexture.Data, 0, d, 0, DefaultTexture.Data.Length);
            texnode.Children.Add(new LUtfNode() { Name = "MIPS", Parent = texnode, Data = d });
            return texnode;
        }

        static bool HasMat(List<Material> materials, Material m)
        {
            foreach (var m2 in materials)
            {
                if (m2.Name == m.Name) return true;
            }
            return false;
        }
        static void IterateMaterials(List<Material> materials, OutModel mdl)
        {
            foreach (var lod in mdl.LODs)
                foreach (var dc in lod.Geometry.Groups)
                    if (dc.Material != null && !HasMat(materials, dc.Material))
                        materials.Add(dc.Material);
            foreach (var child in mdl.Children)
                IterateMaterials(materials, child);
        }

        static void Export3DB(string mdlName, LUtfNode node3db, OutModel mdl, LUtfNode vmeshlibrary = null)
        {
            var vms = vmeshlibrary ?? new LUtfNode() { Name = "VMeshLibrary", Parent = node3db, Children = new List<LUtfNode>() };
            for (int i = 0; i < mdl.LODs.Count; i++)
            {
                var n = new LUtfNode() { Name = string.Format("{0}-{1}.lod{2}.{3}.vms", mdlName, mdl.Name, i, (int)GeometryWriter.FVF(mdl.LODs[i].Geometry)), Parent = vms };
                n.Children = new List<LUtfNode>();
                n.Children.Add(new LUtfNode() { Name = "VMeshData", Parent = n, Data = GeometryWriter.VMeshData(mdl.LODs[i].Geometry) });
                vms.Children.Add(n);
            }
            if(vmeshlibrary == null)
                node3db.Children.Add(vms);
            if (mdl.LODs.Count > 1)
            {
                var multilevel = new LUtfNode() { Name = "MultiLevel", Parent = node3db };
                multilevel.Children = new List<LUtfNode>();
                var switch2 = new LUtfNode() { Name = "Switch2", Parent = multilevel };
                multilevel.Children.Add(switch2);
                for (int i = 0; i < mdl.LODs.Count; i++)
                {
                    var n = new LUtfNode() { Name = "Level" + i, Parent = multilevel };
                    n.Children = new List<LUtfNode>();
                    n.Children.Add(new LUtfNode() { Name = "VMeshPart", Parent = n, Children = new List<LUtfNode>() });
                    n.Children[0].Children.Add(new LUtfNode()
                    {
                        Name = "VMeshRef",
                        Parent = n.Children[0],
                        Data = GeometryWriter.VMeshRef(mdl.LODs[i].Geometry, 
                            string.Format("{0}-{1}.lod{2}.{3}.vms", mdlName, mdl.Name, i, (int)GeometryWriter.FVF(mdl.LODs[i].Geometry)))
                    });
                    multilevel.Children.Add(n);
                }
                //Generate Switch2: TODO - Be more intelligent about this
                var mlfloats = new float[multilevel.Children.Count];
                mlfloats[0] = 0;
                float cutOff = 2250;
                for (int i = 1; i < mlfloats.Length - 1; i++)
                {
                    mlfloats[i] = cutOff;
                    cutOff *= 2;
                }
                mlfloats[mlfloats.Length - 1] = 1000000;
                switch2.Data = UnsafeHelpers.CastArray(mlfloats);
                node3db.Children.Add(multilevel);
            }
            else
            {
                var part = new LUtfNode() { Name = "VMeshPart", Parent = node3db };
                part.Children = new List<LUtfNode>();
                part.Children.Add(new LUtfNode()
                {
                    Name = "VMeshRef",
                    Parent = part,
                    Data = GeometryWriter.VMeshRef(mdl.LODs[0].Geometry,string.Format("{0}-{1}.lod0.{2}.vms", mdlName, mdl.Name,  (int)GeometryWriter.FVF(mdl.LODs[0].Geometry)))
                });
                node3db.Children.Add(part);
            }
        }

        bool _openError;
        string _errorText;
        void ErrorPopup(string text)
        {
            _openError = true;
            _errorText = text;
        }
        public override void Draw()
        {
            ImGui.Text("Tree");
            ImGui.SameLine(ImGui.GetWindowWidth() - 60);
            if (ImGui.Button("Finish"))
            {
                EditableUtf utf;
                if (Finish(out utf))
                    win.AddTab(new UtfTab(win, utf, "Untitled", true));
                else
                {
                    ErrorPopup("Invalid UTF Structure:\nMore than one root node.");
                }
            }
            ImGui.BeginChild("##fl");
            FLPane();
            ImGui.EndChild();
            if (_openError) ImGui.OpenPopup("Error");
            if(ImGui.BeginPopupModal("Error")) {
                ImGui.Text(_errorText);
                if (ImGui.Button("Ok")) ImGui.CloseCurrentPopup();
                ImGui.EndPopup();
            }
            _openError = false;
        }

        float fl_h1 = 200, fl_h2 = 200;
        int curTab = 0;
        bool generateMaterials = true;
        void FLPane()
        {
            var totalH = ImGui.GetWindowHeight();
            ImGuiExt.SplitterV(2f, ref fl_h1, ref fl_h2, 8, 8, -1);
            fl_h1 = totalH - fl_h2 - 6f;
            ImGui.BeginChild("1", new Vector2(-1, fl_h1), false, ImGuiWindowFlags.None);
            ImGui.Separator();
            //3DB list
            if (ImGui.TreeNodeEx("Model/"))
            {
                int i = 0;
                foreach (var mdl in output)
                {
                    FLTree(mdl, ref i);
                }
            }
            ImGui.EndChild();
            ImGui.BeginChild("2", new Vector2(-1, fl_h2), false, ImGuiWindowFlags.None);
            if (ImGuiExt.ToggleButton("Options", curTab == 0)) curTab = 0;
            ImGui.SameLine();
            if (ImGuiExt.ToggleButton("Materials", curTab == 1)) curTab = 1;
            ImGui.Separator();
            switch(curTab) {
                case 0: //OPTIONS
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Model Name:");
                    ImGui.SameLine();
                    modelNameBuffer.InputText("##mdlname", ImGuiInputTextFlags.None);
                    ImGui.Checkbox("Generate Materials", ref generateMaterials);
                    break;
                case 1: //MATERIALS
                    if (selected == null)
                    {
                        ImGui.Text("No object selected");
                    }
                    else
                        MatNameEdit();
                    break;
            }
            ImGui.EndChild();
        }
        void MatNameEdit()
        {
            int i = 0;
            foreach(var name in selected.Materials) {
                ImGui.Text(string.Format("{0} ({1})", name.Geometry.Name, name.Drawcall));
                ImGui.SameLine();
                name.Name.InputText("##" + i++, ImGuiInputTextFlags.None);
            }
        }
        OutModel selected;
        void FLTree(OutModel mdl, ref int i)
        {
            var flags = ImGuiTreeNodeFlags.OpenOnDoubleClick |
                                         ImGuiTreeNodeFlags.DefaultOpen |
                                         ImGuiTreeNodeFlags.OpenOnArrow;
            if (mdl == selected) flags |= ImGuiTreeNodeFlags.Selected;
            var open = Theme.IconTreeNode(Icons.Cube_LightPink, $"{mdl.Name}##{i++}", flags);
            if(ImGui.IsItemClicked(0)) {
                selected = mdl;
            }
            ImGui.SameLine();
            if(open)
            {
                if(ImGui.TreeNode("LODs")) {
                    for (int j = 0; j < mdl.LODs.Count; j++)
                        ImGui.Selectable(string.Format("{0}: {1}", j, mdl.LODs[j].Name));
                    ImGui.TreePop();
                }
                foreach (var child in mdl.Children)
                    FLTree(child, ref i);
                ImGui.TreePop();
            }
            i += 500;
        }
       
       
        public override void Dispose()
        {
            foreach (var buf in nameBuffers) buf.Dispose();
            modelNameBuffer.Dispose();
        }
    }
}
