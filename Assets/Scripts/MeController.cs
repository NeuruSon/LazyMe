using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeController : MonoBehaviour
{
    //Mummy(Me)에 적용시킬 script.

    public GameObject screen, pokeball;
    //GameObject bapObj, workObj, healthObj;
    GameObject dstTgt; //destination target. 인식해서 찾을 타겟을 담아둘 곳.
    GameObject tdCon;
    public Animator meAnim, eAnim, wAnim, hAnim;

    public float spd = 1f; //이동 속도.
    float range = 10f; //캐릭터가 이동하는 범위 체크.
    float rayLength = 0.3f; //ray 길이.

    public bool isScanning = true;
    public bool allFinished = false;

    public bool exerciseBtn = false;
    public float travelled = 0f;
    public Vector3 lastPoint;


    void Start()
    {
        tdCon = GameObject.Find("tdController");
        //Idle Anim.
        meAnim = GetComponent<Animator>();
        //eAnim = bapObj.GetComponent<Animator>(); 
        //wAnim = workObj.GetComponent<Animator>(); 
        //hAnim = healthObj.GetComponent<Animator>(); 
    }

    void Update()
    {
        //수시로 업데이트하며 타겟이 존재하는지 아닌지를 찾아야 함.
        if (isScanning == true || exerciseBtn == true)
        {
            ScanAround();
        }
        else { }

        if (allFinished == true)
        {
            meAnim.SetBool("allFinished", true);
            transform.LookAt(screen.transform.position);
        }
    }

    void ScanAround() //타겟이 주변에 존재하는지를 스캔함.
    {
        //범용성을 포기하고 작성할까? ㄴㄴ 포기하는 순간 정확도는 올라도 화면 바깥 벗어나서 지들끼리 노는 수가 있음 AR은 이제 몸이 따라가기 힘들어서 고려해야 함.. 어게이~

        //일정 범위 내에 / workObj 태그를 단 오브젝트가 존재하나?
        //아냐 그것보다는... 개수가 적은 걸 이용해서 모든 active 상태의 workObj를 찾는다. 그것이 범위 내에 있나? 시간이 여유롭지 않으니 가장 간단하고 지엽적으로 가자.
        dstTgt = FindClosestObj(); //최단거리에 위치한 obj. 

        //존재한다면?
        if (dstTgt)
        {
            //바라봄. 
            transform.LookAt(dstTgt.transform.position);
            spd = 1f;

            //ray cast해서 obj과 일정 거리 이하로 가까워지면 btn. 생성 신호 T0d0Controller에 보내기.
            Vector3 forward = transform.TransformDirection(Vector3.forward) * rayLength;
            Vector3 forwardR = transform.TransformDirection(Vector3.forward) * range;

            RaycastHit rangeRay, interactRay; 
            //Debug.DrawRay(transform.position, forward, Color.red); //debug용.

            if (Physics.Raycast(transform.position, forwardR, out rangeRay, range)) //range 안인지 체크함. 
            { 
                //range 안이면 이동함. 너무 멀리 있으면 안 가려고... 
                transform.Translate(Vector3.forward * spd * Time.deltaTime);
                //이동하는 동안에도 계속 스캔을 반복해 더 가까운 마커가 있다면 그쪽으로 방향을 바꿈. ㅇㅋㅇㅋ 작동확인~

                //운동중이면 얘기가 다르제 
                if (exerciseBtn == true)
                {
                    meAnim.SetBool("Waaang", true);
                    spd = 1f;
                    travelled += Vector3.Distance(transform.position, lastPoint);
                    lastPoint = transform.position;
                }
                else
                    meAnim.SetBool("isWalking", true);
            }
            else
            {
                meAnim.SetBool("isWalking", false);
            }

            if (Physics.Raycast(transform.position, forward, out interactRay, rayLength) && interactRay.collider.gameObject.tag == "WorkObj")
            {
                Debug.Log("hit");
                spd = 0f;
                meAnim.SetBool("isWalking", false);

                //T0d0Controller에 신호 발송.
                switch (interactRay.collider.gameObject.name)
                {
                    case "Bap":
                        tdCon.GetComponent<TodoController>().rayCode = 'e';
                        break;
                    case "Door":
                        tdCon.GetComponent<TodoController>().rayCode = 'h';
                        break;
                    case "Laptop":
                        tdCon.GetComponent<TodoController>().rayCode = 'w';
                        break;
                    //default:
                    //    tdCon.GetComponent<TodoController>().rayCode = 'n';
                    //    break;
                }
            }
            //else if((meAnim.GetBool("isWalking") == true && !(Physics.Raycast(transform.position, forward, out interactRay, rayLength)))
            //    || ) //ray interact가 없고 걷고 있을 때.
            //{

            //}
            //else //ray interact가 없고 걷고 있을 때. 
            //{
            //    tdCon.GetComponent<TodoController>().rayCode = 'n';
            //}

            //이걸... trigger 말고 ray로 쉽게 할 수 있는 방식이 지금 막판에 한번 움찔 하는 상황에서는 따로 없는 것 같은데
            //일단 패스하고 다음 과제에 더 나은 방법을 생각해보자

        }
        else
        {
            dstTgt = FindClosestObj();
        }
    }

    public GameObject FindClosestObj() //Unity3d API 참고. '최단거리 obj' 반환. 
    {
        GameObject[] todoList; //active 상태의 WorkObj 태그 붙은 모든 옵젝을 다 넣어둘 곳.
        if (exerciseBtn == true)
        {
            todoList = GameObject.FindGameObjectsWithTag("ExerciseObj"); //찾는다. 넣는다.
        }
        else
        {
            todoList = GameObject.FindGameObjectsWithTag("WorkObj"); //찾는다. 넣는다.
        }

        if (todoList == null)
        {
            return null;
        }
        GameObject closest = null; //처음에는 없으니 비워둠. 
        float distance = Mathf.Infinity; //무한으로 초기화. 
        Vector3 position = transform.position; //기준점이 될 위치. 나의 위치. 
        foreach (GameObject workObj in todoList) //전수검사. 
        {
            Vector3 diff = workObj.transform.position - position; //거리 차. 
            float curDist = diff.sqrMagnitude; //피타고라스 이용해 최단거리를 현재거리 변수에 저장. 
            if (curDist < distance) //만약 distance보다 방금 계산한 '옵젝과의 거리'가 더 짧으면? 
            {
                closest = workObj; //더 가까운 쪽을 '최단'에 저장.
                distance = curDist; //가장 가까웠던 최근 거리를 '거리'에 저장. 계속 좌우비교. 
            }
        }
        return closest; //검사 끝나면 최종적으로 '최단'을 반환함. 
    }

    //public void ExerciseAnim()
    //{
    //    Animator hAnim = GameObject.Find("Door").GetComponent<Animator>();
    //    float hPosX = GameObject.Find("Door").transform.position.x;
    //    float hPosZ = GameObject.Find("Door").transform.position.z;
    //    transform.position = new Vector3(hPosX + 0.253f, 0, hPosZ + 0.33f);
    //    transform.LookAt(dstTgt.transform.position);
    //    hAnim.SetTrigger("DoorOpen");
    //    meAnim.SetTrigger("DoorOpen");
    //    //meAnim.SetTrigger("Uhehe");
    //    Invoke("ExerciseSwitch", 3);
    //}

    private void OnBecameVisible()
    {
        //화면 안으로 들어오면.
        //아니 근데 이거는... 이렇게 하면 저어어어 멀리 있어도 화면에만 있으면 그거 따라 쫄래쫄래 갈 거 아님? 어케 따라감? 지난번에 머리 부딪혔잖음;
        //앗..아 그렇내여
        //잘라내도 화면 밖에 있는 걸 disable하되, 화면 안에 있는 걸 무조건 active하는 건 유저 자체가 카메라인 AR에서는 위험한 방식일 것 같음. 
    }

    public void ExerciseSwitch()
    {
        pokeball.SetActive(true);
        lastPoint = transform.position;
        exerciseBtn = true;
        //tdCon.GetComponent<TodoController>().isExercising = true;
    }
}
