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
    
        private StringBuilder  allBuilder;
        private StringBuilder  strBuilder;
        private bool shiftPressed = false;
        private bool     capsLock = false;
    
        // Словарь для преобразования виртуальных кодов в символы с учётом Shift/Caps
        private static System.Collections.Generic.Dictionary<Key, char> baseKeyMap;
        private static System.Collections.Generic.Dictionary<Key, char> shiftKeyMap;

        Keyboard keyboard;

        private void Awake()
        {
            keyboard = Keyboard.current;
            if (keyboard == null)
            {   Debug.LogWarning(keyboard == null);
            }
            
            allBuilder = new StringBuilder();
            strBuilder = new StringBuilder();

            if (baseKeyMap == null)
            {   InitializeKeyMaps();
            }
        }

        private void Start()
        {
            // Находим компонент, если не назначен вручную
            if (targetText == null)
                targetText = GetComponent<TextMeshProUGUI>();
            
            if (targetText != null)
                allBuilder.Append(targetText.text);
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

        private void OnEnable()
        {
            if (enableKeyboardInput)
            {
                Keyboard.current.onTextInput += OnTextInput;
            }
        }

        private void OnDisable()
        {
            if (enableKeyboardInput && Keyboard.current != null)
            {
                Keyboard.current.onTextInput -= OnTextInput;
            }
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

                SendText(strBuilder.ToString());
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
    
        // Альтернативный метод через TextInput (рекомендуется для Unicode)
        private void OnTextInput(char character)
        {
            // Фильтруем управляющие символы
            if (character >= 32 && character != 127)
            {
                //AddCharacter(character.ToString());
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
                allBuilder.Clear();
                allBuilder.Append(targetText.text);
            
                // Вставка в позицию курсора (если реализовать)
                // Для простоты добавляем в конец
            
                allBuilder.Append(character);
                targetText.text = allBuilder.ToString();

                if(character != "\n")
                {
                    strBuilder.Append(character);
                }
            }
        }
    
        // Кнопка удаления (Backspace)
        public void DeleteLastCharacter()
        {
            if (targetText != null && targetText.text.Length > 0)
            {
                allBuilder.Clear();
                allBuilder.Append(targetText.text);
                allBuilder.Remove(allBuilder.Length - 1, 1);
                targetText.text = allBuilder.ToString();
            }
        }
    
        // Удаление с позиции курсора (если добавить)
        public void DeleteAtPosition(int position)
        {
            if (targetText != null && position >= 0 && position < targetText.text.Length)
            {
                allBuilder.Clear();
                allBuilder.Append(targetText.text);
                allBuilder.Remove(position, 1);
                targetText.text = allBuilder.ToString();
            }
        }
    
        // Вставка в определённую позицию
        public void InsertAtPosition(string text, int position)
        {
            if (targetText != null && position >= 0 && position <= targetText.text.Length)
            {
                allBuilder.Clear();
                allBuilder.Append(targetText.text);
                allBuilder.Insert(position, text);
            
                if (allBuilder.Length <= maxTextLength)
                    targetText.text = allBuilder.ToString();
            }
        }
    
        // Вспомогательная функция: Очистить всё поле
        public void ClearText()
        {
            if (targetText != null)
            {   targetText.text = "";
                allBuilder.Clear();
                strBuilder.Clear();
            }
        }
    
        // Получить текущую раскладку для отображения (опционально)
        public bool IsShiftPressed => shiftPressed;
        public bool IsCapsLock => capsLock;

        public void On()
        {
            firstPersonController.IsKeys = false;
        }

        public void Off()
        {
            firstPersonController.IsKeys = true;
        }

        void SendText(string strMessage)
        {
            if (strMessage.Length == 0) return;

            Debug.Log(strMessage);

            if(strMessage == "exit")
            {   monitor.Exit();
                return;
            }

            if(strMessage == "cls")
            {   ClearText();
                return;
            }

            if(strMessage == "exe")
            {   pythonExecutor.SendText(allBuilder.ToString());
                allBuilder.Clear();
                strBuilder.Clear();
                return;
            }

            foreach(string item in Monitor.Vob)
            {
                //Debug.Log($"item: {item} : {item.Length}");

                if(item == strMessage)
                {   pythonExecutor.SendCommand(strMessage);
                }
            }

            allBuilder.Append(strMessage);
            strBuilder.Clear();
        }
    }
}
