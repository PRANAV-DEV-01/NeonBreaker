using UnityEngine;

namespace NeonBreaker.Gameplay
{
    public class BoundaryBuilder : MonoBehaviour
    {
        [SerializeField] private float halfWidth = 5.2f;
        [SerializeField] private float topY = 8.9f;
        [SerializeField] private float wallThickness = 0.5f;
        [SerializeField] private float wallHeight = 22f;

        private void Awake()
        {
            PhysicsMaterial2D bounceMaterial = new PhysicsMaterial2D("WallBounce")
            {
                bounciness = 1f,
                friction = 0f
            };

            CreateWall("LeftWall", new Vector2(-halfWidth - wallThickness * 0.5f, 0f), new Vector2(wallThickness, wallHeight), bounceMaterial);
            CreateWall("RightWall", new Vector2(halfWidth + wallThickness * 0.5f, 0f), new Vector2(wallThickness, wallHeight), bounceMaterial);
            CreateWall("TopWall", new Vector2(0f, topY), new Vector2(halfWidth * 2f + wallThickness * 2f, wallThickness), bounceMaterial);
        }

        private void CreateWall(string wallName, Vector2 position, Vector2 size, PhysicsMaterial2D material)
        {
            GameObject wall = new GameObject(wallName);
            wall.transform.SetParent(transform);
            wall.transform.position = position;

            BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
            collider.size = size;
            collider.sharedMaterial = material;
        }
    }
}
