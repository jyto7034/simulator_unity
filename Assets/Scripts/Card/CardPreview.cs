using System.Collections.Generic;
using config;
using events;
using UnityEngine;

namespace Card {
    public class CardPreview : MonoBehaviour {
        
        [SerializeField]
        private float verticalPosition;
        
        [SerializeField]
        private float previewScale = 1f;
        
        [SerializeField]
        private int previewSortingOrder = 1;
        
        private Dictionary<Card, UnityEngine.Transform> previews = new();
        
        public void OnCardHover(CardHover cardHover) {
            print("hover");
            // OnCardPreviewStarted(cardHover.card);
        }
        
        public void OnCardUnhover(CardUnhover cardUnhover) {
            print("unhover");
            // OnCardPreviewEnded(cardUnhover.card);
        }

        public void OnCardPreviewStarted(Card card) {
            if (!previews.ContainsKey(card)) {
                CreateCloneForCard(card);
            }

            var preview = previews[card];
            preview.gameObject.SetActive(true);
            preview.position = new Vector3(card.transform.position.x, verticalPosition, card.transform.position.z);
        }

        private void CreateCloneForCard(Card card) {
            var clone = Instantiate(card.gameObject, transform);
            clone.transform.position = card.transform.position;
            clone.transform.localScale = Vector3.one * previewScale;
            clone.transform.rotation = Quaternion.identity;
            StripCloneComponents(clone);
            previews.Add(card, clone.transform);
        }

        private static void StripCloneComponents(GameObject clone) {
            var cloneWrapper = clone.GetComponent<Card>();
            if (cloneWrapper != null) {
                Destroy(cloneWrapper);
            }
        }

        public void OnCardPreviewEnded(Card card) {
            previews[card].gameObject.SetActive(false);
        }
    }
}