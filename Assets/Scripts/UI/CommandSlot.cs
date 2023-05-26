using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandSlot : MonoBehaviour
{
    [Header("Command Slots")]
    [SerializeField] GameObject slotParent;
    Command command;
    CommandData[] commandDatas;
    GameObject[] slots;
    Image[] img_SlotIcons;

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
        commandDatas = command.GetCommands(key);

        for (int i = 0; i < 15; i++)
        {
            img_SlotIcons[i].sprite = commandDatas[i].icon;
        }
    }

    // ��� ��ư Ŭ��
    public void CommandSlotClick(int idx)
    {
        int key = commandDatas[idx].refKey;

        if (key == 0) return;

        if (key == 3000)
            SetCommandSlot(key);
        else if (key / 1000 == 2)
        {
            BuildManager.Instance.AddBuilding(key);
            // �Ǽ�
        }
    }
}
