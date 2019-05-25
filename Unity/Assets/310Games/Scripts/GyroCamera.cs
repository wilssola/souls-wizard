using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroCamera : MonoBehaviour
{

    private Quaternion RotationFix;

    public GameObject Webcam;

    public GameObject[] Backgrounds;
    public GameObject[] Buttons;

    public GameObject[] MonsterParent;
    private GameObject MonsterModel;

    public GameObject[] PlayerUI;

    private bool UseGyro;

    private int ParentNumber;

    void Start()
    {
        GameObject Parent = new GameObject("Parent Camera");

        Parent.transform.position = transform.position;
        transform.parent = Parent.transform;

        Input.gyro.enabled = true;

        Parent.transform.rotation = Quaternion.Euler(0f, -180f, 180f);

        RotationFix = new Quaternion(0f, 0f, 1f, 0f);

        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;

            Webcam.SetActive(true);
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

            Webcam.SetActive(true);

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

                            MonsterModel.transform.position = new Vector3(0f, 1.25f, -3.5f);
                            MonsterModel.transform.localScale = new Vector3(1, 1, 1);

                            MonsterModel.transform.rotation = new Quaternion(0, 0, 0, 0);
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

            Webcam.SetActive(false);

            Destroy(MonsterModel);
        }
    }

    public void Button()
    {
        UseGyro = !UseGyro;
    }

    public void Exit()
    {
        UseGyro = false;
    }
}
