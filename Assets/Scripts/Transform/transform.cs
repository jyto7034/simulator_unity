using UnityEngine;

namespace Transform {
    public struct TransformData
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
    }
}