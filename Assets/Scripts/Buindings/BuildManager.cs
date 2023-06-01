using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using Unity.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Burst.CompilerServices;

public struct BuildingData
{
    public int key;
    public string name;
    public int matrixSize;
    public int maxHP;
    public int buildTime;
    public int qty_Wood;
    public int qty_Stone;
    public int qty_Copper;
    public GameObject prefab;
    public Sprite icon;
    public string description;
}

public class BuildManager : MonoBehaviour
{
    private static BuildManager instance;
    public static BuildManager Instance
    {
        get
        {
            return instance;
        }
    }

    Dictionary<int, BuildingData> buildingDatas = new Dictionary<int, BuildingData>();


    [Header("Buildings")]
    GameObject townHall;
    // Dictionary <key, List<GameObject>> Buildings
    // AddBuilding
    // RemoveBuilding
    GridManager gridManager;
    GameObject buildingParent;

    GameObject target = null;

    bool isBuild = false;
    
    [SerializeField]
    Citizen tempCitizen;


    private void Awake()
    {
        instance = this;
        DataManager.instance.LoadBuildingData();
        gridManager = GetComponent<GridManager>();
        buildingParent = new GameObject("Buildings");
    }

    // Start is called before the first frame update
    void Start()
    {
        AddBuilding(2000);
        townHall = target;
        townHall.transform.position = Vector3.zero;
        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(isBuild && target != null)
        {
            Building targetBuilding = target.GetComponent<Building>();

            Vector3 mPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
            Ray ray = Camera.main.ScreenPointToRay(mPos);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.tag.Equals("Terrain"))
                {
                    gridManager.SetShowGrid(true);
                    target.transform.position = gridManager.GetBuildPosition(hits[i].point, targetBuilding.GetMatrixSize());
                    //Debug.Log(target.transform.position);
                    //gridManager.SetSlotIsEmpty(hit.point, false);
                    break;
                }
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                if(gridManager.GetBuildable(target.transform.position, targetBuilding.GetMatrixSize()))
                {
                    gridManager.SetSlotIsEmpty(target.transform.position, targetBuilding.GetMatrixSize(), false);
                    gridManager.SetShowGrid(false);
                    tempCitizen.BuildingOrder(target);
                    target = null;
                    isBuild = false;
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.V)) 
        {
            townHall.GetComponent<Building>().CreateUnit(1000);
        }
    }

    public Building GetTownHall()
    {
        return townHall.GetComponent<Building>();
    }

    public BuildingData GetBuildData(int key)
    {
        return buildingDatas[key];
    }

    public void AddBuilding(int key)
    {
        GameObject obj = buildingDatas[key].prefab;
        isBuild = true;
        target = Instantiate(obj, buildingParent.transform);
        target.GetComponent<Building>().SetBuildingProperty(
            buildingDatas[key].key,
            buildingDatas[key].matrixSize,
            buildingDatas[key].maxHP,
            buildingDatas[key].buildTime);
    }

    public void AddBuildingData(BuildingData data)
    {
        buildingDatas.Add(data.key, data);
    }
}
