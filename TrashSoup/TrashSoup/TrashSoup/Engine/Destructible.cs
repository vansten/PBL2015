using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrashSoup.Engine
{
    public class Destructible : ObjectComponent
    {
        #region constants
        private const float PART_MASS = 3.0f;
        private const float PART_DRAGFACTOR = 1.0f;
        #endregion

        #region variables

        private List<GameObject> parts;
        private List<Vector3> offsetVectors;
        private float strengthMultiplier;

        #endregion

        #region properties

        public string PartsPath { get; set; }
        public uint PartCount { get; set; }

        #endregion

        #region methods

        public Destructible(GameObject myObj) : base(myObj)
        {
            parts = new List<GameObject>();
            offsetVectors = new List<Vector3>();
        }

        public Destructible(GameObject myObj, Destructible cc) : base(myObj, cc)
        {
            this.PartsPath = cc.PartsPath;
            this.PartCount = cc.PartCount;
            this.parts = new List<GameObject>();
            offsetVectors = new List<Vector3>();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // nothing
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {
            // nothing
        }

        protected override void Start()
        {
            
        }

        public override void Initialize()
        {
            if (PartsPath == null)
                throw new ArgumentNullException("DESTRUCTIBLE: PartsPath is null");

            GenerateParts();

            base.Initialize();
        }

        private void GenerateParts()
        {
            Model mod;
            List<Material> mat;
            CustomModel cMod;
            GameObject obj;

            for(uint i = 0; i < PartCount; ++i)
            {
                mod = ResourceManager.Instance.LoadModel(PartsPath + "/" + (i + 1).ToString());
                mat = ResourceManager.Instance.LoadBasicMaterialsFromModel(mod, ResourceManager.Instance.Effects[@"Effects\NormalEffect"]);
                obj = new GameObject(MyObject.UniqueID + (uint)PartsPath.GetHashCode() + i + 1, MyObject.Name + "DestrPart" + (i + 1).ToString());
                cMod = new CustomModel(obj, new Model[] {mod, null, null}, mat);
                obj.Components.Add(cMod);
                obj.MyPhysicalObject = new PhysicalObject(obj, PART_MASS, PART_DRAGFACTOR, false);
                obj.MyTransform = new Transform(obj, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), MyObject.MyTransform.Scale);
                obj.MyCollider = new BoxCollider(obj, false);
                obj.Dynamic = true;

                parts.Add(obj);
                MyObject.AddChild(obj);

                Vector3 offset = Vector3.Transform(new Vector3(0.0f, 0.0f, 0.0f), mod.Meshes[0].ParentBone.Transform);
                offset.Z = -offset.Z;
                offset = Vector3.Normalize(offset);
                offsetVectors.Add(offset);
            }

            for (int i = 0; i < PartCount; ++i )
            {
                for(int j = 0; j < PartCount; ++j)
                {
                    if (parts[j] != parts[i])
                    {
                        parts[i].MyCollider.IgnoredColliders.Add(parts[j].MyCollider);
                    }
                }
                parts[i].MyCollider.IgnoredColliders.Add(MyObject.MyCollider);
                Vector3 vec = offsetVectors[i];
                parts[i].MyTransform.Position = vec;
            }

            this.MyObject.Visible = false;
            this.MyObject.ChildrenVisible = true;

            strengthMultiplier = 1.0f;

            Shatter();
        }

        private void Shatter()
        {

        }

        #endregion
    }
}
