using System.Collections.Generic;
using UnityEngine;
using Zone;

namespace Card {
    public class CardSorting : MonoBehaviour {
        public Hand hand_field;
        private const int BASE_QUEUE = 3000; // Transparent 큐 시작점

        void Start()
        {
            // Start 함수가 아닌, 이벤트 발생했을 떄, ( cards 변경된 경우 등 ) 로 수정해야함.
            UpdateRenderQueue();
        }

        private void UpdateRenderQueue()
        {
            for (int i = 0; i < hand_field.cards.Count; i++)
            {
                MeshRenderer meshRenderer = hand_field.cards[i].GetComponent<MeshRenderer>();
                meshRenderer.material.renderQueue = BASE_QUEUE + i;
                
                hand_field.cards[i].transform.AddPosition(y:0.01f * i);
            }
        }
    }
}