using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinningModelLibrary;

namespace TrashSoup.Engine
{
    public class CustomSkinnedModel : CustomModel
    {
        #region variables

        protected SkinningData skinningData;
        protected AnimationPlayer animationPlayer;
        protected string currentAnimString;

        #endregion

        #region properties
        #endregion

        #region methods

        public CustomSkinnedModel(GameObject obj) : base(obj)
        {
            
        }

        public CustomSkinnedModel(GameObject obj, Model[] lods, uint lodCount, List<Material> matList) : base(obj, lods, lodCount, matList)
        {
            if(lods[0] != null)
            {
                skinningData = lods[0].Tag as SkinningData;
                if (skinningData == null) throw new InvalidOperationException("LOD 0 doesn't contain skinning data tag");

                this.currentAnimString = skinningData.AnimationClips.Keys.ElementAt(0);

                animationPlayer = new AnimationPlayer(skinningData);

                animationPlayer.StartClip(skinningData.AnimationClips.Values.ElementAt(0));
            }
        }

        public override void Update(GameTime gameTime)
        {
            animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.Visible)
            {
                Model mod = LODs[(uint)LODState];
                if (mod != null)
                {
                    Camera camera = ResourceManager.Instance.CurrentScene.Cam;
                    Transform transform = myObject.MyTransform;
                    Matrix[] bones = animationPlayer.GetSkinTransforms();

                    foreach (ModelMesh mm in mod.Meshes)
                    {
                        foreach (SkinnedEffect be in mm.Effects)
                        {
                            be.SetBoneTransforms(bones);

                            be.Projection = camera.ProjectionMatrix;
                            be.View = camera.ViewMatrix;
                            be.World = mm.ParentBone.Transform * transform.GetWorldMatrix();
                            be.EnableDefaultLighting();
                        }

                        mm.Draw();
                    }
                }
            }
        }

        public void AddAnimation(KeyValuePair<string, AnimationClip> newClip)
        {
            this.skinningData.AnimationClips.Add(newClip.Key, newClip.Value);
        }

        public void SetCurrentAnim(string which)
        {
            AnimationClip clip;
            this.skinningData.AnimationClips.TryGetValue(which, out clip);
            if (clip == null) throw new InvalidOperationException("Animation name not found in AnimationClip dictionary");
            animationPlayer.StartClip(clip);
        }

        //protected void FlipNormals(Model model)
        //{
        //    ushort tmp;
        //    ushort[] indexTmp;
        //    foreach (ModelMesh mm in model.Meshes)
        //    {
        //        foreach(ModelMeshPart mp in mm.MeshParts)
        //        {
        //            indexTmp = new ushort[mp.IndexBuffer.IndexCount];
        //            mp.IndexBuffer.GetData(indexTmp);
        //            for(int i = 0; i < mp.IndexBuffer.IndexCount; i+=3)
        //            {
        //                tmp = indexTmp[i];
        //                indexTmp[i] = indexTmp[i + 2];
        //                indexTmp[i + 2] = tmp;
        //            }
        //            mp.IndexBuffer.SetData(indexTmp);
        //        }
        //    }
        //}
        #endregion
    }
}
