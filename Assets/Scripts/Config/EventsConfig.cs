using System;
using events;
using UnityEngine;
using UnityEngine.Events;

namespace config {
    [Serializable]
    public class EventsConfig {
        public UnityEvent<CardPlayed> OnCardPlayed;
        
        public UnityEvent<CardDragBegin> OnCardDragBegin;
        
        public UnityEvent<CardDrag> OnCardDrag;
        
        public UnityEvent<CardDragEnd> OnCardDragEnd;
        
        public UnityEvent<CardHover> OnCardHover;
        
        public UnityEvent<CardUnhover> OnCardUnhover;
        
        public UnityEvent<CardDestroy> OnCardDestroy;
    }
}
