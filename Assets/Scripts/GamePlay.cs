using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
        public GameObject gameOverObject;

        [Header("Game Rules")]
        public int startBallsCount = 10;
        public int ballsPerTurn = 3;

        //////////////////////////////////////////////////////////////////////////
        enum State
        {
            Normal,
            AnimatePath,
            GameOver,
        }

        private State _state;
        private int _scoreCount;
        private readonly List<IntVector2> _freeCells = new List<IntVector2>();
        private GridBall[,] _ballsGrid;
        private GridBall _currentBall;
        private List<IntVector2> _movePath;

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

            if (!ballsHolder)
                ballsHolder = transform;

            for (int y = 0; y < grid.rowsCount; y++)
            {
                for (int x = 0; x < grid.colsCount; x++)
                {
                    var button = grid.GetCell(x, y).GetComponent<Button>();
                    int cellX = x;
                    int cellY = y;
                    button.onClick.AddListener(() => CellClick(cellX, cellY));
                }
            }

            _ballsGrid = new GridBall[grid.colsCount, grid.rowsCount];

            NewGame();
        }

        void Update()
        {
            if (_state != State.Normal)
                return;

            if (Input.GetKeyDown(KeyCode.N))
                NextTurn();
        }

        void UpdateScore()
        {
            if (scoreText)
                scoreText.text = _scoreCount.ToString();
        }

        void NewGame()
        {
            _state = State.Normal;
            _scoreCount = 0;
            _currentBall = null;
            UpdateScore();

            if (gameOverObject)
                gameOverObject.SetActive(false);

            _freeCells.Clear();
            for (int y = 0; y < grid.rowsCount; y++)
            {
                for (int x = 0; x < grid.colsCount; x++)
                    _freeCells.Add(new IntVector2(x, y));
            }

            foreach (var ball in _ballsGrid)
            {
                if (ball)
                    Destroy(ball.gameObject);
            }

            CreateBalls(startBallsCount);
        }

        void NextTurn()
        {
            CreateBalls(ballsPerTurn);
        }

        void CreateBalls(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (_freeCells.Count == 0)
                {
                    _state = State.GameOver;
                    if (gameOverObject)
                        gameOverObject.SetActive(true);
                    return;
                }

                int index = Random.Range(0, _freeCells.Count);
                var pos = _freeCells[index];
                _freeCells.RemoveAt(index);

                var cell = grid.GetCell(pos.x, pos.y);
                var go = Instantiate(ballPrefab, cell.transform.position, Quaternion.identity, ballsHolder.transform) as GameObject;
                var ball = go.GetComponent<GridBall>();
                ball.data = balls[Random.Range(0, balls.Length)];
                ball.onClick += () => BallClick(ball);
                _ballsGrid[pos.x, pos.y] = ball;
            }
        }

        void BallClick(GridBall ball)
        {
            if (_state != State.Normal || _currentBall == ball)
                return;

            if (_currentBall)
                _currentBall.selected = false;
            _currentBall = ball;
            if (_currentBall)
                _currentBall.selected = true;
        }

        void CellClick(int x, int y)
        {
            if (_state != State.Normal || _ballsGrid[x, y] || !_currentBall)
                return;

            _movePath.Clear();
            if (!MakePath(_currentBall.position, new IntVector2(x, y), _movePath))
                return;

            _state = State.AnimatePath;
        }

        bool MakePath(IntVector2 from, IntVector2 to, List<IntVector2> path)
        {
            return false;
        }

//         IEnumerator MovePathAnimation()
//         {
//         }
    }
}