using System;
using UnityEngine;
using Zone;

namespace events {
    public class CardHover : CardEvent {
        // 해당 이벤트가 발생하면
        // 숨어있던 카드가 확장되면서, 포커싱되어야함.
        public CardHover(Card.Card card) : base(card) {
            if (GameObject.FindGameObjectWithTag("Hand").TryGetComponent(out Hand hand_comp)) {
                hand_comp.show_cards();
            }
        }
    }
}
