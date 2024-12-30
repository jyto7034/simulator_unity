namespace events {
    
    public class CardEvent {
        public readonly Card.Card card;

        public CardEvent(Card.Card card) {
            this.card = card;
        }
    }
}
