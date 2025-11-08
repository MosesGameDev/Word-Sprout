using System.Collections.Generic;
using UnityEngine;

public class AutoReturnToPool : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private bool _isPlaying = false;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        _isPlaying = true;
    }

    private void Update()
    {
        if (_isPlaying && !_particleSystem.IsAlive(true))
        {
            _isPlaying = false;
            ParticlePool.Instance.ReturnToPool(_particleSystem);
        }
    }
}
