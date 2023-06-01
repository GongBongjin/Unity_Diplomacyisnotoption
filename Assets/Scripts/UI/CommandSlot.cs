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

    // 명령 아이콘 설정
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

    // 명령 데이터 설정
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


    // 명령 버튼 클릭
    public void CommandSlotClick(int idx)
    {
        int key = commandDatas[idx].refKey;

        if (key > 2000)
        {
            // 건설
            BuildManager.Instance.AddBuilding(key);
        }
        else if (key > 1000)
        {
            // 유닛 생산
            // 배럭에게 정보 넘겨주기
        }
        else if (key == 1000)
        {
            BuildManager.Instance.GetTownHall().CreateUnit(key);
            // 시민 생산
            // 시청에게 정보 넘겨주기
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
            case 'B':   // 건설
                SetCommandSlot(buildingKey);
                break;
            case 'C':   // 취소
                break;
            case 'F':   // 어택 땅
            case 'H':   // 스탑
            case 'M':   // 무브
            case 'P':   // 순찰
            case 'Y':   // 홀드
                SelectionBox.instance.InputCommandKey(hotKey);
                break;
            case 'J':   // 공업
                break;
            case 'K':   // 방업
                break;
            case 'L':   // 랠리포인트
                break;
            case 'R':   // 수리
                break;
            case 'U':   // 업글
                break;
            case 'X':   // 삭제
                break;
            case 'Z':   // 뒤로
                break;
        }
    }
}
