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
        public float moveTime = 0.5f;

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

        private readonly List<IntVector2> _movePath = new List<IntVector2>();

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
            RemoveLines();
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
                ball.position = pos;
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

            var target = new IntVector2(x, y);
            if (!MakePath(_currentBall.position, target, _movePath))
                return;

            var pos = _currentBall.position;
            _ballsGrid[pos.x, pos.y] = null;
            _freeCells.Add(pos);
            _freeCells.Remove(target);
            StartCoroutine(MovePathAnimation());
        }

        // Breadth First Search Algorithm
       bool MakePath(IntVector2 start, IntVector2 target, List<IntVector2> path)
        {
            if (start == target)
                return false;

            var frontier = new List<IntVector2>() { start };
            var came_from = new Dictionary<IntVector2, IntVector2>();
            came_from[start] = start;

            while (frontier.Count > 0)
            {
                var current = frontier[0];
                frontier.RemoveAt(0);

                if (current == target)
                {
                    path.Add(current);
                    while (current != start)
                    {
                        current = came_from[current];
                        path.Add(current);
                    }
                    path.Reverse();
                    return true;
                }

                foreach (var next in GetCellNeighbors(current.x, current.y))
                {
                    if (!came_from.ContainsKey(next))
                    {
                        frontier.Add(next);
                        came_from[next] = current;
                    }
                }
            }

            return false;
        }

        IntVector2[] GetCellNeighbors(int x, int y)
        {
            var list = new List<IntVector2>();
            if (y % 2 != 0)
            {
                GetCell(x, y - 1, list);
                GetCell(x + 1, y - 1, list);

                GetCell(x - 1, y, list);
                GetCell(x + 1, y, list);

                GetCell(x, y + 1, list);
                GetCell(x + 1, y + 1, list);
            }
            else
            {
                GetCell(x - 1, y - 1, list);
                GetCell(x, y - 1, list);

                GetCell(x - 1, y, list);
                GetCell(x + 1, y, list);

                GetCell(x - 1, y + 1, list);
                GetCell(x, y + 1, list);
            }

            return list.ToArray();
        }

        bool IsCell(int x, int y)
        {
            if (x < 0 || y < 0 ||
                x >= grid.colsCount ||
                y >= grid.rowsCount)
                return false;

            return true;
        }

        void GetCell(int x, int y, List<IntVector2> list)
        {
            if (!IsCell(x, y))
                return;

            if (_ballsGrid[x, y] == null)
                list.Add(new IntVector2(x, y));
        }

        IEnumerator MovePathAnimation()
        {
            _state = State.AnimatePath;
            _currentBall.selected = false;

            while (_movePath.Count > 0)
            {
                float time = 0;
                Vector3 start = _currentBall.transform.position;
                Vector3 end = grid.GetCell(_movePath[0].x, _movePath[0].y).transform.position;

                while (time < moveTime)
                {
                    float t = time / moveTime;
                    _currentBall.transform.position = Vector3.Lerp(start, end, t);                                        
                    time += Time.deltaTime;
                    yield return null;
                }

                _currentBall.transform.position = end;
                _currentBall.position = _movePath[0];
                _movePath.RemoveAt(0);
            }

            var pos = _currentBall.position;
            _ballsGrid[pos.x, pos.y] = _currentBall;
            _currentBall.selected = false;
            _currentBall = null;
            _state = State.Normal;

            if (!RemoveLines())
                NextTurn();
        }

        bool RemoveLines()
        {
            var removeSet = new HashSet<GridBall>();

            // horizontal lines
            for (int y = 0; y < grid.rowsCount; y++)
            {
                int count = 0;
                BallData color = null;
                for (int x = 0; x < grid.colsCount; x++)
                {
                    var ball = _ballsGrid[x, y];
                    if (ball == null || ball.data != color)
                    {
                        if (count >= 5)
                        {
                            for (int i = x - count; i < x; i++)
                                removeSet.Add(_ballsGrid[i, y]);
                        }

                        if (ball == null)
                        {
                            color = null;
                            count = 0;
                        }
                        else
                        {
                            color = ball.data;
                            count = 1;
                        }
                    }
                    else count++;
                }

                if (count >= 5)
                {
                    for (int i = grid.colsCount - count; i < grid.colsCount; i++)
                        removeSet.Add(_ballsGrid[i, y]);
                }
            }

            // positive diagonal
            for (int x = 0; x < grid.colsCount; x++)
                RemovePositiveDiagonal(x, 0, removeSet);
            for (int y = 2; y < grid.rowsCount; y += 2)
                RemovePositiveDiagonal(0, y, removeSet);

            // negative diagonal
            for (int x = 0; x < grid.colsCount; x++)
                RemoveNegativeDiagonal(x, 0, removeSet);
            for (int y = 1; y < grid.rowsCount; y += 2)
                RemoveNegativeDiagonal(grid.colsCount - 1, y, removeSet);

            if (removeSet.Count == 0)
                return false;

            int N = removeSet.Count;
            _scoreCount += (N * N - 7 * N + 20);
            UpdateScore();

            foreach (var ball in removeSet)
            {
                var pos = ball.position;
                _freeCells.Add(pos);
                _ballsGrid[pos.x, pos.y] = null;
                ball.Hide();               
            }

            return true;
        }

        void RemovePositiveDiagonal(int x, int y, HashSet<GridBall> removeSet)
        {
            var list = new List<GridBall>();
            BallData color = null;

            while (IsCell(x, y))
            {
                var ball = _ballsGrid[x, y];
                if (ball == null || ball.data != color)
                {
                    if (list.Count >= 5)
                        removeSet.UnionWith(list);

                    list.Clear();

                    if (ball == null)
                        color = null;
                    else
                    {
                        color = ball.data;
                        list.Add(ball);
                    }
                }
                else list.Add(ball);

//                 if (_ballsGrid[x, y])
//                 {
//                     if (_ballsGrid[x,y].data != color)
//                         list.Clear();
// 
//                     list.Add(_ballsGrid[x, y]);
//                 }
//                 else if (list.Count > 0)
//                 {
//                     if (list.Count >= 5)
//                         removeSet.UnionWith(list);
// 
//                     list.Clear();
//                 }

                if (y % 2 != 0)
                {
                    x++;
                    y++;
                }
                else
                {
                    y++;
                }
            }

            if (list.Count >= 5)
                removeSet.UnionWith(list);
        }

        void RemoveNegativeDiagonal(int x, int y, HashSet<GridBall> removeSet)
        {
            var list = new List<GridBall>();
            BallData color = null;

            while (IsCell(x, y))
            {
                var ball = _ballsGrid[x, y];
                if (ball == null || ball.data != color)
                {
                    if (list.Count >= 5)
                        removeSet.UnionWith(list);

                    list.Clear();

                    if (ball == null)
                        color = null;
                    else
                    {
                        color = ball.data;
                        list.Add(ball);
                    }
                }
                else list.Add(ball);

                if (y % 2 != 0)
                {
                    y++;
                }
                else
                {
                    x--;
                    y++;
                }
            }

            if (list.Count >= 5)
                removeSet.UnionWith(list);
        }
    }
}