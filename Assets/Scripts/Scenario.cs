using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario : MonoBehaviour
{
   public List<SpriteRenderer> spriteRoots;
   public DownloadableImage nounPrefab;
   public Transform highlightXfm;
   List<DownloadableImage> downloadableImages = new List<DownloadableImage>();
   int currentNounIdx = 0;
   public AudioClip sfxMid;
   public AudioClip sfxPercussion;
   public AudioClip sfxUpper;

   private void Start()
   {
      for (int i = 0; i < spriteRoots.Count; ++i)
      {
         DownloadableImage di = Instantiate(nounPrefab);
         downloadableImages.Add(di);
         di.gameObject.SetActive(false);

         spriteRoots[i].enabled = false;
      }


      GameManager.I.OnWordSubmitted.AddListener(AddNoun);
   }

   public void Reset()
   {
      for (int i = 0; i < downloadableImages.Count; ++i)
      {
         downloadableImages[i].gameObject.SetActive(false);
      }
      currentNounIdx = 0;
   }

   void AddNoun(string word)
   {
      if (gameObject.activeSelf)
         StartCoroutine(GetImages(word));
   }

   IEnumerator GetImages(string word)
   {
      DataContainer<List<string>> urls = new DataContainer<List<string>>();

      Sprite o = NounProjectMgr.TryGetOverride(word);

      if (o != null)
      {
         downloadableImages[currentNounIdx].gameObject.SetActive(true);
         downloadableImages[currentNounIdx].SetSprite(o);
      }
      else
      {

         yield return NounProjectMgr.GetImageUrls(word, urls, 10);
         if (urls.data.Count == 0)
         {
            //Didn't find word :(
            Debug.LogError("Couldn't find it");
            GameManager.I.WordNotFound(word);
            yield break;
         }
         else
         {
            string randomUrl = urls.data[Random.Range(0, urls.data.Count)];
            downloadableImages[currentNounIdx].gameObject.SetActive(true);
            downloadableImages[currentNounIdx].SetURL(randomUrl);
         }
      }
      StartCoroutine(AnimateInWord(currentNounIdx));
      ++currentNounIdx;
      currentNounIdx %= downloadableImages.Count;
      if (currentNounIdx == 0)
      {
         yield return new WaitForSeconds(3f);
         ScenarioMgr.I.NextScenario();
      }
   }

   IEnumerator AnimateInWord(int idx)
   {
      Transform xfm = downloadableImages[idx].transform;
      xfm.parent = null;
      xfm.position = highlightXfm.position;
      xfm.rotation = highlightXfm.rotation;
      xfm.localScale = highlightXfm.localScale;

      yield return new WaitForSeconds(1.0f);

      float t = 0;
      const float kTotalTime = 1.0f;

      while (t < kTotalTime)
      {
         xfm.position = Vector3.Lerp(highlightXfm.position, spriteRoots[idx].transform.position, t / kTotalTime);
         xfm.rotation = Quaternion.Slerp(highlightXfm.rotation, spriteRoots[idx].transform.rotation, t / kTotalTime);
         xfm.localScale = Vector3.Lerp(highlightXfm.localScale, spriteRoots[idx].transform.localScale, t / kTotalTime);
         yield return null;
         t += Time.deltaTime;
      }

      xfm.parent = spriteRoots[idx].transform;
      xfm.localPosition = Vector3.zero;
      xfm.localRotation = Quaternion.identity;
      xfm.localScale = Vector3.one;
   }

   private void Update()
   {
      for (int i = 0; i < downloadableImages.Count; ++i)
      {
         downloadableImages[i].GetComponent<SpriteRenderer>().sortingOrder = spriteRoots[i].sortingOrder;
      }
   }
}
