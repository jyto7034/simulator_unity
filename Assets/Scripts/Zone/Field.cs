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
                        obj.transform.AddPositionRet(x: 0.05f, y: 0.2f),
                        new Vector3(0.08f, 0.08f, 0.08f),
                        Quaternion.Euler(0, 0, 0))
                    );
            }
        }

        // 이 함수에서 뭔가 버그 많이 터질 것 같음
        // 소환의 경우 Zone 에 소속되어있지 않으므로, if 가 실행됨.
        // 그런 경우를 적절히 처리해야함.
        public override void add_card(Card.Card comp, int slot_id = -1) {
            if (slot_id == -1) {
                return;
            }
            
            if (!get_zone(comp.current_zone).remove_card(comp)) {
                // When error occurred
            }

            // FieldZone의 자식 중에서 slot_id + 1에 해당하는 오브젝트를 찾음
            UnityEngine.Transform targetSlot = transform.Find((slot_id + 1).ToString());
    
            if (targetSlot == null) {
                Debug.LogError($"Could not find slot {slot_id + 1} in FieldZone");
                return;
            }

            cards.Add(comp);
            comp.current_zone = ZoneType.Field;
            comp.transform.SetParent(targetSlot);
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