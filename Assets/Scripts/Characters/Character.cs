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

public class Character : Objects
{ 
    [HideInInspector] protected Animator animator;
    [HideInInspector] protected NavMeshAgent nvAgent;
    [HideInInspector] protected Rigidbody rigidBody;
    [HideInInspector] protected CapsuleCollider bodyCollider;
    [HideInInspector] protected SphereCollider atkCollider;

    [SerializeField] protected float hp;
    [SerializeField] protected float maxHp;
    [SerializeField] protected float dmg;
    [SerializeField] protected float def;
    [SerializeField] protected bool isAttacking = false;
    [SerializeField] protected bool isDying = false;
    [SerializeField] protected bool isHitted = false;

    protected float time = 0.0f;

    protected CharacterData data;
    public CharacterKey key;
    protected CharacterType characterType;

    protected void Awake()
    {
        animator = GetComponent<Animator>();
        nvAgent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
        bodyCollider = GetComponent<CapsuleCollider>();
        bodyCollider.enabled = true;
        atkCollider = GetComponent<SphereCollider>();
        atkCollider.enabled = true;
        selectCircle = transform.Find("Circle").gameObject;
        hpBar = transform.Find("HpBar").GetComponent<HpBar>();
        selectCircle.SetActive(false);
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        if (!isDying)
            hpBar.SetProgressBar(hp / maxHp);
        else
            hpBar.SetActiveProgressBar(false);

        base.Update();
    }

    public void SetData(CharacterData characterData)
    {
        data = characterData;
        key = (CharacterKey)data.key;
        characterType = data.characterType;
        maxHp = data.maxHp;
        hp = maxHp;
        dmg = data.dmg;
        def = data.def;
    }

    public bool GetAttacking() { return isAttacking; }
    
    public float GetDmg() { return dmg; }

    //Animation Event Function

    private void StartAttack()
    {
        isAttacking = true;

        atkCollider.enabled = false;

        time = 5.0f;
    }
    private void EndAttack()
    {
        isAttacking = false;

        atkCollider.enabled = true;
    }

    private void StartDead()
    {
        isDying = true;
        bodyCollider.enabled = false;
        atkCollider.enabled = false;
    }

    private void EndDead()
    {
        gameObject.SetActive(false);
    }

    //Children class Function
    public virtual void Move(Vector3 destPos) { }
    public virtual void Attack() { }

    //Collision Function
    public virtual void OnTriggerEnter(Collider other) { }






    /*
     army & enmey 공통부분

    이동시에 적군을 만나면 공격하는 함수
    아군 - 히트당하면
    적군 - range 안에 들어오면

    target 위치받아서 이동 -> 공격함수 실행
     
     */
}