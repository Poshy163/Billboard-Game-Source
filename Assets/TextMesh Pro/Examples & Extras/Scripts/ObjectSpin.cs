﻿#region

using UnityEngine;
// ReSharper disable NotAccessedField.Local

#endregion

namespace TextMesh_Pro.Scripts
{
    public class ObjectSpin:MonoBehaviour
    {
#pragma warning disable 0414

        public float SpinSpeed = 5;
        public int RotationRange = 15;
        private Transform m_transform;

        private float m_time;
        private Vector3 m_prevPOS;
        private Vector3 m_initial_Rotation;
        private Vector3 m_initial_Position;
        private Color32 m_lightColor;
        private int frames;

        public enum MotionType
        {
            Rotation,
            BackAndForth,
            Translation
        }

        public MotionType Motion;

        private void Awake ()
        {
            m_transform = transform;
            m_initial_Rotation = m_transform.rotation.eulerAngles;
            m_initial_Position = m_transform.position;

            var component = GetComponent<Light>();
            m_lightColor = component != null ? component.color : Color.black;
        }


        // Update is called once per frame
        private void Update ()
        {
            switch(Motion)
            {
                case MotionType.Rotation:
                    m_transform.Rotate(0,SpinSpeed * Time.deltaTime,0);
                    break;
                case MotionType.BackAndForth:
                    m_time += SpinSpeed * Time.deltaTime;
                    m_transform.rotation = Quaternion.Euler(m_initial_Rotation.x,
                        Mathf.Sin(m_time) * RotationRange + m_initial_Rotation.y,m_initial_Rotation.z);
                    break;
                default:
                    {
                        m_time += SpinSpeed * Time.deltaTime;

                        var x = 15 * Mathf.Cos(m_time * .95f);
                        float y = 10; // *Mathf.Sin(m_time * 1f) * Mathf.Cos(m_time * 1f);
                        var z = 0f; // *Mathf.Sin(m_time * .9f);    

                        m_transform.position = m_initial_Position + new Vector3(x,z,y);

                        // Drawing light patterns because they can be cool looking.
                        //if (frames > 2)
                        //    Debug.DrawLine(m_transform.position, m_prevPOS, m_lightColor, 100f);

                        m_prevPOS = m_transform.position;
                        frames += 1;
                        break;
                    }
            }
        }
    }
}