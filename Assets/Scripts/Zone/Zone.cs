using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using static Utils.Utils;


namespace Zone {
    public abstract class Zone : MonoBehaviour{
        [HideInInspector] public List<Card.Card> cards;
        [HideInInspector] public ZoneType zone_type;

        public abstract Result<Unit, GameError> add_card(Card.Card comp, AddCardOptions options = null);
        public abstract Result<Unit, GameError> remove_card(Card.Card card);
        public abstract Result<Unit, GameError> move_card(Card.Card card, ZoneType target_zone, AddCardOptions options = null);
    }
}