using UnityEngine;

namespace events {
    public class CardDestroy : MonoBehaviour {
        public void OnCardDestroyed(CardPlayed _event) {
            Debug.Log("destroy");
        }
    }
}
