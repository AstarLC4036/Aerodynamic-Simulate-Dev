using BA.Game;
using System.Collections;
using UnityEngine;

namespace Assets.Script.Flight
{
    public class AircraftWeapons : MonoBehaviour
    {
        public Missle[] missles;

        public delegate void OnLaunchFunc(Missle missle);
        public event OnLaunchFunc OnLaunch;

        public AircraftManager am;
        //public OnLaunchFunc OnLaunch;

        private void Start()
        {
            OnLaunch = new OnLaunchFunc((Missle missle) => { am.launchedMissles.Add(missle); Debug.Log("event fire"); });
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                Missle missle = missles[0];
                missle.Launch();
                OnLaunch(missle);
                Debug.Log("Call");
            }
        }
    }
}