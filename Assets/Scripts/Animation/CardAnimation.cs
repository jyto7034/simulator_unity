using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Transform;
using UnityEngine;
using Zone;

namespace Animation {
    public class Animation{
        public Sequence sc = DOTween.Sequence();

        public Sequence MoveTo(Card.Card card, TransformData transform_data, float dotween_time) {
            sc.Append(card.transform.DOMove(transform_data.position, dotween_time));
            sc.Join(card.transform.DORotateQuaternion(transform_data.rotation, dotween_time));
            sc.Join(card.transform.DOScale(transform_data.scale, dotween_time));
            return sc;
        }
    }
    
    public class CardAnimation : MonoBehaviour {
        public static List<TransformData> field_slots_tf;

        private void Start() {
            var result = GameObject.FindGameObjectsWithTag("Field_Slot").OrderBy(t => t.name).ToArray();
            field_slots_tf = new List<TransformData>();
            
            foreach (var obj in result) {
                field_slots_tf.Add(
                    new TransformData(
                        obj.transform.AddPositionRet(x: 0.05f, y: 0.2f),
                        new Vector3(0.64f, 0.01f, 0.62f),
                        Quaternion.Euler(0, 0, 0))
                );
            }
        }
        
        // Field 에 카드가 play 되는 애니메이션 함수
        public void place_to_field_slot(Card.Card card, int slot_id) {
            var slot = field_slots_tf[slot_id];
            var before = slot.Clone();
            before.AddPosition(x: 0.05f, y: 2f);
            before.AddScale(x: 0.02f, y: 0.02f, z: 0.02f);

            var anim = new Animation();
            
            anim.MoveTo(card, before, 0.2f).AppendInterval(0.3f).Play();
            anim.MoveTo(card, slot, 0.2f).Play();
        }
    }
}