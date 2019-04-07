using UnityEngine;

namespace TecWolf.System
{
    public class SystemBuilding : MonoBehaviour
    {
        private void Update()
        {
            SetBuilding();
        }

        public void SetBuilding()
        {
            GetComponent<MeshRenderer>().enabled = TecWolf.Settings.SettingsInterface.Buildings;
            GetComponent<MeshCollider>().enabled = TecWolf.Settings.SettingsInterface.Buildings;
        }
    }
}
