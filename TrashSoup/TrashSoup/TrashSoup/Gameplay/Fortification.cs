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
            "Models/Home/Fortifications/trap_wnyki"
        };

        private static Vector3[] PartTranslations = 
        {
            // type0
            new Vector3(0.0f, 2.5f, 2.0f),
            new Vector3(0.0f, 0.55f, -0.2f),
            new Vector3(-1.4f, 0.0f, 0.7f),
            // type1
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 0.0f),
            // type2
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 0.0f)
        };

        private static Vector3[] PartRotations = 
        {
            // type0
            new Vector3(0.2f, MathHelper.PiOver2, MathHelper.PiOver2),
            new Vector3(0.0f, 0.15f, 0.0f),
            new Vector3(0.0f, MathHelper.Pi - 0.6f, 0.0f),
            // type1
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 0.0f),
            // type2
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 0.0f)
        };

        private static float[] PartScales = 
        {
            // type0
            3.5f,
            2.7f,
            2.0f,
             // type1
            1.0f,
            1.0f,
            1.0f,
             // type2
            1.0f,
            1.0f,
            1.0f
        };

        private static float[] PartSpeculars = 
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

        private List<Material> selectionMat;
        private List<Material> invisibleMat;
        private List<Material>[] currentMats;
        private CustomModel[] models;
        private GameObject triggerEnemyObj;
        private GameObject triggerPlayerObj;

        #endregion

        #region properties

        public FortificationType MyType { get; set; }
        public FortificationState CurrentState { get; set; }
        public uint CurrentHealth { get; private set; }

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
            Start();
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, GameTime gameTime)
        {
        }

        protected override void Start()
        {
            currentMats = new List<Material>[TYPE_COUNT];
            models = new CustomModel[TYPE_COUNT];
            MyType = (FortificationType)0;
            CurrentState = (FortificationState)0;

            selectionMat = new List<Material>();
            invisibleMat = new List<Material>();
        }

        public override void Initialize()
        {
            int typeNumber = (int)MyType;
            for (int i = 0; i < TYPE_COUNT; ++i )
            {
                GameObject fortPart = new GameObject(MyObject.UniqueID + (uint)MyObject.Name.GetHashCode() + (uint)i, MyObject.Name + "FortificationPart" + (i).ToString());

                int tN = i + TYPE_COUNT * typeNumber;

                List<Material> mMats = ResourceManager.Instance.LoadBasicMaterialsFromModel(
                    ResourceManager.Instance.LoadModel(PartModels[tN]), ResourceManager.Instance.LoadEffect(@"Effects\NormalEffect"));

                currentMats[i] = mMats;

                foreach(Material mat in mMats)
                {
                    mat.Glossiness = PartSpeculars[tN];
                }

                MyObject.AddChild(fortPart);

                fortPart.Components.Add(new CustomModel(fortPart, new Model[] { ResourceManager.Instance.LoadModel(PartModels[tN]), null, null }, mMats));
                fortPart.MyTransform = new Transform(fortPart, PartTranslations[tN], Vector3.Up, PartRotations[tN], PartScales[tN]);
                fortPart.MyCollider = new BoxCollider(fortPart);

            }

            base.Initialize();
        }

        #endregion
    }
}
