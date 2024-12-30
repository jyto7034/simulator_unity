using System.Collections.Generic;
using System.Linq;
using Core;
using Transform;
using UnityEngine;
using static Utils.Utils;

namespace Zone {
    public class Field : Zone {
        public static List<TransformData> field_slots_tf;

        private void Start() {
            var result = GameObject.FindGameObjectsWithTag("Field_Slot").OrderBy(t => t.name).ToArray();
            field_slots_tf = new List<TransformData>();
            
            foreach (var obj in result) {
                field_slots_tf.Add(
                    new TransformData(
                        new Vector3(obj.transform.position.x + 0.05f, obj.transform.position.y + 0.2f,
                            obj.transform.position.z),
                        obj.transform.localScale * Constant.card_size_after_place_to_field,
                        Quaternion.Euler(0, obj.transform.rotation.y, obj.transform.rotation.z)));
            }
        }

        // 이 함수에서 뭔가 버그 많이 터질 것 같음
        // 소환의 경우 Zone 에 소속되어있지 않으므로, if 가 실행됨.
        // 그런 경우를 적절히 처리해야함.
        public override void add_card(Card.Card comp, int slot_id = 0) {
            if (!get_zone(comp.current_zone).remove_card(comp)) {
                // When error occurred
            }
            cards.Add(comp);
            GameSystem.Instance.card_animation.place_to_field_slot(comp, slot_id);
        }

        public override bool remove_card(Card.Card card) {
            if (cards.Contains(card)) {
                cards.Remove(card);
                return true;
            }
            return false;
        }

        public override void pull_card(Card.Card card, ZoneType zone_type) {
            throw new System.NotImplementedException();
        }
    }
}