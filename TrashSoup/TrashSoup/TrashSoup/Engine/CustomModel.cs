﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace TrashSoup.Engine
{
    public class CustomModel : ObjectComponent, IXmlSerializable
    {
        #region staticVariables

        public enum LODStateEnum
        {
            HI,
            MED,
            LO
        }

        public const uint LOD_COUNT = 3;

        #endregion

        #region properties

        public LODStateEnum LODState { get; set; }
        public Model[] LODs { get; set; }
        public List<String> Paths { get; set; }
        // material doesn't change with LOD, but with MeshPart !!!
        public List<Material> Mat { get; set; }

        #endregion

        #region methods
        public CustomModel() 
        {
            this.LODs = new Model[LOD_COUNT];
            this.Paths = new List<string>();
            this.Mat = new List<Material>();
        }

        public CustomModel(GameObject obj) : base(obj)
        {
            this.LODState = LODStateEnum.HI;
            this.LODs = new Model[LOD_COUNT];
            this.Paths = new List<String>();
            for(int i = 0; i < LOD_COUNT; i++)
            {
                this.LODs[i] = null;
            }
            this.Mat = new List<Material>();
        }

        public CustomModel(GameObject obj, Model[] lods, uint lodCount, List<Material> matList)
            : this(obj)
        {
            for(int i = 0; i < lodCount && i < LOD_COUNT; i++)
            {
                this.LODs[i] = lods[i];
                this.Paths.Add(ResourceManager.Instance.Models.FirstOrDefault(x => x.Value == lods[i]).Key);
            }
            Mat = matList;
        }

        public override void Update(GameTime gameTime)
        {
            if(this.Enabled)
            {
                // do nothing currently
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if(this.Visible)
            {
                Model mod = LODs[(uint)LODState];
                if(mod != null)
                {
                    Camera camera = ResourceManager.Instance.CurrentScene.Cam;
                    Transform transform = MyObject.MyTransform;
                    Matrix[] bones = null;
                    if (MyObject.MyAnimator != null) bones = MyObject.MyAnimator.GetSkinTransforms();

                    int ctr = 0;
                    foreach (ModelMesh mm in mod.Meshes)
                    {
                        for (int i = 0; i < mm.MeshParts.Count; ++i)
                        {
                            this.Mat[ctr].UpdateEffect(mm.ParentBone.Transform * transform.GetWorldMatrix(), 
                                 (mm.ParentBone.Transform * transform.GetWorldMatrix()) * camera.ViewMatrix * camera.ProjectionMatrix,
                                 ResourceManager.Instance.CurrentScene.AmbientLight,
                                 ResourceManager.Instance.CurrentScene.DirectionalLights,
                                 ResourceManager.Instance.CurrentScene.GetPointLightDiffuseColors(),
                                 ResourceManager.Instance.CurrentScene.GetPointLightSpecularColors(),
                                 ResourceManager.Instance.CurrentScene.GetPointLightAttenuations(),
                                 ResourceManager.Instance.CurrentScene.GetPointLightPositions(),
                                 ResourceManager.Instance.CurrentScene.GetPointLightCount(),
                                 camera.Position + camera.Translation);
                            mm.MeshParts[i].Effect = this.Mat[ctr].MyEffect;
                            if (bones != null) this.Mat[ctr].SetEffectBones(bones);
                            ++ctr;
                        }

                        mm.Draw();
                    }
                }
            }
        }

        protected override void Start()
        {
        }

        //protected virtual void FlipZAxis()
        //{
        //    for(int i = 0; i < LOD_COUNT; i++)
        //    {
        //        if(LODs[i] != null)
        //        {
        //            foreach (ModelMesh mm in LODs[i].Meshes)
        //            {
        //                Matrix trans = mm.ParentBone.Transform;
        //                trans.Forward = new Vector3(
        //                    trans.Forward.X,
        //                    trans.Forward.Y,
        //                    -trans.Forward.Z
        //                    );
        //                mm.ParentBone.Transform = trans;
        //            }
        //        }
        //    }
        //}

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            reader.ReadStartElement();

            LODState = (LODStateEnum)Enum.Parse(typeof(LODStateEnum), reader.ReadElementString("LODState", ""));

            if(reader.Name == "LODs")
            {
                reader.ReadStartElement();
                while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                {
                    String s = reader.ReadElementString("ModelPath", "");
                    Paths.Add(s);
                }
                reader.ReadEndElement();
            }

            for(int j = 0; j<Paths.Count(); ++j)
            {
                LODs[j] = ResourceManager.Instance.Models[Paths[j]];
            }

            if(reader.Name == "Materials")
            {
                reader.ReadStartElement();
                while(reader.NodeType != System.Xml.XmlNodeType.EndElement)
                {
                    if (reader.Name == "Material")
                    {
                        reader.ReadStartElement();
                        
                        String newName = reader.ReadElementString("Name", "");

                        int newEffectID = reader.ReadElementContentAsInt("EffectID", "");
                        Effect newEf = null;
                        switch(newEffectID)
                        {
                            case -1:
                                newEf = new BasicEffect(TrashSoupGame.Instance.GraphicsDevice);
                                break;
                            case -2:
                                newEf = new SkinnedEffect(TrashSoupGame.Instance.GraphicsDevice);
                                break;
                            default:
                                newEf = ResourceManager.Instance.Effects[newEffectID];
                                break;
                        }

                        Material m = new Material(newName, newEf);

                        m.DiffuseMap = ResourceManager.Instance.Textures[reader.ReadElementString("DiffusePath", "")];
                        m.NormalMap = ResourceManager.Instance.Textures[reader.ReadElementString("NormalPath", "")];
                        m.CubeMap = ResourceManager.Instance.Textures[reader.ReadElementString("CubePath", "")];

                        reader.ReadStartElement("SpecularColor");
                        m.SpecularColor = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                                                       reader.ReadElementContentAsFloat("Y", ""),
                                                       reader.ReadElementContentAsFloat("Z", ""));
                        reader.ReadEndElement();

                        m.Glossiness = reader.ReadElementContentAsFloat("Glossiness", "");

                        reader.ReadStartElement("ReflectivityColor");
                        m.ReflectivityColor = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                                                       reader.ReadElementContentAsFloat("Y", ""),
                                                       reader.ReadElementContentAsFloat("Z", ""));
                        reader.ReadEndElement();

                        m.ReflectivityBias = reader.ReadElementContentAsFloat("ReflectivityBias", "");
                        m.Transparency = reader.ReadElementContentAsFloat("Transparency", "");
                        m.PerPixelLighting = reader.ReadElementContentAsBoolean("PerPixelLighting", "");
                        
                        reader.ReadEndElement();

                        Mat.Add(m);
                    }
                }
               
                reader.ReadEndElement();
            }

            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("LODState", LODState.ToString());

            writer.WriteStartElement("LODs");
            foreach(String path in Paths)
            {
                if(path != null)
                {
                    writer.WriteElementString("ModelPath", path);
                }
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Materials");
            foreach(Material mat in Mat)
            {
                if(mat != null)
                {
                    writer.WriteStartElement("Material");
                    writer.WriteElementString("Name", mat.Name);

                    int effectID = ResourceManager.Instance.Effects.IndexOf(mat.MyEffect);
                    if(effectID == -1)
                    {
                        if (mat.MyEffect is SkinnedEffect) effectID = -2;
                    }
                    writer.WriteElementString("EffectID", XmlConvert.ToString(effectID));

                    writer.WriteElementString("DiffusePath", ResourceManager.Instance.Textures.FirstOrDefault(x => x.Value == mat.DiffuseMap).Key);
                    writer.WriteElementString("NormalPath", ResourceManager.Instance.Textures.FirstOrDefault(x => x.Value == mat.NormalMap).Key);
                    writer.WriteElementString("CubePath", ResourceManager.Instance.Textures.FirstOrDefault(x => x.Value == mat.CubeMap).Key);

                    writer.WriteStartElement("SpecularColor");
                    writer.WriteElementString("X", XmlConvert.ToString(mat.SpecularColor.X));
                    writer.WriteElementString("Y", XmlConvert.ToString(mat.SpecularColor.Y));
                    writer.WriteElementString("Z", XmlConvert.ToString(mat.SpecularColor.Z));
                    writer.WriteEndElement();

                    writer.WriteElementString("Glossiness", XmlConvert.ToString(mat.Glossiness));

                    writer.WriteStartElement("ReflectivityColor");
                    writer.WriteElementString("X", XmlConvert.ToString(mat.ReflectivityColor.X));
                    writer.WriteElementString("Y", XmlConvert.ToString(mat.ReflectivityColor.Y));
                    writer.WriteElementString("Z", XmlConvert.ToString(mat.ReflectivityColor.Z));
                    writer.WriteEndElement();

                    writer.WriteElementString("ReflectivityBias", XmlConvert.ToString(mat.ReflectivityBias));
                    writer.WriteElementString("Transparency", XmlConvert.ToString(mat.Transparency));
                    writer.WriteElementString("PerPixelLighting", XmlConvert.ToString(mat.PerPixelLighting));
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }
        #endregion

    }
}
