using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadImagesTest : MonoBehaviour
{
   public DownloadableImage[] downloadableImages;

   private void Start()
   {
      StartCoroutine(GetImages("cat"));
   }

   IEnumerator GetImages(string word)
   {
      DataContainer<List<string>> urls = new DataContainer<List<string>>();
      yield return NounProjectMgr.GetImageUrls(word, urls, downloadableImages.Length);
      for (int i = 0; i < downloadableImages.Length; ++i)
      {
         if (i >= urls.data.Count)
            downloadableImages[i].Clear();
         else
            downloadableImages[i].SetURL(urls.data[i]);
      }
   }
}
