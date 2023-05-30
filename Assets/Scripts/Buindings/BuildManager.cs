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

    
    bool isBuild = false;
    [Header("Buildings")]
    GridManager gridManager;
    GameObject buildingParent;

    GameObject target = null;

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
        target.transform.position = Vector3.zero;
        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        

        if(isBuild && target != null)
        {
            Vector3 mPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
            Ray ray = Camera.main.ScreenPointToRay(mPos);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.tag.Equals("Terrain"))
                {
                    gridManager.SetShowGrid(true);
                    target.transform.position = gridManager.GetBuildPosition(hits[i].point, target.GetComponent<Building>().GetMatrixSize());
                    //gridManager.SetSlotIsEmpty(hit.point, false);
                    break;
                }
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                gridManager.SetShowGrid(false);

                // 시민에게 적용시켜야함
                // 셀렉션 오브젝트에 넘겨서 시민에게 전달될 수 있도록
                tempCitizen.BuildingOrder(target);
                target = null;
                isBuild = false;
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
