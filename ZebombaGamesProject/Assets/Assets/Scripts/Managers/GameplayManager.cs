using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.Events;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    public static UnityEvent<int> OnScoreUpdate = new();
    public static UnityEvent OnGameOver = new();

    [SerializeField] private List<GameObject> gameplayObjects; 
    [SerializeField] private List<GameObject> mainMenuObjects; 
    [SerializeField] private List<Transform> dropZones = new();
    [SerializeField] private Circle circlePrefab;

    public bool isTutorial { get; set; }
    public int Score {  get; set; }
    public InputSystemActions inputActions { get; private set; }

    private Circle[,] grid = new Circle[3, 3];
    private List<Circle> toDestroyList = new List<Circle>();

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (Instance == null)
        {
            Instance = this;
        }

        isTutorial = true;

        Score = 0;

        inputActions = new InputSystemActions();
        inputActions.Gameplay.Enable();
    }

    public Circle SpawnCircleRandom(Vector3 pos)
    {
        var circle = Instantiate(circlePrefab, pos, Quaternion.identity);
        circle.Initialize();
        return circle;
    }

    public int GetColumnFromPosition(Vector3 position)
    {
        float minDist = float.MaxValue;
        int column = 0;

        for (int i = 0; i < dropZones.Count; i++)
        {
            float dist = Mathf.Abs(position.x - dropZones[i].position.x);

            if (dist < minDist)
            {
                minDist = dist;
                column = i;
            }
        }

        return column;
    }

    public void TryDropCircle(Circle circle)
    {
        WaitAndAddToGrid(circle).Forget();
    }

    private void RemoveOverflowCircle(Circle circle)
    {
        circle.SetKinematic(RigidbodyType2D.Kinematic);

        Destroy(circle.gameObject);
    }

    private async UniTaskVoid WaitAndAddToGrid(Circle circle)
    {
        while (!circle.HasLanded)
        {
            await UniTask.Yield();
        }

        int column = GetColumnFromPosition(circle.transform.position);

        int freeRow = -1;
        for (int row = 0; row < 3; row++)
        {
            if (grid[row, column] == null)
            {
                freeRow = row;
                break;
            }
        }

        if (freeRow != -1)
        {
            grid[freeRow, column] = circle;
            circle.UpdateGridPos(freeRow, column);

            await CheckMatchesLoop();
        }
        else
        {
            RemoveOverflowCircle(circle);
        }
    }

    private async UniTask CheckMatchesLoop()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

        while (true)
        {
            toDestroyList.Clear();

            CheckHorizontalMatches();
            CheckVerticalMatches();
            CheckDiagonalMatches();

            if (toDestroyList.Count == 0)
            {
                if (IsGridFull())
                {
                    GameOver();
                }
                return;
            }
            else
            {
                foreach (var circle in toDestroyList)
                {
                    if (circle != null)
                    {
                        RemoveFromGrid(circle.GridPos.x, circle.GridPos.y);
                    }
                }

                ApplyGravity();
            }

            await WaitForAllCircles();
        }
    }

    private void RemoveFromGrid(int x, int y)
    {
        if (grid[x, y] != null)
        {
            Destroy(grid[x, y].gameObject);

            Score += grid[x, y].scoreByColor;

            grid[x, y] = null;

            OnScoreUpdate?.Invoke(Score);
        }
    }

    private async UniTask WaitForAllCircles()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        bool anyMoving;
        do
        {
            anyMoving = false;

            foreach (var circle in grid)
            {
                if (circle != null && !circle.IsSleeping())
                {
                    anyMoving = true;
                    break;
                }
            }

            await UniTask.Yield();

        } while (anyMoving);
    }

    private void CheckHorizontalMatches()
    {
        for (int x = 0; x < 3; x++)
        {
            if (grid[x, 0] && grid[x, 1] && grid[x, 2])
            {
                if (grid[x, 0].CircleColor == grid[x, 1].CircleColor && grid[x, 0].CircleColor == grid[x, 2].CircleColor)
                {
                    AddRowToDestroy(x);
                }
            }
        }
    }

    private void CheckVerticalMatches()
    {
        for (int y = 0; y < 3; y++)
        {
            if (grid[0, y] && grid[1, y] && grid[2, y])
            {
                if (grid[0, y].CircleColor == grid[1, y].CircleColor && grid[0, y].CircleColor == grid[2, y].CircleColor)
                {
                    AddColumnToDestroy(y);
                }
            }
        }
    }

    private void CheckDiagonalMatches()
    {
        if (grid[0, 0] && grid[1, 1] && grid[2, 2])
        {
            if (grid[0, 0].CircleColor == grid[1, 1].CircleColor && grid[0, 0].CircleColor == grid[2, 2].CircleColor)
            {
                toDestroyList.Add(grid[0, 0]);
                toDestroyList.Add(grid[1, 1]);
                toDestroyList.Add(grid[2, 2]);
            }
        }

        if (grid[2, 0] && grid[1, 1] && grid[0, 2])
        {
            if (grid[2, 0].CircleColor == grid[1, 1].CircleColor && grid[2, 0].CircleColor == grid[0, 2].CircleColor)
            {
                toDestroyList.Add(grid[2, 0]);
                toDestroyList.Add(grid[1, 1]);
                toDestroyList.Add(grid[0, 2]);
            }
        }
    }

    private void AddColumnToDestroy(int y)
    {
        for (int x = 0; x < 3; x++)
        {
            toDestroyList.Add(grid[x, y]);
        }
    }

    private void AddRowToDestroy(int x)
    {
        for (int y = 0; y < 3; y++)
        {
            toDestroyList.Add(grid[x, y]);
        }
    }

    private void ApplyGravity()
    {
        for (int column = 0; column < 3; column++)
        {
            int writeRow = 0;

            for (int row = 0; row < 3; row++)
            {
                if (grid[row, column] != null)
                {
                    if (row != writeRow)
                    {
                        Circle circle = grid[row, column];
                        grid[writeRow, column] = circle;
                        grid[row, column] = null;

                        circle.UpdateGridPos(writeRow, column);
                    }

                    writeRow++;
                }
            }
        }
    }

    private bool IsGridFull()
    {
        foreach (var cell in grid)
        {
            if (cell == null) return false;
        }

        return true;
    }

    public void Play()
    {
        foreach (var go in gameplayObjects)
        {
            go.SetActive(true);
        }

        foreach (var go in mainMenuObjects)
        {
            go.SetActive(false);
        }
    }

    private void GameOver()
    {
        Score = 0;

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (grid[x, y] != null)
                {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }
            }
        }

        foreach (var go in gameplayObjects)
        {
            go.SetActive(false);
        }

        OnScoreUpdate?.Invoke(Score);
        OnGameOver?.Invoke();
    }

    public void ReturnToMainMenu()
    {
        foreach (var go in mainMenuObjects)
        {
            go.SetActive(true);
        }
    }
}
