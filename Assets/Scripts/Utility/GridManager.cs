using System;
using System.Buffers.Text;
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

    //ShowGrid _grid;
    [SerializeField, Tooltip("3d flat(plane) object with Material set to gpu instancing")]
    private GameObject gridSlotPrefab;
    [SerializeField, Tooltip("Add 2 materials of different colors")]
    private Material[] gridSlotMaterial = new Material[2];
    private bool isShowGrid = false;

    private GameObject slotParent;
    private GameObject[] gridSlots;
    private MeshRenderer[] gridSlotsRenderer;

    [Header("Build")]
    private const int TERRAIN_SIZE_WIDTH = 1000;
    private const int TERRAIN_SIZE_HEIGHT = 1000;

    private bool[] slotInfo;

    private void Awake()
    {
        //_grid = Camera.main.gameObject.GetComponent<ShowGrid>();
        slotParent = new GameObject("Slots");
        int count = GRID_SLOT_WIDTH_COUNT * GRID_SLOT_HEIGHT_COUNT;
        gridSlots = new GameObject[count];
        gridSlotsRenderer = new MeshRenderer[count];
        for (int i = 0; i < count; i++)
        {
            gridSlots[i] = Instantiate(gridSlotPrefab, slotParent.transform);
            gridSlotsRenderer[i] = gridSlots[i].GetComponent<MeshRenderer>();
            gridSlots[i].SetActive(false);
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
        //if (isShowGrid)
        //    ShowGridSlot();
    }

    public void SetShowGrid(bool isShow)
    {
        isShowGrid = isShow;
        //slotParent.SetActive(isShowGrid);
        //_grid.enabled = isShowGrid;
    }
    // BuildManager call this function

    public void SetSlotDefault()
    {
        for (int z = 0; z < GRID_SLOT_HEIGHT_COUNT; z++)
        {
            for (int x = 0; x < GRID_SLOT_WIDTH_COUNT; x++)
            {
                int index = z * GRID_SLOT_HEIGHT_COUNT + x;
                gridSlots[index].SetActive(false);
            }
       }
    }
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
        int halfWidth = (int)(GRID_SIZE * (GRID_SLOT_WIDTH_COUNT + 1) * 0.5f);
        int halfHeight = (int)(GRID_SIZE * (GRID_SLOT_HEIGHT_COUNT + 1) * 0.5f);
        basePosition.x -= halfWidth - GRID_SIZE;
        basePosition.z -= halfHeight - GRID_SIZE;

        float baseX = basePosition.x;

        for (int z = 0; z < GRID_SLOT_HEIGHT_COUNT; z++)
        {
            for (int x = 0; x < GRID_SLOT_WIDTH_COUNT; x++)
            {
                int index = z * GRID_SLOT_HEIGHT_COUNT + x;
                gridSlots[index].transform.position = basePosition;
                gridSlotsRenderer[index].material = GetSlotIsEmpty(basePosition) ? gridSlotMaterial[0] : gridSlotMaterial[1];

                basePosition.x += GRID_SIZE;
            }
            basePosition.x = baseX;
            basePosition.z += GRID_SIZE;
        }
    }

    public Vector3 GetBuildPosition(Vector3 pos, int matrixSize = 1)
    {
        pos.x += TERRAIN_SIZE_WIDTH * 0.5f;
        pos.z += TERRAIN_SIZE_HEIGHT * 0.5f;

        float x = pos.x - (pos.x % (GRID_SIZE * 0.5f));
        float z = pos.z - (pos.z % (GRID_SIZE * 0.5f));

        int xSlice = (int)(pos.x / (GRID_SIZE * 0.5f));
        int zSlice = (int)(pos.z / (GRID_SIZE * 0.5f));

        if (matrixSize % 2 == 0)
        {
            if (xSlice % 2 == 1)
            {
                x += GRID_SIZE * 0.5f;
            }
            if (zSlice % 2 == 1)
            {
                z += GRID_SIZE * 0.5f;
            }
        }
        else
        {
            if(xSlice % 2 == 0)
            {
                x += GRID_SIZE * 0.5f;
            }
            if (zSlice % 2 == 0)
            {
                z += GRID_SIZE * 0.5f;
            }
        }
        x -= (int)(TERRAIN_SIZE_WIDTH * 0.5f);
        z -= (int)(TERRAIN_SIZE_HEIGHT * 0.5f);

        Vector3 basePosition = new Vector3(x, 0, z);
        ShowGridSlot(basePosition, matrixSize);
        return basePosition;
    }

    private void ShowGridSlot(Vector3 pos, int size)
    {
        int halfIndex = size / 2;
        float halfGridSize = GRID_SIZE * 0.5f;
        for (int z = 0; z < GRID_SLOT_HEIGHT_COUNT; z++)
        {
            for (int x = 0; x < GRID_SLOT_WIDTH_COUNT; x++)
            {
                int index = z * GRID_SLOT_HEIGHT_COUNT + x;
                gridSlots[index].SetActive(false);

                if(z < size && x < size)
                {
                    Vector3 gridPosition = pos + new Vector3((x- halfIndex ) * GRID_SIZE, 0.1f, (z - halfIndex) * GRID_SIZE);
                    if(size % 2 == 0)
                    {
                        gridPosition += new Vector3(halfGridSize, 0.1f, halfGridSize);
                    }
                    gridSlots[index].SetActive(true);
                    gridSlots[index].transform.position = gridPosition;
                    bool isEmpty = GetSlotIsEmpty(gridPosition);
                    gridSlotsRenderer[index].material = isEmpty ? gridSlotMaterial[0] : gridSlotMaterial[1];
                }
            }
        }
    }

    public bool GetBuildable(Vector3 pos, int size)
    {
        int halfIndex = size / 2;
        bool isBuildable = true;

        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = z * GRID_SLOT_HEIGHT_COUNT + x;
                Vector3 gridPosition = pos + new Vector3((x - halfIndex) * GRID_SIZE, 0.1f, (z - halfIndex) * GRID_SIZE);
                bool isEmpty = GetSlotIsEmpty(gridPosition);
                if (!isEmpty)
                    isBuildable = false;
            }
        }
        return isBuildable;
    }

    public void SetSlotIsEmpty(Vector3 pos, int size = 1, bool isEmpty = false)
    {
        int halfIndex = size / 2;

        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector3 gridPosition = pos + new Vector3((x - halfIndex) * GRID_SIZE, 0.1f, (z - halfIndex) * GRID_SIZE);
                SetSlotIsEmpty(gridPosition, isEmpty);
            }
        }
    }

    public void SetSlotIsEmpty(Vector3 pos, bool isEmpty)
    {
        pos.x += TERRAIN_SIZE_WIDTH * 0.5f;
        pos.z += TERRAIN_SIZE_HEIGHT * 0.5f;

        int xIndex = (int)(pos.x / GRID_SIZE);
        int yIndex = (int)(pos.z / GRID_SIZE);

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

        int xIndex = (int)(pos.x / GRID_SIZE);
        int yIndex = (int)(pos.z / GRID_SIZE);

        int slotWidthCount = (TERRAIN_SIZE_WIDTH / GRID_SIZE);
        int slotHeightCount = (TERRAIN_SIZE_HEIGHT / GRID_SIZE);
        if (xIndex >= slotWidthCount || yIndex >= slotHeightCount)
            return false;

        return slotInfo[yIndex * slotWidthCount + xIndex];
    }
}
