using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Core {
    public class Game : MonoBehaviour {
        public float rayLength = 100f; // 레이의 길이
    
        void Update()
        {
            // 마우스 위치를 스크린 좌표에서 레이로 변환
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 레이캐스트를 실행하고 Debug.DrawRay로 시각화
            if (Physics.Raycast(ray, out hit, rayLength))
            {
                // 레이가 물체에 부딪힌 경우, 부딪힌 지점까지만 그립니다
                Debug.DrawLine(ray.origin, hit.point, Color.red);
            }
            else
            {
                // 레이가 물체에 부딪히지 않은 경우, 최대 길이까지 그립니다
                Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);
            }
        }
    }
}