using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    [SerializeField] NumberController _numberPrefab;
    [SerializeField] Transform _gamePanel;


    private void Start()
    {
        
    }

    private void GetNumber(int num)
    {
        NumberController number = Instantiate(_numberPrefab,_gamePanel);
        number.Init(num);
    }



}
