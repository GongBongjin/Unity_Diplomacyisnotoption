using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductObject : MonoBehaviour
{
    enum Product
    {
        Food,
        Wood,
        Stone,
        Copper
    }

    [SerializeField]
    Product product;

    // �ѷ�, ������
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
}
