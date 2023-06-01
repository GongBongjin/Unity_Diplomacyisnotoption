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
        public string name;
        public Slider slider;
        public Text text;
    }

    [Header("Resources")]
    [SerializeField, Tooltip("(popul, food, wood, stone, copper) Object in ResourcePanel")]
    GameObject[] ResourceParents;
    NaturalResources[] resources;


    private int maxPopulation = 10;
    private int maxStorage = 100;

    private int population;     // 인구
    private int food;           // 식량
    private int wood;           // 목재
    private int stone;          // 석재
    private int copper;         // 구리

    private delegate void onResourcesChange();
    private onResourcesChange OnResourcesChange;

    private void Awake()
    {
        SetResourceObject();
        OnResourcesChange += SetSliderValue;
    }


    // Start is called before the first frame update
    void Start()
    {
        
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
            resources[i].name = ResourceParents[i].name;
            resources[i].slider = ResourceParents[i].transform.Find("Slider").GetComponent<Slider>();
            resources[i].text = ResourceParents[i].transform.Find("Text_Value").GetComponent<Text>();
        }
    }

    private void SetSliderValue()
    {
        int cnt = ResourceParents.Length;
        for (int i = 0; i < cnt; i++)
        {
            resources[i].slider.value = 1.0f;
            //resources[i].text.text
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
        switch(product)
        {
            case Product.POPULATION:
                population += qty;
                break;
            case Product.FOOD:
                food += qty;
                break;
            case Product.WOOD:
                wood += qty; 
                break;
            case Product.STONE:
                stone += qty;
                break;
            case Product.COPPER:
                copper += qty;
                break;
        }
        SetSliderValue();
    }

    public void DecreasesResources(Product product, int qty)
    {
        switch (product)
        {
            case Product.POPULATION:
                if (population >= qty)
                {
                    population -= qty;
                }
                break;
            case Product.FOOD:
                if(food >= qty)
                {
                    food -= qty;
                }
                break;
            case Product.WOOD:
                if(wood >= qty)
                {
                    wood -= qty;
                }
                break;
            case Product.STONE:
                if(stone >= qty)
                {
                    stone -= qty;
                }
                break;
            case Product.COPPER:
                if(copper >= qty)
                {
                    copper -= qty;
                }
                break;
        }
        SetSliderValue();
    }
}
