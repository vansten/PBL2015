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
        #region enums
        public enum State
        {
            NORMAL,
            SHATTERED,
            WAIT_FOR_CLEANUP,
            CLEANUP,
            FINISHED
        };
        #endregion

        #region constants
        private const float PART_MASS = 3.0f;
        private const float PART_DRAGFACTOR = 0.01f;
        private const float ROTATION_BIAS = 80.0f;
        private const float ROTATION_DOWNSPEED = 0.7f;
        private const float DISAPPEAR_DELAY = 2000.0f;
        private const float DISAPPEAR_TIME = 10000.0f;
        private const float SCALE_FACTOR = 0.01f;
        private const float FORCE_MULTIPLIER = 80.0f;
        #endregion

        #region variables

        private List<DestructiblePart> parts;
        private List<Material> partMaterials;
        private float forceMultiplier;
        private float rotationMultiplier = 1.0f;
        private Vector3 intersectionVector = Vector3.Zero;
        private int maxHealth;
        private float currentDisappearTime = 0.0f;

        #endregion

        #region properties

        public string PartsPath { get; set; }
        public int PartCount { get; set; }
        public int PartHealth { get; private set; }
        public State ActualState { get; private set; }

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
            parts = new List<DestructiblePart>();
            partMaterials = new List<Material>();
            ActualState = State.NORMAL;
        }

        public Destructible(GameObject myObj, Destructible cc) : base(myObj, cc)
        {
            this.PartsPath = cc.PartsPath;
            this.PartCount = cc.PartCount;
            this.parts = new List<DestructiblePart>();
            partMaterials = new List<Material>();
            ActualState = State.NORMAL;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(this.Enabled)
            {
                if (ActualState == State.SHATTERED)
                {
                    if (rotationMultiplier > 0.0f)
                    {
                        for (int i = 0; i < PartCount; ++i)
                        {
                            parts[i].MyTransform.Rotation += parts[i].Rotation * rotationMultiplier * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
                        }
                        rotationMultiplier = rotationMultiplier - rotationMultiplier * ROTATION_DOWNSPEED * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
                        if (rotationMultiplier < 0.1f)
                            rotationMultiplier = 0.0f;
                    }
                    else
                    {
                        for (int i = 0; i < PartCount; ++i)
                        {
                            PhysicsManager.Instance.RemoveCollider(parts[i].MyCollider);
                            PhysicsManager.Instance.RemovePhysicalObject(parts[i]);
                            parts[i].MyCollider = null;
                            parts[i].MyPhysicalObject = null;
                        }

                        ActualState = State.WAIT_FOR_CLEANUP;
                        currentDisappearTime = 0.0f;
                    }
                }
                else if(ActualState == State.WAIT_FOR_CLEANUP)
                {
                    currentDisappearTime += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
                    if(currentDisappearTime > DISAPPEAR_DELAY)
                    {
                        ActualState = State.CLEANUP;
                        currentDisappearTime = 0.0f;
                        List<CustomModel> mods = new List<CustomModel>();
                        for (int i = 0; i < PartCount; ++i)
                        {
                            int compCount = parts[i].Components.Count;
                            for(int j = 0; j < compCount; ++j)
                            {
                                if(parts[i].Components[j].GetType() == typeof(CustomModel))
                                {
                                    mods.Add((CustomModel)(parts[i].Components[j]));
                                }
                            }
                        }
                        int cc = mods.Count;
                        for (int i = 0; i < cc; ++i)
                        {
                            int matCount = mods[i].Mat.Count;
                            for(int j = 0; j < matCount; ++j)
                            {
                                this.partMaterials.Add(mods[i].Mat[j]);
                            }
                        }
                    }
                }
                else if(ActualState == State.CLEANUP)
                {
                    currentDisappearTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    int matCount = partMaterials.Count;
                    for (int i = 0; i < matCount; ++i )
                    {
                        partMaterials[i].Transparency = 1.0f - (currentDisappearTime / DISAPPEAR_TIME);
                    }

                    if (currentDisappearTime > DISAPPEAR_TIME)
                    {
                        ActualState = State.FINISHED;
                        Cleanup();
                    }
                }
                //else
                //{
                //    // TO JEST DO WYJEBANIA
                //    intersectionVector = new Vector3(0.1f, 0.0f, 0.0f);
                //    Shatter();4
                //}
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
            if(other.GetType() == typeof(LightPoint))
            {
                return;
            }
            if (ActualState == State.NORMAL)
            {
                PartHealth -= HitDamage;

                if (PartHealth <= 0)
                {
                    intersectionVector = other.MyTransform.PositionChangeNormal;

                    if (intersectionVector == Vector3.Zero)
                    {
                        intersectionVector = -MyObject.MyTransform.PositionChangeNormal;
                    }

                    if (intersectionVector == Vector3.Zero)
                    {
                        Debug.Log("DESTRUCTIBLE: IntersectionVector is still zero!!!!!");
                        intersectionVector = Vector3.Up;
                    }

                    //Debug.Log(intersectionVector.Length().ToString());

                    Shatter();
                }
            }

            //Debug.Log(MyObject.MyCollider.IntersectionVector.ToString());

            base.OnTriggerEnter(other);
        }

        private void GenerateParts()
        {
            Model mod;
            List<Material> mat;
            CustomModel cMod;
            DestructiblePart obj;

            for(uint i = 0; i < PartCount; ++i)
            {
                mod = ResourceManager.Instance.LoadModel(PartsPath + "/" + (i + 1).ToString());
                mat = ResourceManager.Instance.LoadBasicMaterialsFromModel(mod, ResourceManager.Instance.Effects[@"Effects\NormalEffect"]);
                obj = new DestructiblePart(MyObject.UniqueID + (uint)PartsPath.GetHashCode() + i + 1, MyObject.Name + "DestrPart" + (i + 1).ToString());
                cMod = new CustomModel(obj, new Model[] {mod, null, null}, mat);
                obj.Components.Add(cMod);
                obj.MyPhysicalObject = new PhysicalObject(obj, PART_MASS * ((float)(new Random().Next(5) + 5)/10.0f), PART_DRAGFACTOR, false);
                obj.MyTransform = new Transform(obj, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), MyObject.MyTransform.Scale * SCALE_FACTOR);
                obj.MyCollider = new BoxCollider(obj, false);
                obj.Dynamic = true;

                parts.Add(obj);
                MyObject.AddChild(obj);

                Vector3 offset = SCALE_FACTOR * Vector3.Transform(new Vector3(0.0f, 0.0f, 0.0f), mod.Meshes[0].ParentBone.Transform);
                offset.Z = -offset.Z;
                float temp = offset.Z;
                offset.Z = offset.Y;
                offset.Y = temp;
                //offset = Vector3.Normalize(offset);
                obj.Offset = offset;
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
                parts[i].MyCollider.IgnoredColliders.Add(ResourceManager.Instance.CurrentScene.ObjectsDictionary[1].MyCollider);    //player
            }

            this.MyObject.Visible = true;
            this.MyObject.ChildrenVisible = false;
        }

        private void Shatter()
        {
            // calculate force multiplier here
            forceMultiplier = FORCE_MULTIPLIER * intersectionVector.Length();
            //
            ActualState = State.SHATTERED;
            this.MyObject.Visible = false;
            this.MyObject.ChildrenVisible = true;
            for (int i = 0; i < PartCount; ++i)
            {
                parts[i].Enabled = true;
                parts[i].MyTransform.BakeTransformFromParent();
                this.MyObject.RemoveChildAddHigher(parts[i]);
                parts[i].MyPhysicalObject.IsUsingGravity = true;
                float temp = (1.0f / Math.Max((-intersectionVector - parts[i].Offset).Length(), 0.00001f));

                Vector3 rotationVec = Vector3.Zero;
                Random rand = new Random(500);
                rotationVec.X = ((float)rand.Next(2)) * (1.0f / temp) * ROTATION_BIAS;
                rotationVec.Y = ((float)rand.Next(2)) * (1.0f / temp) * ROTATION_BIAS;
                rotationVec.Z = ((float)rand.Next(2)) * (1.0f / temp) * ROTATION_BIAS;
                parts[i].Rotation = rotationVec;

                parts[i].MyPhysicalObject.AddForce((Vector3.Normalize(parts[i].Offset) + Vector3.Normalize(intersectionVector)) * forceMultiplier * temp);
            }
        }

        private void Cleanup()
        {
            this.Enabled = false;
            for(int i = 0; i < PartCount; ++i)
            {
                ResourceManager.Instance.CurrentScene.DeleteObjectRuntime(parts[i]);
                PhysicsManager.Instance.RemoveCollider(parts[i].MyCollider);
                PhysicsManager.Instance.RemovePhysicalObject(parts[i]);
            }
            this.partMaterials.Clear();
            this.parts.Clear();
        }

        #endregion
    }
}
