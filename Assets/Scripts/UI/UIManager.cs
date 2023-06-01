using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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

    [SerializeField] SingleInformation singleInfo;
    [SerializeField] MultiInformation multiInfo;
    [SerializeField] CommandSlot commandSlot;


    private void Awake()
    {
        instance = this;


        LoadCursor();
        LoadSpriteIcon();

        SetResourceObject();
    }

    // Start is called before the first frame update
    void Start()
    {
        singleInfo.gameObject.SetActive(false);
        multiInfo.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            int[] tempKey = new int[1] { 1000 };
            int[] tempCount = new int[1] { 1 };
            ShowInformation(tempKey, tempCount);
            //target = Instantiate(buildings["TownHall"]);
        }
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
    // 0.0f ~ 23.99f
    public void ChangeTime(float hour)
    {
        timeCircle.rotation = Quaternion.Euler(0, 0, hour + 135.0f);
    }

    public void UpdateDay(int day)
    {
        CharacterManager.instance.SetAttackCityHall(day);
        text_Day.text = day.ToString();
    }

    // Resource 변할때마다 이벤트 호출


    // Information
    //public void ShowInformation(int key, int count)
    //{
    //    singleInfo.ShowInformation(key, count);
    //}
    

    public void ShowInformation(int[] keys, int[] count)
    {
        singleInfo.gameObject.SetActive(false);
        multiInfo.gameObject.SetActive(false);

        if (keys.Length == 0)
        {
            //clear
        }
        else if (keys.Length == 1)
        {
            singleInfo.gameObject.SetActive(true);
            //singleInfo.ShowInformation(keys[0], count[0]);
            commandSlot.SetCommandSlot(keys[0]);
        }
        else
        {
            multiInfo.gameObject.SetActive(true);
            multiInfo.ShowInformation(keys, count);
            commandSlot.SetCommandSlot(keys[0]);
        }
    }

}
