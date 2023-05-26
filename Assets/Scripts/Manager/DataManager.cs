using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterData
{
    public int key;
    public CharacterType characterType;
    public float maxHp;
    public float dmg;
    public float def;
    public float moveSpeed;
    public float sightValue;
    public float attackSpeed;
    public int food;
    public int wood;
    public int stone;
    public int copper;
    public GameObject prefab;
    public Sprite sprite;
    public string description;
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
        LoadBuildingData();
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
            characterData.def = float.Parse(data[4]);
            characterData.moveSpeed = float.Parse(data[5]);
            characterData.sightValue = float.Parse(data[6]);
            characterData.attackSpeed = float.Parse(data[7]);
            characterData.food = int.Parse(data[8]);
            characterData.wood = int.Parse(data[9]);
            characterData.stone = int.Parse(data[10]);
            characterData.copper = int.Parse(data[11]);
            characterData.prefab = Resources.Load<GameObject>(data[12]);
            characterData.sprite = Resources.Load<Sprite>(data[13]);
            characterData.description = data[14];

            characterDatas.Add(characterData.key, characterData);
        }
    }

    private void LoadBuildingData()
    {
        //TextAsset textAsset = Resources.Load<TextAsset>("TextData/Building_Table");

        //string temp = textAsset.text.Replace("\r\n", "\n");

        //string[] row = temp.Split("\n");

        //for (int i = 1; i < row.Length; i++)
        //{
        //    if (row[i].Length == 0)
        //        break;

        //    string[] data = row[i].Split(',');

        //    CharacterData characterData;
        //    characterData.key = int.Parse(data[0]);
        //    characterData.characterType = (CharacterType)int.Parse(data[1]);
        //    characterData.maxHp = float.Parse(data[2]);
        //    characterData.dmg = float.Parse(data[3]);
        //    characterData.prefab = data[4];
        //    characterData.sprite = data[5];

        //    characterDatas.Add(characterData.key, characterData);
        //}
    }
}
