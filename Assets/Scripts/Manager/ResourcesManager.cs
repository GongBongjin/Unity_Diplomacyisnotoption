using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Attach MainCanvas ResourcePanel

public enum Product
{
    POPULATION, // 인구
    FOOD,       // 식량
    WOOD,       // 목재
    STONE,      // 석재
    COPPER      // 구리
}
public class ResourcesManager : MonoBehaviour
{
    struct NaturalResources
    {
        public Product name;
        public int value;
        public Slider slider;
        public Text text;
    }

    [Header("Resources")]
    [SerializeField, Tooltip("(popul, food, wood, stone, copper) Object in ResourcePanel")]
    GameObject[] ResourceParents;
    NaturalResources[] resources;


    private int maxPopulation = 10;
    private int maxStorage = 100;


    private void Awake()
    {
        SetResourceObject();
    }


    // Start is called before the first frame update
    void Start()
    {
        SetSliderValue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetResourceObject()
    {
        int cnt = ResourceParents.Length;
        resources = new NaturalResources[cnt];
        for (int i = 0; i < cnt; i++)
        {
            resources[i] = new NaturalResources();
            resources[i].name = (Product)i;
            resources[i].value = 0;
            resources[i].slider = ResourceParents[i].transform.Find("Slider").GetComponent<Slider>();
            resources[i].text = ResourceParents[i].transform.Find("Text_Value").GetComponent<Text>();
        }
    }

    private void SetSliderValue()
    {
        int cnt = ResourceParents.Length;
        for (int i = 0; i < cnt; i++)
        {
            if(i == 0)
            {
                resources[i].slider.value = resources[i].value / (maxPopulation * 1.0f);
                resources[i].text.text = resources[i].value.ToString() + "/" + maxPopulation.ToString();
            }
            else
            {
                resources[i].slider.value = resources[i].value / (maxStorage * 1.0f);
                resources[i].text.text = resources[i].value.ToString() + "/" + maxStorage.ToString();
            }
        }
    }


    public void UpdateMaxPopulation(int value)
    {
        maxPopulation += value;
        SetSliderValue();
    }
    public void UpdateMaxStorage(int value)
    {
        maxStorage += value;
        SetSliderValue();
    }

    public void IncreasesResources(Product product, int qty)
    {
        for(int i = 0; i < resources.Length; i++)
        {
            if (product.Equals(resources[i].name))
            {
                resources[i].value += qty;
            }
        }
        SetSliderValue();
    }

    public void DecreasesResources(Product product, int qty)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            if (product.Equals(resources[i].name))
            {
                if (resources[i].value >= qty)
                {
                    resources[i].value -= qty;
                }
            }
        }
        SetSliderValue();
    }
}
