using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductObject : Objects
{
    [SerializeField]
    Product product;

    // �ѷ�, ������ (������ ���ܷ� �� ���� ��)
    int key = 3000;
    int maxAmount = 100;
    int curAmount;
    float productTime = 5;
    int output = 1;

    float outputTime;

    private void Awake()
    {
        selectCircle = transform.Find("Circle").gameObject;
        hpBar = transform.Find("HpBar").GetComponent<HpBar>();
        selectCircle.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        curAmount = maxAmount;
    }

    private void Update()
    {
        
    }


    public void SetProductProperty(ProductData data)
    {
        key = data.key;
        maxAmount = data.maxAmount;
        curAmount = maxAmount;
        productTime = data.productTime;
        output = data.output;
    }

    public int GetKey()
    {
        return key;
    }

    public Product GetProductName()
    {
        return product;
    }

    public int Production(float speed)
    {
        outputTime += speed * Time.deltaTime;
        if(outputTime > productTime)
        {
            outputTime -= productTime;
            return SpendResources(output);
        }

        return 0;
    }

    public int SpendResources(int amount)
    {
        int value = amount;
        curAmount -= amount;

        if (curAmount <= 0)
        {
            value += curAmount;
            // ��ȯ���� ������ �� �ڿ� ��
            return -value - 1;
        }
        return value;
    }

}
