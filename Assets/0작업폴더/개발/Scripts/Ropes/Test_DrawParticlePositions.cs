using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using System.Linq;

public class Test_DrawParticlePositions : MonoBehaviour
{
    private ObiRope rope;

    [SerializeField] private LineRenderer lineStart;
    [SerializeField] private LineRenderer lineEnd;

    private Vector3 lineHalfSize = new Vector3(0.1f, 0, 0);
    private Vector3 markerPositionOffset = new Vector3(0, 0, -0.1f); // so that the markers are rendered in front of rope

    [SerializeField] [Header("Marker list parent")] private GameObject markers;

    private int firstParticle, lastParticle;
    private Vector3 firstPos, lastPos;

    private void Start()
    {
        rope = gameObject.GetComponent<ObiRope>();
        GetEndParticlePositions();

        Debug.Log(firstParticle);
        Debug.Log(lastParticle);

        SetLineStats();
    }

    private void Update()
    {
        GetEndParticlePositions();

        SetLinePosition(lineStart, firstPos);
        SetLinePosition(lineEnd, lastPos);

        SetMarkerPositions(markerPositionOffset);
    }

    private void SetLineStats()
    {
        //lineStart = (new GameObject("line")).AddComponent<LineRenderer>();
        //lineEnd = (new GameObject("line")).AddComponent<LineRenderer>();

        lineStart.positionCount = 2;
        lineStart.startWidth = lineStart.endWidth = 0.1f;
        lineEnd.positionCount = 2;
        lineEnd.startWidth = lineEnd.endWidth = 0.1f;
    }

    private void SetLinePosition(LineRenderer line, Vector3 pos)
    {
        // A: use [GameObject].transform.localPosition
        line.transform.localPosition = pos;

        // B: use [LineRenderer].SetPosition
        //line.SetPosition(0, pos - lineHalfSize);
        //line.SetPosition(1, pos + lineHalfSize);
    }

    private void GetEndParticlePositions()
    {
        // first particle in the rope is the first particle of the first element:
        // last particle in the rope is the second particle of the last element:
        firstParticle = rope.elements[0].particle1;
        lastParticle = rope.elements[rope.elements.Count - 1].particle2;

        // now get their positions (expressed in solver space):
        firstPos = rope.solver.positions[firstParticle];
        lastPos = rope.solver.positions[lastParticle];
    }

    private void SetMarkerPositions(Vector3 offset)
    {
        Transform[] circles = markers.GetComponentsInChildren<Transform>().Skip(1).ToArray();

        circles[0].localPosition = firstPos;
        for (int i = 0; i < rope.elements.Count && i < circles.Length; ++i)
        {
            circles[i + 1].localPosition = rope.solver.positions[rope.elements[i].particle2];
            circles[i + 1].localPosition += offset;
        }
    }

//#if UNITY_EDITOR
//    private void OnDrawGizmosSelected()
//    {
//        GetParticlePositions();

//        Gizmos.DrawWireSphere(firstPos, 2);
//        Gizmos.DrawWireSphere(lastPos, 2);

//        Gizmos.DrawWireSphere(transform.position, 2);
//    }
//#endif
}
