using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

                    int ctr = 0;
                    foreach (ModelMesh mm in mod.Meshes)
                    {
                        for (int i = 0; i < mm.MeshParts.Count; ++i)
                        {
                            switch (this.Mat[ctr].MyEffectType)
                            {
                                case Material.EffectType.BASIC:
                                    (this.Mat[ctr].MyEffect as BasicEffect).Projection = camera.ProjectionMatrix;
                                    (this.Mat[ctr].MyEffect as BasicEffect).View = camera.ViewMatrix;
                                    (this.Mat[ctr].MyEffect as BasicEffect).World = mm.ParentBone.Transform * transform.GetWorldMatrix();
                                    (this.Mat[ctr].MyEffect as BasicEffect).EnableDefaultLighting();
                                    (this.Mat[ctr].MyEffect as BasicEffect).Texture = this.Mat[ctr].DiffuseMap;

                                    mm.MeshParts[i].Effect = this.Mat[ctr].MyEffect;
                                    ++ctr;
                                    break;

                                case Material.EffectType.DEFAULT:
                                    break;

                                case Material.EffectType.NORMAL:
                                    break;

                                case Material.EffectType.CUBE:
                                    break;
                            }
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
                        Material m = new Material("null");
                        m.Name = reader.ReadElementString("Name", "");
                        String s = reader.ReadElementString("DiffusePath", "");
                        m.DiffuseMap = ResourceManager.Instance.Textures[s];
                        m.MyEffectType = (Material.EffectType)Enum.Parse(typeof(Material.EffectType), reader.ReadElementString("EffectType", ""));
                        switch (m.MyEffectType)
                        {
                            case Material.EffectType.BASIC:
                                m.MyEffect = new BasicEffect(TrashSoupGame.Instance.GraphicsDevice);
                                break;
                            case Material.EffectType.SKINNED:
                                m.MyEffect = new SkinnedEffect(TrashSoupGame.Instance.GraphicsDevice);
                                break;
                        }
                        Mat.Add(m);
                        reader.ReadEndElement();
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
                    writer.WriteElementString("DiffusePath", ResourceManager.Instance.Textures.FirstOrDefault(x => x.Value == mat.DiffuseMap).Key);
                    writer.WriteElementString("EffectType", mat.MyEffectType.ToString());
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }
        #endregion

    }
}
