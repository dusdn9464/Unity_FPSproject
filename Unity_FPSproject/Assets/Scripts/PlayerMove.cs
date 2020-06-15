using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    
    public float speed = 5.0f;

    CharacterController cc;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();    //대각선이동 속도를 상하좌우속도와 동일하게 만들기
        //게임에 따라 일부로 대각선은 빠르게 이동하도로고 하는 경우도 있다
        //이럴때는 벡터의 정규화(노말라이즈)를 하면 안된다
        //transform.Translate(dir * speed * Time.deltaTime);

        //카메라가 보는 방향으로 이동해야 한다
        dir = Camera.main.transform.TransformDirection(dir);
        //transform.Translate(dir * speed * Time.deltaTime);

        //심각한 문제 : 하늘 날라다님, 땅 뚫음, 충돌처리 안됨
        //캐릭터컨트롤러 컴포넌트를 사용한다!!
        //캐릭터컨트롤러는 충돌감지만 되고 물리가 적용안된다
        //따라서 충돌감지를 하기 위해서는 반드시
        //
        cc.Move(dir * speed * Time.deltaTime);

    }
}
