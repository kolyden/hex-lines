using UnityEngine;
using UnityEngine.UI;

namespace Game.HexLines
{
    public class HexGrid : MonoBehaviour
    {
        [Range(5, 30)]
        public int rowsCount = 16;
        [Range(5, 30)]
        public int colsCount = 16;

        public Sprite gridSpr;

        public int cellWidth = 64;
        public int cellHeight = 64;

        public GameObject GetCell(int x, int y)
        {
            return _cells[x, y];
        }

        public Vector3 GetCellPosition(int x, int y)
        {
            return _cellOffset + new Vector3(
                (0.5f + x + (y % 2 == 0 ? 0 : 0.5f)) * cellWidth,
                -(0.5f + CellHeightAdvice * y) * cellHeight);
        }

        //////////////////////////////////////////////////////////////////////////
        private const float CellHeightAdvice = 0.75f;

        private Vector3 _cellOffset;
        private GameObject[,] _cells;

        void Start()
        {
            if (!gridSpr)
            {
                Debug.LogError("gridSpr property is not set in" + gameObject.name);
                return;
            }

            _cellOffset = new Vector3(
                -(colsCount / 2 + CellHeightAdvice) * cellWidth,
                rowsCount * cellHeight * CellHeightAdvice / 2);

            _cells = new GameObject[colsCount, rowsCount];

            for (int y = 0; y < rowsCount; y++)
            {
                for (int x = 0; x < colsCount; x++)
                    CreateCell(x, y);
            }
        }

        void CreateCell(int x, int y)
        {
            var child = new GameObject("cell(" + x + "," + y + ")");
            var tr = child.AddComponent<RectTransform>();
            tr.SetParent(transform, false);
            tr.localPosition = GetCellPosition(x, y);
            tr.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cellWidth);
            tr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cellHeight);

            var image = child.AddComponent<Image>();
            image.sprite = gridSpr;
            _cells[x, y] = child;
        }

    }
}
