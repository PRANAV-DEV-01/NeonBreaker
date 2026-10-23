using System.Collections;
using System.Collections.Generic;
using NeonBreaker.Core;
using UnityEngine;

namespace NeonBreaker.Gameplay
{
    [DefaultExecutionOrder(-50)]
    public class BrickManager : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private int columns = 9;
        [SerializeField] private float columnSpacing = 1.12f;
        [SerializeField] private float rowSpacing = 0.68f;
        [SerializeField] private float firstRowY = 7.0f;
        [SerializeField] private float dangerLineY = -5.05f;
        [SerializeField] private float brickHeight = 0.56f;
        [SerializeField] private float descendDuration = 0.28f;

        private static readonly string[] ColorNames =
        {
            "cyan", "magenta", "orange", "green", "purple", "blue", "red"
        };

        private static readonly Color[] Palette =
        {
            new Color(0.21f, 0.94f, 1.00f),
            new Color(1.00f, 0.24f, 0.80f),
            new Color(1.00f, 0.70f, 0.24f),
            new Color(0.42f, 1.00f, 0.56f),
            new Color(0.65f, 0.48f, 1.00f),
            new Color(0.30f, 0.61f, 1.00f),
            new Color(1.00f, 0.35f, 0.43f)
        };

        private readonly List<Brick> bricks = new List<Brick>();
        private readonly Dictionary<string, Sprite> normalSprites = new Dictionary<string, Sprite>();
        private Sprite toughSprite;
        private Sprite bombSprite;
        private Sprite powerSprite;
        private Transform brickParent;
        private PowerUpManager powerUpManager;

        public int BrickCount => bricks.Count;
        public bool IsBusy { get; set; }
        public float DangerLineY => dangerLineY;

        private void Awake()
        {
            foreach (string colorName in ColorNames)
            {
                Sprite sprite = Resources.Load<Sprite>("Art/brick_" + colorName);
                if (sprite != null)
                {
                    normalSprites[colorName] = sprite;
                }
            }

            toughSprite = Resources.Load<Sprite>("Art/brick_tough");
            bombSprite = Resources.Load<Sprite>("Art/brick_bomb");
            powerSprite = Resources.Load<Sprite>("Art/brick_power");
        }

        public void GenerateLevel(int level)
        {
            ClearBricks();

            if (brickParent == null)
            {
                GameObject parentObject = new GameObject("Bricks");
                parentObject.transform.SetParent(transform);
                brickParent = parentObject.transform;
            }

            int rowCount = Mathf.Clamp(3 + Mathf.CeilToInt(level * 0.55f), 4, 8);
            int pattern = (level - 1) % 6;
            System.Random random = new System.Random(level * 7919 + Random.Range(0, 100000));

            List<Vector2Int> cells = BuildCells(rowCount, pattern, random);
            if (cells.Count < 16)
            {
                cells = FillFallbackCells(rowCount, random);
            }

            int powerUpCount = 1 + (level >= 5 ? 1 : 0);
            int bombCount = Mathf.Clamp(1 + level / 4, 1, 4);
            HashSet<int> powerUpIndices = PickIndices(cells.Count, powerUpCount, random);
            HashSet<int> bombIndices = PickIndicesExcluding(cells.Count, bombCount, random, powerUpIndices);

            double toughChance = Mathf.Clamp01(0.05f + level * 0.022f);
            float center = (columns - 1) * 0.5f;

            for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
            {
                Vector2Int cell = cells[cellIndex];
                BrickType type = BrickType.Normal;

                if (powerUpIndices.Contains(cellIndex))
                {
                    type = BrickType.PowerUp;
                }
                else if (bombIndices.Contains(cellIndex))
                {
                    type = BrickType.Bomb;
                }
                else if (random.NextDouble() < toughChance)
                {
                    type = BrickType.Tough;
                }

                int colorIndex = (cell.y + cell.x / 2) % ColorNames.Length;
                SpawnBrick(cell, center, level, type, colorIndex, random);
            }

            GameEvents.RaiseBricksChanged(bricks.Count);
        }

        private List<Vector2Int> BuildCells(int rowCount, int pattern, System.Random random)
        {
            List<Vector2Int> cells = new List<Vector2Int>();
            int center = columns / 2;

            if (pattern == 5)
            {
                for (int row = 0; row < rowCount; row++)
                {
                    for (int col = 0; col <= center; col++)
                    {
                        if (random.NextDouble() >= 0.72)
                        {
                            continue;
                        }

                        cells.Add(new Vector2Int(col, row));
                        int mirroredColumn = columns - 1 - col;
                        if (mirroredColumn != col)
                        {
                            cells.Add(new Vector2Int(mirroredColumn, row));
                        }
                    }
                }

                return cells;
            }

            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    bool place;

                    switch (pattern)
                    {
                        case 0:
                            place = random.NextDouble() > 0.06;
                            break;
                        case 1:
                            place = (row + col) % 2 == 0;
                            break;
                        case 2:
                            place = Mathf.Abs(col - center) <= row + 1;
                            break;
                        case 3:
                            place = Mathf.Abs(col - center) + Mathf.Abs(row - (rowCount - 1) * 0.5f) <= 3.6f;
                            break;
                        case 4:
                            place = (row + col) % 4 != 0;
                            break;
                        default:
                            place = true;
                            break;
                    }

                    if (place)
                    {
                        cells.Add(new Vector2Int(col, row));
                    }
                }
            }

            return cells;
        }

        private List<Vector2Int> FillFallbackCells(int rowCount, System.Random random)
        {
            List<Vector2Int> cells = new List<Vector2Int>();
            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (random.NextDouble() < 0.7)
                    {
                        cells.Add(new Vector2Int(col, row));
                    }
                }
            }

            return cells;
        }

        private HashSet<int> PickIndices(int count, int amount, System.Random random)
        {
            HashSet<int> indices = new HashSet<int>();
            while (indices.Count < amount && indices.Count < count)
            {
                indices.Add(random.Next(0, count));
            }

            return indices;
        }

        private HashSet<int> PickIndicesExcluding(int count, int amount, System.Random random, HashSet<int> excluded)
        {
            HashSet<int> indices = new HashSet<int>();
            int attempts = 0;

            while (indices.Count < amount && indices.Count < count && attempts < count * 8)
            {
                int candidate = random.Next(0, count);
                attempts++;

                if (!excluded.Contains(candidate))
                {
                    indices.Add(candidate);
                }
            }

            return indices;
        }

        private void SpawnBrick(Vector2Int cell, float center, int level, BrickType type, int colorIndex, System.Random random)
        {
            float x = (cell.x - center) * columnSpacing;
            float y = firstRowY - cell.y * rowSpacing;

            Sprite sprite;
            Color brickColor;
            int hitPoints;
            int scoreValue;

            switch (type)
            {
                case BrickType.Tough:
                    sprite = toughSprite;
                    brickColor = new Color(0.72f, 0.79f, 0.9f);
                    hitPoints = 2 + (level >= 7 && random.NextDouble() < 0.35 ? 1 : 0);
                    scoreValue = 30;
                    break;
                case BrickType.Bomb:
                    sprite = bombSprite;
                    brickColor = new Color(1f, 0.35f, 0.25f);
                    hitPoints = 1;
                    scoreValue = 40;
                    break;
                case BrickType.PowerUp:
                    sprite = powerSprite;
                    brickColor = Palette[1];
                    hitPoints = 1;
                    scoreValue = 20;
                    break;
                default:
                    string colorName = ColorNames[colorIndex];
                    sprite = normalSprites.TryGetValue(colorName, out Sprite cached) ? cached : normalSprites["cyan"];
                    brickColor = Palette[colorIndex];
                    hitPoints = 1;
                    scoreValue = 10;
                    break;
            }

            GameObject brickObject = new GameObject("Brick_" + cell.y + "_" + cell.x);
            brickObject.transform.SetParent(brickParent);
            brickObject.transform.position = new Vector3(x, y, 0f);

            brickObject.AddComponent<SpriteRenderer>();
            brickObject.AddComponent<BoxCollider2D>();
            Brick brick = brickObject.AddComponent<Brick>();
            brick.Initialize(this, type, hitPoints, sprite, brickColor, scoreValue);

            bricks.Add(brick);
        }

        public void NotifyBrickRemoved(Brick brick)
        {
            bricks.Remove(brick);
            GameEvents.RaiseBricksChanged(bricks.Count);
        }

        public void ExplodeAt(Vector2 position, float radius)
        {
            GameEvents.RaiseBombExploded(position);

            Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius);
            foreach (Collider2D hit in hits)
            {
                Brick brick = hit.GetComponent<Brick>();
                if (brick != null && !brick.IsDestroyed)
                {
                    brick.TakeHit(1);
                }
            }
        }

        public void DropPowerUp(Vector2 position)
        {
            if (powerUpManager == null)
            {
                powerUpManager = FindAnyObjectByType<PowerUpManager>();
            }

            if (powerUpManager != null)
            {
                powerUpManager.SpawnDrop(position);
            }
        }

        public IEnumerator DescendRoutine()
        {
            List<Brick> snapshot = new List<Brick>(bricks);
            List<Vector3> startPositions = new List<Vector3>(snapshot.Count);

            foreach (Brick brick in snapshot)
            {
                startPositions.Add(brick != null ? brick.transform.position : Vector3.zero);
            }

            float elapsed = 0f;
            while (elapsed < descendDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / descendDuration);
                float eased = progress * progress;

                for (int i = 0; i < snapshot.Count; i++)
                {
                    if (snapshot[i] != null)
                    {
                        snapshot[i].transform.position = startPositions[i] + Vector3.down * (rowSpacing * eased);
                    }
                }

                yield return null;
            }

            for (int i = 0; i < snapshot.Count; i++)
            {
                if (snapshot[i] != null)
                {
                    snapshot[i].transform.position = startPositions[i] + Vector3.down * rowSpacing;
                }
            }
        }

        public bool HasBrickBelowDangerLine()
        {
            float halfHeight = brickHeight * 0.5f;
            foreach (Brick brick in bricks)
            {
                if (brick != null && brick.transform.position.y - halfHeight <= dangerLineY)
                {
                    return true;
                }
            }

            return false;
        }

        public void ClearBricks()
        {
            foreach (Brick brick in bricks)
            {
                if (brick != null)
                {
                    Destroy(brick.gameObject);
                }
            }

            bricks.Clear();
            GameEvents.RaiseBricksChanged(0);
        }
    }
}
