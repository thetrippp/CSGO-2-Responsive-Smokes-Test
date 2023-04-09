using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponsiveSmokesV3 : MonoBehaviour
{

    [Header("Smoke Dimension Properties")]
    public SmokeState smokeState = SmokeState.None;
    public AnimationCurve smokeExpandCurve, smokeDissipateCurve;
    private Bounds bounds;
    public Vector3 maxSmokeDimensions;
    public float spacing;

    [Header("Smoke Time Properties")]
    public float smokeExpandDuration;
    public float smokeStayDuration;
    public float smokeDissipateDuration;
    public float smokeInactiveTime;

    private float smokeExpandStartTime, smokeExpandEndTime, smokeExpandCurveValue;
    private float smokeDissipatingTime, smokeDissipateCurveValue;

    private float smokeSizeValue;

    [Header("Smoke")]
    public List<Vector3> smokePositions;
    public List<Vector3> outerMostLayer;


    void Start()
    {
        bounds.size = maxSmokeDimensions;
        smokePositions.Add(transform.position);
        outerMostLayer.Add(transform.position);
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0) && smokeState == SmokeState.None) {
            smokeState = SmokeState.Expanding;
            smokeExpandStartTime = Time.time;
            smokeExpandEndTime = smokeExpandStartTime + smokeExpandDuration;
        }

        if (smokeState == SmokeState.Expanding)
        {
            smokeExpandCurveValue = smokeExpandCurve.Evaluate((Time.time - smokeExpandStartTime) / (smokeExpandEndTime - smokeExpandStartTime));

            if (smokeExpandCurveValue >= smokeExpandCurve.Evaluate(smokeExpandCurve.length))
            {
                smokeState = SmokeState.Active;
                smokeExpandEndTime = Time.time;
            }

            smokeSizeValue = smokeExpandCurveValue;

            // Generate Volumes.
            var tempBounds = bounds;
            tempBounds.size *= smokeSizeValue;
            List<Vector3> currentLayer = new List<Vector3>();
            foreach (var item in outerMostLayer)
                currentLayer.Add(item);
            List<Vector3> toRemove = new List<Vector3>();
            foreach(var item in currentLayer)
            {
                Vector3 forward = item + Vector3.forward * spacing;
                Vector3 backward = item + Vector3.forward * -spacing;
                Vector3 right = item + Vector3.right * spacing;
                Vector3 left = item + Vector3.right * -spacing;
                Vector3 up = item + Vector3.up * spacing;

                if (tempBounds.Contains(forward) && !IsClose(smokePositions, forward, spacing) && !IsClose(outerMostLayer, forward, spacing))
                {
                    outerMostLayer.Add(forward);
                    smokePositions.Add(forward);
                }
                if (tempBounds.Contains(backward) && !IsClose(smokePositions, backward, spacing) && !IsClose(outerMostLayer, backward, spacing))
                {
                    outerMostLayer.Add(backward);
                    smokePositions.Add(backward);
                }
                if (tempBounds.Contains(right) && !IsClose(smokePositions, right, spacing) && !IsClose(outerMostLayer, right, spacing))
                {
                    outerMostLayer.Add(right);
                    smokePositions.Add(right);
                }
                if (tempBounds.Contains(left) && !IsClose(smokePositions, left, spacing) && !IsClose(outerMostLayer, left, spacing))
                {
                    outerMostLayer.Add(left);
                    smokePositions.Add(left);
                }
                if (tempBounds.Contains(up) && !IsClose(smokePositions, up, spacing) && !IsClose(outerMostLayer, up, spacing))
                {
                    outerMostLayer.Add(up);
                    smokePositions.Add(up);
                }

            }


            for(int i = 0; i < smokePositions.Count; i++)
                for(int j = 0; j < smokePositions.Count; j++)
                    if (i != j && IsClose(smokePositions[i], smokePositions[j], spacing))
                        toRemove.Add(smokePositions[j]);
            
            foreach (var item in toRemove)
                smokePositions.Remove(item);

        }

        if(smokeState == SmokeState.Active)
        {
            if (Time.time > smokeExpandEndTime + smokeStayDuration)
            {
                smokeState = SmokeState.Dissipating;
                smokeDissipatingTime = Time.time;
            }

        }

        if(smokeState == SmokeState.Dissipating)
        {
            smokeDissipateCurveValue = smokeDissipateCurve.Evaluate((Time.time - smokeDissipatingTime) / (smokeDissipateDuration));
            smokeSizeValue = smokeDissipateCurveValue;
            var tempBounds = bounds;
            tempBounds.size *= smokeSizeValue;
            List<Vector3> toRemove = new List<Vector3>();
            foreach (var item in smokePositions)
                if (!tempBounds.Contains(item))
                    toRemove.Add(item);

            foreach (var item in toRemove)
                smokePositions.Remove(item);

            if (smokeDissipateCurveValue >= smokeDissipateCurve.Evaluate(smokeDissipateCurve.length)
                && Time.time > smokeDissipatingTime + smokeDissipateDuration + smokeInactiveTime)
            {
                smokeState = SmokeState.None;
                smokePositions.Clear();
                outerMostLayer.Clear();
                smokePositions.Add(transform.position);
                outerMostLayer.Add(transform.position);
            }

        }

    }

    private bool IsClose(List<Vector3> list, Vector3 pos, float dist)
    {
        foreach(var item in list)
            if (Vector3.Distance(item, pos) < dist * 0.75f)
                return true;
        return false;
    }

    private bool IsClose(Vector3 pos1, Vector3 pos2, float dist)
    {
        if (Vector3.Distance(pos1, pos2) < dist * 0.75f)
            return true;
        return false;
    }


    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube(transform.position, smokeExpandCurveValue * maxSmokeDimensions);

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, smokeSizeValue * bounds.size);
        foreach (var pos in smokePositions)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawCube(pos, Vector3.one * spacing * 0.75f);
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(pos, Vector3.one * spacing * 0.75f);
        }
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, bounds.size);
    }

    public enum SmokeState
    {
        None,
        Expanding,
        Active,
        Dissipating
    }
}
