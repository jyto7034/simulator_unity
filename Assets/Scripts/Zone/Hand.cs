using System;
using System.Collections.Generic;
using Card;
using config;
using Core;
using UnityEngine;
using Utils;
using static Utils.Option;
using static Utils.Result;

namespace Zone {
    struct CardTransform {
        public Vector3 Position;
        public Vector3 Rotation;
    
        public CardTransform(Vector3 position, Vector3 rotation) {
            Position = position;
            Rotation = rotation;
        }
    }
    // Hand 에 있는 카드들에 대한 처리를 담당하는 클래스.
    // MouseHover 이벤트가 발생하면 hide_cards() 
    // MouseUnHover 이벤트가 발생하면 show_cards()
    public class Hand : Zone {
        public float show_vertical_displacement;
        public float hide_vertical_displacement;
        public int card_rotation = 15;
        public float card_width = 2;

        private const int BASE_QUEUE = 3000; // Transparent 큐 시작점
        private int selected_card_idx;
        private float drop_threshold = 1f; // 카드 사이 판정 거리
        private bool is_hover = true;
        private bool is_drag = false;
        private float lastEventTime = 0f;
        private readonly float eventCooldown = 0.001f; // 100ms 쿨다운
        private Dictionary<int, CardTransform[]> cardPositionsCache;

        // hover 상태일 때, 룬테라처럼 y 값에 가중치를 주고.
        // scale 을 키워서 focusing 을 할 예정임.
        // Hand 에서 public 변수로 다루기 보다는 따로 config 로 빼내는게 깔끔 할지도 모름.
        public ZoomConfig zoom_config;

        // Event 는 UnityEvent 타입을 가지는데, 이 타입의 경우 SerializeField 필드를 가지고 있을 때 
        // 유니티 시스템에서 자동으로 초기화를 해줌.
        // 그렇기 때문에 SerializeField 속성은 필수임.
        [SerializeField]
        private EventsConfig events_config;
        
        // 카드를 한 번 드래그 하면 y 값이 달라지는 문제 있음.
        // 일단 여기저기 SetTransforms 함수를 남발해서 결과물을 얻었고 성능 저하도 별로 없지만
        // 뭔가 마음에 들진 않네..
        
        private void Awake() {
            cardPositionsCache = new Dictionary<int, CardTransform[]>();
            pre_calculate_all_card_positions();
        }
        
        private void Start() {
            zone_type = ZoneType.Hand;
            
            GameObject holder = new GameObject("CardHolder") {
                tag = "holder"
            };
    
            holder.transform.SetParent(transform);
            holder.transform.localPosition = Vector3.zero;
            
            hide_cards();
        }
        
        private void UpdateRenderQueue() {
            for (int i = 0; i < cards.Count; i++) {
                MeshRenderer meshRenderer = cards[i].GetComponent<MeshRenderer>();
                meshRenderer.material.renderQueue = BASE_QUEUE + i;
            
                cards[i].transform.SetLocalPosition(y: i * 0.001f);
                // print(cards[i].name);
            }
        }
        
        public void EndCardDrag() {
            is_drag = false;
            SetTransforms();
        }
    
        public void BeginCardDrag(Card.Card card) {
            is_drag = true;
            selected_card_idx = cards.IndexOf(card);
        }
    
        public void OnCardDrag(Card.Card card) {
            if (find_insertion_index(card.transform.position, selected_card_idx).TryGet(out int newIndex)) {
                reorder_cards(selected_card_idx, newIndex);
                selected_card_idx = newIndex;
            }
            else {
                // print("Error: find_insertion_index()");
            }
        }
    
        private Option<int> find_insertion_index(Vector3 position, int draggingCardIndex) {
            if (cards.Count <= 1) return Option<int>.None();  // 카드가 없거나 1장뿐이면 None

            float minDistance = float.MaxValue;
            int nearestIndex = -1;

            // 모든 카드와의 거리를 검사
            // 가지 치기를 이용해서 조금 더 최적화 할 수 있긴함.
            for (int i = 0; i < cards.Count; i++) {
                if (i == draggingCardIndex) continue;

                float distance = Vector3.Distance(position, cards[i].transform.position);
        
                if (distance < minDistance && distance < drop_threshold) {
                    minDistance = distance;
                    nearestIndex = i;
                }
            }
            
            return nearestIndex != -1 ? 
                Option<int>.Some(nearestIndex) : 
                Option<int>.None();
        }
    
        private void reorder_cards(int oldIndex, int newIndex) {
            var card = cards[oldIndex];
            cards.RemoveAt(oldIndex);
            cards.Insert(newIndex, card);
            
            SetTransforms();
        }


        private void pre_calculate_all_card_positions() {
            for (int cardCount = 0; cardCount < Constant.HAND_CARD_MAX_SIZE; cardCount++) {
                cardPositionsCache[cardCount] = CalculateTransforms(cardCount);
            }
        }

        private CardTransform[] CalculateTransforms(int cardCount) {
            var transforms = new CardTransform[cardCount];

            float total_card_width = card_width * cardCount;
            float start_x = total_card_width / 2 * -1;
            
            for (int i = 0; i < cardCount; i++) {
                float x_pos = start_x;
                float z_pos = GetCardVerticalDisplacement(i, cardCount);
                Vector3 position = new Vector3(x_pos, 0, z_pos);
                
                float yRotation = GetCardRotation(i, cardCount);
                Vector3 rotation = new Vector3(0, yRotation, 0);
                
                transforms[i] = new CardTransform(position, rotation);
                
                start_x += card_width;
            }
            
            return transforms;
        }
        
        private void SetTransforms() {
            if (cards.Count == 0) return;

            var cardTransforms = cardPositionsCache[cards.Count];
            float verticalOffset = is_hover ? show_vertical_displacement : hide_vertical_displacement;
            
            for (int i = 0; i < cards.Count; i++) {
                var targetPosition = cardTransforms[i].Position;
                // 숨김/보임 상태에 따라 수직 오프셋 조정
                targetPosition.z = targetPosition.z * (verticalOffset / show_vertical_displacement);
            
                cards[i].transform.SetLocalPosition(x: targetPosition.x, y: 0f, z: targetPosition.z);
                cards[i].transform.rotation = Quaternion.Euler(cardTransforms[i].Rotation);
            }
            UpdateRenderQueue();
        }

        public void show_cards() {
            if (Time.time - lastEventTime < eventCooldown || is_drag || is_hover) return;
            
            lastEventTime = Time.time;
            // SetTransforms() 은 is_hover 에 의존적이기 때문에
            // 호출 순서를 잘 지켜야함. 
            is_hover = true;
            SetTransforms();
        }

        public void hide_cards(bool flag = false) {
            if (!is_hover && !flag || is_drag) return;

            is_hover = false;
            SetTransforms();
        }
        
        private float GetCardVerticalDisplacement(int index, int cardCount) {
            if (cardCount < 3) return 0;
            float normalizedIndex = (index - (cardCount - 1) / 2f) / ((cardCount - 1) / 2f);
            return (1 - normalizedIndex * normalizedIndex) * show_vertical_displacement;
        }
        
        private float GetCardRotation(int index, int cardCount) {
            if (cardCount < 3) return 0;
            float normalizedIndex = (index - (cardCount - 1) / 2f) / ((cardCount - 1) / 2f);
            return card_rotation * normalizedIndex;
        }

        // 외부에서 카드를 가져올 때, 원하는 자리에 넣지 못하고 마지막 자리에 넣게됨.
        // 외부에서 카드를 가져왔을 때, OnMouseExit 이 제대로 호출되지 않아서 color 가 그대로 남는 문제 있음.
        public override Result<Unit, GameError> add_card(Card.Card comp, AddCardOptions options = null) {
            // ExceededCardLimit 에러 처리해야함.
            if (cards.Count + 1 >= Constant.HAND_CARD_MAX_SIZE) {
                return Err(GameError.ExceededCardLimit);
            }
            var holder = GameObject.FindWithTag("holder").transform;
            
            comp.GetComponent<Card.Card>().zoom_config = zoom_config;
            comp.GetComponent<Card.Card>().event_config = events_config;
            
            comp.current_zone_type = ZoneType.Hand;

            comp.transform.SetParent(holder);
            comp.transform.position = holder.position;
            comp.transform.localPosition = Vector3.zero;
            comp.transform.localScale = new Vector3(0.64f, 1f, 0.62f);
            
            cards.Add(comp);

            return Ok();
        }

        public override Result<Unit, GameError> remove_card(Card.Card card) {
            if (!cards.Contains(card)) return Err(GameError.UnkownFailed);
            
            cards.Remove(card);
            SetTransforms();
            return Ok();
        }

        public override Result<Unit, GameError> move_card(Card.Card card, ZoneType target_zone, AddCardOptions options = null) {
            switch (target_zone) {
                case ZoneType.Hand:
                    return Err(GameError.UnkownFailed);
                case ZoneType.Deck:
                    return to_deck(card);
                case ZoneType.Graveyard:
                    return to_graveyard(card);
                case ZoneType.Field:
                    return to_field(card);
                case ZoneType.None:
                    return Err(GameError.UnkownFailed);
                default:
                    return Err(GameError.UnkownFailed);
            }
        }

        /// <summary>
        /// Hand.move_card 에서 호출하는 하위 함수
        /// </summary>
        /// <param name="card">Card 컴포넌트</param>
        /// <param name="options">Slot Id 등 옵션 정보</param>
        /// <returns></returns>
        private Result<Unit, GameError> to_field(Card.Card card, AddCardOptions options = null) {
            if (options?.SlotId == null) return Err(GameError.UnkownFailed);
            
            var result = remove_card(card);
            if (result.IsErr) {
                // TODO: 에러 만들어야함.
                return Err(GameError.UnkownFailed);
            }

            return 
                GameObject.FindGameObjectWithTag("Field").TryGetComponent<Field>(out var field) 
                ? 
                // if
                field.add_card(card, options) :
                // else
                // TODO: 에러 만들어야함.
                Err(GameError.UnkownFailed);
        }
        
        private Result<Unit, GameError> to_graveyard(Card.Card card, AddCardOptions options = null) {
            return Ok();
        }

        private Result<Unit, GameError> to_deck(Card.Card card, AddCardOptions options = null) {
            return Ok();
        }
    }
}