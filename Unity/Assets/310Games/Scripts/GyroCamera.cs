using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GyroCamera : MonoBehaviour
{

    private Quaternion RotationFix;

    public GameObject[] Webcam;

    public GameObject[] Backgrounds;
    public GameObject[] Buttons;

    public GameObject[] MonsterParent;
    private GameObject MonsterModel;

    public GameObject[] PlayerUI;

    private bool UseGyro;

    private int ParentNumber;

    public float AngleX;

    void Start()
    {
        GameObject Parent = new GameObject("Parent Camera");

        Parent.transform.position = transform.position;
        transform.parent = Parent.transform;

        Input.gyro.enabled = true;

        Parent.transform.rotation = Quaternion.Euler(0f, -180f, 0f);

        RotationFix = new Quaternion(0f, 0f, 1f, 0f);

        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;

            foreach (GameObject Go in Webcam)
            {
                Go.SetActive(true);
            }
        }
        else
        {
#if !UNITY_EDITOR
            foreach (GameObject Go in Buttons)
            {
                Go.SetActive(false);
            }
#endif
        }

    }

    void Update()
    {
        if (UseGyro)
        {
            transform.localRotation = Input.gyro.attitude * RotationFix;

            foreach (GameObject Go in Backgrounds)
            {
                Go.SetActive(false);
            }

            foreach (GameObject Go in Webcam)
            {
                Go.SetActive(true);
            }

            for (int i = 0; i < MonsterParent.Length; i++)
            {
                if (MonsterParent[i].activeSelf && MonsterModel == null)
                {
                    ParentNumber = i;

                    for (int j = 0; j < MonsterParent[i].transform.childCount; j++)
                    {
                        if (MonsterParent[i].transform.GetChild(j).gameObject.activeSelf == true)
                        {
                            MonsterModel = Instantiate(MonsterParent[i].transform.GetChild(j).gameObject);

                            MonsterModel.transform.localScale = new Vector3(1, 1, 1);

                            MonsterModel.transform.parent = transform;

                            MonsterModel.transform.localPosition = new Vector3(0, 0, 6.5f);

                            MonsterModel.transform.LookAt(transform.forward);

                            if(MonsterModel.transform.eulerAngles.y == 180)
                            {
                                MonsterModel.transform.rotation = Quaternion.Euler(0, 180, 180);
                            }

                            MonsterModel.transform.parent = null;
                        }
                    }
                }
            }

            foreach (GameObject Go in MonsterParent)
            {
                Go.SetActive(false);
            }

            foreach (GameObject Go in PlayerUI)
            {
                Go.SetActive(false);
            }
        }
        else
        {
            transform.localRotation = new Quaternion(0, 0, 0, 0);

            foreach (GameObject Go in Backgrounds)
            {
                Go.SetActive(true);
            }

            foreach (GameObject Go in MonsterParent)
            {
                Go.SetActive(true);
            }

            PlayerUI[0].SetActive(true);

            foreach (GameObject Go in Webcam)
            {
                Go.SetActive(false);
            }

            Destroy(MonsterModel);
        }
    }

    public void Button()
    {
        UseGyro = !UseGyro;
    }

    public void Exit()
    {
        foreach (GameObject Go in Buttons)
        {
            Go.GetComponent<Toggle>().isOn = false;
        }

        UseGyro = false;
    }

    private void GetEuler()
    {
        if (transform.eulerAngles.x > 180f)
        {
            if (transform.eulerAngles.x > 256f)
                AngleX = (transform.eulerAngles.x * -1f) + 360f;
            else
                AngleX = -transform.eulerAngles.x;
        }
        else
        {

            if (transform.eulerAngles.x > 256f)
                AngleX = transform.eulerAngles.x - 180f;
            else
                AngleX = ((transform.eulerAngles.x * -1f) + 180f) * -1f;
        }
    }
}
