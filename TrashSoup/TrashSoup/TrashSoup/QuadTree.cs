using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TrashSoup
{
    public class QuadTreeNode<T>
    {
        #region variables
        public T Parent { get; set; }
        public T Me { get; set; }
        public Rectangle MyRect { get; set; }
        public T[] Children = new T[4];
        #endregion
        #region methods
        public QuadTreeNode(T parent, T me, Rectangle rect)
        {
            this.Parent = parent;
            this.Me = me;
            this.MyRect = rect;
            for (int i = 0; i < 4; i++)
                Children[i] = default(T);
        }
        
        public QuadTreeNode(T parent, T me, Rectangle rect,
            T child01, T child02, T child03, T child04) : this(parent, me, rect)
        {
            Children[0] = child01;
            Children[1] = child02;
            Children[3] = child03;
            Children[4] = child04;
        }
        #endregion
    }

    public class QuadTree<T>
    {
        #region variables
        protected QuadTreeNode<T> root;
        #endregion

        #region methods
        public QuadTree()
        {
            // TODO: implement
        }

        public QuadTree(List<T> list)
        {
            // TODO: implement
        }

        public void Add(T obj)
        {
            // TODO: implement
        }

        public bool Remove(uint uniqueID)
        {
            // TOOD: implement
            return false;
        }

        public QuadTreeNode<T> GetRoot()
        {
            return root;
        }
        #endregion
    }
}
