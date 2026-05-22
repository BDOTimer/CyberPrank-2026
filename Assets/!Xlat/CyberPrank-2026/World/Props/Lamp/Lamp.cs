using UnityEngine;

namespace CP2026
{


    public class Lamp : MonoBehaviour, IControl
    {
        [SerializeField] private float accessDistance = 20f;
        [SerializeField] private Material mat;
        [SerializeField] bool    isOn = false;
        [SerializeField] Light   lampLight;

        IGoodbye _goodbye;

        void Awake()
        {
            Debug.Assert( mat   != null );
            Debug.Assert( lampLight != null );
        }

        void Start()
        {   Done();
        }

        public string Name ()
        {   return name;
        }

        public void  Control()
        {
        }

        public void Hello(IGoodbye goodbye)
        {   _goodbye = goodbye;
        }

        public bool IsAccess(float distance)
        {   
            Debug.Log("111");

            if(distance > accessDistance)
            {   return false;
            }

            Debug.Log("222");

            isOn = !isOn;
            Done();

            return false;
        }

        void Done()
        {
            if(isOn)
            {   lampLight.enabled = true;
                mat.color = Color.yellow;
                mat.color = Color.yellow;
                mat.SetColor("_FaceColor", Color.yellow);
            }
            else
            {   lampLight.enabled = false;
                mat.color = Color.black;
                mat.SetColor("_FaceColor", Color.black);
            }
        }
    }
}

