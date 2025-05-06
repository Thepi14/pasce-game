using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DotCircle : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int steps;
    public float radius;
    public float width;
    public float spin = 0;
    public bool dots = false;
    private float angle = 0;

    private void OnValidate()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        lineRenderer.alignment = LineAlignment.TransformZ;
        lineRenderer = GetComponent<LineRenderer>();
    }
    void Update()
    {
        if (transform.parent.rotation.y == -180)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (spin != 0)
            angle += Time.deltaTime * spin;
        DrawCircle(steps, radius, width, angle, dots);
    }

    private void DrawCircle(int steps, float radius, float width, float angle, bool dots)
    {
        lineRenderer.positionCount = steps;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        if (dots) 
        {
            lineRenderer.material = lineRenderer.materials[1];
        }
        for (int currentStep = 0; currentStep < steps; currentStep++)
        {
            float circunferenceProgress = (float)currentStep / (steps-1);

            float currentRadian = circunferenceProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian + angle);
            float yScaled = Mathf.Sin(currentRadian + angle);

            float x = xScaled * radius;
            float y = yScaled * radius;

            Vector3 currentPosition = new Vector3(x, y, 0) + transform.position;

            lineRenderer.SetPosition(currentStep, currentPosition);
        }
    }
}
