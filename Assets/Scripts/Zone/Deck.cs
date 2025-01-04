using UnityEngine;
using Utils;
using static Utils.Option;
using static Utils.Result;

namespace Zone {
    public class Deck : Zone{
        public override Result<Unit, GameError> add_card(Card.Card comp, int slot_id = -1) {
            return Ok();
        }

        public override Result<Unit, GameError> remove_card(Card.Card card) {
            return Ok();
        }

        public override Result<Unit, GameError> move_card(Card.Card card, ZoneType target_zone) {
            return Ok();
        }
    }
}