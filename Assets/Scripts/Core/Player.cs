using System;
using UnityEngine;
using Zone;
using Utils;
using static Utils.Option;
using static Utils.Result;

namespace Core {
    public class Player : MonoBehaviour {
        public PlayerType player_type;
        public Player opponent;
        public Hand hand;
        public Field field;
        public Graveyard graveyard;
        public Deck deck;

        // 그러고 보니 각 Zone 은 Player 에 귀속되는데
        // 유니티 태그를 보면 Player_1_Field, Player_1_Hand 등 구분되어 있지 않고'
        // 그냥 Field, Hand 되어 있음.
        // 구분지어서 태그 재작성 해야함.        
        private void Start() {
            InitializeZone(ref hand, "Hand");
            InitializeZone(ref field, "Field");
            InitializeZone(ref graveyard, "Graveyard");
            InitializeZone(ref deck, "Deck");

            string player_type_str = player_type == PlayerType.Player1 ? "Player1" : "Player2";
            if (GameObject.FindGameObjectWithTag(player_type_str).TryGetComponent<Player>(out var _player)) {
                opponent = _player;
            }
        }
        
        private void InitializeZone<T>(ref T zoneComponent, string tag) where T : Zone.Zone {
            if (GameObject.FindGameObjectWithTag(tag).TryGetComponent<T>(out var component)) {
                zoneComponent = component;
            }
        }

        public Result<Unit, GameError> draw_card_from_deck() {
            return Ok();
        }
    }
}