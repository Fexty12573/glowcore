using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Tool", menuName = "Scriptable Objects/Tool")]
    public class Tool : Item
    {
        public AudioManager.SoundType Sound;
    }
}