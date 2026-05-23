using TMPro;
using UnityEngine;

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
        [SerializeField] private float           accessDistance = 2f;
        [SerializeField] private PythonExecutor  pythonExecutor;
        [SerializeField] private TextMeshProUGUI textTMP;
        [SerializeField] private bool            isOn = false;
        [SerializeField] private Color           colorWork = Color.green;

        IGoodbye _goodbye;

        void Awake()
        {
            Debug.Assert( pythonExecutor != null );
            Debug.Assert( textTMP        != null );
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

            pythonExecutor.LoopInput();
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
            textTMP.text = str + "Добро пожаловать в систему!";

            SetSpeedFans(30);
        }

        void DoOff()
        {   
            isOn = false;

            textTMP.color = Color.red;
            textTMP.fontMaterial.SetColor("_FaceColor", Color.red);
            textTMP.text = "... выключен ...";

            SetSpeedFans(0);
        }

        void SetSpeedFans(float speedPersent)
        {
            FansControl fc = GetComponent<FansControl>();
            if( fc != null )
            {   fc.SetSpeed(speedPersent);
            }
        }
    }
}

