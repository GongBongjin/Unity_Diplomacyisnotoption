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

        //�ǰ� ��
        if (other.GetType() == typeof(SphereCollider))
        {
            //Target ����
            //if (target == null)
                target = other.gameObject.transform.gameObject;

            //Army ���� ��������
            Army army = other.GetComponent<Army>();

            //�ǰ�ó��
            hp -= army.GetDmg() - def;
            Debug.Log(target + "���� ���ݴ���.");
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
        //        //���� �� �����ȵ� �浹�Ҷ����� �������°� ����ħ
        //
        //        float enemyDmg = army.GetDmg();
        //
        //        hp -= enemyDmg - def;
        //
        //        Debug.Log(this.data.prefab + "��" + other.gameObject.transform + "�� �浹");
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
