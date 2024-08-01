using Assets.Script.Flight;
using BA.Flight;
using BA.Utility;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BA.Game
{
    public class AircraftManager : MonoBehaviour
    {
        public Aircraft currentAircraft;
        private Rigidbody aircraftRB;
        private AircraftWeapons aircraftWeapons;
        public Camera mainCam;

        public Transform lockedTarget;

        public Texture2D mousePointerTex;
        public int mouseCursurSize = 16;
        public Texture2D dirCursurTex;
        public int dirCursurSize = 8;
        public Texture2D veloCursurTex;
        public int veloCursurSize = 8;
        public Texture2D mslCursurTex;
        public int mslCursurSize = 8;
        [Header("Pitch Ladder Settings")]
        public GameObject pitchLadderPrefabHorizontal;
        public GameObject pitchLadderPrefabNormal;
        public Transform pitchLadderUIParent;
        public int pitchLadderDeltaAngle = 5;
        public int pitchLadderMaxY = 100;
        public int pitchLadderMinY = -100;
        [Header("Compass Settings")]
        public GameObject compassLadderPrefab;
        public Transform compassLadderUIParent;
        public int compassLadderMaxX = 100;
        public int compassLadderMinX = -100;

        public TMP_Text throttleText;
        public TMP_Text thurstText;
        public TMP_Text heightText;
        public TMP_Text speedText;
        public TMP_Text aoaText;
        public TMP_Text gText;

        private List<Bar> pitchLadderBars = new List<Bar>();
        private List<Bar> compassBars = new List<Bar>();

        public List<Missle> launchedMissles = new List<Missle>();

        // Use this for initialization
        void Start()
        {
            aircraftWeapons = currentAircraft.GetComponent<AircraftWeapons>();
            aircraftRB = currentAircraft.GetComponent<Rigidbody>();

            for (int i = -90; i < 90; i += pitchLadderDeltaAngle)
            {
                GameObject obj;
                if (i == 0)
                {
                    obj = Instantiate(pitchLadderPrefabHorizontal, pitchLadderUIParent);
                }
                else
                {
                    obj = Instantiate(pitchLadderPrefabNormal, pitchLadderUIParent);
                }
                TMP_Text[] angleTexts = obj.GetComponentsInChildren<TMP_Text>();
                foreach (TMP_Text text in angleTexts)
                {
                    text.text = i + "";
                }
                pitchLadderBars.Add(new Bar(obj, i));
            }

            for (int i = 0; i < 360; i += 5)
            {
                GameObject obj = Instantiate(compassLadderPrefab, compassLadderUIParent);
                TMP_Text[] angleTexts = obj.GetComponentsInChildren<TMP_Text>();
                foreach (TMP_Text text in angleTexts)
                {
                    text.text = i + "";
                }
                compassBars.Add(new Bar(obj, i));
            }
        }

        // Update is called once per frame
        void Update()
        {
            throttleText.text = "Throttle: " + Mathf.FloorToInt(currentAircraft.engineThorttle * 100).ToString() + "%";
            if (currentAircraft.engineOn)
            {
                thurstText.text = "Thrust:" + Mathf.FloorToInt(currentAircraft.engineThurst * currentAircraft.engineThorttle).ToString() + " N";
            }
            else
            {
                thurstText.text = "Thrust: 0 N";
            }

            heightText.text = Mathf.FloorToInt(currentAircraft.transform.position.y) + "";
            speedText.text = Mathf.FloorToInt(aircraftRB.velocity.magnitude) + "";
            aoaText.text = MathUtility.FloorToDC(currentAircraft.angleOfAttack, 1) + "";
            gText.text = MathUtility.FloorToDC(currentAircraft.GForce, 1) + "";

            pitchLadderBars.ForEach((Bar bar) => {
                float rollAngle = currentAircraft.transform.eulerAngles.x;
                float covertedAngle = MathUtility.CovertAngle(Mathf.DeltaAngle(currentAircraft.transform.eulerAngles.z, bar.angle));
                float position = MathUtility.TransformAngle(covertedAngle, Camera.main.fieldOfView, Camera.main.pixelHeight);

                if (position >= pitchLadderMinY && position <= pitchLadderMaxY)
                {
                    if (!bar.UIObject.activeSelf)
                        bar.UIObject.SetActive(true);
                    RectTransform barTransform = bar.UIObject.GetComponent<RectTransform>();
                    barTransform.localPosition = new Vector3(0, position, 0);
                    pitchLadderUIParent.localRotation = Quaternion.Euler(0, 0, -rollAngle);
                }
                else
                {
                    if (bar.UIObject.activeSelf)
                        bar.UIObject.SetActive(false);
                }
            });

            compassBars.ForEach((Bar bar) => {
                float covertedAngle = Mathf.DeltaAngle(currentAircraft.transform.eulerAngles.y, bar.angle);
                float position = MathUtility.TransformAngle(MathUtility.CovertAngle(covertedAngle), Camera.main.fieldOfView, Camera.main.pixelWidth);

                if (Mathf.Abs(covertedAngle) <= 90 && position >= compassLadderMinX && position <= compassLadderMaxX)
                {
                    if (!bar.UIObject.activeSelf)
                        bar.UIObject.SetActive(true);
                    RectTransform barTransform = bar.UIObject.GetComponent<RectTransform>();
                    barTransform.localPosition = new Vector3(position, 0, 0);
                }
                else
                {
                    if (bar.UIObject.activeSelf)
                        bar.UIObject.SetActive(false);
                }
            });
        }

        private void OnGUI()
        {
            GUI.DrawTexture(Utilities.CalcucateTextureScreenPos(Input.mousePosition, mouseCursurSize), mousePointerTex);

            Vector3 dirAircraftOnScreen = mainCam.WorldToScreenPoint(mainCam.transform.position + currentAircraft.transform.right);
            GUI.DrawTexture(Utilities.CalcucateTextureScreenPos(dirAircraftOnScreen, dirCursurSize), dirCursurTex);
            Vector3 dirVecOnScreen = aircraftRB.velocity.magnitude > 0.1 ? mainCam.WorldToScreenPoint(mainCam.transform.position + aircraftRB.velocity.normalized) : Camera.main.WorldToScreenPoint(mainCam.transform.position + currentAircraft.transform.right);
            GUI.DrawTexture(Utilities.CalcucateTextureScreenPos(dirVecOnScreen, veloCursurSize), veloCursurTex);

            if (lockedTarget != null/* && Utilities.ObjectVisible(lockedTarget, mainCam) */)
            {
                Vector3 targetOnScreen = mainCam.WorldToScreenPoint(lockedTarget.position);
                GUI.Label(new Rect(targetOnScreen.x, Screen.height - targetOnScreen.y, 100, 50), "Target");
            }

            launchedMissles.ForEach((Missle missle) => { 
                if(missle == null)
                {
                    launchedMissles.Remove(missle);
                }
                if(Utilities.ObjectVisible(missle.transform, mainCam))
                {
                    Vector3 mslOnScreen = mainCam.WorldToScreenPoint(missle.transform.position);
                    float dstToMsl = (currentAircraft.transform.position - missle.transform.position).magnitude;
                    GUI.DrawTexture(Utilities.CalcucateTextureScreenPos(mslOnScreen, mslCursurSize), mslCursurTex);
                    GUI.Label(new Rect(mslOnScreen.x - 25, Screen.height - mslOnScreen.y + 10, 50, 50), "MSL");
                    GUI.Label(new Rect(mslOnScreen.x - 25, Screen.height - mslOnScreen.y + 25, 50, 50), MathUtility.FloorToDC(dstToMsl / 1000, 1) + "km");
                }
            });
        }

        private struct Bar
        {
            public GameObject UIObject;
            public float angle;

            public Bar(GameObject UIObject, float angle)
            {
                this.UIObject = UIObject;
                this.angle = angle;
            }
        }
    }
}