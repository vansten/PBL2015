using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

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
            STATE_EMPTY = -1,
            STATE_FIRST,
            STATE_SECOND,
            STATE_THIRD
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
            5,  //25
            5,  //40
            // type2
            10,
            25,
            40
        };

        private static uint[] PartHealths = 
        {
            // type0
            250,
            250,
            250,
            // type1
            75,
            100,
            225,
            // type2
            75,
            325,
            200
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

        private GameObject triggerEnemyObj;
        private GameObject triggerPlayerObj;

        private HideoutStash stashComponent;

        private Texture2D interactionTexture;
        private Vector2 interactionTexturePos = new Vector2(0.475f, 0.775f);

        private int currentID;

        private bool playerInTrigger;
        private bool actionHelper;
        private bool soundHelper;

        private SoundEffectInstance buildSound;

        #endregion

        #region properties

        public int CurrentID 
        { 
            get
            {
                return currentID;
            }
            set
            {
                if(InRange(value) || value == -1)
                {
                    currentID = value;
                }
                else
                {
                    throw new InvalidOperationException("Value given to CurrentID is invalid!");
                }
            }
        }
        public int NextID
        {
            get
            {
                return CurrentID + 1;
            }
        }

        public FortificationPart[] Parts { get; private set; }
        public FortificationType MyType { get; set; }
        
        /// <summary>
        /// This property works only in runtime!!!
        /// </summary>
        public uint CurrentHealth 
        {
            get
            {
                return Parts[0].Health + Parts[1].Health + Parts[2].Health;
            }
            set
            {
                if (CurrentID == -1)
                    return;

                int difference = (int)value - (int)CurrentHealth;

                while(CurrentID >= 0)
                {
                    if (Parts[CurrentID].Health + difference >= 0 && Parts[CurrentID].Health + difference <= Parts[CurrentID].MaxHealth)
                    {
                        Parts[CurrentID].Health += (uint)difference;
                        return;
                    }
                    else if (Parts[CurrentID].Health + difference > Parts[CurrentID].MaxHealth)  // a miracle has happened!
                    {
                        Parts[CurrentID].Health = Parts[CurrentID].MaxHealth;
                        return;
                    }
                    else // destruction of one or more parts may occur
                    {
                        Parts[CurrentID].Health = 0;
                        difference += (int)Parts[CurrentID].Health;
                        DestroyCurrent();
                    }
                }
                
            }
        }

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
            this.CurrentID = ff.CurrentID;

            Start();
        }

        public override void Update(GameTime gameTime)
        {
            // solve building and fixing

            if(playerInTrigger && InRange(NextID))
            {
                GUIManager.Instance.DrawTexture(this.interactionTexture, this.interactionTexturePos, 0.05f, 0.05f);

                if (InputHandler.Instance.Action())
                {
                    if (actionHelper)
                    {
                        if(!soundHelper)
                        {
                            soundHelper = true;
                            buildSound.Play();
                        }
                        // fixing current one
                        if (InRange(CurrentID) && Parts[CurrentID].Health < Parts[CurrentID].MaxHealth)
                        {
                            Parts[CurrentID].BuildUp(gameTime);
                            //Debug.Log("HideoutStash: Fixing level " + CurrentID.ToString() + ", on " + Parts[CurrentID].Health.ToString() + "/" + Parts[CurrentID].MaxHealth.ToString() + " HP");

                            if(Parts[CurrentID].Health >= Parts[CurrentID].MaxHealth)
                            {
                                Parts[CurrentID].Health = Parts[CurrentID].MaxHealth;
                                actionHelper = false;
                            }
                        }
                        // building further if build is in progress
                        else if (Parts[NextID].State == FortificationPart.PartState.BUILDING)
                        {
                            // check if we built it
                            if (Parts[NextID].Health >= Parts[NextID].MaxHealth)
                            {
                                Debug.Log("HideoutStash: Fortification level " + NextID.ToString() + " has been built.");
                                Parts[NextID].Health = Parts[NextID].MaxHealth;
                                Parts[NextID].State = FortificationPart.PartState.BUILT;
                                ++CurrentID;
                                if (InRange(NextID))
                                {
                                    Parts[NextID].State = FortificationPart.PartState.NEXT_BUILD;
                                    Parts[NextID].MyObject.Visible = true;
                                }
                                    

                                actionHelper = false;
                            }
                            else
                            {
                                // update building of that part
                                Parts[NextID].BuildUp(gameTime);
                            }
                        }
                        // we dont need to fix nor no build is in progress - acquire new build
                        else
                        {
                            // check if we can even build
                            if (stashComponent.CurrentTrashFloat >= Parts[NextID].Price)
                            {
                                Parts[NextID].State = FortificationPart.PartState.BUILDING;
                            }
                            else
                            {
                                Debug.Log("HideoutStash: Haha nie stac cie");
                                actionHelper = false;
                            }
                        }
                    }
                    else
                    {
                        if (soundHelper)
                        {
                            soundHelper = false;
                            buildSound.Stop(true);
                        }
                    }
                }
                else
                {
                    if (soundHelper)
                    {
                        soundHelper = false;
                        buildSound.Stop(true);
                    }

                    actionHelper = true;
                }
            }
            else
            {
                if (soundHelper)
                {
                    soundHelper = false;
                    buildSound.Stop(true);
                }
            }

            

            /////////////////////////////////

            // solve attacking and damage


            //////////////////////////////

            // teting

            if(InputManager.Instance.GetKeyboardButtonDown(Microsoft.Xna.Framework.Input.Keys.OemPlus))
            {
                CurrentHealth += 10;
                Debug.Log("Fortification: New Health " + CurrentHealth.ToString());
            }

            if (InputManager.Instance.GetKeyboardButtonDown(Microsoft.Xna.Framework.Input.Keys.OemMinus))
            {
                CurrentHealth -= 10;
                Debug.Log("Fortification: New Health " + CurrentHealth.ToString());
            }
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, GameTime gameTime)
        {
        }

        protected override void Start()
        {
            Parts = new FortificationPart[PART_IN_TYPE_COUNT];
            MyType = (FortificationType)0;
            CurrentID = -1;
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

                foreach(Material mat in mMats)
                {
                    mat.Glossiness = PartGlosses[tN];
                }

                MyObject.AddChild(fortPart);

                CustomModel mod = new CustomModel(fortPart, new Model[] { ResourceManager.Instance.LoadModel(PartModels[tN]), null, null }, mMats);
                fortPart.Components.Add(mod);
                fortPart.MyTransform = new Transform(fortPart, PartTranslations[tN], Vector3.Up, PartRotations[tN], PartScales[tN]);
                fortPart.MyCollider = new BoxCollider(fortPart);

                FortificationPart p = new FortificationPart(fortPart);
                p.MaxHealth = PartHealths[tN];
                p.Health = 0;
                p.Price = PartPrices[tN];
                p.TimeToBuild = PartTimes[tN];
                fortPart.Components.Add(p);

                p.Initialize();

                p.State = FortificationPart.PartState.PENDING;
                Parts[i] = p;
            }

            if(CurrentID == (int)FortificationState.STATE_EMPTY)
            {
                Parts[0].State = FortificationPart.PartState.NEXT_BUILD;
            }
            else if (CurrentID == (int)FortificationState.STATE_FIRST)
            {
                Parts[0].State = FortificationPart.PartState.BUILT;
                Parts[0].Health = Parts[0].MaxHealth;
                Parts[1].State = FortificationPart.PartState.NEXT_BUILD;
            }
            else if (CurrentID == (int)FortificationState.STATE_SECOND)
            {
                Parts[0].State = FortificationPart.PartState.BUILT;
                Parts[0].Health = Parts[0].MaxHealth;
                Parts[1].State = FortificationPart.PartState.BUILT;
                Parts[1].Health = Parts[1].MaxHealth;
                Parts[2].State = FortificationPart.PartState.NEXT_BUILD;
            }
            else if (CurrentID == (int)FortificationState.STATE_THIRD)
            {
                Parts[0].State = FortificationPart.PartState.BUILT;
                Parts[0].Health = Parts[0].MaxHealth;
                Parts[1].State = FortificationPart.PartState.BUILT;
                Parts[1].Health = Parts[1].MaxHealth;
                Parts[2].State = FortificationPart.PartState.BUILT;
                Parts[2].Health = Parts[2].MaxHealth;
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
                triggerEnemyObj.MyCollider.IgnoredColliders.Add(Parts[i].MyObject.MyCollider);
                triggerPlayerObj.MyCollider.IgnoredColliders.Add(Parts[i].MyObject.MyCollider);
            }
            triggerEnemyObj.MyCollider.IgnoredColliders.Add(ResourceManager.Instance.CurrentScene.ObjectsDictionary[1].MyCollider);

            GameObject player = ResourceManager.Instance.CurrentScene.ObjectsDictionary[1];
            foreach(ObjectComponent comp in player.Components)
            {
                if(comp.GetType() == typeof(HideoutStash))
                {
                    stashComponent = (HideoutStash)comp;
                    break;
                }
            }

            SoundEffect se = TrashSoupGame.Instance.Content.Load<SoundEffect>(@"Audio/Character/walk");
            buildSound = se.CreateInstance();
            buildSound.IsLooped = true;

            base.Initialize();
        }

        private void OnTriggerEnterPlayerHandler(object sender, CollisionEventArgs e)
        {
            playerInTrigger = true;

            if (InRange(NextID) && 
                Parts[NextID].State == FortificationPart.PartState.NEXT_BUILD &&
                ((InRange(CurrentID) && Parts[CurrentID].Health == Parts[CurrentID].MaxHealth) || (!InRange(CurrentID)))) Parts[NextID].MyObject.Visible = true;
        }

        private void OnTriggerExitPlayerHandler(object sender, CollisionEventArgs e)
        {
            playerInTrigger = false;

            if (InRange(NextID) && Parts[NextID].State == FortificationPart.PartState.NEXT_BUILD) Parts[NextID].MyObject.Visible = false;
        }

        private void OnTriggerEnterEnemyHandler(object sender, CollisionEventArgs e)
        {
            
        }

        private void OnTriggerExitEnemyHandler(object sender, CollisionEventArgs e)
        {
            
        }

        private void DestroyCurrent()
        {
            Parts[CurrentID].Destroy();
            Parts[CurrentID].State = FortificationPart.PartState.NEXT_BUILD;
            if(InRange(NextID))
            {
                Parts[NextID].State = FortificationPart.PartState.PENDING;
                Parts[NextID].Health = 0;
            }
            if(playerInTrigger)
            {
                Parts[CurrentID].MyObject.Visible = true;
            }

            --CurrentID;
        }

        private bool InRange(int id)
        {
            if (id >= 0 && id < PART_IN_TYPE_COUNT)
            {
                return true;
            }
            else return false;
        }

        #endregion
    }
}
