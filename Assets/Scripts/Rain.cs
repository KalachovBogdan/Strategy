using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Rain : MonoBehaviour
{
    public Light dirLight;
    private ParticleSystem _ps;
    private bool _isRain;

    private void Start()
    {
        _ps = GetComponent<ParticleSystem>();
        StartCoroutine(Weather());

    }
    private void Update()
    {
        if (_isRain && dirLight.intensity > 0.25f)
        {
            LightIntensity(-1);
        }
        else if (!_isRain && dirLight.intensity < 0.8f)
        {
            LightIntensity(1);
        }
    }

    private void LightIntensity(int mult)
    {
        dirLight.intensity += 0.1f * Time.deltaTime * mult;
    }

    IEnumerator Weather()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(8f , 12f));
            if (_isRain == true) 
                _ps.Stop();
            else 
                _ps.Play();

            _isRain = !_isRain;
        }
    }
}
