using UnityEngine;

namespace TecWolf.Monster
{
    public class MonsterSpawn : MonoBehaviour
    {
        public float SpawnRadius = 5f;
        public GameObject[] Monsters;

        public int MinTime = 15, MaxTime = 60;

        private GameObject Player;
        private int MonsterLimit, MonsterTimer;

        private int LastMonster;

        void Start()
        {
            MonsterLimit = Random.Range(0, 2);

            Player = GameObject.FindGameObjectWithTag("Player");
        }

        void Update()
        {
            if (transform.childCount < MonsterLimit && Vector3.Distance(Player.transform.position, transform.position) < 10f)
            {
                SpawnMonster();
            }

            foreach (Transform Go in transform)
            {
                if (Vector3.Distance(Player.transform.position, Go.position) < 7.5f)
                {
                    Go.gameObject.SetActive(true);
                }
                else
                {
                    Go.gameObject.SetActive(false);
                }

                if (TecWolf.Player.PlayerMission.InMission || LastMonster != TecWolf.Player.PlayerMission.Level && Go != null)
                {
                    Go.gameObject.SetActive(false);
                }
            }
        }

        public void SpawnMonster()
        {
            float DirectionFacing = Random.Range(0f, 360f);

            Vector3 Point = (Random.insideUnitSphere * SpawnRadius) + transform.position;

            int MonsterType = 0; // Random.Range(0, Monsters.Length);

            if (TecWolf.Player.PlayerMission.Level >= 0 && Monsters.Length == TecWolf.Player.PlayerMission.Level)
            {
                MonsterType = TecWolf.Player.PlayerMission.Level;
            }

            int MonsterQuantity = Monsters.Length - 1;

            if (MonsterQuantity == TecWolf.Player.PlayerMission.Level)
            {
                GameObject Monster = Instantiate(Monsters[MonsterType], Point, Quaternion.Euler(new Vector3(0f, DirectionFacing, 0f)));

                LastMonster = MonsterType;

                // Monster.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

                Monster.transform.position = new Vector3(Monster.transform.position.x, 1f, Monster.transform.position.z);
                Monster.transform.parent = transform;

                MonsterTimer = Random.Range(MinTime, MaxTime);
                Destroy(Monster, MonsterTimer);
            }
        }
    }
}
