using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberController : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _efx;
    [SerializeField] private Text _number;
    [SerializeField] private Text _numberShadow;

    public int Num { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }

    private bool _isEfx = false;

    private void Start()
    {
       //Init(9);
    }

    public void Init(int number)
    {
        Num = number;
        _isEfx = false;
        _button.onClick.AddListener(OnClick);
        _number.text = number.ToString();
        _numberShadow.text = number.ToString();
    }

    public void SetPos(int x, int y)
    {
        X = x;
        Y = y;
    }

    private void OnClick()
    {
        if(_isEfx)
        {
            SetEfx(false);
        }
        else
        {
            SetEfx(true);
        }

        GameEventManager.Instance.CallOnClickNumber(this);
        

    }

    public void SetEfx(bool b)
    {
        _isEfx = b;
        _efx.SetActive(b);
    }



}
