using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleInformation : MonoBehaviour
{
    int targetKey = 0;

    [Header("Single Information")]
    [SerializeField] Text text_Name;
    [SerializeField] Image img_Profile;
    [SerializeField] Text text_GroupCount;

    [Header("Production"), Tooltip("Building Component")]
    [SerializeField] GameObject production;
    [SerializeField] Transform productList;
    [SerializeField] Slider productDuration;
    Image[] img_ProductIcons;

    [Header("Abilities"), Tooltip("Character Component")]
    [SerializeField] GameObject ability;
    Image[] abilityIcon;
    Text[] text_AbilityValue;

    [Header("Discription")]
    [SerializeField] Text text_Discription;

    private void Awake()
    {
        SetProductObject();
        SetAbilitiesObject();
    }


    void Start()
    {
        production.SetActive(false);
        ability.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region [AwakeSetting]
    
    private void SetProductObject()
    {
        int cnt = productList.childCount;
        img_ProductIcons = new Image[cnt];
        for (int i = 0; i < cnt; i++)
        {
            img_ProductIcons[i] = productList.GetChild(i).GetChild(0).GetComponent<Image>();
            // img_ProductIcons[i].sprite = GetEmptySprite();
        }
    }

    private void SetAbilitiesObject()
    {
        Transform transform = ability.transform;
        int cnt = transform.childCount;
        
        abilityIcon = new Image[cnt];
        text_AbilityValue = new Text[cnt];
        for(int i = 0; i < cnt; i++) 
        {
            abilityIcon[i] = transform.GetChild(i).GetChild(0).GetComponent<Image>();
            text_AbilityValue[i] = transform.GetChild(i).GetChild(1).GetComponent<Text>();
        }
    }
    #endregion

    public void ShowInformation(int key, int count = 1)
    {
        targetKey = key;

        int type = key / 1000;
        if (type == 1)
        {
            CharacterData characterData = DataManager.instance.GetCharacterDatas(key);

            img_Profile.sprite = characterData.sprite;
            text_Name.text = characterData.prefab.name;
            text_GroupCount.text = count.ToString();
            text_Discription.text = characterData.prefab.name;

            SetAbilities();
        }
        else if (type == 2)
        {
            // Building
            BuildingData buildingData = BuildManager.Instance.GetBuildData(key);
            img_Profile.sprite = buildingData.icon;
            text_Name.text = buildingData.name;
            text_GroupCount.text = count.ToString();
            text_Discription.text = buildingData.discription;
            //for (int i = 0; i < abilities.Length; i++)
            //{
            //    abilities[i].SetActive(false);
            //}
        }
    }

    private void SetAbilities()
    {
        production.SetActive(false);
        ability.SetActive(true);
        // Character
        // getCharacterAbility(key)
    }

    // 유닛이나 업그레이드 시 소요시간 업데이트
    public void UpdateProductionDuration(float curValue, float maxValue)
    {
        production.SetActive(true);
        ability.SetActive(false);

        if (curValue > maxValue)
            return;

        float ratio = curValue / maxValue;
        productDuration.value = ratio;
    }
}
