using System.Collections.Generic;
using System.Linq;
using EssentialManagers.Scripts.Managers;
using UnityEngine;

namespace EssentialManagers.CustomPackages.GridManager.Scripts
{
    public class GridManager : MonoSingleton<GridManager>
    {
        [Header("Config")] [SerializeField] private bool autoGenerate = true;
        public int gridWidth = 10; // Width of the grid
        public int gridHeight = 10; // Height of the grid
        public float cellSpacing = 1f; // Spacing between cells
        [Header("References")] public GameObject cellPrefab;
        public Texture2D obstacleIcon;
        public Texture2D occupantIcon;
        public Texture2D setEmptyIcon;
        [Header("Debug")] public List<CellController> gridPlan;

        protected override void Awake()
        {
            base.Awake();
            if (autoGenerate)
                CreateGrid();
            else
            {
                gridPlan = new List<CellController>();
                List<CellController> tempList = transform.GetComponentsInChildren<CellController>().ToList();

                foreach (var cell in tempList)
                {
                    if (cell == null) continue;
                    if (!cell.gameObject.activeInHierarchy) continue;

                    gridPlan.Add(cell);
                }
            }
        }

        private void CreateGrid()
        {
            for (var x = 0; x < gridWidth; x++)
            {
                for (var y = 0; y < gridHeight; y++)
                {
                    Vector2Int coordinates = new Vector2Int(x, y);
                    GameObject cell = Instantiate(cellPrefab, new Vector3(x * cellSpacing, 0, y * cellSpacing),
                        cellPrefab.transform.rotation);
                    CellController cellController = cell.GetComponent<CellController>();
                    cellController.Initialize(coordinates);
                    cell.transform.parent = transform;
                    gridPlan.Add(cellController);
                }
            }
        }

        public CellController GetNearestCell(Vector3 from)
        {
            if (gridPlan == null || gridPlan.Count == 0)
            {
                Debug.LogWarning("GridPlan list is empty or null!");
                return null;
            }

            CellController closestCellController = null;
            float closestDistance = Mathf.Infinity;

            for (int i = 0; i < gridPlan.Count; i++)
            {
                CellController cellController = gridPlan[i];
                float distance = Vector3.Distance(cellController.transform.position, from);

                if (distance < closestDistance)
                {
                    closestCellController = cellController;
                    closestDistance = distance;
                }
            }

            return closestCellController;
        }

        public CellController GetGridCellByCoordinates(Vector2Int coordinates)
        {
            if (gridPlan == null || gridPlan.Count == 0)
            {
                return null;
            }

            for (int i = 0; i < gridPlan.Count; i++)
            {
                CellController cellController = gridPlan[i];
                if (cellController.GetCoordinates() == coordinates)
                {
                    return cellController;
                }
            }


            return null;
        }
    }
}