using UnityEngine;

namespace Game.HexLines
{
    [CreateAssetMenu(fileName = "ColorBall", menuName = "Game/Color")]
    public class BallData : ScriptableObject
    {
        public Color color;
        public Sprite sprite;

        public void Init(GridBall ball)
        {
            if (ball.image)
            {
                ball.image.sprite = sprite;
                ball.image.color = color;
            }
        }
    }
}