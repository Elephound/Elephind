using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RotateEli : MonoBehaviour
{
    [SerializeField] Transform _eliObject;
    public float rotationSpeed = 100f;

    bool _shouldRotate = false;


    // Update is called once per frame
    void Update()
    {
        if (_shouldRotate)
            Rotate();
    }

    public void StartRotate()
    {
        _shouldRotate = true;
    }

    public void StopRotate()
    {
        _shouldRotate = false;
        _eliObject.transform.rotation = Quaternion.identity;
    }

    void Rotate()
    {
        transform.localRotation = Quaternion.Euler(0 , 0 , transform.localRotation.eulerAngles.z - (rotationSpeed * Time.deltaTime));
    }
}
