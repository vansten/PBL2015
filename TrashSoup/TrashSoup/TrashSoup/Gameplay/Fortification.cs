using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Gameplay
{
    public class Fortification : ObjectComponent
    {
        #region constants

        private const int TYPE_COUNT = 3;
        private const int PART_IN_TYPE_COUNT = 3;

        #endregion

        #region enums

        public enum FortificationType
        {
            METAL_WIRE_SNARES,
            WOOD1_WIRE_SNARES,
            WOOD2_TRAPWIRE_SNARES
        }

        public enum FortificationState
        {
            STATE_EMPTY,
            STATE_FIRST,
            STATE_SECOND,
            STATE_THIRD
        }

        private enum PartState
        {
            NEXT_BUILD,
            PENDING,
            BUILDING,
            BUILT
        }

        #endregion

        #region variables

        private static uint[] PartPrices = 
        {
            // type0
            100,
            200,
            500,
            // type1
            50,
            150,
            350,
            // type2
            75,
            200,
            400
        };

        private static uint[] PartTimes = 
        {
            // type0
            20,
            30,
            40,
            // type1
            5,
            25,
            40,
            // type2
            10,
            25,
            40
        };

        private static uint[] PartHealths = 
        {
            // type0
            250,
            500,
            750,
            // type1
            75,
            175,
            500,
            // type2
            75,
            400,
            650
        };

        private static string[] PartModels = 
        {
            // type0
            "Models/Home/Fortifications/metal_fortif",
            "Models/Home/Fortifications/wire_fortif",
            "Models/Home/Fortifications/trap_wnyki_double",
            // type1
            "Models/Home/Fortifications/wood_fortif",
            "Models/Home/Fortifications/wire_fortif",
            "Models/Home/Fortifications/trap_wnyki",
            // type2
            "Models/Home/Fortifications/wood_fortif2",
            "Models/Home/Fortifications/trap_wire",
            "Models/Home/Fortifications/trap_wnyki_double"
        };

        private static Vector3[] PartTranslations = 
        {
            // type0
            new Vector3(0.0f, 2.5f, 0.0f),
            new Vector3(0.0f, 0.55f, -2.2f),
            new Vector3(-1.4f, 0.0f, -2.7f),
            // type1
            new Vector3(0.0f, 2.5f, 0.0f),
            new Vector3(-0.3f, 0.55f, -0.75f),
            new Vector3(0.6f, 0.0f, -1.7f),
            // type2
            new Vector3(0.0f, 2.7f, 0.0f),
            new Vector3(0.0f, 1.5f, -0.1f),
            new Vector3(1.5f, 0.0f, -0.7f)
        };

        private static Vector3[] PartRotations = 
        {
            // type0
            new Vector3(0.2f, MathHelper.PiOver2, MathHelper.PiOver2),
            new Vector3(0.0f, 0.15f, 0.0f),
            new Vector3(0.0f, MathHelper.Pi - 0.6f, 0.0f),
            // type1
            new Vector3(0.0f, MathHelper.PiOver2, 0.0f),
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(0.0f, MathHelper.PiOver4, 0.0f),
            // type2
            new Vector3(0.0f, MathHelper.PiOver2, 0.0f),
            new Vector3(0.0f, 0.0f, 0.2f),
            new Vector3(0.0f, 0.3f, 0.0f)
        };

        private static float[] PartScales = 
        {
            // type0
            3.5f,
            2.7f,
            2.0f,
             // type1
            3.0f,
            2.7f,
            2.0f,
             // type2
            4.0f,
            7.5f,
            2.0f
        };

        private static float[] PartGlosses = 
        {
            // type0
            0.8f,
            0.5f,
            0.5f,
             // type1
            0.2f,
            0.5f,
            0.5f,
             // type2
            0.2f,
            0.5f,
            0.5f
        };

        private static Vector3 NotBuiltColor = new Vector3(0.8f, 0.8f, 1.0f);
        private static Vector3 BuiltColor = new Vector3(0.1f, 1.0f, 0.2f);

        private List<Material> selectionMat;
        private List<Material>[] currentMats;
        private CustomModel[] models;
        private GameObject[] objects;
        private PartState[] states;
        private GameObject triggerEnemyObj;
        private GameObject triggerPlayerObj;

        private int currentlyBuildingID = -1;
        private float currentlyBuildingProgress = 0;
        private bool playerInTrigger = false;
        private bool buildInProgress = false;
        private uint currentMaxTime;

        private Texture2D interactionTexture;
        private Vector2 interactionTexturePos = new Vector2(0.475f, 0.775f);

        #endregion

        #region properties

        public FortificationType MyType { get; set; }
        public FortificationState CurrentState { get; set; }
        public uint CurrentHealth { get; set; }

        #endregion

        #region methods

        public Fortification(GameObject mObj)
            : base(mObj)
        {
            Start();
        }

        public Fortification(GameObject mObj, Fortification ff)
            : base(mObj, ff)
        {
            this.MyType = ff.MyType;
            this.CurrentHealth = ff.CurrentHealth;
            this.CurrentState = ff.CurrentState;

            Start();
        }

        public override void Update(GameTime gameTime)
        {
            // building
            if(playerInTrigger)
            {
                GUIManager.Instance.DrawTexture(this.interactionTexture, this.interactionTexturePos, 0.05f, 0.05f);

                if(InputHandler.Instance.Action())
                {
                    if(buildInProgress)
                    {
                        // have we finished?
                        if((uint)currentlyBuildingProgress >= currentMaxTime)
                        {
                            // reset everything and change state to upper
                            Debug.Log("Built!");
                        }
                        else
                        {
                            currentlyBuildingProgress += (float)(gameTime.ElapsedGameTime.TotalSeconds);
                            float lerpFactor = (float)currentlyBuildingProgress / (float)currentMaxTime;
                            selectionMat[0].DiffuseColor = Vector3.Lerp(NotBuiltColor, BuiltColor, lerpFactor);
                        }
                    }
                    else
                    {
                        // get object ID we want to build
                        for(int i = 0; i < PART_IN_TYPE_COUNT; ++i)
                        {
                            if(states[i] == PartState.NEXT_BUILD)
                            {
                                states[i] = PartState.BUILDING;
                                currentlyBuildingID = i;
                                break;
                            }
                        }
                        currentlyBuildingProgress = 0;  // reset progress
                        buildInProgress = true;
                        currentMaxTime = PartTimes[currentlyBuildingID + TYPE_COUNT * (int)MyType];
                    }
                }
            }
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, GameTime gameTime)
        {
        }

        protected override void Start()
        {
            currentMats = new List<Material>[PART_IN_TYPE_COUNT];
            models = new CustomModel[PART_IN_TYPE_COUNT];
            objects = new GameObject[PART_IN_TYPE_COUNT];
            states = new PartState[PART_IN_TYPE_COUNT];
            MyType = (FortificationType)0;
            CurrentState = (FortificationState)0;

            selectionMat = new List<Material>();
        }

        public override void Initialize()
        {
            int typeNumber = (int)MyType;

            this.interactionTexture = ResourceManager.Instance.LoadTexture(@"Textures/HUD/x_button");

            for (int i = 0; i < PART_IN_TYPE_COUNT; ++i )
            {
                GameObject fortPart = new GameObject(MyObject.UniqueID + (uint)MyObject.Name.GetHashCode() + (uint)i, MyObject.Name + "FortificationPart" + (i).ToString());

                int tN = i + TYPE_COUNT * typeNumber;

                List<Material> mMats = ResourceManager.Instance.LoadBasicMaterialsFromModel(
                    ResourceManager.Instance.LoadModel(PartModels[tN]), ResourceManager.Instance.LoadEffect(@"Effects\NormalEffect"));

                currentMats[i] = mMats;

                foreach(Material mat in mMats)
                {
                    mat.Glossiness = PartGlosses[tN];
                }

                MyObject.AddChild(fortPart);
                objects[i] = fortPart;

                models[i] = new CustomModel(fortPart, new Model[] { ResourceManager.Instance.LoadModel(PartModels[tN]), null, null }, mMats);
                fortPart.Components.Add(models[i]);
                fortPart.MyTransform = new Transform(fortPart, PartTranslations[tN], Vector3.Up, PartRotations[tN], PartScales[tN]);
                fortPart.MyCollider = new BoxCollider(fortPart);

                states[i] = PartState.PENDING;
            }

            // mats!
            Material selMat = new Material(MyObject.Name + "FortificationSelectionMat", ResourceManager.Instance.LoadEffect(@"Effects\DefaultEffect"));
            selMat.DiffuseMap = ResourceManager.Instance.Textures["DefaultDiffuseWhite"];
            selMat.DiffuseColor = NotBuiltColor;
            selMat.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
            selMat.Transparency = 0.25f;
            selectionMat.Add(selMat);

            if(CurrentState == FortificationState.STATE_EMPTY)
            {
                if(CurrentHealth != 0)
                {
                    CurrentHealth = 0;
                }

                states[0] = PartState.NEXT_BUILD;
            }
            else if(CurrentState == FortificationState.STATE_FIRST)
            {
                if(CurrentHealth == 0)
                {
                    CurrentHealth = PartHealths[TYPE_COUNT * typeNumber];
                }

                states[0] = PartState.BUILT;
                states[1] = PartState.NEXT_BUILD;
            }
            else if(CurrentState == FortificationState.STATE_SECOND)
            {
                if (CurrentHealth == 0)
                {
                    CurrentHealth = PartHealths[TYPE_COUNT * typeNumber] + PartHealths[TYPE_COUNT * typeNumber + 1];
                }

                states[0] = PartState.BUILT;
                states[1] = PartState.BUILT;
                states[2] = PartState.NEXT_BUILD;
            }
            else if(CurrentState == FortificationState.STATE_THIRD)
            {
                if (CurrentHealth == 0)
                {
                    CurrentHealth = PartHealths[TYPE_COUNT * typeNumber] + PartHealths[TYPE_COUNT * typeNumber + 1] + PartHealths[TYPE_COUNT * typeNumber + 2];
                }

                states[0] = PartState.BUILT;
                states[1] = PartState.BUILT;
                states[2] = PartState.BUILT;
            }

            for (int i = 0; i < PART_IN_TYPE_COUNT; ++i)
            {
                if(states[i] == PartState.BUILT)
                {
                    objects[i].Visible = true;
                    objects[i].MyCollider.Enabled = true;
                }
                else
                {
                    objects[i].Visible = false;
                    objects[i].MyCollider.Enabled = false;
                    models[i].Mat = selectionMat;
                }
            }

            triggerEnemyObj = new GameObject(MyObject.UniqueID + (uint)MyObject.Name.GetHashCode() + (uint)PART_IN_TYPE_COUNT, MyObject.Name + "FortificationTriggerEnemy");

            MyObject.AddChild(triggerEnemyObj);

            triggerEnemyObj.MyTransform = new Transform(triggerEnemyObj);
            triggerEnemyObj.MyTransform.Position = new Vector3(0.0f, 0.0f, 2.0f);
            triggerEnemyObj.MyTransform.Scale = 2.0f;
            triggerEnemyObj.MyCollider = new BoxCollider(triggerEnemyObj, true);

            triggerEnemyObj.OnTriggerEnterEvent += new GameObject.OnTriggerEnterEventHandler(OnTriggerEnterEnemyHandler);
            triggerEnemyObj.OnTriggerExitEvent += new GameObject.OnTriggerExitEventHandler(OnTriggerExitEnemyHandler);

            ///

            triggerPlayerObj = new GameObject(MyObject.UniqueID + (uint)MyObject.Name.GetHashCode() + (uint)PART_IN_TYPE_COUNT + 1, MyObject.Name + "FortificationTriggerPlayer");

            MyObject.AddChild(triggerPlayerObj);

            triggerPlayerObj.MyTransform = new Transform(triggerPlayerObj);
            triggerPlayerObj.MyTransform.Position = new Vector3(0.0f, 0.0f, -2.0f);
            triggerPlayerObj.MyTransform.Scale = 3.0f;
            triggerPlayerObj.MyCollider = new SphereCollider(triggerPlayerObj, true);

            triggerPlayerObj.OnTriggerEnterEvent += new GameObject.OnTriggerEnterEventHandler(OnTriggerEnterPlayerHandler);
            triggerPlayerObj.OnTriggerExitEvent += new GameObject.OnTriggerExitEventHandler(OnTriggerExitPlayerHandler);

            for (int i = 0; i < PART_IN_TYPE_COUNT; ++i )
            {
                triggerEnemyObj.MyCollider.IgnoredColliders.Add(objects[i].MyCollider);
                triggerPlayerObj.MyCollider.IgnoredColliders.Add(objects[i].MyCollider);
            }
            triggerEnemyObj.MyCollider.IgnoredColliders.Add(ResourceManager.Instance.CurrentScene.ObjectsDictionary[1].MyCollider);
            

            base.Initialize();
        }

        private void OnTriggerEnterPlayerHandler(object sender, CollisionEventArgs e)
        {
            playerInTrigger = true;

            for(int i = 0; i < PART_IN_TYPE_COUNT; ++i)
            {
                if(states[i] == PartState.NEXT_BUILD)
                {
                    objects[i].Visible = true;
                }
            }
        }

        private void OnTriggerExitPlayerHandler(object sender, CollisionEventArgs e)
        {
            playerInTrigger = false;

            for (int i = 0; i < PART_IN_TYPE_COUNT; ++i)
            {
                if (states[i] == PartState.NEXT_BUILD)
                {
                    objects[i].Visible = false;
                }
            }
        }

        private void OnTriggerEnterEnemyHandler(object sender, CollisionEventArgs e)
        {
            
        }

        private void OnTriggerExitEnemyHandler(object sender, CollisionEventArgs e)
        {
            
        }

        #endregion
    }
}
