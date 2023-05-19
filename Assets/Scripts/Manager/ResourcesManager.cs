using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    private static ResourcesManager instance = null;
    public ResourcesManager Instance
    {
        get 
        {
            if (instance == null)
                instance = this;
            return instance; 
        }
    }

    public enum Product
    {
        FOOD,       // 식량
        WOOD,       // 목재
        STONE,      // 석재
        COPPER      // 구리
    }

    private void Awake()
    {
        instance = this;
    }

    private int maxPopulation;
    private int population;     // 인구

    private int food;           // 식량
    private int wood;           // 목재
    private int stone;          // 석재
    private int copper;         // 구리

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreasesResources(Product product, int qty)
    {
        switch(product)
        {
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
    }

    public bool DecreasesResources(Product product, int qty)
    {
        bool isDecrease = false;
        switch (product)
        {
            case Product.FOOD:
                if(food >= qty)
                {
                    food -= qty;
                    isDecrease = true;
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
        return isDecrease;
    }
}
