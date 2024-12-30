using System;
using System.Collections.Generic;
using UnityEngine;
using Zone;
using Object = UnityEngine.Object;

namespace Utils {
    public static class Utils {
        private static readonly Dictionary<ZoneType, Zone.Zone> zoneCache = new();

        public static void transfer_to_zone(Card.Card card, ZoneType target_zone) {
            get_zone(card.current_zone).remove_card(card);
            get_zone(target_zone).add_card(card);
        }
        
        public static Zone.Zone get_zone(ZoneType type) {
            // 캐시에 있으면 반환
            if (zoneCache.TryGetValue(type, out var zone1))
                return zone1;
            
            // 캐시에 없으면 찾아서 저장
            Zone.Zone zone = type switch {
                ZoneType.Hand => Object.FindObjectOfType<Hand>(),
                ZoneType.Deck => Object.FindObjectOfType<Deck>(),
                ZoneType.Graveyard => Object.FindObjectOfType<Graveyard>(),
                ZoneType.Field => Object.FindObjectOfType<Field>(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            if (zone != null)
                zoneCache[type] = zone;
            else
                Debug.LogWarning($"Zone of type {type} not found!");
            
            return zone;
        }
    }
    
    public static class DebugExtensions
    {
        // 여러 객체를 받아서 공백으로 구분하여 출력
        public static void log(params object[] objects)
        {
            Debug.Log(string.Join(" ", objects));
        }

        // 구분자를 지정하여 출력
        public static void log_with_separator(string separator, params object[] objects)
        {
            Debug.Log(string.Join(separator, objects));
        }

        // 레이블과 함께 출력
        public static void log_with_label(string label, params object[] objects)
        {
            Debug.Log($"{label}: {string.Join(" ", objects)}");
        }

        // 컬러 지원
        public static void log_with_color(Color color, params object[] objects)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{string.Join(" ", objects)}</color>");
        }

        // 조건부 로깅
        public static void log_with_if(bool condition, params object[] objects)
        {
            if (condition)
                Debug.Log(string.Join(" ", objects));
        }

        // 포맷 지정 로깅
        public static void log_with_format(string format, params object[] args)
        {
            Debug.Log(string.Format(format, args));
        }
    }
    
}

public static class TransformExtensions
{
    public static void AddPosition(this UnityEngine.Transform transform, float? x = null, float? y = null, float? z = null)
    {
        var currentPos = transform.position;
        transform.position = new Vector3(
            currentPos.x + (x ?? 0),
            currentPos.y + (y ?? 0),
            currentPos.z + (z ?? 0)
        );
    }
    
    public static void AddLocalPosition(this UnityEngine.Transform transform, float? x = null, float? y = null, float? z = null)
    {
        var currentPos = transform.localPosition;
        transform.localPosition = new Vector3(
            currentPos.x + (x ?? 0),
            currentPos.y + (y ?? 0),
            currentPos.z + (z ?? 0)
        );
    }
    
    public static void SetPosition(this UnityEngine.Transform transform, float? x = null, float? y = null, float? z = null)
    {
        transform.position = transform.position.With(x, y, z);
    }
    public static void SetLocalPosition(this UnityEngine.Transform transform, float? x = null, float? y = null, float? z = null)
    {
        var currentPos = transform.localPosition;
        transform.localPosition = new Vector3(
            x ?? currentPos.x,
            y ?? currentPos.y,
            z ?? currentPos.z
        );
    }

    public static Vector3 AddPositionRet(this UnityEngine.Transform transform, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(
            transform.position.x + (x ?? 0),
            transform.position.y + (y ?? 0),
            transform.position.z + (z ?? 0)
        );
    }
    
    
    public static void WithLocalPosition(this UnityEngine.Transform transform, float? x = null, float? y = null, float? z = null)
    {
        transform.localPosition = transform.localPosition.With(x, y, z);
    }

    public static void WithRotation(this UnityEngine.Transform transform, float? x = null, float? y = null, float? z = null)
    {
        Vector3 currentRotation = transform.eulerAngles;
        transform.eulerAngles = currentRotation.With(x, y, z);
    }

    public static void WithScale(this UnityEngine.Transform transform, float? x = null, float? y = null, float? z = null)
    {
        transform.localScale = transform.localScale.With(x, y, z);
    }
}

public static class Vector3Extensions
{
    public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(
            x ?? original.x,
            y ?? original.y,
            z ?? original.z
        );
    }
    public static Vector3 WithMul(this Vector3 original, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(
            original.x * x ?? original.x,
            original.y * y ?? original.y,
            original.z * z ?? original.z
        );
    }
}

public class Option<T> {
    private readonly T _value;
    private readonly bool _hasValue;

    private Option(T value, bool hasValue) {
        _value = value;
        _hasValue = hasValue;
    }

    public static Option<T> Some(T value) {
        return new Option<T>(value, true);
    }

    public static Option<T> None() {
        return new Option<T>(default(T), false);
    }

    public bool HasValue => _hasValue;

    public T Value {
        get {
            if (!_hasValue) {
                throw new InvalidOperationException("No value present");
            }
            return _value;
        }
    }
}