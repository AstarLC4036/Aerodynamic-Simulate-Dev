using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script
{
    //一定要加可序列化的attr
    [Serializable]
    //自定义的数据类
    public class m_DataClass
    {
        public Vector3 testVector3;
        public int testInt;
        public GameObject testObject;
    }

    public class Sample : MonoBehaviour
    {
        //创建一个列表来存储自定义的数据
        public List<m_DataClass> datas = new List<m_DataClass>();
        public m_DataClass[] datasArray;

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}