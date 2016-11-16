using UnityEngine;
using UnityEngine.UI;

namespace Game.HexLines
{
    public class GamePlay : MonoBehaviour
    {
        public HexGrid grid;
        public Text scoreText;

        public BallData[] balls;

        //////////////////////////////////////////////////////////////////////////
        private int _scoreCount;

        void Start()
        {
            UpdateScore();
        }

        void UpdateScore()
        {
            if (scoreText)
                scoreText.text = _scoreCount.ToString();
        }
    }
}