using UnityEngine;
using Zone;

namespace events {
    public class CardDragBegin : CardEvent {
        public CardDragBegin(Card.Card card) : base(card) {
            GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>().BeginCardDrag(card);
        }
    }
}
