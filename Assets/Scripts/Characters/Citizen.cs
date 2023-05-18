using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour
{
    [SerializeField, Tooltip("RightHand-JointItemR")]
    Transform rightHand;
    GameObject[] tools = new GameObject[4]; // ����, ��ġ, ����, ���

    enum CitizenState
    {
        Idle,
        Move,
        Die,
        Felling,    // ����
        Building,   // �Ǽ�
        Fix,        // ����
        Hoeing,     // ���
        Mining     // ä��
    }
    CitizenState curState = CitizenState.Idle;

    private void Awake()
    {
        if (rightHand == null)
            Debug.Log("<color=red>RightHand is Null</color>");

        tools[0] = rightHand.Find("Axe").gameObject;
        tools[1] = rightHand.Find("Hammer").gameObject;
        tools[2] = rightHand.Find("Hoe").gameObject;
        tools[3] = rightHand.Find("PickAxe").gameObject;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void SetCitizenState(CitizenState state)
    {
        if (curState == state) return;

        curState = state;
    }
}
