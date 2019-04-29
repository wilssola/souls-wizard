using UnityEngine;
using UnityEngine.UI;

namespace TecWolf.Quest
{
    /// <summary>
    /// Sistema de Poinstops.
    /// </summary>
    public class QuestPointstop : MonoBehaviour
    {
        /// <value>
        /// A identificação do Pointstop.
        /// </value>
        public static string PointstopID = "0";

        /// <value>
        /// As lâmpadas do Pointstop.
        /// </value>
        public GameObject[] Halos;

        private bool Touched;
        private GameObject Player;

        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Update()
        {
            Halo();
            Move();

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Exit();
            }
#endif
        }

        private void OnMouseUpAsButton()
        {
            Open();
        }

        /// <summary>
        /// Ativar e desativar lâmpadas de acordo com a distância do Player ao Pointstop.
        /// </summary>
        public void Halo()
        {
            if (Vector3.Distance(Player.transform.position, transform.position) < 7.5f)
            {
                foreach (GameObject Go in Halos)
                {
                    Go.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject Go in Halos)
                {
                    Go.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Mover câmera para o Pointstop.
        /// </summary>
        public void Move()
        {
            if (Touched)
            {
                GameObject PlayerCamera = Camera.main.gameObject;

                PlayerCamera.transform.position = Vector3.Lerp(PlayerCamera.transform.position, transform.GetChild(1).transform.position, Time.deltaTime * 5.0f);
                PlayerCamera.transform.LookAt(transform.GetChild(0).transform);
            }
        }

        /// <summary>
        /// Entrar no Pointstop.
        /// Futuramente seria bom mudar para Raycast.
        /// </summary>
        public void Open()
        {
            if (FirebaseController.SignedIn && FirebaseController.CreatedIn)
            {
                if (TecWolf.Player.PlayerCamera.Active)
                {
                    Touched = true;
                    TecWolf.Player.PlayerCamera.Active = false;
                }
                TecWolf.Player.PlayerInterface.PointstopBar.SetActive(true);

                if (Vector3.Distance(Player.transform.position, transform.position) < 7.5f)
                {
                    TecWolf.Player.PlayerInterface.PointstopBar.GetComponentsInChildren<Button>()[0].onClick.AddListener(Photo);

                    TecWolf.Player.PlayerInterface.PointstopBar.GetComponentsInChildren<Button>()[0].enabled = true;
                    TecWolf.Player.PlayerInterface.PointstopBar.GetComponentsInChildren<Button>()[2].enabled = true;

                    TecWolf.Player.PlayerInterface.PointstopBar.GetComponentsInChildren<Image>()[1].color = Color.white;
                    TecWolf.Player.PlayerInterface.PointstopBar.GetComponentsInChildren<Image>()[3].color = Color.white;
                }
                else
                {
                    TecWolf.Player.PlayerInterface.PointstopBar.GetComponentsInChildren<Button>()[0].onClick.RemoveAllListeners();

                    TecWolf.Player.PlayerInterface.PointstopBar.GetComponentsInChildren<Button>()[0].enabled = false;
                    TecWolf.Player.PlayerInterface.PointstopBar.GetComponentsInChildren<Button>()[2].enabled = false;

                    TecWolf.Player.PlayerInterface.PointstopBar.GetComponentsInChildren<Image>()[1].color = Color.gray;
                    TecWolf.Player.PlayerInterface.PointstopBar.GetComponentsInChildren<Image>()[3].color = Color.gray;
                }
            }
            TecWolf.Player.PlayerInterface.PointstopBar.GetComponentsInChildren<Button>()[1].onClick.AddListener(Exit);
        }

        /// <summary>
        /// Sair do Pointstop.
        /// </summary>
        public void Exit()
        {
            TecWolf.Player.PlayerCamera.Active = true;
            Touched = false;

            TecWolf.Player.PlayerInterface.PointstopBar.SetActive(false);
        }

        /// <summary>
        /// Tirar foto do Pointstop.
        /// </summary>
        public void Photo()
        {
            PointstopID = transform.name;
            QuestPhoto.TakePictureMission(4096, "Nenhum", PointstopID);
        }

        public void OnTriggerEnter(Collider Other)
        {
            if (Other.gameObject.tag == "Building")
            {
                Destroy(gameObject);
            }
        }
    }
}
