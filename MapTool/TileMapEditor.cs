using Sirenix.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileMapEditor : SingletonComponent<TileMapEditor>
{
    private int gridSize = 10;
    public int GridSize
    {
        get { return gridSize;}
        set { gridSize = value;}
    }

    public float floorIndex;
    public float floorSpaceSize;

    public CustomPaletteItem drawTile;
    public GameObject editFloor;
    public GameObject editTilemap;

    private Vector3 gridPos;

    private LineRenderer lineRenderer;
    private float gridSellSize = 1;

    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
    }
    private void Start()
    {
        InitLineRenderer();
        MakeGrid(gridSize);
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;


        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = Input.mousePosition;
            gridPos = MousePosToGridPos(mousePosition);

            if (!IsValidPos(gridPos)) return;

            if (gridPos != Vector3.negativeInfinity)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    CreateTile(gridPos);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    RemoveTile(gridPos);
                }
            }
        }
    }

    public void SetGridSize(string text)
    {
        if (int.TryParse(text, out int result))
            GridSize = result;

        if (gridSize > 0)
            MakeGrid(gridSize);
    }

    //private void OnValidate()
    //{
    //    if(gridSize > 0)
    //        makeGrid(gridSize);
    //}

    private void InitLineRenderer()
    {
        lineRenderer.startWidth = lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineRenderer.endColor = Color.blue; 
    }

    private void MakeGrid(int gridSize)
    {

        float startOffset =  -0.5f; 

        List<Vector3> gridPos = new List<Vector3>();

        float ec = startOffset + gridSize * gridSellSize; 

        gridPos.Add(new Vector3(startOffset, this.transform.position.y, startOffset));
        gridPos.Add(new Vector3(startOffset, this.transform.position.y, ec));

        int toggle = -1;
        Vector3 currentPos = new Vector3(startOffset, this.transform.position.y, ec);

        for (int i = 0; i < gridSize; i++)
        {
            Vector3 nextPos = currentPos;
            nextPos.x += gridSellSize;
            gridPos.Add(nextPos);

            nextPos.z += gridSize * toggle * gridSellSize;
            gridPos.Add(nextPos);

            currentPos = nextPos;
            toggle *= -1;
        }

        currentPos.x = startOffset;
        gridPos.Add(currentPos);

        int colToggle = toggle = 1;
        if (currentPos.z == ec) colToggle = -1;

        for (int i = 0; i < gridSize; i++)
        {
            Vector3 nextPos = currentPos;
            nextPos.z += colToggle * gridSellSize;
            gridPos.Add(nextPos);

            nextPos.x += gridSize * toggle * gridSellSize;
            gridPos.Add(nextPos);

            currentPos = nextPos;
            toggle *= -1;
        }

        lineRenderer.positionCount = gridPos.Count;
        lineRenderer.SetPositions(gridPos.ToArray());
    }
    private Vector3 MousePosToGridPos(Vector2 clickPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(clickPos);
        Vector3 floorPosition = editTilemap.transform.position + (Vector3.up * floorIndex);
        Plane plane = new Plane(Vector3.up, floorPosition);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 rayPoint = ray.GetPoint(enter);
            rayPoint.x = Mathf.Round(rayPoint.x);
            rayPoint.y = Mathf.Round(rayPoint.y);
            rayPoint.z = Mathf.Round(rayPoint.z);

            return rayPoint;
        }

        return Vector3.negativeInfinity;
    }

    private void PreviewTile(Vector3 position)
    {

    }

    private void CreateTile(Vector3 position)
    {
        if (drawTile == null || editFloor == null) return;
        

        if (IsExistAt(position))
            RemoveTile(position);

        CustomFloor customFloor = editFloor.GetComponent<CustomFloor>();
        if (customFloor != null)
        {
            customFloor.AddMapData(position, new CustomPaletteItem { id = drawTile.id, tileObject = drawTile.tileObject });
        }
    }

    private void RemoveTile(Vector3 position)
    {
        if (editFloor == null) return;
        

        CustomFloor customFloor = editFloor.GetComponent<CustomFloor>();
        if (customFloor == null) return;
        

        if (IsExistAt(position))
        {
            customFloor.RemoveMapData(position);
        }
    }

    private bool IsExistAt(Vector3 pos)
    {
        if (editFloor == null)
            return false;

        CustomFloor customFloor = editFloor.GetComponent<CustomFloor>();

        if (customFloor == null)
            return false;

        return customFloor.IsDataExist(pos);
    }

    bool IsValidPos(Vector3 position)
    {
        if (position == Vector3.negativeInfinity)
            return false;

        if (position.x < 0 || position.z < 0)
            return false;

        if (position.x >= gridSize || position.z >= gridSize)
            return false;

        return true;
    }
}