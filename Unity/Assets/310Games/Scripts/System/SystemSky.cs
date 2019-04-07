using System;
using UnityEngine;

namespace TecWolf.System
{
    public class SystemSky : MonoBehaviour
    {
        public GameObject Sun, Sky;

        public Material Limit, Beacon;
        public float SkyRotation = 0.5f;

        public Color DayColor, NightColor;
        public float DayDensity, NightDensity;

        private float SkyActualRotation;
        private float Hour, Minute, FullTime;

        public Color LampDayColor, LampNightColor;

        private void Update()
        {
            Hour = DateTime.Now.Hour;
            Minute = DateTime.Now.Minute;

            FullTime = Hour + (((Minute * 100) / 60) / 100);
            Sun.transform.eulerAngles = new Vector3(((FullTime * 360f) / 24f) - 90f, 0, 0);

            if (Sun.transform.eulerAngles.x > 0f && Sun.transform.eulerAngles.x <= 180f)
            {
                Sky.transform.GetChild(0).gameObject.SetActive(false);
                Sky.transform.GetChild(1).gameObject.SetActive(true);

                Limit.SetColor("_Color", DayColor);
                Beacon.SetColor("_Color", LampDayColor);

                RenderSettings.fogColor = DayColor;
                RenderSettings.fogDensity = DayDensity;
            }
            else if (Sun.transform.eulerAngles.x > 180f && Sun.transform.eulerAngles.x <= 360f)
            {
                Sky.transform.GetChild(0).gameObject.SetActive(true);
                Sky.transform.GetChild(1).gameObject.SetActive(false);

                Limit.SetColor("_Color", NightColor);
                Beacon.SetColor("_Color", LampNightColor);

                RenderSettings.fogColor = NightColor;
                RenderSettings.fogDensity = NightDensity;
            }

            SkyActualRotation += SkyRotation * Time.deltaTime;
            SkyActualRotation %= 360f;
            Sky.transform.eulerAngles = new Vector3(0, SkyActualRotation, 0);

            RenderSettings.skybox.SetFloat("_Rotation", SkyActualRotation);
        }

        private void OnApplicationQuit()
        {
            Limit.SetColor("_Color", DayColor);
            Beacon.SetColor("_Color", LampDayColor);

            RenderSettings.skybox.SetFloat("_Rotation", 0);
        }
    }
}
