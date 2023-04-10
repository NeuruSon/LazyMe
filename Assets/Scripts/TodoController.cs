using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TodoController : MonoBehaviour
{
    public GameObject eatBtn, healthBtn, workBtn, acBtn; //ray로 활성화 될 btn. +acBtn. 
    public GameObject eatSlider, healthSlider, workSlider; //증가할 slider.
    public bool eatFin, healthFin, workFin = false; //일 다 했나?
    public bool isEating, isExercising, isWorking = false; //시작합시다.
    bool doingTodo, acSwitch = false;
    public TextMeshProUGUI healthText, workText, acText;
    GameObject meCon;

    AudioSource curSfx;
    public AudioClip eSfx, hSfx, wSfx, acSfx;

    public char rayCode = 'n'; //interact한 obj 구별용. none. 

    void Start()
    {
        meCon = GameObject.Find("Me");
        curSfx = GetComponent<AudioSource>();
    }

    void Update()
    {
        //정보를 받아서 ray가 가리키고 있는 obj의 활성 버튼만 띄울 것.
        if (doingTodo != true)
        {
            switch (rayCode)
            {
                case 'e':
                    if (eatFin != true && meCon.GetComponent<MeController>().isScanning == true)
                    {
                        Debug.Log("Eat!");
                        OffAllBtn();
                        //eat btn.
                        eatBtn.SetActive(true);
                    }
                    else if (eatFin == true && meCon.GetComponent<MeController>().isScanning == true)
                        Debug.Log("Already finished; eat");
                    break;
                case 'h':
                    if (healthFin != true && meCon.GetComponent<MeController>().isScanning == true)
                    {
                        Debug.Log("Health!");
                        OffAllBtn();
                        //health btn.
                        healthBtn.SetActive(true);
                    }
                    else if (healthFin == true && meCon.GetComponent<MeController>().isScanning == true)
                        Debug.Log("Already finished; exercise");
                    break;
                case 'w':
                    if (workFin != true && meCon.GetComponent<MeController>().isScanning == true)
                    {
                        Debug.Log("Work!");
                        OffAllBtn();
                        //work btn.
                        workBtn.SetActive(true);
                    }
                    else if (workFin == true && meCon.GetComponent<MeController>().isScanning == true)
                        Debug.Log("Already finished; work");
                    break;
                case 'n':
                    OffAllBtn();
                    break;
            }
        }
        if (isEating == true)
        {
            doingTodo = true;
            Eating();
        }
        if (isExercising == true)
        {

            doingTodo = true;
            Exercising();
        }
        if (isWorking == true)
        {
            doingTodo = true;
            Working();
        }

        if (eatFin == true && healthFin == true && workFin == true)
        {
            meCon.GetComponent<MeController>().isScanning = false;
            meCon.GetComponent<MeController>().allFinished = true;
            Debug.Log("All Finished!!!");
            acText.text = "오늘의 할일 완료!\n이제 놀아도 괜찮다구!!";
            if (acSwitch == false)
            {
                acSwitch = true;
                PlaySfx('a'); //올클이라구~~~~
            }
            //버튼 액티브하고, 버튼 누르면 다시 스토리 씬으로.
            //스토리 씬 변경은 그냥 버튼에 종속시키기.
            acBtn.SetActive(true);
        }
    }

    //Btn이 눌렸을 경우 실행될 method.
    public void StartEating()
    {
        isEating = true;
    }

    public void StartExercising()
    {
        healthText.text = "리오르를 움직여서\n걸음수를 채워주세요!";
        GameObject.Find("Door").tag = "ExerciseObj";
        meCon.GetComponent<MeController>().ExerciseSwitch();
        isExercising = true;
    }

    public void StartWorking()
    {
        workText.text = "화면을 마구 터치해서\n과제를 끝내주세요!";
        isWorking = true;
    }


    //업데이트될 method.
    void Eating()
    {
        Debug.Log("Eating...");
        meCon.GetComponent<MeController>().isScanning = false;
        eatBtn.SetActive(false);
        meCon.GetComponent<MeController>().meAnim.SetBool("Waguwagu", true);
        if (eatSlider.GetComponent<Slider>().value < 30)
            eatSlider.GetComponent<Slider>().value += Time.deltaTime * 3f;
        else if (eatSlider.GetComponent<Slider>().value >= 30 && eatFin != true)
        {
            eatSlider.GetComponent<Slider>().value = 30;
            //끝
            Debug.Log("Eat Finished!");
            meCon.GetComponent<MeController>().meAnim.SetBool("Waguwagu", false);
            isEating = false;
            eatFin = true;
            PlaySfx('e'); //다 먹었으니까 효과음. 
            doingTodo = false;
            rayCode = 'n';
            GameObject.Find("Bap").tag = "DoneObj";
            meCon.GetComponent<MeController>().isScanning = true;
            meCon.GetComponent<MeController>().spd = 1f;
        }
    }

    void Exercising()
    {
        Debug.Log("Exercising...");
        meCon.GetComponent<MeController>().isScanning = false;
        healthBtn.SetActive(false);
        if (healthSlider.GetComponent<Slider>().value < 30)
            healthSlider.GetComponent<Slider>().value = meCon.GetComponent<MeController>().travelled;
        else if (healthSlider.GetComponent<Slider>().value >= 30 && healthFin != true)
        {
            healthSlider.GetComponent<Slider>().value = 30;
            //끝
            Debug.Log("Exercise Finished!");
            healthText.text = " ";
            meCon.GetComponent<MeController>().exerciseBtn = false;
            meCon.GetComponent<MeController>().meAnim.SetBool("Waaang", false);
            meCon.GetComponent<MeController>().pokeball.SetActive(false);
            isExercising = false;
            healthFin = true;
            PlaySfx('h'); //운동 끝났으니까 효과음. 
            doingTodo = false;
            rayCode = 'n';
            GameObject.Find("Door").tag = "DoneObj";
            meCon.GetComponent<MeController>().isScanning = true;
            meCon.GetComponent<MeController>().spd = 1f;
        }
    }

    void Working()
    {
        Debug.Log("Working...");
        meCon.GetComponent<MeController>().isScanning = false; 
        workBtn.SetActive(false);
        meCon.GetComponent<MeController>().meAnim.SetBool("UdadaIdle", true);
        if (workSlider.GetComponent<Slider>().value < 30)
            DestroyWork();
        else if (workSlider.GetComponent<Slider>().value >= 30)
        {
            workSlider.GetComponent<Slider>().value = 30;
            //끝
            Debug.Log("Work Finished!");
            workText.text = " ";
            meCon.GetComponent<MeController>().meAnim.SetBool("UdadaIdle", false);
            isWorking = false;
            workFin = true;
            doingTodo = false;
            rayCode = 'n';
            GameObject.Find("Laptop").tag = "DoneObj";
            meCon.GetComponent<MeController>().isScanning = true;
            meCon.GetComponent<MeController>().spd = 1f;
        }
    }

    void DestroyWork()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began) //터치 시작만을 기준점으로 잡아 홀드 방지. 
            {
                meCon.GetComponent<MeController>().meAnim.SetTrigger("Uda");
                PlaySfx('w'); //타닥.. 타닥.. 탁.. 
                workSlider.GetComponent<Slider>().value++;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            meCon.GetComponent<MeController>().meAnim.SetTrigger("Uda");
            PlaySfx('w'); //타닥.. 타닥.. 탁.. 
            workSlider.GetComponent<Slider>().value++;
        }
    }

    void OffAllBtn()
    {
        eatBtn.SetActive(false);
        healthBtn.SetActive(false);
        workBtn.SetActive(false);
    }

    void PlaySfx(char c)
    {
        switch(c)
        {
            case 'e':
                curSfx.clip = eSfx;
                curSfx.Play();
                break;
            case 'h':
                curSfx.clip = hSfx;
                curSfx.Play();
                break;
            case 'w':
                curSfx.clip = wSfx;
                curSfx.Play();
                break;
            case 'a':
                curSfx.clip = acSfx;
                curSfx.Play();
                break;
        }
    }
}
