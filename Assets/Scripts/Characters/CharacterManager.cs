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

    private void Start()
    {
        characterDatas = DataManager.instance.GetCharacterDatas();

        character = new GameObject("Character");

        foreach (KeyValuePair<int, CharacterData> data in characterDatas)
        {
            GameObject characterPrefab = data.Value.prefab;

            characterPrefabs.Add((CharacterKey)data.Value.key, characterPrefab);//���⼭ DataManager�� ���� key��, prefab �ְ��ְ�
        }

        CreateCharacter(CharacterKey.KNIGHT,2);
        CreateCharacter(CharacterKey.DOGNIGHT, 2);
        CreateCharacter(CharacterKey.SPEARMAN, 2);
        CreateCharacter(CharacterKey.WIZARD, 2);
        CreateCharacter(CharacterKey.GRUNT, 2);

        //CreateCharacter(CharacterKey.TURTLE, 1);
        //CreateCharacter(CharacterKey.SLIME, 10);
        //CreateCharacter(CharacterKey.MUSHROOM, 10);
        //CreateCharacter(CharacterKey.CACTUS, 10);
        //CreateCharacter(CharacterKey.BEHOLDER, 10);
        //CreateCharacter(CharacterKey.GOLEM, 3);
        CreateCharacter(CharacterKey.USURPER, 1);
    }

    // �߰�(����� �ʿ� �ڿ������;���)
    public CharacterData GetCharacterData(int key)
    {
        //if(characterDatas.ContainsKey(key))
        return characterDatas[key];
    }

    // �߰�
    public void CreateCharacter(int key, Vector3 position, Vector3 rallyPoint = new Vector3())
    {

        if(rallyPoint.Equals(Vector3.zero))
        {
            // not move
        }
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

    public Dictionary<CharacterKey, List<GameObject>> GetCharacters() { return characterPools; }
    
    public Dictionary<CharacterKey, List<GameObject>> GetEnemyData()
    {
        Dictionary<CharacterKey, List<GameObject>> temp = new Dictionary<CharacterKey, List<GameObject>>();

        foreach (CharacterKey key in characterPools.Keys)
        {            
            if (key>=CharacterKey.TURTLE)
            {
                temp.Add(key, characterPools[key]);
            }
        }
        return temp;
    }

    public Vector3 GetArmyPos(GameObject enmy)
    {
        Vector3 pos = enmy.gameObject.transform.position;

        foreach (CharacterKey key in characterPools.Keys)
        {
            foreach (GameObject obj in characterPools[key])
            {
                if (Vector3.Distance(obj.gameObject.transform.position, enmy.gameObject.transform.position) < 10.0f)
                { 
                    return obj.gameObject.transform.position - new Vector3(1,0,1); 
                }
            }
        }
        return pos;
    }

    // �߰�
    public void GetObjectsContainedInRect(Rect rect)
    {
        foreach (CharacterKey key in characterPools.Keys)
        {
            foreach (GameObject obj in characterPools[key])
            {
                // Ȱ��ȭ �Ǿ����� ���� ������Ʈ���� �˻� ���� ����
                if (!obj.activeSelf) 
                    continue;

                // �Ʊ���ü�� ���鼭 ������ �ڽ��� �߰�
                if(rect.Contains(Camera.main.WorldToScreenPoint(obj.transform.position)))
                {
                    SelectionBox.instance.AddSelectObject((int)key, obj);
                }
            }
        }
    }
}



/*//UIManager
//���� : ĳ���͵� ������ characterData
//���� : ���� ������ buildingData
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
    image.transform.position = ����;
    
    
    vector<Image> images;

    images.push_back(image);

    foreach(Image obj in images)
    obj.enable = true;
}


*/