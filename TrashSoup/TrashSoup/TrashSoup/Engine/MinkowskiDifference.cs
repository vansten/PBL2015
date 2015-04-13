using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrashSoup.Engine
{
    public class MinkowskiDifference
    {
        public List<Vector3> Vertices = new List<Vector3>();

        public MinkowskiDifference()
        {

        }

        public void CalculateMikowskiDifference(Vector3[] shape1Verts, Vector3[] shape2Verts)
        {
            foreach(Vector3 v1 in shape1Verts)
            {
                foreach(Vector3 v2 in shape2Verts)
                {
                    this.Vertices.Add(v1 - v2);
                }
            }
        }

        public bool ContainsOrigin()
        {
            bool mXmYmZ, mXmYpZ, mXpYmZ, mXpYpZ, pXmYmZ, pXmYpZ, pXpYmZ, pXpYpZ;
            mXmYmZ = mXmYpZ = mXpYmZ = mXpYpZ = pXmYmZ = pXpYmZ = pXmYpZ = pXpYpZ = false;
            foreach(Vector3 v in this.Vertices)
            {
                if(v.X < 0.0f)
                {
                    if(v.Y < 0.0f)
                    {
                        if(v.Z < 0.0f)
                        {
                            mXmYmZ = true;
                        }
                        else
                        {
                            mXmYpZ = true;
                        }
                    }
                    else
                    {
                        if (v.Z < 0.0f)
                        {
                            mXpYmZ = true;
                        }
                        else
                        {
                            mXpYpZ = true;
                        }
                    }
                }
                else
                {
                    if (v.Y < 0.0f)
                    {
                        if (v.Z < 0.0f)
                        {
                            pXmYmZ = true;
                        }
                        else
                        {
                            pXmYpZ = true;
                        }
                    }
                    else
                    {
                        if (v.Z < 0.0f)
                        {
                            pXpYmZ = true;
                        }
                        else
                        {
                            pXpYpZ = true;
                        }
                    }
                }
            }

            return mXmYmZ && mXmYpZ && mXpYmZ && mXpYpZ && pXmYmZ && pXmYpZ && pXpYmZ && pXpYpZ;
        }

        public Vector3 CalculatePenetrationVector()
        {
            Vector3 penetrationVector = new Vector3();



            return penetrationVector;
        }
    }
}
