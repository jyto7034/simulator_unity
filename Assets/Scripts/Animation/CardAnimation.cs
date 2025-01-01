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
        // Field 에 카드가 play 되는 애니메이션 함수
        public void place_to_field_slot(Card.Card card, int slot_id) {
            var slot = Field.field_slots_tf[slot_id];
            var before = slot.Clone();
            before.AddPosition(x: 0.05f, y: 2f);
            before.AddScale(x: 0.02f, y: 0.02f, z: 0.02f);

            var anim = new Animation();
            
            anim.MoveTo(card, before, 0.2f).AppendInterval(0.3f).Play();
            anim.MoveTo(card, slot, 0.2f).Play();
        }

        public static void place_to_hand(GameObject card) { }
    }
}