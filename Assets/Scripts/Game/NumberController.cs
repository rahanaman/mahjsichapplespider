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

    private bool _isEfx = false;


    public void Init(int number)
    {
        _isEfx = false;
        _button.onClick.AddListener(OnClick);
        _number.text = number.ToString();
        _numberShadow.text = number.ToString();
    }

    private void OnClick()
    {
        if(_isEfx)
        {
            _isEfx = false;
            _efx.SetActive(false);
        }
        else
        {
            _isEfx = true;
            _efx.SetActive(true);
        }

    }



}
