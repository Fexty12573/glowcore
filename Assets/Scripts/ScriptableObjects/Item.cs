using System;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
    public class Item : ScriptableObject, ISerializationCallbackReceiver
    {
        public string Name;
        [TextArea(1, 3)] public string Description;
        public GameObject Prefab;
        public Sprite Icon;
        [Tooltip("Euler rotation (degrees) applied to the prefab only when generating the icon. Use it when the prefab's world orientation lays the mesh flat.")]
        public Vector3 IconRotation;
        public float DropScale = 1f;
        public float InHandScale = 1f;
        [Min(0)] public int MaxStack = 99;
        [SerializeField] private byte[] m_idBytes;
        public Guid Id { get; private set; }

        public void OnBeforeSerialize()
        {
            // Transform Id to array of bytes for serialization
            m_idBytes = Id.ToByteArray();
        }

        public void OnAfterDeserialize()
        {
            // Transform array of bytes back to Guid after deserialization
            if (m_idBytes != null && m_idBytes.Length == 16)
                Id = new Guid(m_idBytes);
            else
                Id = Guid.Empty;
        }

#if UNITY_EDITOR
        public void RegenerateId() => Id = Guid.NewGuid();
#endif
    }
}

