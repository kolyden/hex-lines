using UnityEngine;
using UnityEngine.UI;

namespace Game.HexLines
{
    public class GridBall : MonoBehaviour
    {
        public delegate void Callback();

        public Image image;
        public Animator animator;
        public Button button;

        [HideInInspector]public IntVector2 position;
        
        public BallData data
        {
            get { return _data; }
            set
            {
                if (_data == value)
                    return;

                _data = value;
                if (_data)
                {
                    _data.Init(this);
                    if (animator)
                        animator.SetTrigger(_showTrigger);
                }
                else gameObject.SetActive(false);
            }
        }

        public bool selected
        {
            get { return _selected; }
            set
            {
                if (_selected == value)
                    return;

                _selected = value;
                if (animator)
                    animator.SetBool(_selectBool, _selected);
            }
        }

        public void Hide()
        {
            if (animator)
                animator.SetTrigger(_hideTrigger);
        }

        public event Callback onClick;

        //////////////////////////////////////////////////////////////////////////
        private static int _showTrigger = Animator.StringToHash("Show");
        private static int _hideTrigger = Animator.StringToHash("Hide");
        private static int _selectBool = Animator.StringToHash("Selected");

        private BallData _data;
        private bool _selected;

        void Start()
        {
            if (button)
                button.onClick.AddListener(ButtonClick);
        }

        void ButtonClick()
        {
            if (onClick != null)
                onClick();
        }
    }
}