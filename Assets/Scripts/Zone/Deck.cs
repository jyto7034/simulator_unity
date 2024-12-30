using UnityEngine;

namespace Zone {
    public class Deck : Zone{
        public override void add_card(Card.Card comp, int slot_id = 0) {
            throw new System.NotImplementedException();
        }

        public override bool remove_card(Card.Card card) {
            throw new System.NotImplementedException();
        }

        public override void pull_card(Card.Card card, ZoneType zone_type) {
            throw new System.NotImplementedException();
        }
    }
}