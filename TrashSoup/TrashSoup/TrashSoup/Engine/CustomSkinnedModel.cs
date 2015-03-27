using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    public class CustomSkinnedModel : CustomModel
    {
        #region variables

        #endregion

        #region properties
        #endregion

        #region methods

        public CustomSkinnedModel(GameObject obj) : base(obj)
        {
            
        }

        public CustomSkinnedModel(GameObject obj, Model[] lods, uint lodCount, List<Material> matList) : base(obj, lods, lodCount, matList)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            if (this.Visible)
            {
                Model mod = LODs[(uint)LODState];
                if (mod != null)
                {
                    Camera camera = ResourceManager.Instance.CurrentScene.Cam;
                    Transform transform = MyObject.MyTransform;
                    Matrix[] bones = null;
                    if (MyObject.MyAnimator != null) bones = MyObject.MyAnimator.GetSkinTransforms();

                    foreach (ModelMesh mm in mod.Meshes)
                    {
                        foreach (SkinnedEffect be in mm.Effects)
                        {
                            if(bones != null) be.SetBoneTransforms(bones);

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
