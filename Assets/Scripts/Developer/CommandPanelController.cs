using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using Unity.VisualScripting;

public class CommandPanelController : MonoBehaviour
{
    [SerializeField] Button _commandPanelButton;
    [SerializeField] InputField _commandInput;
    [SerializeField] Button _submitButton;


    private Transform _transform;

    private string _commandInputString;

    private bool _isPanelOn = false;
    
    private Vector3 _OnPos = new Vector3(235, -750, 0);
    private Vector3 _OffPos = new Vector3(235, -960, 0);


    private void Awake()
    {
        

        
        
        
    }

    private void Start()
    {
        _commandInputString = _commandInput.text;
        _transform = GetComponent<Transform>();
        _transform.localPosition = _OffPos;
        _commandPanelButton.onClick.AddListener(OnClickCommandPanelButton);
        _commandInput.onValueChanged.AddListener(ValueChanged);
        _submitButton.onClick.AddListener(InvokeCommand);

        
    }
    void Update()
    {
        //Debug.Log(_commandInputString);
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            OnClickCommandPanelButton();
           
        }
        
        if (_isPanelOn && _commandInputString.Length>0 && Input.GetKeyDown(KeyCode.Return))
        {
            InvokeCommand();
        }
        
    }

    

    void ValueChanged(string text) // string 매개변수는 기본으로 들어가는 매개변수이다
    {
        _commandInputString = _commandInput.text;
        //Debug.Log(text + " - 글자 입력 중");
    }
    private void InvokeCommand()
    {
        CommandPanelManager.Instance.InvokeCommand(_commandInputString);
        
        _commandInput.text = "";
        _commandInputString = _commandInput.text;
        _transform.localPosition = _OffPos;
        _isPanelOn = false;
    }
    

    private void OnClickCommandPanelButton()
    {
        if(!_isPanelOn) //세팅창 열기
        {
            _transform.localPosition = _OnPos;
            _commandInput.ActivateInputField();
            _isPanelOn = true;
        }
        else //세팅창 닫기
        {
            _transform.localPosition = _OffPos;
            _commandInput.ActivateInputField();
            if(_commandInput.IsActive()) _commandInput.Select();
            _isPanelOn = false;
        }
    }

    private void IsCommand(string str)
    {
        string PARSE = @"\s\s*";
       
        var mc = Regex.Split(str, PARSE);
        
        foreach (var match in mc)
        { 
        }
    }
}
public class CommandPanelManager
{
    private const string PARSE = @"\s\s*";
    private Dictionary<string, Func<string[],bool>> CommandID = new Dictionary<string, Func<string[], bool>>
    {
        {"/set", SetCommand},
        {"/table", TableCommand },
        {"/new", NewCommand },
        {"/plus", PlusCommand }
    };

    private static CommandPanelManager _instance;
    public static CommandPanelManager Instance
    {
        get
        {
            if( _instance == null)
            {
                _instance = new CommandPanelManager();
                
            }
            return _instance;
        }
    }

    public void InvokeCommand(string str)
    {
       
        str = str.Trim();
        if (IsCommand(str))
        {
            CheckCommand(str);
        }
        return;
        
    }

    public bool IsCommand(string str)
    {
        if (str[0] == '/')
        {
            return true;
        }
        return false;
    }
    private bool CheckCommand(string str)
    {
        string[] fields = Regex.Split(str, PARSE);
        string cmd = fields[0];
        if (CommandID.ContainsKey(cmd))
        {
            return CommandID[cmd](fields);
        }
        else
        {
            return false;
        }
    }
    private static bool SetCommand(string[] fields) //오류 문구 알아서 넣기
    {
        
        List<int> nums = new List<int>();
        for (int i = 1; i < fields.Length; i++)
        {
            int num;
            if (int.TryParse(fields[i], out num))
            {
                if(0>num||num>9) return false;
                nums.Add(num);
            }
            else return false;
        }
        foreach(int num in nums)
        {
            GameEventManager.Instance.CallOnCreateNum(num);
        }
        return true;
    }

    private static bool TableCommand(string[] fields)
    {
        List<int> nums = new List<int>();
        if (fields.Length != 3) return false;
        for(int i = 1; i < 3; ++i)
        {
            int num;
            if (int.TryParse(fields[i], out num))
            {
                if (0 > num || num > 9) return false;
                nums.Add(num);
            }
        }
        GameEventManager.Instance.CallOnSetTable(nums[0], nums[1]);
        //Debug.Log("성공");
        return true;
    }

    private static bool NewCommand(string[] fields)
    {
        GameEventManager.Instance.CallOnNewGame();
        return true;
    }

    private static bool PlusCommand(string[] fields)
    {
        GameEventManager.Instance.CallOnPlusAction();
        return true;
    }
}


