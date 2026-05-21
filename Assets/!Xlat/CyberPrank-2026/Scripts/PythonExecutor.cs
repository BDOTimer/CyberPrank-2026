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

        [SerializeField] private TextMeshProUGUI textTMP;
        [SerializeField] private bool       isOn = false;

        private PythonTask task;
        private int        idTask = 0;
        private bool       isWait = false;
        

        public enum Mode
        {
            Start,
            Name,
            Condition,
            Decision,
            Answer,
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

            if (!isOn)
            {
                textTMP.color = Color.red;

                textTMP.fontMaterial.SetColor("_FaceColor", Color.red);

                textTMP.text = "... выключен ...";
                return;
            }

            #if !UNITY_EDITOR && UNITY_WEBGL
                // Инициализируем Skulpt при старте игры
                InitSkulpt();
                textTMP.text = "Skulpt инициализирован (WebGL). Тест-версия. Жми ENTER!";
            #else
                textTMP.text = "Запустите WebGL билд для работы Python";
            #endif
        }

        private void Update()
        {
            if (!isOn) return;

            if(Keyboard.current.enterKey.wasPressedThisFrame && !isWait)
            {
                switch(_mode)
                {
                    case Mode.Start:
                        textTMP.text = "Чтобы начать нажми ENTER!";
                        _mode = Mode.Name;
                        break;
                        
                    case Mode.Name :
                        task = CP2026.XlatPythonTasks.Instance.GetTask(idTask++);
                        textTMP.text = task.Name;
                        _mode = Mode.Condition;
                        break;

                    case Mode.Condition :
                        textTMP.text = task.Condition;
                        _mode = Mode.Decision;
                        break;

                    case Mode.Decision :
                        textTMP.text = task.Decision;
                        _mode = Mode.Answer;
                        break;

                    case Mode.Answer :
                        isWait = true;
                        ExecutePlayerCode(task.Decision);
                        _mode = Mode.End;
                        break;

                    case Mode.End  :
                        textTMP.text = "Сеанс закончен!"; /// 🔔
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
                Debug.Log("⛔ Код выполняется только в WebGL сборке");
                textTMP.text = "Код выполняется только в WebGL сборке"; /// ⛔
                isWait = false;
            #endif
        }

        // Этот метод будет вызван из JavaScript через SendMessage
        public void OnPythonOutput(string text)
        {
            isWait = false;

            if (textTMP != null)
            {   textTMP.text = "Ответ Питона: " + text +

                (ValidateAnswer(text) ? "Чувак, ты ошибься ..." : "Ответ принят!"); /// ❌ ✅
            }
        }

        public void OnPythonSuccess(string message)
        {   
            isWait = false;

            Debug.Log($"Python успех: {message}");
        }

        public void OnPythonError(string error)
        {   
            isWait = false;

            if (textTMP != null)
            {   textTMP.text = $"Ошибка: {error}";
            }
        }

        private bool ValidateAnswer(string answer)
        {   return answer == task.Answer;
        }
    }
}



