using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public enum CharacterKey
{
    //Army
    KNIGHT = 1001,
    DOGNIGHT,
    SPEARMAN,
    WIZARD,
    GRUNT,
    //Enemy
    TURTLE,
    SLIME,
    MUSHROOM,
    CACTUS,
    MIMIC,
    BEHOLDER,
    //Boss
    GOLEM,
    USURPER
}

public class CharacterManager : MonoBehaviour
{
    static public CharacterManager instance;   

    private GameObject character;

    private Dictionary<int, CharacterData> characterDatas = new Dictionary<int, CharacterData>();
    private Dictionary<CharacterKey, GameObject> characterPrefabs = new Dictionary<CharacterKey, GameObject>();
    private Dictionary<CharacterKey, List<GameObject>> characterPools = new Dictionary<CharacterKey, List<GameObject>>();

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        characterDatas = DataManager.instance.GetCharacterDatas();

        character = new GameObject("Character");

        foreach (KeyValuePair<int, CharacterData> data in characterDatas)
        {
            GameObject characterPrefab = Resources.Load<GameObject>(data.Value.prefab);

            characterPrefabs.Add((CharacterKey)data.Value.key, characterPrefab);
        }

        CreateCharacter(CharacterKey.KNIGHT,1);
        CreateCharacter(CharacterKey.DOGNIGHT, 1);
        CreateCharacter(CharacterKey.SPEARMAN, 1);
        CreateCharacter(CharacterKey.WIZARD, 1);
        CreateCharacter(CharacterKey.GRUNT,1);

        //SpawnCharacter(CharacterKey.KNIGHT);
    }

    private void CreateCharacter(CharacterKey key, int poolCount)
    {
        List<GameObject> temp = new List<GameObject>();

        for (int i = 0; i < poolCount; i++)
        {
            GameObject obj = Instantiate(characterPrefabs[key], this.character.transform);

            Character character = obj.GetComponent<Character>();

            character.SetData(characterDatas[(int)key]);
            obj.transform.position = new Vector3(Random.Range(-20f, 10f), 0, Random.Range(-10f, 20f));
            obj.SetActive(true);

            temp.Add(obj);
        }
        characterPools.Add(key, temp);
    }

    private void SpawnCharacter(CharacterKey key)
    {
        foreach (GameObject obj in characterPools[(CharacterKey)key])
        {
            if(!obj.activeSelf)
            {
                obj.transform.position = new Vector3(Random.Range(-20f, 10f), 0, Random.Range(-10f, 20f));
                obj.SetActive(true);
            }
        }
    }

    public Dictionary<CharacterKey, List<GameObject>> GetEnemyData()
    {
        Dictionary<CharacterKey, List<GameObject>> temp = new Dictionary<CharacterKey, List<GameObject>>();

        foreach (CharacterKey key in characterPools.Keys)
        {            
            if (key==CharacterKey.DOGNIGHT)
            {
                temp.Add(key, characterPools[key]);
            }
        }
        return temp;
    }
}



/*//UIManager
//변수 : 캐릭터들 데이터 characterData
//변수 : 빌딩 데이터 buildingData
//
//enum DataType
//{
    chData
    bData
//}
//private void showInfo(DataType type)
{
    if(type == chdata)

    {
        atkinfo = chData.value.atkvalue;
        definfo = chData.value.defvalue;
    
    }

//private void UIfunction()
{
    Image image = chdata.value.sprite;
    image.transform.position = 수기;
    
    
    vector<Image> images;

    images.push_back(image);

    foreach(Image obj in images)
    obj.enable = true;
}


*/