using System.Collections;
using UnityEngine;

namespace BA.Utility
{
    public class SimpleAnimator : MonoBehaviour
    {
        public float time = 0;
        public float totalTime = 0;
        [SerializeField]
        public SimpleAnimation[] animations;

        private bool playing = false;

        public enum AnimationType
        {
            None,
            Position,
            Rotation
        }

        public void Play()
        {
            playing = true;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(playing && time < totalTime)
            {
                time += Time.deltaTime;

                foreach (SimpleAnimation jointAnimation in animations)
                {
                    float x = jointAnimation.x.Evaluate(time);
                    float y = jointAnimation.y.Evaluate(time);
                    float z = jointAnimation.z.Evaluate(time);

                    if (jointAnimation.animationType == AnimationType.Position)
                    {
                        jointAnimation.transform.position = new Vector3(x, y, z);
                    }
                    else if (jointAnimation.animationType == AnimationType.Rotation)
                    {
                        //jointAnimation.joint.axis = new Vector3(x, y, z);
                        jointAnimation.transform.rotation = Quaternion.Euler(new Vector3(x, y, z));
                    }
                }
            }
            if(time > totalTime)
            {
                time = totalTime;
            }
        }
    }
}