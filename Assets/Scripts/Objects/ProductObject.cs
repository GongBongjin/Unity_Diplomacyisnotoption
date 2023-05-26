using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductObject : MonoBehaviour
{
    [SerializeField]
    Product product;

    // ÃÑ·®, ³²Àº¾ç
    int maxAmount = 100;
    int curAmount;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        curAmount = maxAmount;
    }

    private void Update()
    {
        
    }

    public Product GetProductName()
    {
        return product;
    }

    public int SpendResources(int amount)
    {
        int value = amount;
        curAmount -= amount;

        if (curAmount <= 0)
        {
            value += curAmount;
        }

        return value;
    }
}
