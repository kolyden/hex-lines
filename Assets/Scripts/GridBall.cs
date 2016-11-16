using UnityEngine;
using UnityEngine.UI;

namespace Game.HexLines
{
    public class GridBall : MonoBehaviour
    {
        public Image image;
        public Animator animator;
        
        public BallData data
        {
            get { return _data; }
            set
            {
                if (_data == data)
                    return;

                _data = data;
                if (_data)
                    _data.Init(this);
            }
        }

        //////////////////////////////////////////////////////////////////////////
        private BallData _data;
    }
}