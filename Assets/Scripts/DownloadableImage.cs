using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DownloadableImage : MonoBehaviour
{
   public Color[] colors;
   SpriteRenderer spriteRenderer;
   Image uiImage;
   public Sprite empty;
   public Sprite downloading;
   public Sprite error;

   Coroutine downloadCoroutine;

   Texture2D downloadedTexture;
   Sprite downloadedSprite;

   void StopDownload()
   {
      if (downloadCoroutine != null)
      {
         StopCoroutine(downloadCoroutine);
         downloadCoroutine = null;
      }
      if (downloadedTexture)
      {
         Destroy(downloadedTexture);
         downloadedTexture = null;
      }
      if (downloadedSprite)
      {
         Destroy(downloadedSprite);
         downloadedSprite = null;
      }
   }

   void SetSprite(Sprite sprite)
   {
      if (spriteRenderer)
         spriteRenderer.sprite = sprite;
      if (uiImage)
         uiImage.sprite = sprite;
   }

   void SetColor(Color c)
   {
      if (spriteRenderer)
         spriteRenderer.color = c;
      if (uiImage)
         uiImage.color = c;
   }

   private void Awake()
   {
      spriteRenderer = GetComponent<SpriteRenderer>();
      uiImage = GetComponent<Image>();
      Clear();
   }

   public void Clear()
   {
      StopDownload();
      SetSprite(empty);
   }

   public void SetURL(string url)
   {
      StopDownload();
      SetSprite(downloading);
      downloadCoroutine = StartCoroutine(DownloadSprite(url));
   }

   IEnumerator DownloadSprite(string url)
   {
      using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
      {
         yield return www.SendWebRequest();
         if (www.result != UnityWebRequest.Result.Success)
         {
            SetSprite(error);
         }
         else
         {
            downloadedTexture = DownloadHandlerTexture.GetContent(www);
            var mip0Data = downloadedTexture.GetPixelData<Color32>(0);
            for (int i = 0; i < mip0Data.Length; i++)
            {
               mip0Data[i] = new Color32(mip0Data[i][0],255, 255, 255);
            }
            downloadedTexture.Apply(false);
            float w = downloadedTexture.width;
            float h = downloadedTexture.height;
            downloadedSprite = Sprite.Create(downloadedTexture, new Rect(0, 0, w, h), new Vector2(.5f,.5f), 200);
            SetSprite(downloadedSprite);
            SetColor(colors[Random.Range(0, colors.Length)]);
         }
      }
      downloadCoroutine = null;
   }
}
