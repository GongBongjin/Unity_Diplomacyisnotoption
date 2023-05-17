using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour
{
    [SerializeField]
    Transform rightHand;
    GameObject[] tools = new GameObject[3]; // ��ġ, ����, ���

    enum CitizenState
    {
        Idle,
        Move,
        Die,
        Building,
        Fix,
        Felling,
        Mining
    }
    CitizenState state = CitizenState.Idle;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
