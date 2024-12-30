using UnityEngine;
using Zone;

namespace events {
    public class CardDrag : CardEvent {
        public CardDrag(Card.Card card) : base(card) {
            GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>().OnCardDrag(card);
        }
    }
}
