using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    static public UIManager Instance
    {
        get
        {
            return instance;
        }
    }


    Texture2D defaultCursor;
    Texture2D targetCursor;

    Sprite defaultSprite;

    [Header("Timer")]
    [SerializeField, Tooltip("Time Object in TimePanel")]
    private RectTransform timeCircle;
    [SerializeField, Tooltip("Day Text in TimePanel")]
    private Text text_Day;

    #region [Resources]
    struct NaturalResources {
        public string name;
        public Slider slider;
        public Text text;
    }

    [Header("Resources")]
    [SerializeField, Tooltip("(popul, food, wood, stone, copper) Object in ResourcePanel")]
    GameObject[] ResourceParents;
    NaturalResources[] resources;
    #endregion

    #region [SingleInformation]
    [Header("Single Information")]
    [SerializeField] GameObject singleInfo;
    [SerializeField] Text text_Name;
    [SerializeField] Image img_Profile;
    [SerializeField] Text text_GroupCount;

    [SerializeField] GameObject[] abilities;
    Text[] text_AbilityValue;

    [SerializeField] Text text_Discription;
    #endregion

    #region [MultiInformation]
    [Header("Multi Information")]
    [SerializeField] GameObject multiInfo;
    [SerializeField] GameObject unitParent;
    GameObject[] units;
    Image[] img_Icons;
    Text[] text_UnitCounts;
    #endregion

    #region [CommandSlots]
    [Header("Command Slots")]
    [SerializeField] GameObject slotParent;
    Command command;
    CommandData[] commandDatas;
    GameObject[] slots;
    Image[] img_SlotIcons;


    #endregion

    private void Awake()
    {
        instance = this;

        command = new Command();
        command.LoadData();

        LoadCursor();
        LoadSpriteIcon();

        SetResourceObject();
        SetMultiInformation();
        SetCommandSlotObject();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCommandSlot(1000);
        ShowInformation(2000);
    }

    // Update is called once per frame
    void Update()
    {
        // ChangeTimeTest
        //if(Input.GetKeyDown(KeyCode.U)) 
        //{
        //    ChangeTime(temp); 
        //}
    }

    private void LoadCursor()
    {
        defaultCursor = Resources.Load<Texture2D>("CursorIcon/default");
        targetCursor = Resources.Load<Texture2D>("CursorIcon/target");

        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    private void LoadSpriteIcon()
    {
        defaultSprite = Resources.Load<Sprite>("Icons/default");
    }

    // 자원 관리용 오브젝트 설정
    private void SetResourceObject()
    {
        int cnt = ResourceParents.Length;
        resources = new NaturalResources[cnt];
        for (int i = 0; i < cnt; i++)
        {
            resources[i] = new NaturalResources();
            resources[i].name = ResourceParents[i].name;
            resources[i].slider = ResourceParents[i].transform.Find("Slider").GetComponent<Slider>();
            resources[i].text = ResourceParents[i].transform.Find("Text_Value").GetComponent<Text>();
        }
    }
    // 다수 객체 선택용 오브젝트 설정
    private void SetMultiInformation()
    {
        int cnt = unitParent.transform.childCount;
        units = new GameObject[cnt];
        img_Icons = new Image[cnt];
        text_UnitCounts = new Text[cnt];

        for (int i = 0; i < cnt; i++)
        {
            units[i] = unitParent.transform.GetChild(i).gameObject;
            img_Icons[i] = units[i].transform.GetChild(0).GetComponent<Image>();
            text_UnitCounts[i] = img_Icons[i].transform.GetChild(0).GetComponent<Text>();
        }
    }
    // 명령 아이콘 설정
    private void SetCommandSlotObject()
    {
        int cnt = slotParent.transform.childCount;
        slots = new GameObject[cnt];
        img_SlotIcons = new Image[cnt];

        for(int i = 0; i < cnt; i++)
        {
            slots[i] = slotParent.transform.GetChild(i).gameObject;
            img_SlotIcons[i] = slots[i].transform.GetChild(0).GetComponent<Image>();
        }
    }

    // 0.0f ~ 23.99f
    public void ChangeTime(float hour)
    {
        // 0 = 90
        // 23.99 = -270
        float ratio = hour / 24.0f;
        float zRot = ratio * -360 + 90;
        timeCircle.rotation = Quaternion.Euler(0, 0, zRot);
    }

    public void UpdateDay(int day)
    {
        text_Day.text = day.ToString();
    }

    // Resource 변할때마다 이벤트 호출


    // Information
    public void ShowInformation(int key, int count = 1)
    {
        int type = key / 1000;
        if (type == 1)
        {
            //Character
        }
        else if(type == 2)
        {
            // Building
            BuildingData buildingData = BuildManager.Instance.GetBuildData(key);
            img_Profile.sprite = buildingData.icon;
            text_Name.text = buildingData.name;
            text_GroupCount.text = count.ToString();
            text_Discription.text = buildingData.discription;
            for(int i = 0; i < abilities.Length; i++)
            {
                abilities[i].SetActive(false);
            }
        }

        // character
        multiInfo.SetActive(false);
        singleInfo.SetActive(true);
        //img_Profile.sprite = CharacterImage key


        // 각각의 캐릭터가 정보를 가지고 있는 경우
        //Character character = obj.GetComponent<Character>();
        // 캐릭터 매니저가 정보를 가지고 있는 경우
        //CharacterManager.instance.GetCharacterInformation()

        // abilities[0].SetActive(true);
        // text_AbilityValue[0].text = GetAttack();

        // text_Discription.text = key_discription
        SetCommandSlot(key);
    }

    public void ShowInformation(int[] keys, int[] count)
    {
        int cnt = units.Length;
        for (int i = 0; i < cnt; i++)
        {
            units[i].SetActive(false);
        }
        cnt = keys.Length;
        for (int i = 0; i < cnt; i++)
        {
            units[i].SetActive(true);
            // 캐릭들 정보
            //img_Icons[i].sprite = objects[i]. GetSprite()
            //text_UnitCounts[i].text = counts[i].ToString();
        }
    }

    // 명령 데이터 설정
    public void SetCommandSlot(int key)
    {
        //commandDatas = null;
        commandDatas = command.GetCommands(key);

        for(int i = 0; i < 15; i++)
        {
            img_SlotIcons[i].sprite = commandDatas[i].icon;
        }
    }

    // 명령 버튼 클릭
    public void CommandSlotClick(int idx)
    {
        int key = commandDatas[idx].refKey;

        if (key == 0) return;

        if (key == 3000)
            SetCommandSlot(key);
        else if(key / 1000 == 2)
        {
            BuildManager.Instance.AddBuilding(key);
            // 건설
        }
    }
}
