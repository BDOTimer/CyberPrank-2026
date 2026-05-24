using UnityEngine;

namespace CP2026
{


    public class Lamp : MonoBehaviour, IControl
    {
        [SerializeField] private float accessDistance = 20f;
        [SerializeField] private Material matOn;
        [SerializeField] private Material matOff;
        [SerializeField] bool    isOn = false;
        [SerializeField] Light   lampLight;

        IGoodbye _goodbye;

        void Awake()
        {   Debug.Assert( matOn     != null );
            Debug.Assert( matOff    != null );
            Debug.Assert( lampLight != null );
        }

        void Start()
        {   Done();
        }

        public string Name ()
        {   return name;
        }

        public void Control()
        {
        }

        public void Hello(IGoodbye goodbye)
        {   _goodbye = goodbye;
        }

        public bool IsAccess(float distance)
        {   
            if(distance > accessDistance)
            {   return false;
            }

            isOn = !isOn;
            Done();

            return false;
        }

        void Done()
        {
            if(isOn)
            {   lampLight.enabled = true;
                GetComponent<Renderer>().material = matOn;
            }
            else
            {   lampLight.enabled = false;
                GetComponent<Renderer>().material = matOff;
            }
        }
    }
}

