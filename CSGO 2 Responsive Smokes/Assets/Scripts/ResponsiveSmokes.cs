using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WithCode.Projects.CSGO2ResponsiveSmokes
{
    public class ResponsiveSmokes : MonoBehaviour
    {
        [Header("Gizmos")]
        public bool showSmokeRange;
        public bool showBoxGrid;
        public bool showSmokeBoxes;


        [Header("Smoke Properties")]
        public float smokeRange;
        public float smokeUnitDimension;
        public float verticalBias;
        public GameObject box;

       
        public List<Vector3> smokePositions;

        public int iterations = 10;


        void Start()
        {
            float spacing = smokeUnitDimension * 1.25f;

            smokePositions.Add(transform.position);
            GenerateVolume(smokePositions, spacing);
            //CheckVolume();

            if (showSmokeBoxes)
                GenerateBoxes();


        }

        private void GenerateBoxes()
        {
            foreach(var item in smokePositions)
            {
                Instantiate(box, item, Quaternion.identity);
            }
        }

        private void CheckVolume()
        {
            List<Vector3> correctedSmokePositions = new List<Vector3>();
            foreach(var item in smokePositions)
            {
                var pos = transform.position;
                    pos.y = item.y;
                    Ray ray = new Ray(item, pos - item);
                    if (!Physics.SphereCast(ray, 1f))
                        correctedSmokePositions.Add(item);
            }

            smokePositions = correctedSmokePositions;
        }

        private void GenerateVolume(List<Vector3> smokePositions, float spacing)
        {
            if (iterations == 0) return;

            List<Vector3> PositionsToAdd = new List<Vector3>();

            foreach (var position in smokePositions)
            {
                if (Vector3.Distance(position + Vector3.up * spacing, transform.position) < verticalBias || Vector3.Distance(position, transform.position) < verticalBias)
                    if (!Physics.CheckSphere(position + Vector3.up * spacing, 0.1f))
                        if (!CheckClose(smokePositions,position + Vector3.up * spacing, spacing) && !CheckClose(PositionsToAdd, position + Vector3.up * spacing, spacing))
                            PositionsToAdd.Add(position + Vector3.up * spacing);
                if (Vector3.Distance(position + Vector3.forward * spacing, transform.position) < smokeRange || Vector3.Distance(position, transform.position) < smokeRange)
                    if (!Physics.CheckSphere(position + Vector3.forward * spacing, 0.1f))
                        if (!CheckClose(smokePositions, position + Vector3.forward * spacing, spacing) && !CheckClose(PositionsToAdd, position + Vector3.forward * spacing, spacing))
                            PositionsToAdd.Add(position + Vector3.forward * spacing);
                if (Vector3.Distance(position + -Vector3.forward * spacing, transform.position) < smokeRange || Vector3.Distance(position, transform.position) < smokeRange)
                    if (!Physics.CheckSphere(position + -Vector3.forward * spacing, 0.1f))
                        if (!CheckClose(smokePositions, position + -Vector3.forward * spacing, spacing) && !CheckClose(PositionsToAdd, position + -Vector3.forward * spacing, spacing))
                            PositionsToAdd.Add(position + -Vector3.forward * spacing);
                if (Vector3.Distance(position + -Vector3.right * spacing, transform.position) < smokeRange || Vector3.Distance(position, transform.position) < smokeRange)
                    if (!Physics.CheckSphere(position + -Vector3.right * spacing, 0.1f))
                        if (!CheckClose(smokePositions, position + -Vector3.right * spacing, spacing) && !CheckClose(PositionsToAdd, position + -Vector3.right * spacing, spacing))
                            PositionsToAdd.Add(position + -Vector3.right * spacing);
                if (Vector3.Distance(position + Vector3.right * spacing, transform.position) < smokeRange || Vector3.Distance(position, transform.position) < smokeRange)
                    if (!Physics.CheckSphere(position + Vector3.right * spacing, 0.1f))
                        if (!CheckClose(smokePositions, position + Vector3.right * spacing, spacing) && !CheckClose(PositionsToAdd, position + Vector3.right * spacing, spacing))
                            PositionsToAdd.Add(position + Vector3.right * spacing);

            }

            foreach (var item in PositionsToAdd)
                if(!smokePositions.Contains(item))
                smokePositions.Add(item);

            iterations--;
            GenerateVolume(smokePositions, spacing);
        }

        void Update()
        {

        }


        public bool CheckClose(List<Vector3> list, Vector3 element, float minDist)
        {
            bool isClose = false;

            foreach(var e in list)
            {
                if(Vector3.Distance(e, element) < minDist)
                {
                    isClose = true;
                    break;
                }
            }
 

            return isClose;
        }

        private void OnDrawGizmos()
        {
            if (showSmokeRange)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, smokeRange);
            }

            if (showBoxGrid)
            {
                Gizmos.color = Color.black;
                // Show grid.
                foreach(var pos in smokePositions)
                {
                    Gizmos.DrawWireCube(pos, Vector3.one * smokeUnitDimension);
                }
            }
        }
    }
}