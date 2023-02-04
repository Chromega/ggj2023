using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoupFloat : MonoBehaviour
{
   Rigidbody2D rb;
   float targetSpeed = 1f;
   // Start is called before the first frame update
   void Start()
   {
      rb = GetComponent<Rigidbody2D>();
      rb.velocity = new Vector2(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f));
   }

   // Update is called once per frame
   void FixedUpdate()
   {
      float speed = rb.velocity.magnitude;
      Vector2 targetVel = rb.velocity * targetSpeed / speed;
      rb.velocity = Vector2.Lerp(rb.velocity, targetVel, 1-Mathf.Pow(.5f, Time.deltaTime));
   }
}
