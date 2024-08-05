using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinRound : MonoBehaviour
{
    [SerializeField] private float waitTime;
    [SerializeField] private Text timerText;
    private float counter;
    public bool canCount;
    public bool doneCounting;

    void Start()
    {
        counter = waitTime;
        doneCounting = false;
        canCount = true;
    }
    public void resetTimer()
    {
        counter = waitTime;
        doneCounting = false;
        canCount = false;
    }
    void Update()
    {
        //Debug.Log("update called" + counter + canCount);
        if (canCount && counter >= 0)
        {
            counter -= Time.deltaTime;
            //Debug.Log("counting down");
            timerText.text = counter.ToString();
        }
        else if (counter <= 0)
        {
            doneCounting = true;
        }
    }

    public void startTimer()
    {
        counter = waitTime;
        canCount = true;
        doneCounting = false;
        //Debug.Log("start timer called" + counter + canCount);
    }
}
