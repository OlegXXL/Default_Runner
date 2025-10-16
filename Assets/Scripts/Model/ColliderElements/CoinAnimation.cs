using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 2f; // швидкість зникнення
    private Material mat;
    private float fade = 1f;
    private bool fading = false;
    private bool upCoin = false;
    private float rotatingSpeed = 100f;
    private void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.SetFloat("_Fade", 1f);
    }

    private void Update()
    {
        transform.Rotate(0, rotatingSpeed * Time.deltaTime, 0);
        if (fading)
        {
            fade -= Time.deltaTime * fadeSpeed;
            mat.SetFloat("_Fade", fade);

            if (fade <= 0f)
            {
                Destroy(gameObject);
            }
        }
        if (upCoin)
        {
            transform.position += Vector3.up * 2 * Time.deltaTime;
        }
    }

    public void StartFade()
    {
        fading = true;
    }
    private void UpCoin()
    {
        upCoin = true;
        rotatingSpeed *= 5;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade();
            UpCoin();
            GameData.Coin += 1;
            EventManager.CoinCollect?.Invoke();
        }
    }
}
