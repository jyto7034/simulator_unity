using System;
using UnityEngine;
using Zone;

namespace events {
    public class CardUnhover : CardEvent {
        public CardUnhover(Card.Card card) : base(card) {
            if (GameObject.FindGameObjectWithTag("Hand").TryGetComponent(out Hand hand_comp)) {
                Debug.Log("hide");
                hand_comp.hide_cards();
            }
        }
    }
}
