using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryController : MonoBehaviour
{
    public GameObject s1, s2, s3, s4, sCon;
    public int scriptCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        s1.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (scriptCount == 2)
        {
            s2.SetActive(true);
        }
        else if (scriptCount == 4)
        {
            s3.SetActive(true);
        }
        else if (scriptCount == 6)
        {
            s4.SetActive(true);
        }
        else if (scriptCount > 7)
        {
            //터치해서 다음 씬으로.
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began) //터치 시작만을 기준점으로 잡아 홀드 방지. 
                {
                    sCon.GetComponent<SceneController>().ToMainScene();
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                sCon.GetComponent<SceneController>().ToMainScene();
            }
        }
    }
}
