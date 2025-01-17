﻿// MIT License - Copyright (c) Malte Rupprecht
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package


using System;
using System.Collections.Generic;
using System.Numerics;
using LibreLancer.Utf.Vms;

using LibreLancer.Primitives;

namespace LibreLancer.Utf.Mat
{
   
    /// <summary>
    /// Represents a UTF Sphere File (.sph)
    /// </summary>
    public class SphFile : UtfFile, IRigidModelFile
    {
        private QuadSphere sphere;
		
		private bool ready;

        private ILibFile library;

		public MatFile MaterialLibrary;
		public TxmFile TextureLibrary;
		public VmsFile VMeshLibrary;
        public float Radius { get; private set; }

        private List<string> sideMaterialNames;
        private Material[] sideMaterials;
        public class SphMaterials
        {
            SphFile sph;
            internal SphMaterials(SphFile sph)
            {
                this.sph = sph;
            }
            void CheckNullArray()
            {
                if (sph.sideMaterials == null)
                {
                    sph.sideMaterials = new Material[sph.sideMaterialNames.Count];
                    for (int i = 0; i < sph.sideMaterialNames.Count; i++)
                    {
                        sph.sideMaterials[i] = sph.library.FindMaterial(CrcTool.FLModelCrc(sph.sideMaterialNames[i]));
                    }
                }
            }
            public int Length
            {
                get
                {
                    CheckNullArray();
                    return sph.sideMaterials.Length;
                }
            }
            public Material this[int i]
            {
                get
                {
                    CheckNullArray();
                    if (sph.sideMaterials[i] == null)
                    {
                        var crc = CrcTool.FLModelCrc(sph.sideMaterialNames[i]);
                        sph.sideMaterials[i] = sph.library.FindMaterial(CrcTool.FLModelCrc(sph.sideMaterialNames[i]));
                    }
                    return sph.sideMaterials[i];
                }
                set
                {
                    CheckNullArray();
                    sph.sideMaterials[i] = value;
                }
            }
           
        }

        SphMaterials materialsAccessor;
        public SphMaterials SideMaterials => materialsAccessor;

		public List<string> SideMaterialNames
		{
			get
			{
				return sideMaterialNames;
			}
		}
        public SphFile(IntermediateNode root, ILibFile library, string path = "/")
        {
            if (root == null) throw new ArgumentNullException("root");
            if (library == null) throw new ArgumentNullException("materialLibrary");

            materialsAccessor = new SphMaterials(this);
            ready = false;

			this.library = library;
            sideMaterialNames = new List<string>();

			bool sphereSet = false;
			foreach (Node node in root)
			{
				switch (node.Name.ToLowerInvariant())
				{
					case "sphere":
						if (sphereSet) throw new Exception("Multiple sphere nodes");
						sphereSet = true;
						var sphereNode = (IntermediateNode)node;
						foreach (LeafNode sphereSubNode in sphereNode)
						{
							string name = sphereSubNode.Name.ToLowerInvariant();

							if (name.StartsWith("m", StringComparison.OrdinalIgnoreCase)) sideMaterialNames.Add(sphereSubNode.StringData);
							else if (name == "radius") Radius = sphereSubNode.SingleArrayData[0];
							else if (name == "sides")
							{
								int count = sphereSubNode.Int32ArrayData[0];
								if (count != sideMaterialNames.Count) throw new Exception("Invalid number of sides in " + node.Name + ": " + count);
							}
							else throw new Exception("Invalid node in " + node.Name + ": " + sphereSubNode.Name);
						}
						break;
					case "vmeshlibrary":
						IntermediateNode vMeshLibraryNode = node as IntermediateNode;
						if (VMeshLibrary == null) VMeshLibrary = new VmsFile(vMeshLibraryNode, library);
						else throw new Exception("Multiple vmeshlibrary nodes in 3db root");
						break;
					case "material library":
						IntermediateNode materialLibraryNode = node as IntermediateNode;
						if (MaterialLibrary == null) MaterialLibrary = new MatFile(materialLibraryNode, library);
						else throw new Exception("Multiple material library nodes in 3db root");
						break;
					case "texture library":
						IntermediateNode textureLibraryNode = node as IntermediateNode;
						if (TextureLibrary == null) TextureLibrary = new TxmFile(textureLibraryNode);
						else throw new Exception("Multiple texture library nodes in 3db root");
						break;
				}
			}

            if (sideMaterialNames.Count < 6)
            {
                FLLog.Warning("Sph", $"Sph {path} does not contain all 6 sides and will not render");
            }
        }
		Material defaultMaterial;
		public void Initialize(ResourceManager cache)
        {
            if (SideMaterials.Length >= 6)
            {
                sphere = cache.GetQuadSphere(RenderContext.GLES ? 26 : 32);
                defaultMaterial = cache.DefaultMaterial;
                ready = true;
            }
        }
        
        static CubeMapFace[] faces = new CubeMapFace[] {
			CubeMapFace.PositiveZ,
			CubeMapFace.PositiveX,
			CubeMapFace.NegativeZ,
			CubeMapFace.NegativeX,
			CubeMapFace.PositiveY,
			CubeMapFace.NegativeY
		};
        
        public RigidModel CreateRigidModel(bool drawable)
        {
            var model = new RigidModel();
            var part = new RigidModelPart();
            var dcs = new List<MeshDrawcall>();
            var scale = Matrix4x4.CreateScale(Radius);
            if (drawable && SideMaterials.Length >= 6)
            {
                for (int i = 0; i < 6; i++)
                {
                    int start, count;
                    Vector3 pos;
                    sphere.GetDrawParameters(faces[i], out start, out count, out pos);
                    var dc = new MeshDrawcall();
                    dc.Buffer = sphere.VertexBuffer;
                    dc.MaterialCrc = CrcTool.FLModelCrc(sideMaterialNames[i]);
                    dc.BaseVertex = 0;
                    dc.StartIndex = start;
                    dc.PrimitiveCount = count;
                    dc.HasScale = true;
                    dc.Scale = scale;
                    dcs.Add(dc);
                }

                if (SideMaterials.Length > 6)
                {
                    var crc = CrcTool.FLModelCrc(sideMaterialNames[6]);
                    for (int i = 0; i < 6; i++)
                    {
                        int start, count;
                        Vector3 pos;
                        sphere.GetDrawParameters(faces[i], out start, out count, out pos);
                        var dc = new MeshDrawcall();
                        dc.Buffer = sphere.VertexBuffer;
                        dc.MaterialCrc = crc;
                        dc.BaseVertex = 0;
                        dc.StartIndex = start;
                        dc.PrimitiveCount = count;
                        dc.HasScale = true;
                        dc.Scale = scale;
                        dcs.Add(dc);
                    }
                }
            }
            var vmesh = new VisualMesh();
            vmesh.Radius = Radius;
            vmesh.BoundingBox = BoundingBox.CreateFromSphere(new BoundingSphere(Vector3.Zero, Radius));
            vmesh.Levels = new[] {dcs.ToArray()};
            part.Hardpoints = new List<Hardpoint>();
            part.Mesh = vmesh;
            model.Root = part;
            model.AllParts = new[] {part};
            return model;
        }

        public void ClearResources()
        {
            MaterialLibrary = null;
            TextureLibrary = null;
        }
    }
}