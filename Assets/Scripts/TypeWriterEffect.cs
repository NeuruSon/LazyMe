﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class TypeWriterEffect : MonoBehaviour
{
    public TextMeshProUGUI touchText;
    GameObject canvas;

    //변경할 변수
	public float delay;
    public float Skip_delay;
    public int cnt;

    //타이핑효과 변수
    public string[] fulltext;
    public int dialog_cnt;
    string currentText;

    //타이핑확인 변수
    public bool text_exit;
    public bool text_full;
    public bool text_cut;


    //시작과 동시에 타이핑시작
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        Get_Typing(dialog_cnt,fulltext);
    }


    //모든 텍스트 호출완료시 탈출
    void Update()
    {
        if(text_exit==true)
        {
            gameObject.SetActive(false);
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) //터치 시작만을 기준점으로 잡아 홀드 방지. 
            {
                End_Typing();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            End_Typing();
        }
    }

    //다음버튼함수
    public void End_Typing()
    {
        //다음 텍스트 호출
        if (text_full == true)
        {
            canvas.GetComponent<StoryController>().scriptCount++;
            cnt++;
            text_full = false;
            text_cut = false;
            StartCoroutine(ShowText(fulltext));
        }
        //텍스트 타이핑 생략
        else
        {
            text_cut = true;
        }
    }

    //텍스트 시작호출
    public void Get_Typing(int _dialog_cnt, string[] _fullText)
    {
        //재사용을 위한 변수초기화
        text_exit = false;
        text_full = false;
        text_cut = false;
        cnt = 0;

        //변수 불러오기
        dialog_cnt = _dialog_cnt;
        fulltext = new string[dialog_cnt];
        fulltext = _fullText;

        //타이핑 코루틴시작
        StartCoroutine(ShowText(fulltext));
    }

    IEnumerator ShowText(string[] _fullText)
    {
        //모든텍스트 종료
        if (cnt >= dialog_cnt)
        {
            text_exit = true;
            StopCoroutine("showText");
        }
        else
        {

            //기존문구clear
            currentText = "";
            touchText.text = " ";
            //타이핑 시작
            for (int i = 0; i < _fullText[cnt].Length; i++)
            {
                //타이핑중도탈출
                if (text_cut == true)
                {
                    break;
                }
                //단어하나씩출력
                currentText = _fullText[cnt].Substring(0, i + 1);
                this.GetComponent<TextMeshProUGUI>().text = currentText;
                yield return new WaitForSeconds(delay);
            }
            //탈출시 모든 문자출력
            Debug.Log("Typing 종료");
            this.GetComponent<TextMeshProUGUI>().text = _fullText[cnt];
            yield return new WaitForSeconds(Skip_delay);

            //스킵_지연후 종료
            touchText.text = "터치해서 다음으로";
            Debug.Log("Enter 대기");
            text_full = true;
        }
    }
}
