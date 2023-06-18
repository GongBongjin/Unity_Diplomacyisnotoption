using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CitizenData
{
    public int key;
    public string name;
    public float maxHP;
    public float workSpeed;
    public int food;
    public int wood;
    public int stone;
    public int copper;
    public GameObject prefab;
    public Sprite icon;
    public string description;
}

public class CitizenManager : MonoBehaviour
{
    private static CitizenManager instance;
    public static CitizenManager Instance
    {
        get
        {
            return instance;
        }
    }

    CitizenData citizenData;
    GameObject citizenParent;

    List<GameObject> citizenObjects;

    const int AddPoolCount = 10;

    private void Awake()
    {
        instance = this;

        LoadCitizenData();

        citizenParent = new GameObject("citizenParent");
        citizenObjects = new List<GameObject>();
        CreateCharacterPool(AddPoolCount);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void SetMinimapPosition()
    {
        for (int i = 0; i < citizenObjects.Count; i++)
        {
            MiniMap.Instance.AddPosition(citizenObjects[i].transform.position);
        }
    }

    public int GetCitizenKey()
    {
        return citizenData.key;
    }

    public CitizenData GetCitizenData()
    {
        return citizenData;
    }

    private void CreateCharacterPool(int poolCount)
    {
        for (int i = 0; i < poolCount; i++)
        {
            // 생성
            GameObject obj = Instantiate(citizenData.prefab, citizenParent.transform);
            obj.SetActive(false);
            citizenObjects.Add(obj);
        }
    }

    public void CreateCharacter(Vector3 position, Vector3 rallyPoint = new Vector3())
    {
        bool isCreatable = false;
        int idx = -1;
        // check Character Pool
        for(int i = 0; i < citizenObjects.Count; i++)
        {
            if (citizenObjects[i].activeSelf) continue;

            isCreatable = true;
            idx = i;
            break;
        }
        if(!isCreatable)
        {
            // 캐릭터 풀이 모자랄 때
            idx = citizenObjects.Count;
            CreateCharacterPool(AddPoolCount);
        }

        citizenObjects[idx].GetComponent<Citizen>().SetCitizenProperty(citizenData.key, citizenData.maxHP, citizenData.workSpeed);
        citizenObjects[idx].transform.position = position;
        citizenObjects[idx].SetActive(true);

        // 생산 후 이동
        if (!rallyPoint.Equals(Vector3.zero))
        {
            citizenObjects[idx].GetComponent<Citizen>().MoveDestination(rallyPoint);
        }

    }

    public void GetObjectsContainedInRect(Rect rect)
    {
        foreach (GameObject obj in citizenObjects)
        {
            if (!obj.activeSelf)
                continue;

            if (rect.Contains(Camera.main.WorldToScreenPoint(obj.transform.position)))
            {
                SelectionBox.instance.AddSelectObject(citizenData.key, obj);
            }
        }
    }

    private void LoadCitizenData()
    {
        List<Dictionary<string, object>> reader = CSVReader.Read("TextData/CitizenData");

        citizenData.key = int.Parse(reader[0]["Key"].ToString());
        citizenData.name = reader[0]["Name"].ToString();
        citizenData.maxHP = float.Parse(reader[0]["MaxHP"].ToString());
        citizenData.workSpeed = float.Parse(reader[0]["WorkSpeed"].ToString());
        citizenData.food = int.Parse(reader[0]["Food"].ToString());
        citizenData.wood = int.Parse(reader[0]["Wood"].ToString());
        citizenData.stone = int.Parse(reader[0]["Stone"].ToString());
        citizenData.copper = int.Parse(reader[0]["Copper"].ToString());
        citizenData.prefab = Resources.Load<GameObject>(reader[0]["Prefab"].ToString());
        citizenData.icon = Resources.Load<Sprite>(reader[0]["Icon"].ToString());
        citizenData.description = reader[0]["Description"].ToString();
    }
}
