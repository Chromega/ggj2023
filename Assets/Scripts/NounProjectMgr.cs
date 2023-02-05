using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NounProjectMgr : MonoBehaviour
{
   private static string key = "abe8b9af979a4f57b674a97095ee090d";
   private static string secret = "91f0e90960114905af5cb61b6d610575";

   public static NounProjectMgr I { get; private set; }


   [System.Serializable]
   public struct NounOverride
   {
      public string word;
      public Sprite sprite;
   }
   public NounOverride[] overrides;
   Dictionary<string, Sprite> overrideDict = new Dictionary<string, Sprite>();

   void Awake()
   {
      I = this;

      foreach (var o in overrides)
      {
         overrideDict[o.word.ToUpper()] = o.sprite;
      }
   }

   void OnDestroy()
   {
      if (I == this)
         I = null;
   }

   //Your code would look something like this
   /*
   private IEnumerator ExampleCoroutine(string word)
   {
      DataContainer<List<string>> urlContainer = new DataContainer<List<string>>();
      yield return StartCoroutine(GetImageUrls(word, urlContainer, 3));
      Debug.Log("Done");
   }*/

   //Call GetImageUrls from a coroutine.  Yield to it, and your data will be in result.  This is asynchronous.


   public static Sprite TryGetOverride(string word)
   {
      word = word.ToUpper();
      if (I.overrideDict.ContainsKey(word))
         return I.overrideDict[word];
      else
         return null;
   }

   public static IEnumerator GetImageUrls(string word, DataContainer<List<string>> result, int count=3)
   {
      string data;
      while (true)
      {
         OAuth_CSharp oauth = new OAuth_CSharp(key, secret);

         List<string> p = new List<string>();
         p.Add("limit=" + count);
         string requestURL = oauth.GenerateRequestURL("https://api.thenounproject.com/collection/" + word + "/icons", "GET", p);
         using (UnityWebRequest www = UnityWebRequest.Get(requestURL))
         {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
               if (www.responseCode == 403)
               {
                  Debug.LogError("Net Error");
                  Debug.Log(www.downloadHandler.text);
                  yield return new WaitForSeconds(.5f);
                  continue;
               }
               else
               {
                  data = "";
                  break;
               }
            }
            data = www.downloadHandler.text;
            break;
         }
      }

      List<string> imageUrls = new List<string>();
      int idx = 0;
      while (true)
      {
         int previewIdx = data.IndexOf("\"preview_url\"", idx);
         if (previewIdx == -1)
            break;
         int quoteIdx = data.IndexOf("\"", previewIdx + 13);
         int endQuoteIdx = data.IndexOf("\"", quoteIdx + 1);
         string imageUrl = data.Substring(quoteIdx + 1, endQuoteIdx - quoteIdx - 1);
         Debug.Log(imageUrl);
         imageUrls.Add(imageUrl);
         idx = endQuoteIdx;
      }

      result.data = imageUrls;
   }
}
