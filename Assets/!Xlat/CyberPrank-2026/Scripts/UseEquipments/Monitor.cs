using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CP2026
{
    public interface IGoodbye
    {
        public void Goodbye();
    }

    public interface IControl
    {
        public string Name ();
        public void Control ();
        public bool IsAccess(float distance);
        public void Hello (IGoodbye goodbye);
    }

    public class Monitor : MonoBehaviour, IControl
    {
        [Header("Settings")]
        [SerializeField] private float accessDistance = 2f;
        [SerializeField] private TextMeshProUGUI textTMP;
        [SerializeField] private bool isOn = false;
        [SerializeField] private Color colorWork = Color.green;

        [Header("Control")]
        [SerializeField] private PythonExecutor        pythonExecutor;
        [SerializeField] private CustomKeyboardManager customKeyboardManager;

        [Header("Опционально")]
        [SerializeField] private DesktopPC desktopPC;

        IGoodbye _goodbye;

        static string Help =
        "\nСПРАВКА:\n" +
        "  task - Получить задание\n" +
        "  exe  - Послать  решение\n" +
        "  help - Получить решение\n" +
        "  cls  - Очистить экран\n"   +
        "  exit - Выключить компьютер\n";

        public static string[] Vob = new string[]
        {   "task",
            "help"
        };

        void Awake()
        {
            Debug.Assert( pythonExecutor        != null );
            Debug.Assert( textTMP               != null );
            Debug.Assert( customKeyboardManager != null );
        }

        void Start()
        {
            if (isOn) DoOn ();
            else      DoOff();

            textTMP.fontMaterial.EnableKeyword("GLOW_ON");
            textTMP.fontMaterial.SetFloat("_GlowPower", 0.7f);
        }

        public string Name ()
        {   return name;
        }

        public void Control()
        {
            if (!isOn) return;

            customKeyboardManager.Loop();

            //pythonExecutor.DoStep();
        }

        public void Hello(IGoodbye goodbye)
        {   _goodbye = goodbye;

            if( _goodbye != null ) DoOn ();
            else                   DoOff();
        }

        public bool IsAccess(float distance)
        {
            if(distance > accessDistance)
            {   return false;
            }
            return true;
        }

        ///////////////////////////////////////////////////////////////////////
        
        void DoOn()
        {
            isOn = true;

            pythonExecutor.SetTextTMP( textTMP );

            textTMP.color = colorWork;
            textTMP.fontMaterial.SetColor("_FaceColor", colorWork);
            textTMP.text = "Добро пожаловать в систему!\n" + Help;

            SetSpeedFans(FanSpeed.Low);

            customKeyboardManager.On();
        }

        void DoOff()
        {   
            isOn = false;

            textTMP.color = Color.red;
            textTMP.fontMaterial.SetColor("_FaceColor", Color.red);
            textTMP.text = "... выключен ...";

            SetSpeedFans(FanSpeed.Off);

            customKeyboardManager.Off();
        }

        async void SetSpeedFans(FanSpeed mode)
        {
            if( desktopPC != null )
            {   await Awaitable.NextFrameAsync();
                desktopPC.SetFanSpeed(mode);
            }
        }

        public void Exit()
        {   _goodbye.Goodbye();
            Hello(null);
        }
    }
}

