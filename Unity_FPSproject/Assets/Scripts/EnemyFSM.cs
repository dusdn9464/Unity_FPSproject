using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//몬스터 유한상태머신
public class EnemyFSM : MonoBehaviour
{
    //몬스터 상태 이넘문
    enum EnemyState
    {
        Idle, Move, Attack, Return, Damaged, Die
    }

    EnemyState state;   //몬스터 상태변수
    EnemyState saveState;   //몬스터 상태변수

    NavMeshAgent navi;

    //유용한 기능
    #region "Idle 상태에 필요한 변수들"
    public float traceDist = 10.0f;     //플레이어 탐지범위
    #endregion

    #region "Move 상태에 필요한 변수들"
    public float attackDist = 4.0f;     //플레이어 공격범위
    public float enemySpeed = 5.0f;
    #endregion

    #region "Attack 상태에 필요한 변수들"
    float attackCount = 2.0f;
    float currentCount = 0f;
    #endregion

    #region "Return 상태에 필요한 변수들"
    Transform spawnPoint;        //에너미 스폰 위치
    #endregion

    #region "Damaged 상태에 필요한 변수들"
    public float enemyHp = 100.0f;
    float currentHp = 100.0f;
    #endregion

    #region "Die 상태에 필요한 변수들"
    //float enemyHp = 10.0f;
    #endregion

    Transform player;
    Animator anim;
    Quaternion startRotation;   //몬스터 시작위치

    
    
    
    // Start is called before the first frame update
    void Start()
    {
        //몬스터 상태 초기화
        state = EnemyState.Idle;
        anim = GetComponentInChildren<Animator>();
        startRotation = Quaternion.identity;
        player = GameObject.Find("Player").transform;
        spawnPoint = GameObject.Find("SpawnPoint").transform;
        //Quaternion.identity => 0으로 초기화
        navi = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //상태에 따른 행동처리
        switch (state)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Damaged:
                Damaged();
                break;
            case EnemyState.Die:
                Die();
                break;
        }

    }

    private void Idle()
    {
        //1. 플레이어와 일정범위가 되면 이동상태로 변경 (탐지범위)
        //- 플레이어 찾기 (GameObject.Find("Player"))
        
        //- 일정거리 20미터 (거리비교 : Distance, magnitude 아무거나)
        float distance = Vector3.Distance(GameObject.Find("Player").transform.position, transform.position);


        //- 상태변경 //state = EnemyState.Move;
        if (distance <= traceDist)
        {
            state = EnemyState.Move;

            //애니메이션
            anim.SetTrigger("Move");
        }
        //- 상태전환 출력
        Debug.Log("상태 : Idle");
    }

    private void Move()
    {
        //1. 플레이어를 향해 이동 후 공격범위 안에 들어오면 공격상태로 변경
        //transform.LookAt(GameObject.Find("Player").transform);
        //Vector3 dir = Vector3.forward;

        //transform.Translate(dir * enemySpeed * Time.deltaTime);

        navi.SetDestination(player.position);

        float distance = Vector3.Distance(GameObject.Find("Player").transform.position, transform.position);
        float returnDist = Vector3.Distance(spawnPoint.transform.position, transform.position);

        if(distance <= attackDist)
        {
            state = EnemyState.Attack;
            anim.SetTrigger("Attack");
        }
        if( returnDist >= 30.0f )
        {
            state = EnemyState.Return;
            anim.SetTrigger("Return");
        }
        //2. 플레이어를 추격하더라도 처음위치에서 일정범위를 넘어가면 리턴상태로 돌아오기
        //- 플레이어 처럼 캐릭터컨트롤러를 이용하기
        //- 공격범위 1미터
        //- 상태변경 
        //- 상태전환 출력
        Debug.Log("상태 : Move");
    }

    private void Attack()
    {
        //1. 플레이어가 공격범위 안에 있다면 일정한 시간간격으로 플레이어 공격
        //2. 플레이어를 추격하더라도 처음위치에서 일정범위를 넘어가면 리턴상태로 돌아오기
        currentCount += Time.deltaTime;

        if(currentCount > attackCount)
        {
            print("공격");
            currentCount = 0;
        }

        float distance = Vector3.Distance(GameObject.Find("Player").transform.position, transform.position);

        if(distance > attackDist)
        {
            state = EnemyState.Move;
            anim.SetTrigger("Move");
        }
        //- 플레이어처럼 캐릭터컨트롤러 이용하기
        //- 공격범위 2미터
        //- 상태변경
        //- 상태전환 출력
        Debug.Log("상태 : Attack");
    }

    private void Return()
    {
        //1. 몬스터가 플레이어를 추격하더라도 처음 위치에서 일정범위를 벗어나면 다시 돌아옴
        //transform.LookAt(spawnPoint.transform);
        //Vector3 dir = Vector3.forward;

        //transform.Translate(dir * enemySpeed * Time.deltaTime);

        navi.SetDestination(spawnPoint.position);

        float idleDist = Vector3.Distance(spawnPoint.transform.position, transform.position);
        float distance = Vector3.Distance(GameObject.Find("Player").transform.position, transform.position);

        if(idleDist <0.5f)
        {
            state = EnemyState.Idle;
            anim.SetTrigger("Idle");
        }
        if(distance < traceDist)
        {
            state = EnemyState.Move;
            anim.SetTrigger("Move");
        }
        //- 처음위치에서 일정범위 30미터
        //- 상태변경
        //- 상태전환 출력
        Debug.Log("상태 : return");
    }

    //피격상태 (Any State)
    private void Damaged()
    {
        //코루틴을 사용하자
        //1. 몬스터 체력이 1이상
        //2. 다시 이전상태로 변경0
        //- 상태변경
        //- 상태전환 출력
        
        StartCoroutine(EnemeyDameged());
    }

    IEnumerator EnemeyDameged()
    {
        currentHp -= 30.0f;
        print("Hp : 100/" + currentHp);

        state = saveState;
        yield return new WaitForSeconds(1.0f);
    }

    //죽음상태 (Any State)
    private void Die()
    {
        //코루틴을 사용하자
        //1. 체력이 0이하
        //2. 몬스터 오브젝트 삭제
        //- 상태변경
        //- 상태전환 출력 (죽었다)
        StopAllCoroutines();
        StartCoroutine(EnemyDie());
    }

    IEnumerator EnemyDie()
    {
        //Destroy(gameObject);
        yield return new WaitForSeconds(0.5f);
    }

    public void EnemeyDamege()
    {
        //적 체력 -= 데미지
        if(currentHp > 0)
        {
            saveState = state;
            state = EnemyState.Damaged;
            anim.SetTrigger("Damaged");
            print("State : " + state.ToString());
        }
        else if(currentHp <= 0)
        {
            state = EnemyState.Die;
            anim.SetTrigger("Die");
            print("State : " + state.ToString());
        }
    }
}
