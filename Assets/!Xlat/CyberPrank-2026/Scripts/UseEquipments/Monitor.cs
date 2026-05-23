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
        [SerializeField] private PythonExecutor pythonExecutor;
        [SerializeField] private CustomKeyboardManager customKeyboardManager;

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

            string str = textTMP.text + "\r\n";

            textTMP.color = colorWork;
            textTMP.fontMaterial.SetColor("_FaceColor", colorWork);
            textTMP.text = str + "Добро пожаловать в систему!\n" + Help;

            SetSpeedFans(30);

            customKeyboardManager.On();
        }

        void DoOff()
        {   
            isOn = false;

            textTMP.color = Color.red;
            textTMP.fontMaterial.SetColor("_FaceColor", Color.red);
            textTMP.text = "... выключен ...";

            SetSpeedFans(0);

            customKeyboardManager.Off();
        }

        void SetSpeedFans(float speedPersent)
        {
            FansControl fc = GetComponent<FansControl>();
            if( fc != null )
            {   fc.SetSpeed(speedPersent);
            }
        }

        public void Exit()
        {   _goodbye.Goodbye();
            Hello(null);
        }
    }
}

