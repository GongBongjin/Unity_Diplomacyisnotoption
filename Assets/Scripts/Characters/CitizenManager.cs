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
    CitizenData citizenData;
    GameObject citizenParent;

    List<Citizen> citizenObjects;

    private void Awake()
    {
        LoadCitizenData();

        citizenParent = new GameObject("citizenParent");
        citizenObjects = new List<Citizen>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateCharacterPool(int poolCount)
    {
        for (int i = 0; i < poolCount; i++)
        {
            // 생성
        }
    }
    public void CreateCharacter(Vector3 position, Vector3 rallyPoint = new Vector3())
    {

        if (rallyPoint.Equals(Vector3.zero))
        {
            // not move
        }
    }

    public void GetObjectsContainedInRect(Rect rect)
    {
        //foreach (CharacterKey key in characterPools.Keys)
        //{
        //    foreach (GameObject obj in characterPools[key])
        //    {
        //        // 활성화 되어있지 않은 오브젝트들은 검사 하지 않음
        //        if (!obj.activeSelf)
        //            continue;
        //
        //        // 아군객체만 돌면서 셀렉션 박스에 추가
        //        if (rect.Contains(Camera.main.WorldToScreenPoint(obj.transform.position)))
        //        {
        //            SelectionBox.instance.AddSelectObject((int)key, obj);
        //        }
        //    }
        //}
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
