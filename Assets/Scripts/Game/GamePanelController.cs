using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanelController : MonoBehaviour
{

    [SerializeField] private NumberController _number;
    [SerializeField] private Transform _canvas;
    private int _xSize = 9;
    private int _ySize = 10;
    //private NumberController[][] _numberTable; //0~_xSize-1, 0~_ySize-1
    private int _gamePanelX = 900;
    private int _gamePanelY = 1500;

    private int _endX;
    private int _endY;

    private NumberController _onClickNumber;

    private void Awake()
    {

    }

    private void OnDestroy()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.CreateNum -= SetNewNumber;
            GameEventManager.Instance.SetXY -= SetXY;
            GameEventManager.Instance.NewGame -= NewGame;
            GameEventManager.Instance.ClickNumber -= OnClickNumber;
        }

    }
    private void Start()
    {
        GameEventManager.Instance.CreateNum += SetNewNumber;
        GameEventManager.Instance.SetXY += SetXY;
        GameEventManager.Instance.NewGame += NewGame;
        GameEventManager.Instance.ClickNumber += OnClickNumber;
        GamePanelManager.Instance.Init();
        GamePanelManager.Instance.SetXY(_xSize, _ySize);
        NewGame();

        /*
        var num = NewNumberButton(5);
        SetNumber(num, 1, 0);
        var num2 = NewNumberButton(5);
        SetNumber(num2, 0, 0);
        */
    }

    private void Update()
    {
        //Debug.Log("xPos : "+_endX.ToString() + "yPos : " + _endY.ToString());
    }


    /// <summary>
    /// 계산으로 가져오는 값
    /// </summary>
    private Vector3 _scale
    {
        get
        {
            return new Vector3(4.5f / _xSize, 4.5f / _xSize, 4.5f / _xSize);
        }
    } // 가로 개수에 따른 스케일

    private float _buttonX
    {
        get
        {
            return 900 / _xSize;
        }
    } //버튼의 가로 길이
    private float _buttonY
    {
        get
        {
            return 1.25f * _buttonX;
        }
    } //버튼의 세로길이
    private float _xPos(int x)
    {
        return -(_gamePanelX / 2.0f) + (0.5f * _buttonX * (2 * x + 1));
    }

    private float _yPos(int y)
    {
        return (_gamePanelY / 2.0f) - (0.5f * _buttonY) * (2 * y + 1);
    }


    private void NewGame()
    {
        for (int i = 0; i < GamePanelManager.Instance.NumberTable.Length; ++i)
        {
            for (int j = 0; j < GamePanelManager.Instance.NumberTable[i].Length; ++j)
            {
                if (GamePanelManager.Instance.NumberTable[i][j] != null)
                {
                    Destroy(GamePanelManager.Instance.NumberTable[i][j].gameObject);
                }
            }
        }
        _endX = -1;
        _endY = 0;
        _onClickNumber = null;
        GamePanelManager.Instance.Init();

    }


    private void SetNewNumber(int num)
    {
        if (num == 0)
        {
            _endX++;
            if (_endX >= _xSize)
            {
                _endX = 0;
                _endY++;
            }
            if (IsGameOver())
            {
                
            }
            return;
        }
        var number = NewNumberButton(num);
        SetNumber(number);
        _endX = number.X;
        _endY = number.Y;
        return;
    }

    private void SetNumber(NumberController number) // 끝에 새로운 숫자를 만드는 경우 // 게임 오버 리턴
    {

        _endX++;
        if (_endX >= _xSize)
        {
            _endX = 0;
            _endY++;
        }

        if (IsGameOver())
        {
            //game over code
            return;
        }
        SetNumber(number, _endX, _endY);

    }

    private bool IsGameOver() { return (_endY >= _ySize); }

    private bool IsAllClear() { return (_endX == -1 && _endY == -1); }
    private void SetNumber(NumberController number, int x, int y)
    {
        //_numberTable[x][y] = number;
        GamePanelManager.Instance.SetButton(number, x, y);
        number.SetPos(x, y);
        number.transform.localPosition = new Vector3(_xPos(x), _yPos(y), 0);
    }

    private NumberController NewNumberButton(int number)
    {
        var num = Instantiate(_number, _canvas);
        num.Init(number);
        num.transform.localScale = _scale;
        return num;
    }

    private void SearchEnd(int x = -1, int y = -1) // 기준 없으면 끝에서부터 서치
    {
        if (x<0 || y < 0)
        {
            x = _xSize-1; y = _ySize-1;
        }
        while (true)
        {
            if (GamePanelManager.Instance.NumberTable[x][y] != null)
            {
                _endX = x;
                _endY = y;
                return;
            }
            x--;
            if (x < 0)
            {
                y--;
                x = _xSize-1;
            }
            if(y < 0)
            {
                _endX = -1;
                _endY = -1;
                return;
            }
        }
        
    }

    private void SetEnd(int x, int y)
    {
        _endX = x;
        _endY = y;
    }

    private void SetXY(int x, int y)
    {
        _xSize = x;
        _ySize = y;
        GamePanelManager.Instance.SetXY(x, y);
    }

    private void OnClickNumber(NumberController number)
    {
        if(_onClickNumber == number)
        {
            _onClickNumber = null;
            
        }
        else if(_onClickNumber == null)
        {
            _onClickNumber = number;
            Debug.Log(_onClickNumber);

        }
        else
        {
            var number2 = _onClickNumber;
            number2.SetEfx(false);
            number.SetEfx(false);
            ResetClickNumber();
            //체크 페어 함수
            if (IsPair(number, number2))
            {
                DeleteNumber(number);
                DeleteNumber(number2);
                if (IsAllClear())
                {
                    // All Clear Code 들어갈 곳
                }
                if (GamePanelManager.Instance.IsLineClear(_endX, _endY))
                {
                    UpdateTable();
                }
                
            }
            
            
            

        }
        //Test(number);
    }

    private void Test(NumberController number)
    {
        DeleteNumber(number);
    }

    private void DeleteNumber(NumberController number)
    {
        GamePanelManager.Instance.DeleteNumber(number);

        if (_endX == number.X && _endY == number.Y)
        {
            SearchEnd(number.X, number.Y);
        }
        
        Destroy(number.gameObject);
    }

    private void ResetClickNumber()
    {
        _onClickNumber = null;
    }

    private bool IsPair(NumberController num1, NumberController num2)
    {
        if(GamePanelManager.Instance.IsPair(num1, num2))
        {
            return true;  
        }
        else
        {
            return false;
        }
    }

    private void UpdateTable()
    {
        for(int i = 0;i<_xSize;i++)
        {
            for(int j = 0;j<_ySize;j++)
            {
                if (GamePanelManager.Instance.NumberTable[i][j] != null)
                {
                    SetNumber(GamePanelManager.Instance.NumberTable[i][j], i, j);
                }
            }
        }
    }

}


public class GamePanelManager
{
    private static GamePanelManager _instance;
    public static GamePanelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GamePanelManager();
            }
            return _instance;
        }
    }

    public NumberController[][] NumberTable
    {
        get; private set;
    }
    private int _xSize = 9;
    private int _ySize = 10;
    public GamePanelManager()
    {
        Init();
    }

    public void Init()
    {
        SetTable();
    }

    public void SetXY(int x, int y)
    {
        _xSize = x;
        _ySize = y;
    }

    public void SetButton(NumberController number, int x, int y)
    {
        NumberTable[x][y] = number;
    }

    public void DeleteNumber(NumberController number)
    {
        NumberTable[number.X][number.Y] = null;
    }

    private void SetTable()
    {
        NumberTable = new NumberController[_xSize][];
        for (int i = 0; i < _xSize; i++)
        {
            NumberTable[i] = new NumberController[_ySize+1];
        }
    }

    public bool IsPair(NumberController num1, NumberController num2)
    {
        if (!IsPairPos(num1, num2)) return false;
        if (IsPairNumber(num1, num2)) return true;
        return false;

    }

    private bool IsPairPos(NumberController num1, NumberController num2)
    {
        if (num1.X == num2.X)
        {
            int index = num1.Y < num2.Y ? num1.Y + 1 : num2.Y + 1;
            int end = num1.Y > num2.Y ? num1.Y : num2.Y;
            while (index < end)
            {
                if (NumberTable[num1.X][index] != null) return false;
                index++;
            }
            return true;
        }
        else if (num1.Y == num2.Y)
        {
            int index = num1.X < num2.X ? num1.X + 1 : num2.X + 1;
            int end = num1.X > num2.X ? num1.X : num2.X;
            while (index < end)
            {
                if (NumberTable[index][num1.Y] != null) return false;
                index++;
            }
            return true;
        }
        int xDis = num1.X - num2.X;
        int yDis = num1.Y - num2.Y;
        if (Math.Abs(xDis) == Math.Abs(yDis))
        {
            int delX = xDis > 0 ? -1 : 1;
            int delY = yDis > 0 ? -1 : 1;

            int indexX = num1.X + delX;
            int indexY = num1.Y + delY;
            while (indexX != num2.X)
            {
                if (NumberTable[indexX][indexY] != null) return false;
                indexX += delX;
                indexY += delY;
            }
            return true;
        }
        return false;
    }

    private bool IsPairNumber(NumberController num1, NumberController num2)
    {
        if (num1.Num == num2.Num || num1.Num + num2.Num == 10) return true;
        else return false;
    }

    public bool IsLineClear(int x, int y)
    {
        var change = false;
        for(int i = y; i >= 0; --i)
        {
            var ch = true;
            for(int j = 0; j < _xSize; ++j)
            {
                if (NumberTable[j][i] != null)
                {
                    ch = false; break;
                }
            }
            if (ch)
            {
                change = true;
                for (int j = 0; j < _xSize; ++j)
                {
                    NumberTable[j][i] = NumberTable[j][i + 1];
                    NumberTable[j][i + 1] = null;
                    if(NumberTable[j][i]!= null) NumberTable[j][i].SetPos(j, i);
                }
            }
        }

        return change;
    }

}
