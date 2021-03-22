using UnityEngine;

namespace ScriptableObjects
{
    public abstract class AudioEvent : ScriptableObject
    {
        public abstract void Play(AudioSource source);

        public static void InstantiateEvent(AudioEvent audioEvent, Vector3 position, float delay = 10f)
        {
            AudioSource source = new GameObject().AddComponent<AudioSource>();
            source.transform.position = position;
            audioEvent.Play(source);
            source.gameObject.AddComponent<Despawner>().delay = delay;
        } 
    }
}