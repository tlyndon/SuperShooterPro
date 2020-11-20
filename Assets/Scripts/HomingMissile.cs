//--------------------------------
// help with this provided by https://www.youtube.com/watch?v=jWUz5J-vfGA
//--------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//--------------------------------
[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : MonoBehaviour
{

  [SerializeField]
  private GameObject _explosionPrefab;

  public float speed=3;
  public float rotatingSpeed = 200;
  private GameObject target;

  Rigidbody2D rb;
  //--------------------------------
  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    target = GameObject.FindGameObjectWithTag("Enemy");
  }
  //--------------------------------
  void FixedUpdate()
  {
    if (target==null)
      {
        target = GameObject.FindGameObjectWithTag("Enemy");
      }
    else
    {
      Vector2 point2Target = (Vector2)transform.position-(Vector2)target.transform.position;
      point2Target.Normalize();
      float value = Vector3.Cross(point2Target,transform.up).z;
      rb.angularVelocity=rotatingSpeed * value;
      rb.velocity = transform.up*speed;
    }
  }
  //--------------------------------
  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Enemy")
    {
        Instantiate(_explosionPrefab, other.transform.position, Quaternion.identity);
        Destroy(other.gameObject,0.02f);
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject,0.02f);
    }
  }
  //--------------------------------
}
