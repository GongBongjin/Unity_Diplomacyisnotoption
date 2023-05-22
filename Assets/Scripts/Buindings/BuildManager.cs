using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using Unity.Collections;
using System.Runtime.InteropServices.WindowsRuntime;

public class BuildManager : MonoBehaviour
{
    struct BuildingInfomation
    {
        public GameObject building;
        public int matrixSize;
        public float maxHP;
    }

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

    public void AddBuilding(string buildingName)
    {

    }

    void SetBuildingDatas()
    {
        buildings = new Dictionary<string, GameObject>();

        _JsonFileManager = new JsonFileManager();
        string datas = _JsonFileManager.ReadJsonFile(dataFilePath);
        JObject jsonData = JObject.Parse(datas);
        //Debug.Log(jsonData);

        //JToken jData = jsonData["BuildingData"];
        JToken jToken = jsonData["BuildingData"];
        foreach (JToken members in jToken)
        {
            buildings.Add(members["Name"].ToString(), buildingPrefabs[int.Parse(members["Index"].ToString())]);

        }

        // 있어도 되나?
        buildingPrefabs.Clear();
        buildingPrefabs = null;

        _JsonFileManager = null;
        //Destroy(_JsonFileManager);
        //Resources.Load("BuildingData.json")
    }
}
