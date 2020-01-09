using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float fireTime;
    public float speed;

    private float additiveSpeed;
    private float addToSpeed;
    private Coroutine fire;
    private AudioSource bulletShot;

    private void OnEnable()
    {
        bulletShot = GetComponent<AudioSource>();
        bulletShot.Play();

        additiveSpeed = speed;
        addToSpeed = 10;
        fire = StartCoroutine(C_Fire());
    }

    IEnumerator C_Fire()
    {
        float time = fireTime;

        while (time > 0)
        {
            time -= Time.deltaTime;
            transform.position += transform.forward * additiveSpeed * Time.deltaTime;
            additiveSpeed += Time.deltaTime * addToSpeed;
            addToSpeed += 2;

            yield return new WaitForEndOfFrame();
        }

        if (!bulletShot.isPlaying)
        {
            gameObject.SetActive(false);
        }
        
    }

    private void OnDisable()
    {
        StopCoroutine(C_Fire());
        ObjectPooler.Instance.RecycleObject(gameObject);
        additiveSpeed = speed;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground")     // Colliding with ground destroys the bullet
        {
            gameObject.SetActive(false);
        }
    }
}
