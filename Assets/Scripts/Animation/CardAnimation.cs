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
            var before = new TransformData(
                new Vector3(slot.position.x + 0.05f, slot.position.y + 2f, slot.position.z), 
                card.transform.localScale * Constant.card_size_before_place_to_field,
                Quaternion.Euler(0, slot.rotation.y, slot.rotation.z));

            var anim = new Animation();
            
            anim.MoveTo(card, before, 0.2f).AppendInterval(0.3f).Play();

            anim.MoveTo(card, slot, 0.2f);
        }

        public static void place_to_hand(GameObject card) { }
    }
}