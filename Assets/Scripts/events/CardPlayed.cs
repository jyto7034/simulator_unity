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
                        break;
                    case "Field_Slot":
                        // field_slot 은 Field 스크립트가 부모 오브젝트에 있기 때문에 따로 처리 해줌.
                        var field_slot = _object.transform.parent;
                        if (field_slot.TryGetComponent<Field>(out var field_comp)) {
                            field_comp.add_card(card, int.Parse(_object.transform.name) - 1);
                        }
                        break;
                    default:
                        // 카드 재정렬 때문에 잠깐 비활성화.
                        // GameObject.FindGameObjectWithTag("Hand").GetComponent<Hand>().hide_cards(true);
                        break;
                }
            }
        }
    }
}
