using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private GameObject target;
    public Vector3 targetPos = new Vector3(0,0,0);

    public bool isMainEvent = false;

    protected override void Start()
    {
        hpBar.SetFrontColor(Color.red);
        base.Start();
    }

    protected override void Update()
    {
        if(Input.GetKey(KeyCode.Z)) 
        {
            CharacterManager.instance.SetAttackCityHall(3); 
        }

        if(isMainEvent) 
        {
            if (target == null)
                Move(targetPos);
        }

        ApporachDestination();

        ShowHpBar();

        base.Update();
    }

    public override void Move(Vector3 destPos)
    {
        if (isAttacking) return;
        if (isDying) return;
        
        animator.SetFloat("MoveSpeed", 3.0f);
        //nvAgent.destination = transform.position + new Vector3(10,0,10);
        nvAgent.SetDestination(transform.position + new Vector3(10, 0, 10));

        //Debug.Log("cibla" + nvAgent.destination);
        //Debug.Log(nvAgent.SetDestination(destPos));

        //float distance = Vector3.Distance(transform.position, destPos);
        //if (Vector3.Distance(transform.position, destPos) < 1.0f || isAttacking)
        //{
        //    animator.SetFloat("MoveSpeed", 0.0f);
        //    nvAgent.isStopped = true;
        //    nvAgent.ResetPath();
        //}
    }

    private void ApporachDestination()
    {
        if (isDying) return;

        float distance = Vector3.Distance(transform.position, nvAgent.destination);
        if (distance < 1.0f || isAttacking)
        {
            animator.SetFloat("MoveSpeed", 0.0f);
            nvAgent.isStopped = true;
            nvAgent.ResetPath();
        }
    }

    public override void Attack()
    {
        if (isAttacking) return;
        if (isDying) return;

        isAttacking = true;
        nvAgent.isStopped = true;
        nvAgent.ResetPath();
        animator.SetTrigger("Attack");
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (isDying) return;

        if (other.gameObject.transform.tag == "Enemy") return;
        if (other.gameObject.transform.tag == "Product") return;
        if (other.gameObject.transform.tag == "Terrain") return;

        //피격 시
        if (other.GetType() == typeof(SphereCollider))
        {
            //Target 설정
            //if (target == null)
                target = other.gameObject.transform.gameObject;

            //Army 정보 가져오기
            Army army = other.GetComponent<Army>();

            //피격처리
            //if (army.GetAttacking())//
                hp -= army.GetDmg() - (army.GetDmg() * (def / 100));

            //Debug.Log(transform.gameObject +"가 " + target + "에게" + (army.GetDmg() - def));
            if (hp <= 0)
            {
                isDying = true;
                animator.SetTrigger("Dead");
            }
            isHitted = true;
            if (isHitted)
                Move(target.transform.position);

        }
        else if(other.GetType() == typeof(CapsuleCollider) || other.GetType() == typeof(BoxCollider))
        {
            isHitted = false;
            transform.LookAt(other.gameObject.transform.position);
            Attack();
        }
    }
    private void ShowHpBar()
    {
        //if(isSelected)
        //    hpBar.SetActiveProgressBar(true);

        if (time > 0.0f)
            hpBar.SetActiveProgressBar(true);
        else
            hpBar.SetActiveProgressBar(false);

        time -= 1.0f * Time.deltaTime;
    }
}
