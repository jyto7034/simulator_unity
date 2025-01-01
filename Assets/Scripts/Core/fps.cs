using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core {
    public class FrameCounter : MonoBehaviour {
        private float deltaTime = 0f;

        [SerializeField] private int size = 25;
        [SerializeField] private Color color = Color.red;
        [SerializeField]
        private bool enableVSync = true;  // Inspector에서 설정 가능
        [SerializeField]
        private int targetFrameRate = 60;  // 목표 FPS

        private void Start() {
            QualitySettings.vSyncCount = enableVSync ? 1 : 0;
            Application.targetFrameRate = targetFrameRate;  // FPS 제한 설정
        }
        void Update() {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        private void OnGUI() {
            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(30, 30, Screen.width, Screen.height);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = size;
            style.normal.textColor = color;

            float ms = deltaTime * 1000f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);

            GUI.Label(rect, text, style);
        }
    }
}