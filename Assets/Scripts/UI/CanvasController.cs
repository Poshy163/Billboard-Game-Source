#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace UI
{
    public class CanvasController : MonoBehaviour
    {
        private int _counter;
        private Text _fps;

        private void Start()
        {
            _fps = GameObject.Find("FPS").GetComponent<Text>();
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 90;
        }

        private void Update()
        {
            GetFPS();
        }

        private void GetFPS()
        {
            _counter++;
            if (_counter <= 10) return;
            _counter = 0;
            _fps.text = $"{(int) (1 / Time.unscaledDeltaTime)}";
        }
    }
}