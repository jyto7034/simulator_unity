using System.Collections.Generic;
using UnityEngine;
using static Utils.Utils;


namespace Zone {
    public abstract class Zone : MonoBehaviour{
        [HideInInspector] public List<Card.Card> cards;

        public abstract void add_card(Card.Card comp, int slot_id = 0);

        public abstract bool remove_card(Card.Card card);

        public abstract void pull_card(Card.Card card, ZoneType zone_type);
    }
}