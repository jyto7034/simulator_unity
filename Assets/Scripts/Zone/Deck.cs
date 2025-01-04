using System;
using UnityEngine;
using Utils;
using static Utils.Option;
using static Utils.Result;

namespace Zone {
    public class Deck : Zone{
        private void Start() {
            zone_type = ZoneType.Deck;
        }

        public override Result<Unit, GameError> add_card(Card.Card comp, AddCardOptions options = null) {
            return Ok();
        }

        public override Result<Unit, GameError> remove_card(Card.Card card) {
            return Ok();
        }

        public override Result<Unit, GameError> move_card(Card.Card card, ZoneType target_zone, AddCardOptions options = null) {
            return Ok();
        }
    }
}