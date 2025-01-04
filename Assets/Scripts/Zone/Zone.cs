using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using static Utils.Utils;


namespace Zone {
    public abstract class Zone : MonoBehaviour{
        [HideInInspector] public List<Card.Card> cards;

        public abstract Result<Unit, GameError> add_card(Card.Card comp, int slot_id = -1);

        public abstract Result<Unit, GameError> remove_card(Card.Card card);

        public abstract Result<Unit, GameError> move_card(Card.Card card, ZoneType target_zone);
    }
}