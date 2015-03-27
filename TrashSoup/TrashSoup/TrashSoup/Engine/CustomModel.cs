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
        public String[] Paths { get; set; }
        // material doesn't change with LOD, but with MeshPart !!!
        public List<Material> Mat { get; set; }

        #endregion

        #region methods

        public CustomModel(GameObject obj) : base(obj)
        {
            this.LODState = LODStateEnum.HI;
            this.LODs = new Model[LOD_COUNT];
            this.Paths = new String[LOD_COUNT];
            for(int i = 0; i < LOD_COUNT; i++)
            {
                this.LODs[i] = null;
                this.Paths[i] = null;
            }
            this.Mat = new List<Material>();
        }

        public CustomModel(GameObject obj, Model[] lods, uint lodCount, List<Material> matList)
            : this(obj)
        {
            for(int i = 0; i < lodCount && i < LOD_COUNT; i++)
            {
                this.LODs[i] = lods[i];
                this.Paths[i] = ResourceManager.Instance.Models.FirstOrDefault(x => x.Value == lods[i]).Key;
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
                    Matrix[] transforms = new Matrix[mod.Bones.Count];
                    mod.CopyAbsoluteBoneTransformsTo(transforms);

                    foreach (ModelMesh mm in mod.Meshes)
                    {
                        foreach (BasicEffect be in mm.Effects)
                        {
                             be.Projection = camera.ProjectionMatrix;
                             be.View = camera.ViewMatrix;
                             be.World = mm.ParentBone.Transform * transform.GetWorldMatrix();
                             be.TextureEnabled = true;
                             be.EnableDefaultLighting();
                             be.Texture = Mat[0].Diffuse;
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

            LODState = (LODStateEnum)reader.ReadElementContentAsObject("LODState", "");

            if(reader.Name == "LODs")
            {
                for(int i = 0; i<Paths.Count(); ++i)
                {
                    Paths[i] = reader.ReadElementString("ModelPath", "");
                    LODs[i] = ResourceManager.Instance.Models[Paths[i]];
                }
            }

            //materials

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
                    writer.WriteElementString("DiffusePath", mat.Diffuse.ToString());
                    if (mat.MyEffect is BasicEffect)
                        mat.MyEffect.Name = "BasicEffect";
                    writer.WriteElementString("EffectPath", mat.MyEffect.ToString());
                }
            }
            writer.WriteEndElement();
        }
        #endregion

    }
}
