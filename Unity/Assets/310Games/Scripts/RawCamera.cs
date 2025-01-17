﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawCamera : MonoBehaviour
{
    public RawImage Raw;
    private WebCamTexture WebCam;

    public void OnEnable()
    {
        WebCam = new WebCamTexture();
        Raw.texture = WebCam;
        Raw.material.mainTexture = WebCam;
        WebCam.Play();
    }

    public void OnDisable()
    {
        WebCam.Stop();
        Destroy(WebCam);
    }
}
