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

    Building building;

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
        if (!gameObject.activeSelf)
            return;

        if (building == null)
        {
            production.SetActive(false);
            return;
        }

        if(building.GetIsProduction())
        {
            production.SetActive(true);

            int[] keys = building.GetProductionKeys();
            for(int i = 0; i < keys.Length; i++)
            {
                if (keys[i] == 0)
                    img_ProductIcons[i].sprite = UIManager.Instance.GetDefaultSpriteIcon();
                else if (keys[i] == 1000)
                    img_ProductIcons[i].sprite = CitizenManager.Instance.GetCitizenData().icon;
                else
                    img_ProductIcons[i].sprite = CharacterManager.instance.GetCharacterData(keys[i]).sprite;

            }
            productDuration.value = building.GetProductionProgress();
        }
        else
        {
            production.SetActive(false);
        }
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

    public void SetProductionBuilding(Building building)
    {
        //Debug.Log(building.name);
        this.building = building;
    }

    public void ShowInformation(int key, int count = 1)
    {
        targetKey = key;
        int type = key / 1000;
        SetAbilities(false);
        if (key == 1000)
        {
            building = null;
            CitizenData characterData = CitizenManager.Instance.GetCitizenData();
            img_Profile.sprite = characterData.icon;
            text_Name.text = characterData.name;
            text_GroupCount.text = count.ToString();
            text_Discription.text = characterData.description;
        }
        else if (type == 1)
        {
            building = null;
            CharacterData characterData = DataManager.instance.GetCharacterDatas(key);
            
            img_Profile.sprite = characterData.sprite;
            text_Name.text = characterData.prefab.name;
            text_GroupCount.text = count.ToString();
            text_Discription.text = characterData.description;
            SetAbilities(true);
            text_AbilityValue[0].text = characterData.dmg.ToString();
            text_AbilityValue[1].text = characterData.def.ToString();
            text_AbilityValue[2].text = characterData.moveSpeed.ToString();
            text_AbilityValue[3].text = characterData.sightValue.ToString();
            text_AbilityValue[4].text = characterData.attackSpeed.ToString();
        }
        else if (type == 2)
        {
            // Building
            BuildingData buildingData = BuildManager.Instance.GetBuildData(key);
            img_Profile.sprite = buildingData.icon;
            text_Name.text = buildingData.name;
            text_GroupCount.text = count.ToString();
            text_Discription.text = buildingData.description;
            //for (int i = 0; i < abilities.Length; i++)
            //{
            //    abilities[i].SetActive(false);
            //}
        }
        else if (type == 3)
        {
            building = null;
            // Building
            ProductData productData = TerrainObjectManager.Instance.GetProductData(key);
            img_Profile.sprite = productData.icon;
            text_Name.text = productData.name;
            text_GroupCount.text = count.ToString();
            text_Discription.text = productData.description;
            //for (int i = 0; i < abilities.Length; i++)
            //{
            //    abilities[i].SetActive(false);
            //}
        }
    }

    private void SetAbilities(bool active)
    {
        ability.SetActive(active);
    }
    private void SetProduction(bool active)
    {
        production.SetActive(active);
    }
        
    // 빌딩에서 업그레이드 하고 있는 정보를 가져와야 함
    // 현재 싱글에서는 빌딩 정보를 가지고 있지 않음
    // UI 매니저에서 받아서 사용하기에는 어떤 것을 선택하고 있는지 모름
    // 빌딩 정보를 가지고 있는게 제일 베스트일지도
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
