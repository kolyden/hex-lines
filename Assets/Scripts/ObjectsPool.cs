using UnityEngine;
using System.Collections.Generic;

namespace Game.HexLines
{
    public class ObjectsPool<T> where T : Component 
    {
        //////////////////////////////////////////////////////////////////////////
        private readonly List<T> _list = new List<T>();
    }
}
