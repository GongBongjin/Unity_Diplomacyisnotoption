using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GridManager : MonoBehaviour
{
    public const int GRID_SIZE = 5;

    [Header("Grid")]
    [Tooltip("It is recommended to create an even number of grid slots.")]
    private const int GRID_SLOT_WIDTH_COUNT = 11;
    private const int GRID_SLOT_HEIGHT_COUNT = 11;

    ShowGrid _grid;
    [SerializeField, Tooltip("3d flat(plane) object with Material set to gpu instancing")]
    private GameObject gridSlotPrefab;
    [SerializeField, Tooltip("Add 2 materials of different colors")]
    private Material[] gridSlotMaterial = new Material[2];
    private bool isShowGrid = false;

    [SerializeField]
    private Vector2 baseGridSlotSize;

    private GameObject slotParent;
    private GameObject[] gridSlots;

    [Header("Build")]
    private const int TERRAIN_SIZE_WIDTH = 1000;
    private const int TERRAIN_SIZE_HEIGHT = 1000;

    private bool[] slotInfo;

    private void Awake()
    {
        _grid = Camera.main.gameObject.GetComponent<ShowGrid>();
        slotParent = new GameObject("Slots");
        int count = GRID_SLOT_WIDTH_COUNT * GRID_SLOT_HEIGHT_COUNT;
        gridSlots = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            gridSlots[i] = Instantiate(gridSlotPrefab);
            gridSlots[i].transform.SetParent(slotParent.transform);
        }

        // 맵 전체의 slot정보 건물이 존재하는지 여부를 판단
        int widthCount = (int)(TERRAIN_SIZE_WIDTH / GRID_SIZE);
        int heightCount = (int)(TERRAIN_SIZE_HEIGHT / GRID_SIZE);
        slotInfo = new bool[widthCount * heightCount];
        for(int i = 0; i < slotInfo.Length; i++) { slotInfo[i] = true; }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetShowGrid(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isShowGrid)
            ShowGridSlot();
    }

    public void SetShowGrid(bool isShow)
    {
        isShowGrid = isShow;
        slotParent.SetActive(isShowGrid);
        _grid.enabled = isShowGrid;
    }
    // BuildManager call this function
    public void ShowGridSlot()
    {
        Vector3 mPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Ray ray = Camera.main.ScreenPointToRay(mPos);
        RaycastHit hit;
        Vector3 basePosition = Vector3.zero;
        if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane))
        {
            basePosition = GetBuildPosition(hit.point);
        }
        basePosition.y = 0.1f;  // zFighting
        int halfWidth = (int)(baseGridSlotSize.x * (GRID_SLOT_WIDTH_COUNT + 1) * 0.5f);
        int halfHeight = (int)(baseGridSlotSize.y * (GRID_SLOT_HEIGHT_COUNT + 1) * 0.5f);
        basePosition.x -= halfWidth - baseGridSlotSize.x;
        basePosition.z -= halfHeight - baseGridSlotSize.y;

        float baseX = basePosition.x;

        for (int z = 0; z < GRID_SLOT_HEIGHT_COUNT; z++)
        {
            for (int x = 0; x < GRID_SLOT_WIDTH_COUNT; x++)
            {
                int index = z * GRID_SLOT_HEIGHT_COUNT + x;
                gridSlots[index].transform.position = basePosition;
                gridSlots[index].GetComponent<MeshRenderer>().material
                    = GetSlotIsEmpty(basePosition) ? gridSlotMaterial[0] : gridSlotMaterial[1];

                basePosition.x += baseGridSlotSize.x;
            }
            basePosition.x = baseX;
            basePosition.z += baseGridSlotSize.y;
        }
    }

    public Vector3 GetBuildPosition(Vector3 pos, int matrixSize = 1)
    {
        pos.x += TERRAIN_SIZE_WIDTH * 0.5f;
        pos.z += TERRAIN_SIZE_HEIGHT * 0.5f;

        float x = pos.x - (pos.x % (baseGridSlotSize.x * 0.5f));
        float z = pos.z - (pos.z % (baseGridSlotSize.y * 0.5f));

        int xSlice = (int)(pos.x / (baseGridSlotSize.x * 0.5f));
        int zSlice = (int)(pos.z / (baseGridSlotSize.y * 0.5f));

        if (matrixSize % 2 == 0)
        {
            if (xSlice % 2 == 1)
            {
                x += baseGridSlotSize.x * 0.5f;
            }
            if (zSlice % 2 == 1)
            {
                z += baseGridSlotSize.x * 0.5f;
            }
        }
        else
        {
            if(xSlice % 2 == 0)
            {
                x += baseGridSlotSize.x * 0.5f;
            }
            if (zSlice % 2 == 0)
            {
                z += baseGridSlotSize.x * 0.5f;
            }
        }
        x -= (int)(TERRAIN_SIZE_WIDTH * 0.5f);
        z -= (int)(TERRAIN_SIZE_HEIGHT * 0.5f);

        return new Vector3(x, 0, z);
    }

    public void SetSlotIsEmpty(Vector3 pos, bool isEmpty)
    {
        pos.x += TERRAIN_SIZE_WIDTH * 0.5f;
        pos.z += TERRAIN_SIZE_HEIGHT * 0.5f;

        int xIndex = (int)(pos.x / baseGridSlotSize.x);
        int yIndex = (int)(pos.z / baseGridSlotSize.y);

        int slotWidthCount = (TERRAIN_SIZE_WIDTH / GRID_SIZE);
        int slotHeightCount = (TERRAIN_SIZE_HEIGHT / GRID_SIZE);
        if (xIndex >= slotWidthCount || yIndex >= slotHeightCount)
            return;

        slotInfo[yIndex * slotWidthCount + xIndex] = isEmpty;
    }

    public bool GetSlotIsEmpty(Vector3 pos)
    {
        pos.x += TERRAIN_SIZE_WIDTH * 0.5f;
        pos.z += TERRAIN_SIZE_HEIGHT * 0.5f;

        int xIndex = (int)(pos.x / baseGridSlotSize.x);
        int yIndex = (int)(pos.z / baseGridSlotSize.y);

        int slotWidthCount = (TERRAIN_SIZE_WIDTH / GRID_SIZE);
        int slotHeightCount = (TERRAIN_SIZE_HEIGHT / GRID_SIZE);
        if (xIndex >= slotWidthCount || yIndex >= slotHeightCount)
            return false;

        return slotInfo[yIndex * slotWidthCount + xIndex];
    }
}
