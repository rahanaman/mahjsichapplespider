using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager: MonoBehaviour 
{
    private static GameEventManager _instance;
    public static GameEventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                return null;
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public delegate void VoidEvent();
    public delegate void IntEvent(int num);
    public delegate void Int2Event(int num1, int num2);
    public delegate void NumberControllerEvent(NumberController number);

    public VoidEvent NewGame;
    public IntEvent CreateNum;
    public Int2Event SetXY;
    public NumberControllerEvent ClickNumber;

    public void CallOnCreateNum(int num)
    {
        CreateNum?.Invoke(num);
    }

    public void CallOnSetTable(int num1, int num2)
    {
        SetXY?.Invoke(num1, num2);
    }

    public void CallOnNewGame()
    {
        NewGame?.Invoke();
    }

    public void CallOnClickNumber(NumberController number)
    {
        ClickNumber?.Invoke(number);
    }
}
