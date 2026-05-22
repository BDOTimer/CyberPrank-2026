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
        [SerializeField] private float     accessDistance = 2f;
        [SerializeField] private PythonExecutor pythonExecutor;

        IGoodbye _goodbye;

        void Awake()
        {
            Debug.Assert( pythonExecutor != null );
        }

        void Start()
        {
        }

        public string Name ()
        {   return name;
        }

        public void Control()
        {
            pythonExecutor.LoopInput();
        }

        public void Hello(IGoodbye goodbye)
        {   _goodbye = goodbye;

            if( _goodbye != null ) pythonExecutor.DoOn ();
            else                   pythonExecutor.DoOff();
        }

        public bool IsAccess(float distance)
        {
            if(distance > accessDistance)
            {   return false;
            }
            return true;
        }
    }
}

