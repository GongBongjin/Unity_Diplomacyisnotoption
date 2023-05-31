using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Army : Character
{
    private Enemy target;

    public bool isTargeting;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if(target != null && !isTargeting) 
            Move(target.transform.position);

        base.Update();
    }

    public override void Move(Vector3 destPos)
    {
        if (!isTargeting)
        {
            if (isAttacking) return;
            if (isDying) return;
        }

        animator.SetFloat("MoveSpeed", 3.0f);
        nvAgent.SetDestination(destPos);
        isTargeting = false;
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
        if(isDying) return;

        target = other.gameObject.GetComponent<Enemy>();

        if (other.gameObject.transform.tag == "Enemy")
        {
            if(other.GetType() == typeof(SphereCollider))
            {
                hp -= target.GetDmg() - def;

                if (hp <= 0)
                {
                    isDying = true;
                    animator.SetTrigger("Dead");
                }
            }
            else if(other.GetType() == typeof(CapsuleCollider))
            {
                transform.LookAt(target.transform.position);
                Attack();
            }
        }
    }

    //SelectionBox Command Functions
    public void StopCommand()
    {
        nvAgent.isStopped = true;
        nvAgent.ResetPath();
    }

    public void AttackCommand(Vector3 destPos)
    {
        //destPos�� ������������ ������ �ִ����� ���� �Ǵ��� �ؾ���
        //�ǰ��� ���Ѵٸ�?
        //
    }

    public void PatrolCommand(Vector3 currentPos, Vector3 nextPos)
    {
        //���� ��ġ�� ���� ��ġ�� �Է¹޾Ƽ� �Ʒ��� ���� ����.
        animator.SetFloat("MoveSpeed", 3.0f);
        nvAgent.SetDestination(nextPos);

        if (Vector3.Distance(transform.position, nextPos) < 1.0f)
        {
            nvAgent.ResetPath();
        }

    }


}
