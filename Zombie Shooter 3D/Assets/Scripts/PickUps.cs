using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PickUps : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(0f,1f,0f);
    [SerializeField] float period = 1f;
    [SerializeField] float rotationSpeed = 1f;
    float movementFactor;
    Vector3 startingPos;
    // Start is called before the first frame update
    void Start()
    {
        startingPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Oscillations();
        Rotations();
    }

    private void Oscillations()
    {
        if (period <= Mathf.Epsilon) { return; }
        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau);
        movementFactor = rawSinWave / 2f + .5f;
 
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
 
    private void Rotations()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
