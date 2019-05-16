using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;

namespace TecWolf.Monster
{
    public class MonsterFirst : MonoBehaviour
    {
        [SerializeField]
        public AbstractMap Map;

        [SerializeField]
        private Vector2d Location;
        private GameObject Player;

        private bool Spawned;

        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Update()
        {
            if (FirebaseController.SignedIn && !Spawned)
            {
                Location = new Vector2d(TecWolf.Player.PlayerController.NewLatitude + 0.0001f, TecWolf.Player.PlayerController.NewLongitude + 0.0001f);
                // GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

                transform.localPosition = Map.GeoToWorldPosition(Location, true);
                transform.localPosition = new Vector3(transform.localPosition.x, 1.25f, transform.localPosition.z);

                // Spawned = true;
            }

            if (Vector3.Distance(Player.transform.position, transform.position) > 12.5f || TecWolf.Player.PlayerMission.InMission || (TecWolf.Player.PlayerMission.Level > GetComponent<MonsterMission>().MonsterID && TecWolf.Player.PlayerMission.Level < 10))
            {
                // gameObject.SetActive(false);
            }
        }
    }
}