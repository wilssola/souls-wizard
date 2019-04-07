using UnityEngine;

namespace TecWolf.Quest
{
    public class QuestTarget : MonoBehaviour
    {
        void Start()
        {
            Spawn();
        }

        public void Spawn()
        {
            Vector3 Point = (Random.insideUnitSphere * 2.5f) + transform.position;

            transform.position = Point;
        }
    }
}
