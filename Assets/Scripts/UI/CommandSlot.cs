using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandSlot : MonoBehaviour
{
    const int buildingKey = 2100;
    
    [Header("Command Slots")]
    [SerializeField] GameObject slotParent;
    Command command;
    CommandData[] commandDatas;

    GameObject[] slots;
    Image[] img_SlotIcons;

    [Header("Description")]
    [SerializeField] GameObject Description;
    [SerializeField] Text title;
    [SerializeField] GameObject icons;
    [SerializeField] GameObject values;
    [SerializeField] Text[] text_values;


    List<KeyCode> keys = new List<KeyCode>()
        {
            KeyCode.B, KeyCode.C, KeyCode.F, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L,
            KeyCode.M, KeyCode.P, KeyCode.R, KeyCode.U, KeyCode.X, KeyCode.Y, KeyCode.Z
        };

    int targetKey;

    private void Awake()
    {
        command = new Command();
        command.LoadData();


        SetCommandSlotObject();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        {
            foreach(KeyCode keyCode in keys)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    HotKeyCommand((int)keyCode - 32);
                    break;
                }
            }
        }
    }

    // ��� ������ ����
    private void SetCommandSlotObject()
    {
        int cnt = slotParent.transform.childCount;
        slots = new GameObject[cnt];
        img_SlotIcons = new Image[cnt];

        for (int i = 0; i < cnt; i++)
        {
            slots[i] = slotParent.transform.GetChild(i).gameObject;
            img_SlotIcons[i] = slots[i].transform.GetChild(0).GetComponent<Image>();
        }
    }

    // ��� ������ ����
    public void SetCommandSlot(int key)
    {
        //commandDatas = null;
        targetKey = key;
        commandDatas = command.GetCommands(targetKey);

        for (int i = 0; i < commandDatas.Length; i++)
        {
            img_SlotIcons[i].sprite = commandDatas[i].icon;
        }
    }
    public void SetDefaultCommandSlot()
    {
        //commandDatas = null;
        targetKey = 0;
        CommandData cmd = command.GetCommand("NONE");

        for (int i = 0; i < slots.Length; i++)
        {
            img_SlotIcons[i].sprite = cmd.icon;
        }
    }

    public void CommandSlotMouseEnter(int idx)
    {
        if (targetKey == 0) return;
        Description.SetActive(true);
        title.text = commandDatas[idx].name;
        if(commandDatas[idx].refKey >= 1000)
        {
            icons.SetActive(true);
            values.SetActive(true);
            if(commandDatas[idx].refKey == 1000)
            {
                CitizenData data = CitizenManager.Instance.GetCitizenData();
                text_values[0].text = data.food.ToString();
                text_values[1].text = data.wood.ToString();
                text_values[2].text = data.stone.ToString();
                text_values[3].text = data.copper.ToString();
            }
            else if(commandDatas[idx].refKey < 2000)
            {
                CharacterData data = CharacterManager.instance.GetCharacterData(commandDatas[idx].refKey);
                text_values[0].text = data.food.ToString();
                text_values[1].text = data.wood.ToString();
                text_values[2].text = data.stone.ToString();
                text_values[3].text = data.copper.ToString();
            }
            else
            {
                BuildingData data = BuildManager.Instance.GetBuildData(commandDatas[idx].refKey);
                text_values[0].text = "0";
                text_values[1].text = data.qty_Wood.ToString();
                text_values[2].text = data.qty_Stone.ToString();
                text_values[3].text = data.qty_Copper.ToString();
            }
        }
        else
        {
            icons.SetActive(false);
            values.SetActive(false);
        }
    }
    public void CommandSlotMouseExit(int idx)
    {
        Description.SetActive(false);
    }

    // ��� ��ư Ŭ��
    public void CommandSlotClick(int idx)
    {
        int key = commandDatas[idx].refKey;

        if (key > 2000)
        {
            // �Ǽ�
            BuildManager.Instance.ReadyConstruction(key);
        }
        else if (key > 1000)
        {
            SelectionBox.instance.CreateUnit(targetKey, key);
        }
        else if (key == 1000)
        {
            BuildManager.Instance.GetTownHall().AddUnitProduct(key);
            // �ù� ����
            // ��û���� ���� �Ѱ��ֱ�
        }
        else
        {
            HotKeyCommand(key);
        }
    }

    private void HotKeyCommand(int key)
    {
        bool isCommandable = false;
        for (int i = 0; i < commandDatas.Length; i++)
        {
            if(commandDatas[i].refKey == key)
            {
                isCommandable = true;
                break;
            }
        }
        if (!isCommandable) return;

        char hotKey = (char)key;
        
        switch(hotKey)
        {
            case 'B':   // �Ǽ�
                SetCommandSlot(buildingKey);
                break;
            case 'C':   // ���
                //SelectionBox.instance.CreateUnit(targetKey, hotKey);
                break;
            case 'F':   // ���� ��
            case 'H':   // ��ž
            case 'M':   // ����
            case 'P':   // ����
            case 'Y':   // Ȧ��
                SelectionBox.instance.InputCommandKey(hotKey);
                break;
            case 'J':   // ����
                //break;
            case 'K':   // ���
                //break;
            case 'L':   // ��������Ʈ
                //break;
            case 'R':   // ����
                //break;
            case 'U':   // ����
                //break;
            case 'X':   // ����
                //break;
            case 'Z':   // �ڷ�
                //SelectionBox.instance.CreateUnit(targetKey, hotKey);
                break;
        }
    }
}
