using System;
using config;
using DG.Tweening;
using events;
using Transform;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Card { 
    public class Card : MonoBehaviour{
        public ZoneType current_zone;
        
        [HideInInspector] 
        public Guid uuid;
        
        private bool is_hover = false;
        private bool is_dragged = false;
        
        public EventsConfig event_config;
        public ZoomConfig zoom_config;
        
        private Vector3 mousePosition;
        private Vector3 lastPosition;
        private const float maxTilt = 10f; // 최대 기울기 값
        
        private void Start() {
            lastPosition = transform.position;
        }
        
        public void OnMouseEnter() {
            if (is_dragged) {
                return;
            }
            is_hover = true;
            GetComponent<MeshRenderer>().material.color = Color.yellow; 
            event_config?.OnCardHover?.Invoke(new CardHover(this));
        }

        public void OnMouseExit() {
            if (is_dragged) {
                return;
            }
            is_hover = false;
            // TODO: 이렇게 new CardUnhover(this) 를 매 이벤트마다 호출하는건 성능 저하를 유발할 수 있음.
            // 사실 그렇게 최적화 해야 할 사안은 아닌데, 숙지만 해두고
            // 이벤트 유발 방식에 대해서 좀 검토해야 할 필요가 있어보임.
            // 근데 일단, 이벤트의 근간이 되는 객체가 유일하지 않다는 점이 문제로 이루어질 수 있긴함.
            GetComponent<MeshRenderer>().material.color = Color.white;
            event_config?.OnCardUnhover?.Invoke(new CardUnhover(this));
        }
        
        private Vector3 GetMousePos(Vector3 pos) {
            return Camera.main!.WorldToScreenPoint(pos);
        }

        // 이벤트 발생 순서도 신경써야 할 수도
        private void OnMouseDown() {
            event_config?.OnCardUnhover?.Invoke(new CardUnhover(this));
            event_config?.OnCardDragBegin?.Invoke(new CardDragBegin(this));
            is_dragged = true;
            var ray = Camera.main!.ScreenPointToRay(Input.mousePosition);
            var result = new RaycastHit[10];
            var size = Physics.RaycastNonAlloc(ray, result);
            
            for (var i = 0; i < size; i++) {
                var _object = result[i];
            
                if (!_object.collider.CompareTag("Card")) continue;
                
                var anim = new Animation.Animation();
                var tfd = new TransformData(_object.transform.position, _object.transform.localScale * Constant.card_size_while_movement,
                    _object.transform.rotation);
                anim.MoveTo(_object.transform.GetComponent<Card>(), tfd, 0.1f);
                break;
            }
            
            var screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                GetMousePos(transform.position).z);
            var clickPosition = Camera.main!.ScreenToWorldPoint(screenPoint);
            
            var target_position = new Vector3(clickPosition.x, transform.position.y, clickPosition.z);
            mousePosition = Input.mousePosition - GetMousePos(target_position);
        }

        private void OnMouseUp() {
            if (!is_dragged) return;

            is_dragged = false;
            event_config?.OnCardDragEnd?.Invoke(new CardDragEnd(this));
            event_config?.OnCardPlayed.Invoke(new CardPlayed(this));
        }

        private void OnMouseDrag() {
            var target_position = Camera.main!.ScreenToWorldPoint(Input.mousePosition - mousePosition);
            
            var movement = target_position - lastPosition;
            var movementMagnitude = movement.magnitude;

            var tiltFactor = Mathf.Clamp01(movementMagnitude / 0.1f);

            var tiltX = Mathf.Clamp(-movement.z * maxTilt * tiltFactor, -maxTilt, maxTilt);
            var tiltZ = Mathf.Clamp(movement.x * maxTilt * tiltFactor, -maxTilt, maxTilt);

            transform.rotation = Quaternion.Euler(tiltX, 0, tiltZ);
            
            transform.SetPosition(y: Constant.card_y_position_while_drag);
            
            lastPosition = transform.position;
            
            transform.position = Vector3.Lerp(transform.position, target_position, Time.deltaTime * 15f);
            event_config?.OnCardDrag.Invoke(new CardDrag(this));
        }
    }
}