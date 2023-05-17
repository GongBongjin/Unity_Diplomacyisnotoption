using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterData
{
    public int key;
    public CharacterType characterType;
    public float maxHp;
    public float dmg;
    public float moveSpeed;
}

public class DataManager : MonoBehaviour
{
    static public DataManager instance;

    private Dictionary<int, CharacterData> characterDatas = new Dictionary<int, CharacterData>();

    public Dictionary<int, CharacterData> GetCharacterDatas() { return characterDatas; }
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
            characterData.moveSpeed = float.Parse(data[4]);

            characterDatas.Add(characterData.key, characterData);
        }
    }
}
