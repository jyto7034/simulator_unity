using System;
using System.Collections.Generic;
using Card;
using config;
using UnityEngine;
using Utils;
using static Utils.Option;

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
        public const int MAX_CARDS = 10;

        private const int BASE_QUEUE = 3000; // Transparent 큐 시작점
        private int selected_card_idx;
        private float drop_threshold = 0.5f; // 카드 사이 판정 거리
        private bool is_hover = true;
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
        
        // 그리고 카드 위치 변경 기능 제대로 고쳐야함.
        
        private void Awake() {
            cardPositionsCache = new Dictionary<int, CardTransform[]>();
            pre_calculate_all_card_positions();
        }
        
        private void UpdateRenderQueue() {
            for (int i = 0; i < cards.Count; i++) {
                MeshRenderer meshRenderer = cards[i].GetComponent<MeshRenderer>();
                meshRenderer.material.renderQueue = BASE_QUEUE + i;
            
                cards[i].transform.SetLocalPosition(y: i * 0.001f);
            }
        }
        
        public void EndCardDrag() {
            SetTransforms(is_hover); // SetCardPosition 대신 기존의 SetTransforms 사용
        }
    
        public void BeginCardDrag(Card.Card card) {
            selected_card_idx = cards.IndexOf(card);
        }
    
        public void OnCardDrag(Card.Card card) {
            if (find_insertion_index(card.transform.position).TryGet(out int newIndex)) {
                if (newIndex != selected_card_idx) {
                    Debug.Log(newIndex);
                    Debug.Log(selected_card_idx);
                    // reorder_cards(selected_card_idx, newIndex);
                    // selected_card_idx = newIndex;
                }
            }
            else {
                print("Error: find_insertion_index()");
            }
        }
    
        private Option<int> find_insertion_index(Vector3 position) {
            for (var i = 0; i < cards.Count; i++) {
                if (Vector3.Distance(position, cards[i].transform.position) < drop_threshold) {
                    return Some(i);
                }
            }
            return None<int>();
        }
    
        private void reorder_cards(int oldIndex, int newIndex) {
            var card = cards[oldIndex];
            cards.RemoveAt(oldIndex);
            cards.Insert(newIndex, card);
        
            // 카드 순서가 변경되었으므로 렌더 큐 업데이트
            UpdateRenderQueue();
        }


        private void pre_calculate_all_card_positions() {
            for (int cardCount = 0; cardCount < MAX_CARDS; cardCount++) {
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
        
        private void SetTransforms(bool isShow) {
            if (cards.Count == 0) return;

            var cardTransforms = cardPositionsCache[cards.Count];
            float verticalOffset = isShow ? show_vertical_displacement : hide_vertical_displacement;
            
            for (int i = 0; i < cards.Count; i++) {
                var targetPosition = cardTransforms[i].Position;
                // 숨김/보임 상태에 따라 수직 오프셋 조정
                targetPosition.z = targetPosition.z * (verticalOffset / show_vertical_displacement);
            
                cards[i].transform.SetLocalPosition(x: targetPosition.x, z: targetPosition.z);
                cards[i].transform.rotation = Quaternion.Euler(cardTransforms[i].Rotation);
            }
        }
        private void Start() {
            GameObject holder = new GameObject("CardHolder") {
                tag = "holder"
            };
    
            holder.transform.SetParent(transform);
            holder.transform.localPosition = Vector3.zero;
            
            InitializeCards();
            hide_cards();
        }

        public void show_cards() {
            if (Time.time - lastEventTime < eventCooldown) return;
        
            lastEventTime = Time.time;
            if (!is_hover) {
                SetTransforms(true);
                is_hover = true;
            }
        }

        public void hide_cards(bool flag = false) {
            if (is_hover || flag) {
                SetTransforms(false);
                is_hover = false;
            }
        }
        
        private void InitializeCards() {
            var gs = CardManager.player_cards;
            foreach (var item in gs) {
                item.SetActive(true);
                add_card(item.GetComponent<Card.Card>());
            }
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

        public override void add_card(Card.Card comp, int slot_id = 0) {
            var holder = GameObject.FindWithTag("holder").transform;
            comp.GetComponent<Card.Card>().zoom_config = zoom_config;
            comp.GetComponent<Card.Card>().event_config = events_config;
            comp.transform.SetParent(holder);
            comp.transform.position = holder.position;
            comp.transform.localPosition = Vector3.zero;
            cards.Add(comp);
        
            // 카드가 추가되었으므로 렌더 큐 업데이트
            UpdateRenderQueue();
        }

        public override bool remove_card(Card.Card card) {
            if (cards.Contains(card)) {
                cards.Remove(card);
                // 카드가 제거되었으므로 렌더 큐 업데이트
                UpdateRenderQueue();
                return true;
            }
            return false;
        }

        public override void pull_card(Card.Card card, ZoneType zone_type) {
            // 카드 당기기 로직 구현
        }

    }
}