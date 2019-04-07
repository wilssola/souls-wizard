using UnityEngine;
using System;
using System.Collections;
using Mapbox.Utils;
using Mapbox.Unity.Map;

namespace TecWolf.Player
{
    public class PlayerController : MonoBehaviour
    {
        public float Latitude = 0f, Longitude = 0f;

        public float Format = 100000f;
        public static float NewLatitude = 0f, NewLongitude = 0f;
        private float OldLatitude = 0f, OldLongitude = 0f;

        public AbstractMap Map;

        public PlayerInterface Interface;
        public PlayerAnimator Animation;

        public GameObject PlayerModel, PlayerModelBuilding;

        public static bool PlayerModelActive = true;
        public static DateTime Date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);

        public static float Speed, Acceleration, Distance;
        public float SpeedRotation = 5f, SpeedMovement = 10f;

        private float OverallDistance, LastDistance, Timer, LastTime, SpeedZero;
        private bool FirstTime, AllowTimer;

        // IEnumerator Coroutine;

        private void Awake()
        {
#if UNITY_EDITOR
            NewLatitude = Latitude;
            NewLongitude = Longitude;
#endif
        }

        IEnumerator Start()
        {
            // Coroutine = UpdateGPS();

            if (!Input.location.isEnabledByUser)
            {
                TecWolf.System.SystemInterface.Alert("Localização desativada, por favor ative o seu GPS.");
                yield break;
            }

            Input.location.Start();

            int MaxWait = 30;
            while (Input.location.status == LocationServiceStatus.Initializing && MaxWait > 0)
            {
                yield return new WaitForSeconds(1);
                MaxWait--;
            }

            if (MaxWait < 1)
            {
                TecWolf.System.SystemInterface.Alert("Localização expirada.");

                yield break;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                TecWolf.System.SystemInterface.Alert("Localização impossível de determinar.");

                yield break;
            }
            else
            {
                Latitude = Input.location.lastData.latitude;
                Longitude = Input.location.lastData.longitude;

                // Debug.Log("Latitude: " + Input.location.lastData.latitude + " - Longitude: " + Input.location.lastData.longitude + " - Altitude: " + Input.location.lastData.altitude + " - Horizonte: " + Input.location.lastData.horizontalAccuracy + " - Segundos: " + Input.location.lastData.timestamp);

                // StartCoroutine(Coroutine);
            }
        }

        IEnumerator UpdateGPS()
        {
            float TimeUpdate = 2.5f;

            while (true)
            {
                // Debug.Log("Latitude: " + Input.location.lastData.latitude + " - Longitude: " + Input.location.lastData.longitude + " - Altitude: " + Input.location.lastData.altitude + " - Horizonte: " + Input.location.lastData.horizontalAccuracy + " - Segundos: " + Input.location.lastData.timestamp);

                NewLatitude = Input.location.lastData.latitude;
                NewLongitude = Input.location.lastData.longitude;

                yield return new WaitForSeconds(TimeUpdate);
            }
        }

        void Update()
        {
#if UNITY_EDITOR
            NewLatitude += Input.GetAxis("Vertical") / Format;
            NewLongitude += Input.GetAxis("Horizontal") / Format;
#endif

#if !UNITY_EDITOR
        bool EnableGps = false;

        if (!Input.location.isEnabledByUser && !EnableGps)
        {
            EnableGps = true;
            StartCoroutine(Start());
        }

        if(Input.location.isEnabledByUser && EnableGps)
        {
            EnableGps = false;
        }
#endif

            Timer += Time.deltaTime;

            if (Input.location.status == LocationServiceStatus.Running)
            {
                NewLatitude = Input.location.lastData.latitude;
                NewLongitude = Input.location.lastData.longitude;
            }

            Latitude = Mathf.Lerp(Latitude, NewLatitude, Time.deltaTime * SpeedMovement);
            Longitude = Mathf.Lerp(Longitude, NewLongitude, Time.deltaTime * SpeedMovement);

            if (Latitude < 0 || Latitude > 0)
            {
                if (Latitude < OldLatitude)
                {
                    PlayerRotateByCoordinates(180f);
                }
                else if (Latitude > OldLatitude)
                {
                    PlayerRotateByCoordinates(360f);
                }
            }

            if (Longitude < 0 || Longitude > 0)
            {
                if (Longitude < OldLongitude)
                {
                    PlayerRotateByCoordinates(-90f);
                }
                else if (Longitude > OldLongitude)
                {
                    PlayerRotateByCoordinates(90f);
                }
            }

            if (OldLatitude != Latitude || OldLongitude != Longitude)
            {
                CalculateDistances(OldLatitude, OldLongitude, Latitude, Longitude);

                OldLatitude = Latitude;
                OldLongitude = Longitude;

                Animation.Moving = true;

                LastTime = Timer;
                Timer = 0;

                SpeedZero = Speed;

                CalculateSpeed();
                CalculateAcceleration();
            }
            else
            {
                Speed = 0;
                Acceleration = 0;

                Animation.Moving = false;
            }

            Map.UpdateMap(new Vector2d(Latitude, Longitude), 16);
        }

        private void OnTriggerStay(Collider Other)
        {
            if (Other.gameObject.tag == "Building")
            {
                PlayerModel.SetActive(false);
                PlayerModelBuilding.SetActive(true);

                PlayerModelActive = false;
            }
        }

        private void OnTriggerExit(Collider Other)
        {
            if (Other.gameObject.tag == "Building")
            {
                PlayerModel.SetActive(true);
                PlayerModelBuilding.SetActive(false);

                PlayerModelActive = true;
            }
        }

        private void PlayerRotateByCoordinates(float Direction)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, Direction, 0f), Time.deltaTime * SpeedRotation);
        }

        private double PlayerMovementLerpByCoordinates(double a, double b, float t)
        {
            return a + (b - a) * Mathf.Clamp01(t);
        }

        private float Radians(float x)
        {
            return x * Mathf.PI / 180;
        }

        public void CalculateDistances(float OldLatitude, float OldLongitude, float Latitude, float Longitude)
        {
            float DistanceLongitude = Radians(Longitude - OldLongitude);
            float DistanceLatitude = Radians(Latitude - OldLatitude);

            float DistanceTotal = Mathf.Pow(Mathf.Sin(DistanceLatitude / 2), 2) + Mathf.Cos(Radians(OldLatitude)) * Mathf.Cos(Radians(Latitude)) * Mathf.Pow(Mathf.Sin(DistanceLongitude / 2), 2);

            float c = 2 * Mathf.Atan2(Mathf.Sqrt(DistanceTotal), Mathf.Sqrt(1 - DistanceTotal));

            LastDistance = 6371 * c * 1000;

            // Debug.Log("Distância: " + LastDistance.ToString());

            Distance = LastDistance;

            OverallDistance += LastDistance;

            StartCoroutine(Overall());
        }

        IEnumerator Overall()
        {
            if (FirstTime)
            {
                FirstTime = false;

                yield return new WaitForSeconds(2);

                if (OverallDistance > 6000000)
                {
                    OverallDistance = 0;
                    LastDistance = 0;
                }
            }

            OverallDistance += LastDistance;
        }

        private void CalculateSpeed()
        {
            Speed = LastDistance / LastTime * 3.6f;

            // Debug.Log("Velocidade: " + Speed.ToString());
        }

        private void CalculateAcceleration()
        {
            Acceleration = (Speed - SpeedZero) / LastTime;

            // Debug.Log("Aceleração: " + Acceleration.ToString());
        }
    }
}