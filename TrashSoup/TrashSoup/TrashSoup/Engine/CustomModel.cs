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

        public override void Draw(Camera cam, Effect effect, GameTime gameTime)
        {
            if(this.Visible)
            {
                Model mod = LODs[(uint)LODState];
                if(mod != null)
                {
                    Camera camera;
                    if (cam == null)
                        camera = ResourceManager.Instance.CurrentScene.Cam;
                    else
                        camera = cam;

                    Transform transform = MyObject.MyTransform;
                    Matrix[] bones = null;
                    if (MyObject.MyAnimator != null) bones = MyObject.MyAnimator.GetSkinTransforms();

                    if(bones != null && effect != null)
                    {
                        effect.CurrentTechnique = effect.Techniques["Skinned"];
                    }
                    else if(effect != null)
                    {
                        effect.CurrentTechnique = effect.Techniques["Main"];
                    }

                    int ctr = 0;
                    foreach (ModelMesh mm in mod.Meshes)
                    {
                        for (int i = 0; i < mm.MeshParts.Count; ++i)
                        {
                            this.Mat[ctr].UpdateEffect(
                                 effect,
                                 mm.ParentBone.Transform * transform.GetWorldMatrix(), 
                                 (mm.ParentBone.Transform * transform.GetWorldMatrix()) * camera.ViewProjMatrix,
                                 ResourceManager.Instance.CurrentScene.AmbientLight,
                                 ResourceManager.Instance.CurrentScene.DirectionalLights,
                                 ResourceManager.Instance.CurrentScene.GetPointLightDiffuseColors(),
                                 ResourceManager.Instance.CurrentScene.GetPointLightSpecularColors(),
                                 ResourceManager.Instance.CurrentScene.GetPointLightAttenuations(),
                                 ResourceManager.Instance.CurrentScene.GetPointLightPositions(),
                                 ResourceManager.Instance.CurrentScene.GetPointLightCount(),
                                 ResourceManager.Instance.CurrentScene.GetPointLight0ShadowMap(),
                                 ResourceManager.Instance.CurrentScene.GetPointLight0ViewProj(),
                                 camera.Position + camera.Translation,
                                 camera.Bounds,
                                 gameTime);
                            mm.MeshParts[i].Effect = this.Mat[ctr].MyEffect;
                            if (bones != null) this.Mat[ctr].SetEffectBones(effect, bones);
                            this.Mat[ctr].FlushMaterialEffect();
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

        public override System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            reader.ReadStartElement();

            base.ReadXml(reader);

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
                LODs[j] = ResourceManager.Instance.LoadModel(Paths[j]);
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
                        Effect newEf = ResourceManager.Instance.LoadEffect(reader.ReadElementString("EffectPath", ""));

                        //if (reader.Name == "Effect")
                        //{
                        //    reader.ReadStartElement();
                        //    int newEffectID = reader.ReadElementContentAsInt("EffectID", "");
                        //    if(newEffectID <= ResourceManager.Instance.Effects.Count - 1)
                        //    {
                        //        switch (newEffectID)
                        //        {
                        //            case -1:
                        //                newEf = new BasicEffect(TrashSoupGame.Instance.GraphicsDevice);
                        //                break;
                        //            case -2:
                        //                newEf = new SkinnedEffect(TrashSoupGame.Instance.GraphicsDevice);
                        //                break;
                        //            default:
                        //                //newEf = ResourceManager.Instance.Effects[newEffectID];
                        //                break;
                        //        }
                        //        //odczyt do próżni
                        //    }
                        //    else
                        //    {
                        //        if(reader.Name == "EffectParameters")
                        //        {
                        //            newEf = EffectParameterDeserialization(reader);
                        //        }

                        //    }
                        //    //switch (newEffectID)
                        //    //{
                        //    //    case -1:
                        //    //        newEf = new BasicEffect(TrashSoupGame.Instance.GraphicsDevice);
                        //    //        break;
                        //    //    case -2:
                        //    //        newEf = new SkinnedEffect(TrashSoupGame.Instance.GraphicsDevice);
                        //    //        break;
                        //    //    default:
                        //    //        newEf = ResourceManager.Instance.Effects[newEffectID];
                        //    //        break;
                        //    //}
                        //    reader.ReadEndElement();
                        //}

                        Material m = new Material(newName, newEf);

                        if (!ResourceManager.Instance.Materials.TryGetValue(newName, out m))
                        {
                            Material tmp = new Material(newName, newEf);
                            (tmp as IXmlSerializable).ReadXml(reader);
                            m = tmp;
                            Debug.Log("Material successfully loaded - " + newName);
                        }
                        else
                        {
                            (m as IXmlSerializable).ReadXml(reader);
                            if(!ResourceManager.Instance.Materials.ContainsKey(newName))
                            {
                                ResourceManager.Instance.Materials.Add(newName, m);
                            }
                            Debug.Log("New material successfully loaded - " + newName);
                        }
                        
                        reader.ReadEndElement();

                        Mat.Add(m);
                    }
                }
               
                reader.ReadEndElement();
            }

            reader.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);

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

                    writer.WriteElementString("EffectPath", ResourceManager.Instance.Effects.FirstOrDefault(x => x.Value == mat.MyEffect).Key);

                    //writer.WriteStartElement("Effect");

                    //int effectID = ResourceManager.Instance.Effects.IndexOf(mat.MyEffect);
                    //if (effectID == -1)
                    //{
                    //    if (mat.MyEffect is SkinnedEffect) effectID = -2;
                    //}
                    //writer.WriteElementString("EffectID", XmlConvert.ToString(effectID));

                    //writer.WriteStartElement("EffectParameters", "");
                    //foreach(EffectParameter param in mat.MyEffect.Parameters)
                    //{
                    //     writer.WriteElementString("Name", param.Name);
                    //     EffectParametersSerialization(writer, param);
                    //}
                    //writer.WriteEndElement();

                    //writer.WriteEndElement();

                    (mat as IXmlSerializable).WriteXml(writer);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }

        private void EffectParametersSerialization(System.Xml.XmlWriter writer, EffectParameter param)
        {
            switch(param.ParameterType)
            {
                case EffectParameterType.Bool:
                    writer.WriteElementString("Value", XmlConvert.ToString(param.GetValueBoolean()));
                    break;
                case EffectParameterType.Int32:
                    writer.WriteElementString("Value", XmlConvert.ToString(param.GetValueInt32()));
                    break;
                case EffectParameterType.Texture2D:
                    writer.WriteElementString("Value", ResourceManager.Instance.Textures.FirstOrDefault(x => x.Value == param.GetValueTexture2D()).Key);
                    break;
                case EffectParameterType.TextureCube:
                    writer.WriteElementString("Value", ResourceManager.Instance.TexturesCube.FirstOrDefault(x => x.Value == param.GetValueTextureCube()).Key);
                    break;
                case EffectParameterType.String:
                    writer.WriteElementString("Value", param.GetValueString());
                    break;
                case EffectParameterType.Single:
                    CheckParameterClass(writer, param);
                    break;
                default:
                    writer.WriteElementString("Value", param.ParameterType.ToString());
                    break;
            }
        }

        private Effect EffectParameterDeserialization(System.Xml.XmlReader reader)
        {
            Effect newEf = null;
            String s;
            List<float> array1 = new List<float>();
            List<Vector3> array2 = new List<Vector3>();
            reader.ReadStartElement();
            while(reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                if(reader.Name == "Name")
                {
                    s = reader.ReadElementString("Name", "");
                    switch(s)
                    {
                        case "World":
                            newEf.Parameters["World"].SetValue(this.MyObject.MyTransform.GetWorldMatrix());
                            s = reader.ReadElementString("Value", "");
                            break;
                        case "WorldViewProj":
                            break;
                        case "WorldInverseTranspose":
                            break;
                        default:
                            if(reader.Name == "Values")
                            {
                                reader.ReadStartElement();
                                while(reader.NodeType != System.Xml.XmlNodeType.EndElement)
                                {

                                }
                                reader.ReadEndElement();
                            }
                            else
                            {
                                newEf.Parameters[s].SetValue(reader.ReadElementString("Value", ""));
                            }
                            break;
                    }
                }                       
            }
            reader.ReadEndElement();
            return newEf;
        }

        private void CheckParameterClass(System.Xml.XmlWriter writer, EffectParameter param)
        {
            switch(param.ParameterClass)
            {
                case EffectParameterClass.Vector:
                    if(param.RowCount == 1)
                    {
                        if (param.Elements.Count > 0)
                        {
                            writer.WriteStartElement("VectorValues");
                            for (int i = 0; i < param.Elements.Count; ++i)
                            {
                                writer.WriteElementString("Value", XmlConvert.ToString(param.Elements[i].GetValueVector3().X));
                            }
                            writer.WriteEndElement();
                        }
                        else
                        {
                            writer.WriteElementString("Value", XmlConvert.ToString(param.GetValueVector3().X));
                        }
                    }
                    if(param.RowCount == 2)
                    {
                        writer.WriteStartElement("Vector2Values");
                        writer.WriteElementString("Value", XmlConvert.ToString(param.GetValueVector2().X));
                        writer.WriteElementString("Value", XmlConvert.ToString(param.GetValueVector2().Y));
                        writer.WriteEndElement();
                    }
                    if(param.RowCount == 3)
                    {
                        writer.WriteStartElement("Vector3Values");
                        writer.WriteElementString("Value", XmlConvert.ToString(param.GetValueVector3().X));
                        writer.WriteElementString("Value", XmlConvert.ToString(param.GetValueVector3().Y));
                        writer.WriteElementString("Value", XmlConvert.ToString(param.GetValueVector3().Z));
                        writer.WriteEndElement();
                    }
                    if(param.RowCount == 4)
                    {
                        writer.WriteStartElement("Vector4Values");
                        writer.WriteElementString("Value", XmlConvert.ToString(param.GetValueVector4().X));
                        writer.WriteElementString("Value", XmlConvert.ToString(param.GetValueVector4().Y));
                        writer.WriteElementString("Value", XmlConvert.ToString(param.GetValueVector4().Z));
                        writer.WriteElementString("Value", XmlConvert.ToString(param.GetValueVector4().W));
                        writer.WriteEndElement();
                    }
                    break;
                case EffectParameterClass.Scalar:
                    if(param.Elements.Count > 0)
                    {
                        writer.WriteStartElement("ArrayValues");
                        for(int i = 0; i<param.Elements.Count; ++i)
                        {
                            writer.WriteElementString("Value", XmlConvert.ToString(param.Elements[i].GetValueSingle()));
                        }
                        writer.WriteEndElement();
                    }
                    else
                    {
                        writer.WriteElementString("Value", XmlConvert.ToString(param.GetValueSingle()));
                    }
                    break;
                case EffectParameterClass.Matrix:
                    writer.WriteElementString("Value", "Matrix");
                    break;
                default:
                    writer.WriteElementString("Value", param.ParameterClass.ToString());
                    break;
            }
        }
        #endregion

    }
}
