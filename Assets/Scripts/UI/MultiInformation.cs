using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiInformation : MonoBehaviour
{
    int[] targetKeys;

    [SerializeField] GameObject unitParent;
    GameObject[] units;
    Image[] img_Icons;
    Text[] text_UnitCounts;

    private void Awake()
    {
        SetMultiInformation();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 다수 객체 선택용 오브젝트 설정
    private void SetMultiInformation()
    {
        int cnt = unitParent.transform.childCount;
        targetKeys = new int[cnt];
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

    public void ShowInformation(int[] keys, int[] count)
    {
        int cnt = units.Length;
        for (int i = 0; i < cnt; i++)
        {
            targetKeys[i] = 0;
            units[i].SetActive(false);
        }
        cnt = keys.Length;
        for (int i = 0; i < cnt; i++)
        {
            targetKeys[i] = keys[i];
            units[i].SetActive(true);
            if (keys[i] == 1000)
            {
                CitizenData citizenData = CitizenManager.Instance.GetCitizenData();
                img_Icons[i].sprite = citizenData.icon;
                text_UnitCounts[i].text = count[i].ToString();
                continue;
            }
            CharacterData characterData = DataManager.instance.GetCharacterDatas(keys[i]);

            img_Icons[i].sprite = characterData.sprite;
            text_UnitCounts[i].text = count[i].ToString();
        }
    }

    public void OnClickObjectIcon(int idx)
    {
        SelectionBox.instance.SelectCharacterKey(targetKeys[idx]);
    }
}
