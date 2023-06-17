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

    public int GetMaxPopulation() {  return maxPopulation; }
    public int GetMaxStorage() { return maxStorage; }
    public int GetPopulation() { return GetProductResources(Product.POPULATION); }
    public int GetFood() { return GetProductResources(Product.FOOD); }
    public int GetWood() { return GetProductResources(Product.WOOD); }
    public int GetStone() { return GetProductResources(Product.STONE); }
    public int GetCopper() { return GetProductResources(Product.COPPER); }


    public int GetProductResources(Product product)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            if (product.Equals(resources[i].name))
            {
                return resources[i].value;
            }
        }
        return 0;
    }
    public bool GetCheckResourceCount(Product product, int value)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            if (product.Equals(resources[i].name))
            {
                if (resources[i].value >= value)
                    return true;
            }
        }
        return false;
    }

    private void SetResourceObject()
    {
        int cnt = ResourceParents.Length;
        resources = new NaturalResources[cnt];
        for (int i = 0; i < cnt; i++)
        {
            resources[i] = new NaturalResources();
            resources[i].name = (Product)i;
            if(i != 0)
                resources[i].value = 50;
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
                if(resources[i].value > maxStorage)
                    resources[i].value = maxStorage;
                break;
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
                    break;
                }
            }
        }
        SetSliderValue();
    }
}
