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
        private const float PART_DRAGFACTOR = 0.01f;
        private const float ROTATION_BIAS = 6.0f;
        private const float ROTATION_DOWNSPEED = 0.7f;
        #endregion

        #region variables

        private List<GameObject> parts;
        private List<Vector3> offsetVectors;
        private List<Vector3> rotations;
        private float forceMultiplier;
        private float rotationMultiplier = 1.0f;
        private bool isShattered = false;
        private Vector3 intersectionVector = Vector3.Zero;
        private int maxHealth;
        private int frameCtr = 0;

        #endregion

        #region properties

        public string PartsPath { get; set; }
        public int PartCount { get; set; }
        public int PartHealth { get; private set; }

        /// <summary>
        /// setting this property will result in PartHealth resetting to this level!!!
        /// </summary>
        public int MaxHealth
        { 
            get
            {
                return maxHealth;
            }
            set
            {
                maxHealth = value;
                PartHealth = value;
            }
        }
        public int HitDamage { get; set; }

        #endregion

        #region methods

        public Destructible(GameObject myObj) : base(myObj)
        {
            parts = new List<GameObject>();
            offsetVectors = new List<Vector3>();
            rotations = new List<Vector3>();
        }

        public Destructible(GameObject myObj, Destructible cc) : base(myObj, cc)
        {
            this.PartsPath = cc.PartsPath;
            this.PartCount = cc.PartCount;
            this.parts = new List<GameObject>();
            offsetVectors = new List<Vector3>();
            rotations = new List<Vector3>();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(this.Enabled)
            {
                if (isShattered)
                {
                    if (rotationMultiplier > 0.0f)
                    {
                        for (int i = 0; i < PartCount; ++i)
                        {
                            if (!parts[i].MyCollider.IsCollision)
                            {
                                rotations[i] = Vector3.Zero;
                            }
                            parts[i].MyTransform.Rotation += rotations[i] * rotationMultiplier * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
                        }
                        rotationMultiplier = rotationMultiplier - rotationMultiplier * ROTATION_DOWNSPEED * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
                        if (rotationMultiplier < 0.1f)
                            rotationMultiplier = 0.0f;
                    }
                    else
                    {
                        Cleanup();
                    }
                }
                else
                {
                    intersectionVector = new Vector3(0.1f, 0.0f, 0.0f);
                    Shatter();
                }
            }
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

        public override void OnTriggerEnter(GameObject other)
        {
            if(!isShattered)
            {
                PartHealth -= HitDamage;
            }

            if(PartHealth <= 0)
            {
                intersectionVector = MyObject.MyCollider.IntersectionVector;
                Shatter();
            }

            base.OnTriggerEnter(other);
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
                obj.MyPhysicalObject = new PhysicalObject(obj, PART_MASS * ((float)(new Random().Next(5) + 5)/10.0f), PART_DRAGFACTOR, false);
                obj.MyTransform = new Transform(obj, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), MyObject.MyTransform.Scale);
                obj.MyCollider = new BoxCollider(obj, false);
                obj.Dynamic = true;

                parts.Add(obj);
                MyObject.AddChild(obj);

                Vector3 offset = Vector3.Transform(new Vector3(0.0f, 0.0f, 0.0f), mod.Meshes[0].ParentBone.Transform);
                offset.Z = -offset.Z;
                offset = Vector3.Normalize(offset);
                offsetVectors.Add(offset);

                obj.Enabled = false;
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
                //Vector3 vec = offsetVectors[i];
                //parts[i].MyTransform.Position = vec;
            }

            parts[0].MyTransform.Rotation = new Vector3(MathHelper.PiOver2, 0.0f, 0.0f);
            this.MyObject.Visible = true;
            this.MyObject.ChildrenVisible = false;
        }

        private void Shatter()
        {
            // calculate force multiplier here
            forceMultiplier = 40.0f;
            //
            isShattered = true;
            this.MyObject.Visible = false;
            this.MyObject.ChildrenVisible = true;
            for (int i = 0; i < PartCount; ++i)
            {
                parts[i].Enabled = true;
                parts[i].MyTransform.BakeTransformFromParent();
                this.MyObject.RemoveChildAddHigher(parts[i]);
                parts[i].MyPhysicalObject.IsUsingGravity = true;
                float temp = (1.0f / Math.Max((-intersectionVector - offsetVectors[i]).Length(), 0.00001f));

                Vector3 rotationVec = Vector3.Zero;
                Random rand = new Random();
                rotationVec.X = ((float)rand.Next(2)) * (1.0f / temp) * ROTATION_BIAS;
                rotationVec.Y = ((float)rand.Next(2)) * (1.0f / temp) * ROTATION_BIAS;
                rotationVec.Z = ((float)rand.Next(2)) * (1.0f / temp) * ROTATION_BIAS;
                rotations.Add(rotationVec);

                parts[i].MyPhysicalObject.AddForce(offsetVectors[i] * forceMultiplier * temp);
            }
        }

        private void Cleanup()
        {
            this.Enabled = false;
        }

        #endregion
    }
}
