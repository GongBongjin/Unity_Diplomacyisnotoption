using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterData
{
    public int key;
    public CharacterType characterType;
    public float maxHp;
    public float dmg;
    public GameObject prefab;
    public Sprite sprite;
}



public class DataManager : MonoBehaviour
{
    static public DataManager instance;

    private Dictionary<int, CharacterData> characterDatas = new Dictionary<int, CharacterData>();

    public Dictionary<int, CharacterData> GetCharacterDatas() { return characterDatas; }
    public CharacterData GetCharacterDatas(int key)
    {
        return characterDatas[key];
    }
    private void Awake()
    {
        instance = this;

        LoadData();
    }

    public void LoadData()
    {
        LoadCharacterData();
    }
    
    private void LoadCharacterData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("TextData/Character_Table");

        string temp = textAsset.text.Replace("\r\n", "\n");

        string[] row = temp.Split("\n");

        for (int i = 1; i < row.Length; i++)
        {
            if (row[i].Length == 0)
                break;

            string[] data = row[i].Split(',');

            CharacterData characterData;
            characterData.key = int.Parse(data[0]);
            characterData.characterType = (CharacterType)int.Parse(data[1]);
            characterData.maxHp = float.Parse(data[2]);
            characterData.dmg = float.Parse(data[3]);
            characterData.prefab = Resources.Load<GameObject>(data[4]);
            characterData.sprite = Resources.Load<Sprite>(data[5]);

            characterDatas.Add(characterData.key, characterData);
        }
    }

    public void LoadBuildingData()
    {
        List<Dictionary<string, object>> reader = CSVReader.Read("TextData/BuildingData");

        for (int i = 0; i < reader.Count; i++)
        {
            BuildingData buildingData;
            buildingData.key = int.Parse(reader[i]["Keys"].ToString());
            buildingData.name = reader[i]["Name"].ToString();
            buildingData.matrixSize = int.Parse(reader[i]["Size"].ToString());
            buildingData.maxHP = int.Parse(reader[i]["HP"].ToString());
            buildingData.buildTime = int.Parse(reader[i]["BuildTime"].ToString());
            buildingData.qty_Wood = int.Parse(reader[i]["Wood"].ToString());
            buildingData.qty_Stone = int.Parse(reader[i]["Stone"].ToString());
            buildingData.qty_Copper = int.Parse(reader[i]["Copper"].ToString());
            buildingData.prefab = Resources.Load<GameObject>(reader[i]["Prefab"].ToString());
            buildingData.icon = Resources.Load<Sprite>(reader[i]["Icon"].ToString());
            buildingData.description = reader[i]["Description"].ToString();
            BuildManager.Instance.AddBuildingData(buildingData);
        }
    }
}
