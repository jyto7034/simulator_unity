using System;
using System.Collections.Generic;
using UnityEngine;

namespace Card {
    public class CardManager : MonoBehaviour {
        [HideInInspector] public static List<GameObject> player_cards;
        [HideInInspector] public static List<GameObject> opponent_cards;
        [SerializeField]
        private GameObject cardPrefab;

        // Awake 와 Start 함수 의존성 잘 확인해야함.
        // 아마 고루틴 쓰는게 편할듯.
        // Awake와 Start: Awake에서 초기화하고 Start에서 의존성을 확인합니다.
        // Find 메서드: Start에서 Find 메서드를 사용하여 의존적인 오브젝트를 찾습니다.
        // Coroutine: Coroutine을 사용하여 의존적인 오브젝트가 생성될 때까지 기다립니다.
        // 이벤트 시스템: 의존적인 오브젝트가 생성될 때 이벤트를 발생시키고 이를 구독하여 처리합니다.
        private void Awake() {
            (player_cards, opponent_cards) = CardGenerator.CreateMaterialsAndApplyToPrefab(cardPrefab);
        }

        void set_origin_order(bool is_mine) {
            var count = is_mine ? player_cards.Count : opponent_cards.Count;
            for (var i = 0; i < count; i++) {
                var target_card = is_mine ? player_cards[i] : opponent_cards[i];
                target_card?.GetComponent<Order>().set_origin_order(i);
            }
        }
    }
}