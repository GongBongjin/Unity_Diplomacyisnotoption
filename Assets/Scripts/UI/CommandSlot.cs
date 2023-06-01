using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandSlot : MonoBehaviour
{
    const int buildingKey = 3000;
    
    [Header("Command Slots")]
    [SerializeField] GameObject slotParent;
    Command command;
    CommandData[] commandDatas;

    GameObject[] slots;
    Image[] img_SlotIcons;

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


    // ��� ��ư Ŭ��
    public void CommandSlotClick(int idx)
    {
        int key = commandDatas[idx].refKey;

        if (key > 2000)
        {
            // �Ǽ�
            BuildManager.Instance.AddBuilding(key);
        }
        else if (key > 1000)
        {
            // ���� ����
            // �跰���� ���� �Ѱ��ֱ�
        }
        else if (key == 1000)
        {
            BuildManager.Instance.GetTownHall().CreateUnit(key);
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
        Debug.Log(key + " : " + hotKey);
        
        switch(hotKey)
        {
            case 'B':   // �Ǽ�
                SetCommandSlot(buildingKey);
                break;
            case 'C':   // ���
                break;
            case 'F':   // ���� ��
            case 'H':   // ��ž
            case 'M':   // ����
            case 'P':   // ����
            case 'Y':   // Ȧ��
                SelectionBox.instance.InputCommandKey(hotKey);
                break;
            case 'J':   // ����
                break;
            case 'K':   // ���
                break;
            case 'L':   // ��������Ʈ
                break;
            case 'R':   // ����
                break;
            case 'U':   // ����
                break;
            case 'X':   // ����
                break;
            case 'Z':   // �ڷ�
                break;
        }
    }
}
