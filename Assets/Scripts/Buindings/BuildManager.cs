using System.Collections.Generic;
using UnityEngine;

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
    Dictionary<int, List<GameObject>> buildings = new Dictionary<int, List<GameObject>>();

    GridManager gridManager;

    [Header("Buildings")]
    GameObject townHall;

    GameObject buildingParent;

    int targetKey = 0;
    GameObject target = null;

    bool isBuild = false;
        
    
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
        CreateBaseTownHall();
        CreateBaseCitizen(4);
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
                    target.transform.position = gridManager.GetBuildPosition(hits[i].point, GetBuildingMatrixSize(targetKey));
                    break;
                }
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                AddBuilding();
            }
        }

        //if(Input.GetKeyDown(KeyCode.V)) 
        //{
        //    townHall.GetComponent<Building>().CreateUnit(1000);
        //}
    }

    public Building GetTownHall()
    {
        return buildings[2000][0].GetComponent<Building>();
    }

    public BuildingData GetBuildData(int key)
    {
        return buildingDatas[key];
    }

    public int GetBuildingMatrixSize(int key)
    {
        return buildingDatas[key].matrixSize;
    }

    public void CreateBaseTownHall()
    {
        ReadyConstruction(2000);
        townHall = target;
        target = null;
        townHall.transform.position = Vector3.zero;
        buildings[2000].Add(townHall);
        gridManager.SetSlotIsEmpty(townHall.transform.position, GetBuildData(2000).matrixSize, false);
    }

    public void CreateBaseCitizen(int count)
    {
        for(int i = 0; i < count; i++)
        {
            townHall.GetComponent<Building>().CreateUnit(CitizenManager.Instance.GetCitizenKey());
        }
    }

    public void AddBuilding()
    {
        Building targetBuilding = target.GetComponent<Building>();

        if (gridManager.GetBuildable(target.transform.position, targetBuilding.GetMatrixSize()))
        {
            gridManager.SetSlotIsEmpty(target.transform.position, targetBuilding.GetMatrixSize(), false);
            gridManager.SetShowGrid(false);
            SelectionBox.instance.BuildingOrder(CitizenManager.Instance.GetCitizenKey(), target);
            buildings[targetKey].Add(target);
            targetKey = 0;
            target = null;
            isBuild = false;
            gridManager.SetSlotDefault();
        }
    }
    public void ReadyConstruction(int key)
    {
        targetKey = key;
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
        buildings[data.key] = new List<GameObject>();
    }

    public GameObject GetNearestStorage(Vector3 pos)
    {
        int storageKey = 2003;
        GameObject nearTarget = townHall;
        float dist = Vector3.Distance(pos, nearTarget.transform.position);

        for(int i = 0; i < buildings[storageKey].Count; i++)
        {
            Vector3 bp = buildings[storageKey][i].transform.position;
            float compareDist = Vector3.Distance(pos, bp);
            if (compareDist < dist)
            {
                nearTarget = buildings[storageKey][i];
            }
        }

        return nearTarget;
    }
}
