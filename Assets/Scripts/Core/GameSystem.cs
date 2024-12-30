using Animation;
using UnityEngine;

namespace Core {
    public class GameSystem : MonoBehaviour {
        private static GameSystem _instance;
        
        public static GameSystem Instance {
            get {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<GameSystem>();

                if (_instance != null) return _instance;
                var singleton = new GameObject(typeof(GameSystem).ToString());
                _instance = singleton.AddComponent<GameSystem>();
                return _instance;
            }
        }

        public CardAnimation card_animation;
        
        void Awake() {
            if (_instance == null) {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }

            // 자주 사용하는 컴포넌트 캐싱
            card_animation = gameObject.AddComponent<CardAnimation>();
        }
    }
}