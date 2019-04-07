using UnityEngine;

namespace TecWolf.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        public static bool Active = true;

        public GameObject Target;
        public float DistanceCamera = 15.0f;

        public Vector3 FixedPosition;
        public float[] ZoomBounds = new float[] { 20f, 80f };

        public float TouchSensivityX = 0.75f;
        public float TouchSensivityY = 0.5f;
        public float ZoomSpeedTouch = 0.15f;

        public float MouseSensivityX = 0.75f;
        public float MouseSensivityY = 0.5f;
        public float ZoomSpeedMouse = 0.5f;

        private float OrbitX, OrbitY;
        private float Cron = 0f;

        private int PanFingerId;
        private bool WasZoomingLastFrame;
        private Vector2[] LastZoomPositions;

        private Camera[] Cameras;

        private void Start()
        {
            Cameras = GetComponentsInChildren<Camera>();
            OrbitX = transform.eulerAngles.x;
            OrbitY = transform.eulerAngles.y;
        }

        void Update()
        {
            if (Active)
            {
                if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    HandleTouch();
                }
                else
                {
                    HandleMouse();
                }
            }
        }

        private void LateUpdate()
        {
            if (Active)
            {
                float SpeedOfTimeScale = 1f / Time.timeScale;
                OrbitY = ClampAngle(OrbitY, 0f, 90f);

                Quaternion Rotation = Quaternion.Euler(OrbitY, OrbitX, 0f);
                Quaternion ActualRotation = transform.rotation;

                Vector3 NegativeDistance = new Vector3(0f, 0f, -DistanceCamera);

                Vector3 Position = Rotation * NegativeDistance + Target.transform.position;
                Vector3 ActualPosition = transform.position;

                transform.rotation = Quaternion.Lerp(ActualRotation, Rotation, Time.deltaTime * 5.0f * SpeedOfTimeScale);
                transform.position = Vector3.Lerp(ActualPosition, Position, Time.deltaTime * 5.0f * SpeedOfTimeScale);
            }
        }

        private void HandleMouse()
        {
            if (Input.GetAxis("Fire1") > 0)
            {
                OrbitX += Input.mousePosition.x * MouseSensivityX / 100f;
                OrbitY -= Input.mousePosition.y * MouseSensivityY * 10f / 100f;
                Cron = 0f;
            }
            else
            {
                if (Cron < 10f)
                {
                    Cron += Time.deltaTime;
                }
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            ZoomCamera(scroll, ZoomSpeedMouse);
        }

        private void HandleTouch()
        {
            switch (Input.touchCount)
            {
                case 1:
                    WasZoomingLastFrame = false;

                    Touch TouchZero = Input.GetTouch(0);
                    OrbitX += TouchZero.deltaPosition.x * TouchSensivityX;
                    OrbitY -= TouchZero.deltaPosition.y * TouchSensivityX * TouchSensivityY * 10.0f;
                    Cron = 0f;

                    break;

                case 2:
                    Vector2[] NewPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };

                    if (!WasZoomingLastFrame)
                    {
                        LastZoomPositions = NewPositions;
                        WasZoomingLastFrame = true;
                    }
                    else
                    {
                        float NewDistance = Vector2.Distance(NewPositions[0], NewPositions[1]);
                        float OldDistance = Vector2.Distance(LastZoomPositions[0], LastZoomPositions[1]);
                        float Offset = NewDistance - OldDistance;

                        ZoomCamera(Offset, ZoomSpeedTouch);

                        LastZoomPositions = NewPositions;
                    }

                    break;

                default:
                    WasZoomingLastFrame = false;

                    if (Cron < 10f)
                    {
                        Cron += Time.deltaTime;
                    }

                    break;
            }
        }

        private void ZoomCamera(float Offset, float Speed)
        {
            if (Offset == 0)
            {
                return;
            }

            foreach (Camera Go in Cameras)
            {
                Go.fieldOfView = Mathf.Clamp(Go.fieldOfView - (Offset * Speed), ZoomBounds[0], ZoomBounds[1]);
            }
        }

        private float ClampAngle(float Angle, float Min, float Max)
        {
            if (Angle < -360F)
            {
                Angle += 360F;
            }
            if (Angle > 360F)
            {
                Angle -= 360F;
            }

            return Mathf.Clamp(Angle, Min, Max);
        }
    }
}