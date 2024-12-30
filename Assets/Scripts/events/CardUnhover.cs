using System;
using UnityEngine;
using Zone;

namespace events {
    public class CardUnhover : CardEvent {
        public CardUnhover(Card.Card card) : base(card) {
        }
    }
}
