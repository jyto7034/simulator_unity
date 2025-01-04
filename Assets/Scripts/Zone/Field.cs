using System.Collections.Generic;
using System.Linq;
using Core;
using Transform;
using UnityEngine;
using Utils;
using static Utils.Utils;
using static Utils.Option;
using static Utils.Result;

namespace Zone {
    public class Field : Zone {
        // 이 함수에서 뭔가 버그 많이 터질 것 같음
        // 소환의 경우 Zone 에 소속되어있지 않으므로, if 가 실행됨.
        // 그런 경우를 적절히 처리해야함.
        public override Result<Unit, GameError> add_card(Card.Card comp, int slot_id = -1) {
            if (slot_id == -1) {
                return Err(GameError.UnkownFailed);
            }
            
            // TODO: 수정해야함.
            if (!get_zone(comp.current_zone).remove_card(comp).TryGetOk(out var _)) {
                return Err(GameError.UnkownFailed);
                // When error occurred
            }

            // FieldZone의 자식 중에서 slot_id + 1에 해당하는 오브젝트를 찾음
            UnityEngine.Transform targetSlot = transform.Find((slot_id + 1).ToString());
    
            if (targetSlot == null) {
                Debug.LogError($"Could not find slot {slot_id + 1} in FieldZone");
                return Err(GameError.UnkownFailed);
            }

            cards.Add(comp);
            comp.current_zone = ZoneType.Field;
            comp.transform.SetParent(targetSlot);
            GameSystem.Instance.card_animation.place_to_field_slot(comp, slot_id);
            return Ok();
        }

        public override Result<Unit, GameError> remove_card(Card.Card card) {
            if (cards.Contains(card)) {
                cards.Remove(card);
                return Ok();
            }
            return Err(GameError.UnkownFailed);
        }

        public override Result<Unit, GameError> move_card(Card.Card card, ZoneType zone_type) {
            return Ok();
        }
    }
}