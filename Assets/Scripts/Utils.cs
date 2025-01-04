using System;
using System.Collections.Generic;
using UnityEngine;
using Zone;
using Object = UnityEngine.Object;



namespace Utils {
    public static class Option {
        public static Option<T> Some<T>(T value) => Option<T>.Some(value);
        public static Option<T> None<T>() => Option<T>.None();
    }
    public abstract class Option<T> {
        private Option() {}

        public sealed class _Some : Option<T> {
            public T Value { get; }
            
            public _Some(T value) {
                Value = value;
            }
        }

        public sealed class _None : Option<T> {
            public static _None Instance { get; } = new _None();
            private _None() {}
        }

        public static Option<T> Some(T value) => new _Some(value);
        public static Option<T> None() => _None.Instance;
        
        public TResult Match<TResult>(
            Func<T, TResult> some,
            Func<TResult> none
        ) => this switch {
            _Some s => some(s.Value),
            _None => none(),
            _ => throw new InvalidOperationException()
        };
        
        public bool TryGet(out T value) {
            switch (this) {
                case _Some some:
                    value = some.Value;
                    return true;
                default:
                    value = default;
                    return false;
            }
        }
    }
    
    public struct Unit
    {
        public static Unit Value => default;
    }
    
    public static class Result {
        // 기존 메서드들
        public static Result<T, E> Ok<T, E>(T value) => Result<T, E>.Ok(value);
        public static Result<T, E> Err<T, E>(E error) => Result<T, E>.Err(error);
    
        // Unit을 위한 편의 메서드들
        public static Result<Unit, E> Ok<E>() => Result<Unit, E>.Ok(Unit.Value);
        public static Result<T, Unit> Err<T>() => Result<T, Unit>.Err(Unit.Value);

        // public static Result<Unit, Unit> Ok() => Result<Unit, Unit>.Ok(Unit.Value);
        // public static Result<Unit, Unit> Err() => Result<Unit, Unit>.Err(Unit.Value);
        
        public static Result<Unit, GameError> Ok() => Result<Unit, GameError>.Ok(Unit.Value);
        public static Result<Unit, GameError> Err(GameError error) => Result<Unit, GameError>.Err(error);
    }

    public abstract class Result<T, E> {
        private Result() {}

        public sealed class _Ok : Result<T, E> {
            public T Value { get; }
        
            public _Ok(T value) {
                Value = value;
            }
        }

        public sealed class _Err : Result<T, E> {
            public E Error { get; }
        
            public _Err(E error) {
                Error = error;
            }
        }

        public static Result<T, E> Ok(T value) => new _Ok(value);
        public static Result<T, E> Err(E error) => new _Err(error);
    
        public TResult Match<TResult>(
            Func<T, TResult> ok,
            Func<E, TResult> err
        ) => this switch {
            _Ok s => ok(s.Value),
            _Err e => err(e.Error),
            _ => throw new InvalidOperationException()
        };
    
        public bool IsOk => this is _Ok;
        public bool IsErr => this is _Err;
    
        public bool TryGetOk(out T value) {
            if (this is _Ok ok) {
                value = ok.Value;
                return true;
            }
            value = default;
            return false;
        }
    
        public bool TryGetErr(out E error) {
            if (this is _Err err) {
                error = err.Error;
                return true;
            }
            error = default;
            return false;
        }
    }
    
    public static class Utils {
        private static readonly Dictionary<ZoneType, Zone.Zone> zoneCache = new();

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

