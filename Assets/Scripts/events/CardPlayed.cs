using UnityEngine;
using Zone;

namespace events {

    public class CardPlayed : CardEvent {
        public CardPlayed(Card.Card card) : base(card) {
            var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
            var result = new RaycastHit[10];
            var size = Physics.RaycastNonAlloc(ray, result);
            
            for (var i = 0; i < size; i++) {
                var _object = result[i];
                
                switch (_object.collider.tag) {
                    case "Untagged":
                        continue;
                    case "Card":
                        if (card.current_zone_type == ZoneType.Hand) {
                            return;
                        }
                        if (GameObject.FindGameObjectWithTag("Hand").TryGetComponent<Hand>(out var hand)) {
                            Debug.Log("entry");
                            hand.add_card(card);
                        }
                        return;
                    case "Field_Slot":
                        card.current_zone.move_card(card, ZoneType.Field);
                        // field_slot 은 Field 스크립트가 부모 오브젝트에 있기 때문에 따로 처리 해줌.
                        return;
                    default:
                        GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>().EndCardDrag();
                        return;
                }
            }
        }
    }
}
