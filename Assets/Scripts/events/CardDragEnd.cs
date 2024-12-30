using UnityEngine;
using Zone;

namespace events {
    public class CardDragEnd : CardEvent {
        public CardDragEnd(Card.Card card) : base(card){
            GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>().EndCardDrag();
        }
    }
}
