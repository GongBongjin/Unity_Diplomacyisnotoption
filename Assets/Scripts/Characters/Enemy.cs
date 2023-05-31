using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private GameObject target;
    private Vector3 targetPos;
    private SphereCollider rangeCollider;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if(target!=null)
            Move(target.transform.position);

        base.Update();
    }

    public override void Move(Vector3 destPos)
    {
        if (isAttacking) return;
        if (isDying) return;

        animator.SetFloat("MoveSpeed", 3.0f);
        nvAgent.SetDestination(destPos);
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

        //피격 시
        if (other.GetType() == typeof(SphereCollider))
        {
            //Target 설정
            //if (target == null)
                target = other.gameObject.transform.gameObject;

            //Army 정보 가져오기
            Army army = other.GetComponent<Army>();

            //피격처리
            hp -= army.GetDmg() - def;
            Debug.Log(target + "에게 공격당함.");
            if (hp <= 0)
            {
                isDying = true;
                animator.SetTrigger("Dead");
            }
        }
        else if(other.GetType() == typeof(CapsuleCollider))
        {
            transform.LookAt(other.gameObject.transform.position);
            Attack();
        }
       

        //if (other.gameObject.transform.tag == "Army")
        //{
        //    if (other.GetType() == typeof(SphereCollider))
        //    {
        //        Army army = other.gameObject.GetComponent<Army>();
        //        //여기 넘 맘에안듬 충돌할때마다 가져오는거 개빡침
        //
        //        float enemyDmg = army.GetDmg();
        //
        //        hp -= enemyDmg - def;
        //
        //        Debug.Log(this.data.prefab + "가" + other.gameObject.transform + "와 충돌");
        //        if (hp <= 0)
        //        {
        //            isDying = true;
        //            //atkCollider.enabled = false;
        //            //bodyCollider.enabled = false;
        //            animator.SetTrigger("Dead");
        //        }
        //    }
        //    else if (other.GetType() == typeof(CapsuleCollider))
        //    {
        //        Attack();
        //    }
        //}
    }

    private void SetDestination()
    {

    }
}
