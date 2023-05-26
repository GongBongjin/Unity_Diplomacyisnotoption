using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using Unity.Collections;
using System.Runtime.InteropServices.WindowsRuntime;

public struct BuildingData
{
    public int key;
    public string name;
    public int matrixSize;
    public int maxHP;
    public int qty_Wood;
    public int qty_Stone;
    public int qty_Copper;
    public GameObject prefab;
    public Sprite icon;
    public string discription;
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

    
    private bool isBuild = false;
    [Header("Buildings")]
    [SerializeField]
    GridManager gridManager;
    private GameObject buildingParent;

    [SerializeField]
    GameObject tempBuilding;


    [Header("Building Dictionary")]
    [SerializeField, Tooltip("BuildingDataFile in Resources Folder")]
    string dataFilePath = "/Resources/BuildingData.json";
    JsonFileManager _JsonFileManager;
    [SerializeField, Tooltip("It should match 1-to-1 with the jsonFileName and index.")]
    List<GameObject> buildingPrefabs;
    Dictionary<string, GameObject> buildings;

    GameObject target = null;



    private void Awake()
    {
        instance = this;
        buildingParent = new GameObject("Buildings");
        SetBuildingDatas();
        
        //tempBuilding = Instantiate(tempBuilding, buildingParent.transform);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            isBuild = !isBuild;
            target = Instantiate(buildings["TownHall"]);
        }

        if(isBuild && target != null)
        {
            Vector3 mPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
            Ray ray = Camera.main.ScreenPointToRay(mPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane))
            {
                
                gridManager.SetShowGrid(true);
                //gridManager.ShowGridSlot(hit.point);
                target.transform.position = gridManager.GetBuildPosition(hit.point, target.GetComponent<Building>().matrixSize);
                //gridManager.SetSlotIsEmpty(hit.point, false);
            }
            if(Input.GetMouseButtonDown(0))
            {
                target = null;
                isBuild = false;
                gridManager.SetShowGrid(false);
            }
        }
    }

    public BuildingData GetBuildData(int key)
    {
        return buildingDatas[key];
    }

    public void AddBuilding(int key)
    {
        GameObject obj = buildingDatas[key].prefab;
        isBuild = true;
        target = Instantiate(obj);
    }


    public void AddBuilding(string buildingName)
    {

    }

    void SetBuildingDatas()
    {
        List<Dictionary<string, object>> reader = CSVReader.Read("TextData/BuildingData");

        for(int i = 0; i < reader.Count; i++)
        {
            BuildingData buildingData;
            buildingData.key = int.Parse(reader[i]["Keys"].ToString());
            buildingData.name = reader[i]["Name"].ToString();
            buildingData.matrixSize = int.Parse(reader[i]["Size"].ToString());
            buildingData.maxHP = int.Parse(reader[i]["HP"].ToString());
            buildingData.qty_Wood = int.Parse(reader[i]["Wood"].ToString());
            buildingData.qty_Stone = int.Parse(reader[i]["Stone"].ToString());
            buildingData.qty_Copper = int.Parse(reader[i]["Copper"].ToString());
            buildingData.prefab = Resources.Load<GameObject>(reader[i]["Prefab"].ToString());
            buildingData.icon = Resources.Load<Sprite>(reader[i]["Icon"].ToString());
            buildingData.discription = reader[i]["Discription"].ToString();
            buildingDatas.Add(buildingData.key, buildingData);
        }
    }
}
