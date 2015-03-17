﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    public class CustomModel : ObjectComponent
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
        // material doesn't change with LOD, but with MeshPart !!!
        public List<Material> Mat { get; set; }

        #endregion

        #region methods

        public CustomModel(GameObject obj) : base(obj)
        {
            this.LODState = LODStateEnum.HI;
            this.LODs = new Model[LOD_COUNT];
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
                    Transform transform = myObject.MyTransform;
                    Matrix[] transforms = new Matrix[mod.Bones.Count];
                    mod.CopyAbsoluteBoneTransformsTo(transforms);

                    foreach (ModelMesh mm in mod.Meshes)
                    {
                        foreach (BasicEffect be in mm.Effects)
                        {
                            be.Projection = camera.ProjectionMatrix;
                            be.View = camera.ViewMatrix;
                            be.World = transform.GetWorldMatrix() /** mm.ParentBone.Transform*/;  // no parentBone transform yet!
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

        #endregion
    }
}