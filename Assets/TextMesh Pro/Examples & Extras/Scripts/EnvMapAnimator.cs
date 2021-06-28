﻿#region

using System.Collections;
using TMPro;
using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable IteratorNeverReturns

#endregion

namespace TextMesh_Pro.Scripts
{
    public class EnvMapAnimator : MonoBehaviour
    {
        //private Vector3 TranslationSpeeds;
        public Vector3 RotationSpeeds;
        private Material m_material;
        private TMP_Text m_textMeshPro;
        private static readonly int EnvMatrix = Shader.PropertyToID("_EnvMatrix");


        private void Awake()
        {
            //Debug.Log("Awake() on Script called.");
            m_textMeshPro = GetComponent<TMP_Text>();
            m_material = m_textMeshPro.fontSharedMaterial;
        }

        // Use this for initialization
        private IEnumerator Start()
        {
            var matrix = new Matrix4x4();

            while (true)
            {
                //matrix.SetTRS(new Vector3 (Time.time * TranslationSpeeds.x, Time.time * TranslationSpeeds.y, Time.time * TranslationSpeeds.z), Quaternion.Euler(Time.time * RotationSpeeds.x, Time.time * RotationSpeeds.y , Time.time * RotationSpeeds.z), Vector3.one);
                matrix.SetTRS(Vector3.zero,
                    Quaternion.Euler(Time.time * RotationSpeeds.x, Time.time * RotationSpeeds.y,
                        Time.time * RotationSpeeds.z), Vector3.one);

                m_material.SetMatrix(EnvMatrix, matrix);

                yield return null;
            }
        }
    }
}