using UnityEngine;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.InputSystem;

namespace CP2026
{
    public class PythonExecutor : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void InitSkulpt();

        [DllImport("__Internal")]
        private static extern void RunPython(string code);

        [SerializeField] private string textCode =
            "# Программа подсчета суммы натуральных чисел до 100\r\n" +
            "total = 0\r\nfor i in range(1, 101):\r\n" +
            "    total += i\r\n" +
            "print(\"Сумма чисел от 1 до 100 равна:\", total)";

        [SerializeField] private TextMeshProUGUI textTMP;

        public enum Mode
        {
            Start,
            Code,
            End
        }

        Mode _mode = Mode.Start;

        private void Start()
        {   
            if (textTMP == null)
            {
                // Ищем на том же объекте
                textTMP = GetComponent<TextMeshProUGUI>();
        
                // Если не нашли, ищем в дочерних объектах
                if (textTMP == null)
                    textTMP = GetComponentInChildren<TextMeshProUGUI>();
        
                // Если всё ещё null — ошибка
                if (textTMP == null)
                {
                    Debug.LogError("TextMeshProUGUI не найден! Повесьте компонент вручную.", this);
                    return;
                }
            }

            #if !UNITY_EDITOR && UNITY_WEBGL
                // Инициализируем Skulpt при старте игры
                InitSkulpt();
                textTMP.text = "Skulpt инициализирован (WebGL)";
            #else
                textTMP.text = "Запустите WebGL билд для работы Python";
            #endif
        }

        private void Update()
        {
            if(Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                switch(_mode)
                {
                    case Mode.Start:
                        textTMP.text = textCode;
                        _mode = Mode.Code;
                        break;
                    case Mode.Code :
                        ExecutePlayerCode(textCode);
                        _mode = Mode.End;
                        break;
                    case Mode.End  :
                        textTMP.text = "> ...";
                        _mode = Mode.Start;
                        break;
                }
            }
        }

        public void ExecutePlayerCode(string pythonCode)
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
                RunPython(pythonCode);
            #else
                Debug.Log("Код выполняется только в WebGL сборке");
            #endif
        }

        // Этот метод будет вызван из JavaScript через SendMessage
        public void OnPythonOutput(string text)
        {
            Debug.Log("Python вывод: " + text);
            // Здесь можно обновить ваш TextMeshProUGUI
            if (textTMP != null) textTMP.text = text;
        }

        public void OnPythonSuccess(string message)
        {
            Debug.Log($"Python успех: {message}");
        }

        public void OnPythonError(string error)
        {
            Debug.LogError($"Python err: {error}");
            if (textTMP != null) textTMP.text = $"Ошибка: {error}";
        }
    }
}



