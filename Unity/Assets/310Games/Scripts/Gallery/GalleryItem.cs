using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TecWolf.Gallery
{
    public class GalleryItem : MonoBehaviour
    {
        public string ImageID, Mission, Local, User, Link, Format;

        IEnumerator Start()
        {
            Texture2D Texture;

            Texture = new Texture2D(256, 256, TextureFormat.DXT1, false);

            using (WWW www = new WWW(Link))
            {
                yield return www;
                www.LoadImageIntoTexture(Texture);
                GetComponent<RawImage>().texture = Texture;
            }
        }
    }
}