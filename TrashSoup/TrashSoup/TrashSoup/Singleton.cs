using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrashSoup
{
    class Singleton <T> where T : new()
    {
        private static T m_instance;
        private static object locker = new object();

        public static T Instance
        {
            get
            {
                if(m_instance == null)
                {
                    lock (locker)
                    {
                        if(m_instance == null)
                        {
                            m_instance = new T();
                        }
                    }
                }

                return m_instance;
            }
        }
    }
}
