using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CharacterType
{
    CITIZEN = 1,
    ARMY,
    ENMEY
}

public enum CharacterState
{
    IDLE, MOVE, ATTACK, DEAD
}

public class Character : MonoBehaviour
{
    protected Rigidbody rigidbody;
    protected Animator animator;

    protected NavMeshAgent nvAgent;

    protected float hp;
    protected float maxHp;

    private Dictionary<int, CharacterData> characterDatas = new Dictionary<int, CharacterData>();

    protected void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        nvAgent = GetComponent<NavMeshAgent>();
    }

    protected void Start()
    {
        characterDatas = DataManager.instance.GetCharacterDatas();
    }

    protected void Update()
    {
        
    }

    protected void Move()
    {
        
    }
}
