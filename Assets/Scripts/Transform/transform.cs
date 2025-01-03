using UnityEngine;

namespace Transform {
    public class TransformData
    {
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;

        public TransformData(Vector3 position, Vector3 scale, Quaternion rotation)
        {
            this.position = position;
            this.scale = scale;
            this.rotation = rotation;
        }
        
        public TransformData(UnityEngine.Transform data, Vector3 to)
        {
            position = to;
            scale = data.localScale;
            rotation = data.rotation;
        }
        
        public TransformData(UnityEngine.Transform data, Vector3 to, Quaternion rot)
        {
            position = to;
            scale = data.localScale;
            rotation = rot;
        }
        public TransformData Clone()
        {
            return new TransformData(
                new Vector3(position.x, position.y, position.z),
                new Vector3(scale.x, scale.y, scale.z),
                new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w)
            );
        }
    }
    
    
    public static class TransformDataExtensions 
    {
        public static void AddPosition(this TransformData data, float? x = null, float? y = null, float? z = null) {
            data.position.x += x ?? 0;
            data.position.y += y ?? 0;
            data.position.z += z ?? 0;
        }
        public static void AddScale(this TransformData data, float? x = null, float? y = null, float? z = null) {
            data.scale.x += x ?? 0;
            data.scale.y += y ?? 0;
            data.scale.z += z ?? 0;
        }
    }
}