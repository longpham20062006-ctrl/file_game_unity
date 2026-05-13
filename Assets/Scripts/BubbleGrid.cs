using System.Collections.Generic;
using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 12;
    public int cols = 7;
    public float cellSize = 0.68f;
    public Vector2 startPosition = new Vector2(-2.04f, 4.15f);

    [Header("Start Bubbles")]
    public int startRows = 5;
    public int colorCount = 4;

    [Header("Attach Settings")]
    public float maxAttachDistanceMultiplier = 1.45f;

    [Header("References")]
    public BubbleFactory bubbleFactory;

    private Bubble[,] grid;

    public int LastDestroyedCount { get; private set; }
    public int LastDroppedCount { get; private set; }

    public int TotalBubbleCount
    {
        get
        {
            int count = 0;

            if (grid == null)
                return 0;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (grid[row, col] != null)
                        count++;
                }
            }

            return count;
        }
    }

    private void Start()
    {
        grid = new Bubble[rows, cols];
        CreateStartGrid();
    }

    private void CreateStartGrid()
    {
        if (bubbleFactory == null)
        {
            Debug.LogError("BubbleGrid thiếu BubbleFactory.");
            return;
        }

        startRows = Mathf.Clamp(startRows, 1, rows);
        colorCount = Mathf.Clamp(colorCount, 2, 6);

        for (int row = 0; row < startRows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int colorId = Random.Range(0, colorCount);
                Vector3 pos = GetWorldPosition(row, col);

                GameObject bubbleObj = bubbleFactory.CreateBubble(colorId, pos);
                Bubble bubble = bubbleObj.GetComponent<Bubble>();

                bubble.SetGridPosition(row, col);
                grid[row, col] = bubble;
            }
        }
    }

    public Vector3 GetWorldPosition(int row, int col)
    {
        float offsetX = row % 2 == 0 ? 0f : cellSize * 0.5f;

        float x = startPosition.x + col * cellSize + offsetX;
        float y = startPosition.y - row * cellSize * 0.86f;

        return new Vector3(x, y, 0f);
    }

    public void AttachBubble(Bubble bubble, Vector3 hitPosition)
    {
        LastDestroyedCount = 0;
        LastDroppedCount = 0;

        if (bubble == null)
            return;

        Vector2Int cell = GetNearestValidEmptyCell(hitPosition);

        if (cell.x == -1 || cell.y == -1)
        {
            Destroy(bubble.gameObject);
            return;
        }

        int row = cell.x;
        int col = cell.y;

        bubble.transform.position = GetWorldPosition(row, col);
        bubble.SetGridPosition(row, col);

        grid[row, col] = bubble;

        CheckMatch(row, col);

        if (LastDestroyedCount > 0)
        {
            DropDisconnectedBubbles();
        }
    }

    private Vector2Int GetNearestValidEmptyCell(Vector3 worldPosition)
    {
        float bestDistance = float.MaxValue;
        Vector2Int bestCell = new Vector2Int(-1, -1);

        float maxAttachDistance = cellSize * maxAttachDistanceMultiplier;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (grid[row, col] != null)
                    continue;

                if (!IsValidAttachCell(row, col))
                    continue;

                Vector3 cellPosition = GetWorldPosition(row, col);
                float distance = Vector3.Distance(worldPosition, cellPosition);

                if (distance > maxAttachDistance)
                    continue;

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestCell = new Vector2Int(row, col);
                }
            }
        }

        return bestCell;
    }

    private bool IsValidAttachCell(int row, int col)
    {
        if (!IsInside(row, col))
            return false;

        if (grid[row, col] != null)
            return false;

        if (row == 0)
            return true;

        foreach (Vector2Int n in GetNeighbors(row, col))
        {
            if (!IsInside(n.x, n.y))
                continue;

            if (grid[n.x, n.y] != null)
                return true;
        }

        return false;
    }

    private void CheckMatch(int row, int col)
    {
        Bubble startBubble = grid[row, col];

        if (startBubble == null)
            return;

        List<Bubble> matched = FindSameColorGroup(row, col, startBubble.colorId);

        if (matched.Count < 3)
            return;

        LastDestroyedCount = matched.Count;

        foreach (Bubble b in matched)
        {
            if (b == null)
                continue;

            if (IsInside(b.row, b.col))
                grid[b.row, b.col] = null;

            Destroy(b.gameObject);
        }
    }

    private List<Bubble> FindSameColorGroup(int startRow, int startCol, int colorId)
    {
        List<Bubble> result = new List<Bubble>();

        if (!IsInside(startRow, startCol))
            return result;

        bool[,] visited = new bool[rows, cols];

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startRow, startCol));
        visited[startRow, startCol] = true;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            int row = current.x;
            int col = current.y;

            Bubble bubble = grid[row, col];

            if (bubble == null)
                continue;

            if (bubble.colorId != colorId)
                continue;

            result.Add(bubble);

            foreach (Vector2Int neighbor in GetNeighbors(row, col))
            {
                int nr = neighbor.x;
                int nc = neighbor.y;

                if (!IsInside(nr, nc))
                    continue;

                if (visited[nr, nc])
                    continue;

                if (grid[nr, nc] == null)
                    continue;

                if (grid[nr, nc].colorId != colorId)
                    continue;

                visited[nr, nc] = true;
                queue.Enqueue(new Vector2Int(nr, nc));
            }
        }

        return result;
    }

    public void AddNewTopRow()
    {
        LastDestroyedCount = 0;
        LastDroppedCount = 0;

        if (grid == null || bubbleFactory == null)
            return;

        Bubble[,] newGrid = new Bubble[rows, cols];

        /*
         * Dịch bóng cũ xuống 1 hàng.
         * Không tạo lại bóng cũ.
         * Không đổi màu bóng cũ.
         */
        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < cols; col++)
            {
                Bubble bubble = grid[row, col];

                if (bubble == null)
                    continue;

                int newRow = row + 1;

                if (newRow >= rows)
                {
                    bubble.Drop();
                    LastDroppedCount++;
                    continue;
                }

                newGrid[newRow, col] = bubble;
                bubble.SetGridPosition(newRow, col);
                bubble.transform.position = GetWorldPosition(newRow, col);
            }
        }

        /*
         * Chỉ sinh hàng mới ở hàng 0.
         * Chỉ hàng mới này được random màu.
         */
        for (int col = 0; col < cols; col++)
        {
            int colorId = Random.Range(0, colorCount);
            Vector3 pos = GetWorldPosition(0, col);

            GameObject bubbleObj = bubbleFactory.CreateBubble(colorId, pos);
            Bubble bubble = bubbleObj.GetComponent<Bubble>();

            bubble.SetGridPosition(0, col);
            newGrid[0, col] = bubble;
        }

        grid = newGrid;

        DropDisconnectedBubbles();
    }

    public void MoveGridDown(float amount)
    {
        startPosition.y -= amount;

        if (grid == null)
            return;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (grid[row, col] == null)
                    continue;

                grid[row, col].transform.position = GetWorldPosition(row, col);
            }
        }
    }

    public void DropDisconnectedBubbles()
    {
        LastDroppedCount = 0;

        if (grid == null)
            return;

        bool[,] connectedToTop = new bool[rows, cols];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        /*
         * Bóng ở hàng 0 được xem là đang nối với trần.
         */
        for (int col = 0; col < cols; col++)
        {
            if (grid[0, col] == null)
                continue;

            connectedToTop[0, col] = true;
            queue.Enqueue(new Vector2Int(0, col));
        }

        /*
         * BFS tìm tất cả bóng còn liên kết với hàng trên cùng.
         */
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (Vector2Int n in GetNeighbors(current.x, current.y))
            {
                if (!IsInside(n.x, n.y))
                    continue;

                if (connectedToTop[n.x, n.y])
                    continue;

                if (grid[n.x, n.y] == null)
                    continue;

                connectedToTop[n.x, n.y] = true;
                queue.Enqueue(n);
            }
        }

        /*
         * Bóng nào không còn liên kết với hàng trên cùng thì rơi.
         */
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Bubble bubble = grid[row, col];

                if (bubble == null)
                    continue;

                if (connectedToTop[row, col])
                    continue;

                grid[row, col] = null;
                LastDroppedCount++;

                bubble.Drop();
            }
        }
    }

    private List<Vector2Int> GetNeighbors(int row, int col)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (row % 2 == 0)
        {
            neighbors.Add(new Vector2Int(row, col - 1));
            neighbors.Add(new Vector2Int(row, col + 1));
            neighbors.Add(new Vector2Int(row - 1, col - 1));
            neighbors.Add(new Vector2Int(row - 1, col));
            neighbors.Add(new Vector2Int(row + 1, col - 1));
            neighbors.Add(new Vector2Int(row + 1, col));
        }
        else
        {
            neighbors.Add(new Vector2Int(row, col - 1));
            neighbors.Add(new Vector2Int(row, col + 1));
            neighbors.Add(new Vector2Int(row - 1, col));
            neighbors.Add(new Vector2Int(row - 1, col + 1));
            neighbors.Add(new Vector2Int(row + 1, col));
            neighbors.Add(new Vector2Int(row + 1, col + 1));
        }

        return neighbors;
    }

    private bool IsInside(int row, int col)
    {
        return row >= 0 && row < rows && col >= 0 && col < cols;
    }
}