using System;
using System.Collections.Generic;
using Transform;
using UnityEngine;

// Card 애니메이션 수행 시, 사용되는 위치값을 모아둔 class
public class Constant : MonoBehaviour {
    public static readonly float card_size_while_movement = 1f;
    public static readonly float card_size_before_place_to_field = 1.2f;
    public static readonly float card_size_after_place_to_field = 0.3f;
    public static readonly float card_y_position_while_drag = 10f;
}