///---------------------------------------------------------------------------|
/// Редактор кода.
///     - редактируется только нижняя строка.
///---------------------------------------------------------------------------|
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

namespace CP2026
{
    public class CustomKeyboardManager : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private TextMeshProUGUI targetText;

        [Header("Control")]
        [SerializeField] private FirstPersonController firstPersonController;
        [SerializeField] private PythonExecutor        pythonExecutor;
        [SerializeField] private Monitor               monitor;
    
        [Header("Settings")]
        [SerializeField] private bool enableKeyboardInput = true;
    /// [SerializeField] private bool enableShiftModifier = true;
        [SerializeField] private int  maxTextLength = 1000;
    
        private bool shiftPressed = false;
        private bool     capsLock = false;
    
        // Словарь для преобразования виртуальных кодов в символы с учётом Shift/Caps
        private static System.Collections.Generic.Dictionary<Key, char> baseKeyMap;
        private static System.Collections.Generic.Dictionary<Key, char> shiftKeyMap;

        Keyboard keyboard;

        ///------------------|
        /// Буфер экрана.    |
        ///------------------:
        BuffersScreen _buffer;

        private void Awake()
        {
            keyboard = Keyboard.current;
            if (keyboard == null)
            {   Debug.LogWarning(keyboard == null);
            }

            if (baseKeyMap == null)
            {   InitializeKeyMaps();
            }
        }

        private void Start()
        {
            // Находим компонент, если не назначен вручную
            if (targetText == null)
                targetText = GetComponent<TextMeshProUGUI>();

            _buffer = new BuffersScreen(targetText);
        }

        private void InitializeKeyMaps()
        {
            baseKeyMap  = new System.Collections.Generic.Dictionary<Key, char>();
            shiftKeyMap = new System.Collections.Generic.Dictionary<Key, char>();
        
            // Буквы (A-Z)
            for (char c = 'a'; c <= 'z'; c++)
            {
                Key key = (Key)System.Enum.Parse(typeof(Key), c.ToString().ToUpper());
                baseKeyMap[key] = c;
                shiftKeyMap[key] = char.ToUpper(c);
            }
        
            // Цифры и символы на них
            baseKeyMap[Key.Digit1] = '1'; shiftKeyMap[Key.Digit1] = '!';
            baseKeyMap[Key.Digit2] = '2'; shiftKeyMap[Key.Digit2] = '@';
            baseKeyMap[Key.Digit3] = '3'; shiftKeyMap[Key.Digit3] = '#';
            baseKeyMap[Key.Digit4] = '4'; shiftKeyMap[Key.Digit4] = '$';
            baseKeyMap[Key.Digit5] = '5'; shiftKeyMap[Key.Digit5] = '%';
            baseKeyMap[Key.Digit6] = '6'; shiftKeyMap[Key.Digit6] = '^';
            baseKeyMap[Key.Digit7] = '7'; shiftKeyMap[Key.Digit7] = '&';
            baseKeyMap[Key.Digit8] = '8'; shiftKeyMap[Key.Digit8] = '*';
            baseKeyMap[Key.Digit9] = '9'; shiftKeyMap[Key.Digit9] = '(';
            baseKeyMap[Key.Digit0] = '0'; shiftKeyMap[Key.Digit0] = ')';
        
            // Специальные символы
            baseKeyMap[Key.Minus       ] = '-' ; shiftKeyMap[Key.Minus       ] = '_';
            baseKeyMap[Key.Equals      ] = '=' ; shiftKeyMap[Key.Equals      ] = '+';
            baseKeyMap[Key.LeftBracket ] = '[' ; shiftKeyMap[Key.LeftBracket ] = '{';
            baseKeyMap[Key.RightBracket] = ']' ; shiftKeyMap[Key.RightBracket] = '}';
            baseKeyMap[Key.Semicolon   ] = ';' ; shiftKeyMap[Key.Semicolon   ] = ':';
            baseKeyMap[Key.Quote       ] = '\''; shiftKeyMap[Key.Quote       ] = '"';
            baseKeyMap[Key.Comma       ] = ',' ; shiftKeyMap[Key.Comma       ] = '<';
            baseKeyMap[Key.Period      ] = '.' ; shiftKeyMap[Key.Period      ] = '>';
            baseKeyMap[Key.Slash       ] = '/' ; shiftKeyMap[Key.Slash       ] = '?';
            baseKeyMap[Key.Backslash   ] = '\\'; shiftKeyMap[Key.Backslash   ] = '|';
            baseKeyMap[Key.Backquote   ] = '`' ; shiftKeyMap[Key.Backquote   ] = '~';
        
            // Пробел
            baseKeyMap[Key.Space] = ' '; shiftKeyMap[Key.Space] = ' ';
        }

        public void Loop()
        {
            if (!enableKeyboardInput) return;
        
            // Обработка модификаторов
            shiftPressed = keyboard.shiftKey.isPressed || keyboard.rightShiftKey.isPressed;
        
            // Переключение Caps Lock (при нажатии)
            if (keyboard.capsLockKey.wasPressedThisFrame)
            {
                capsLock = !capsLock;
            }
        
            // Обработка специальных клавиш
            if (keyboard.backspaceKey.wasPressedThisFrame)
            {
                DeleteLastCharacter();
            }
        
            if (keyboard.deleteKey.wasPressedThisFrame)
            {
                DeleteLastCharacter();
            }
        
            if (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame)
            {
                AddCharacter("\n"); // Перевод строки

                SendText(_buffer.GetStr());
            }
        
            if (keyboard.tabKey.wasPressedThisFrame)
            {
                AddCharacter("    "); // Табуляция (4 пробела)
            }
        
            // Обработка обычных клавиш
            foreach (var kvp in baseKeyMap)
            {
                if (keyboard[kvp.Key].wasPressedThisFrame)
                {
                    char resultChar = GetCharacter(kvp.Key);
                    AddCharacter(resultChar.ToString());
                    break;
                }
            }
        }
    
        private char GetCharacter(Key key)
        {
            // Получаем символ с учётом Shift и Caps Lock
            bool useShift = shiftPressed;
            bool isLetter = baseKeyMap.ContainsKey(key) && char.IsLetter(baseKeyMap[key]);
        
            if (isLetter)
            {
                // Для букв: Caps Lock инвертирует Shift
                useShift = shiftPressed ^ capsLock;
            }
        
            return useShift ? shiftKeyMap[key] : baseKeyMap[key];
        }
    
        // Вызывается по нажатию на буквенную кнопку (UI)
        public void AddCharacter(string character)
        {
            if (targetText != null && targetText.text.Length < maxTextLength)
            {
                _buffer.AddCharacter(character);
            }
        }
    
        // Кнопка удаления (Backspace)
        public void DeleteLastCharacter()
        {
            if (targetText != null)
            {
                _buffer.DeleteLastCharacter();
            }
        }
    
        // Удаление с позиции курсора (если добавить)
        public void xDeleteAtPosition(int position)
        {
            if (targetText != null && position >= 0 && position < targetText.text.Length)
            {
                //allBuilder.Clear();
                //allBuilder.Append(targetText.text);
                //allBuilder.Remove(position, 1);
                //targetText.text = allBuilder.ToString();
            }
        }
    
        // Вставка в определённую позицию
        public void xInsertAtPosition(string text, int position)
        {
            if (targetText != null && position >= 0 && position <= targetText.text.Length)
            {
                //allBuilder.Clear();
                //allBuilder.Append(targetText.text);
                //allBuilder.Insert(position, text);
            
                //if (allBuilder.Length <= maxTextLength)
                //    targetText.text = allBuilder.ToString();
            }
        }
    
        // Получить текущую раскладку для отображения (опционально)
        public bool IsShiftPressed => shiftPressed;
        public bool IsCapsLock => capsLock;

        public void On()
        {
            firstPersonController.IsKeys = false;

            _buffer.Init();
        }

        public void Off()
        {
            firstPersonController.IsKeys = true;
        }

        void SendText(string strMessage)
        {
            if (strMessage.Length == 0) return;

            //Debug.Log(strMessage);

            if(strMessage == "exit")
            {   _buffer.ClearAll();
                monitor.Exit();
                return;
            }

            if(strMessage == "cls")
            {   _buffer.ClearAll();
                return;
            }

            if(strMessage == "exe")
            {   pythonExecutor.SendText(_buffer.GetNow());
                _buffer.AfterCommand();
                return;
            }

            foreach(string item in Monitor.Vob)
            {
                //Debug.Log($"item: {item} : {item.Length}");

                if(item == strMessage)
                {   pythonExecutor.SendCommand(strMessage);
                }
            }

            _buffer.Str2Now();
        }
    }

    ///-----------------------------------------------------------------------|
    /// Экран.
    ///-----------------------------------------------------------------------:
    public class BuffersScreen
    {
        public BuffersScreen(TextMeshProUGUI screen)
        {
            Debug.Assert(screen != null);
            
            _screen = screen;
        }

        private TextMeshProUGUI     _screen;
        private string               _const;
        private StringBuilder  _now = new();
        private StringBuilder  _str = new();

        public string GetStr() => _str.ToString();
        public string GetNow() => _now.ToString();

        public void Init()
        {
            _const = _screen.text;

            _now.Clear();
            _str.Clear();
        }

        public void AfterCommand()
        { 
            _const = _screen.text + "\n";
            _now.Clear();
            _str.Clear();
        }

        public void All2Screen()
        {   
            _screen.text = _const + _now.ToString() + _str.ToString() + "\n";
            _const = _screen.text;
            _now.Clear();
            _str.Clear();
        }

        /// После отрапвки команды на Питон-сервер,
        /// Команда стирается с экрана.
        public void Now2Screen()
        {   
            _screen.text = _const + _now.ToString() + "\n";
            _const = _screen.text;
            _now.Clear();
            _str.Clear();
        }

        public void Str2Now()
        {   _now.Append(_str + "\n");
            _str.Clear();
        }

        public void ClearAll()
        {   _screen.text = "";
            _const       = "";
            _now.Clear();
            _str.Clear();
        }

        public void AddCharacter(string character)
        {
            // Вставка в позицию курсора (если реализовать)
            // Для простоты добавляем в конец
            
            if(character != "\n")
            {
                _str.Append(character);
                _screen.text = _const + _now.ToString() + _str.ToString();
            }
        }

        // Кнопка удаления (Backspace)
        public void DeleteLastCharacter()
        {
            if (_str.Length <= 0) return;

            _str.Remove(_str.Length - 1, 1);
            _screen.text = _const + _now.ToString() + _str.ToString();
        }
    }
}
