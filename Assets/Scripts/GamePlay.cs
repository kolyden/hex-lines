using UnityEngine;
using UnityEngine.UI;

namespace Game.HexLines
{
    public class GamePlay : MonoBehaviour
    {
        public HexGrid grid;
        public Text scoreText;

        public Button newGameBtn;

        public GameObject ballPrefab;
        public BallData[] balls;
        public Transform ballsHolder;

        [Header("Game Rules")]
        public int startBallsCount = 10;
        public int ballsPerTurn = 3;

        //////////////////////////////////////////////////////////////////////////
        private int _scoreCount;

        void Start()
        {
            if (ballPrefab == null || balls == null || balls.Length == 0)
            {
                Debug.LogError("Balls and(or) Ball Prefab not set in " + gameObject.name);
                return;
            }

            if (!grid)
            {
                Debug.LogError("Grid not set in " + gameObject.name);
                return;
            }

            if (newGameBtn)
                newGameBtn.onClick.AddListener(NewGame);

            if (!ballsHolder) ballsHolder = transform;

            NewGame();
        }

        void UpdateScore()
        {
            if (scoreText)
                scoreText.text = _scoreCount.ToString();
        }

        void NewGame()
        {
            _scoreCount = 0;
            UpdateScore();
            CreateBalls(startBallsCount);
        }

        void CreateBalls(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int x = Random.Range(0, grid.colsCount);
                int y = Random.Range(0, grid.rowsCount);

                var cell = grid.GetCell(x, y);
                var go = Instantiate(ballPrefab, cell.transform.position, Quaternion.identity, ballsHolder.transform) as GameObject;
                var ball = go.GetComponent<GridBall>();
                ball.data = balls[Random.Range(0, balls.Length)];
            }
        }
    }
}