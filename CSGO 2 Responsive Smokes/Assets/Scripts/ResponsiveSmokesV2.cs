using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WithCode.Projects.CSGO2ResponsiveSmokes
{
    public class ResponsiveSmokesV2 : MonoBehaviour
    {
        [Header("Gizmos")]
        public bool showSmokeRange;
        public bool showSmokeGrid;
        public bool showBoxes;

        [Header("Smoke Properties")]
        public float smokeRange;
        public float maxYHeight;
        public float smokeCellSize = 0.35f;
        [HideInInspector] public List<Vector3> smokePositions;
        [HideInInspector] public List<float> distances;
        public float layerTime;
        public int layers;
        public bool isAccelerating;

        [Header("Testing")]
        public GameObject cube;

        [Header("Grenade Properties")]
        Rigidbody rb;
        public GrenadeState grenadeState;
        Vector3 genPos;


        void Start()
        {
            rb = GetComponent<Rigidbody>();

            if (maxYHeight > smokeRange * 0.75f) maxYHeight = smokeRange * 0.75f;

            grenadeState = GrenadeState.inactive;

        }

        void Update()
        {

            if(rb.velocity.magnitude <= 0f && grenadeState == GrenadeState.active)
            {
                grenadeState = GrenadeState.generating;
                genPos = transform.position;
                //genPos.y = 0.125f;
                smokePositions.Add(genPos);
                rb.isKinematic = true;
            }

            if(grenadeState == GrenadeState.generating)
            {
                GenerateSmokeVolume();
                FixSmokeVolume();
                Sort(ref smokePositions); // Ordering smokes.
                if(showBoxes) StartCoroutine("DisplaySmoke", layerTime);
                grenadeState = GrenadeState.doneGenerating;
            }

            if(grenadeState == GrenadeState.doneGenerating)
            {
                grenadeState = GrenadeState.used;
                Destroy(gameObject, 5f);
            }

            if(grenadeState == GrenadeState.used)
            {
                grenadeState = GrenadeState.inactive;
            }
        }

        public void GenerateSmokeVolume()
        {
            for (float height = smokeCellSize / 2; height <= maxYHeight; height += smokeCellSize * 1.1f)
                for (float x = -smokeRange; x <= smokeRange; x += smokeCellSize * 1.1f)
                    for (float y = -smokeRange; y <= smokeRange; y += smokeCellSize * 1.1f)
                        if (Vector3.Distance(genPos, genPos + Vector3.forward * x + Vector3.right * y + Vector3.up * height) < smokeRange)
                            if (!smokePositions.Contains(genPos + Vector3.forward * x + Vector3.right * y + Vector3.up * height) || 
                                !CheckClose(smokePositions, genPos + Vector3.forward * x + Vector3.right * y + Vector3.up * height, smokeCellSize * 0.75f))
                                smokePositions.Add(genPos + Vector3.forward * x + Vector3.right * y + Vector3.up * height);

        }

        void FixSmokeVolume()
        {
            // Fixing overlapping with gameobjects.
            List<Vector3> fixedOverlap = new List<Vector3>();
            foreach(var position in smokePositions)
            {
                if (!Physics.CheckSphere(position, smokeCellSize * 0.5f))
                    fixedOverlap.Add(position);
            }
            smokePositions = fixedOverlap;

            // Fixing object wraparound.
            //fixedOverlap = new List<Vector3>();
            //foreach(var position in smokePositions)
            //{
            //    var dist = Vector3.Distance(position, genPos);
            //    //if (dist > smokeRange * 0.75f)
            //    //{
            //        Ray ray = new Ray(position, genPos - position);
            //        if (!Physics.Raycast(ray, dist, 6))
            //                fixedOverlap.Add(position);
            //    //}
            //    //else
            //    //    fixedOverlap.Add(position);
            //}
            //smokePositions = fixedOverlap;
        }

        IEnumerator DisplaySmoke(float waitTime)
        {
            int batchSize = smokePositions.Count / layers;
            for(int i = 0; i < smokePositions.Count; i++)
            {
                while (batchSize > 0)
                {
                    if (i >= smokePositions.Count) break;

                    var c = Instantiate(cube, smokePositions[i], Quaternion.identity);
                    c.transform.parent = this.transform;
                    i++;
                    batchSize--;
                }
                batchSize = smokePositions.Count / layers;
                yield return new WaitForSeconds(waitTime);
                if(isAccelerating) waitTime *= 0.35f;
            }
            grenadeState = GrenadeState.doneGenerating;
        }

        public bool CheckClose(List<Vector3> list, Vector3 element, float minDist)
        {
            bool isClose = false;
            foreach (var e in list)
                if (Vector3.Distance(e, element) < minDist)
                {
                    isClose = true;
                    break;
                }
            return isClose;
        }

        private void Sort(ref List<Vector3> list)
        {
            
            List<int> indices = new();
            int num = 0;
            foreach (var item in list)
            {
                distances.Add(Vector3.Distance(genPos, item));
                indices.Add(num);
                num++;
            }
            for (int i = 1; i < distances.Count; i++)
            {
                int j = i;
                var dist1 = Vector3.Distance(genPos, list[j]);
                var dist2 = Vector3.Distance(genPos, list[j - 1]);
                while (j > 0 && distances[j] <= distances[j-1])
                {
                    var temp = distances[j];
                    distances[j] = distances[j - 1];
                    distances[j - 1] = temp;
                    var t = indices[j];
                    indices[j] = indices[j - 1];
                    indices[j - 1] = t;
                    j--;
                }
            }

            List<Vector3> ordered = new List<Vector3>();

            for(int i = 0; i < indices.Count; i++)
            {
                ordered.Add(list[indices[i]]);
            }
            list = ordered;

        }



        private void OnDrawGizmos()
        {
            if (showSmokeRange)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, smokeRange);
            }

            if (showSmokeGrid)
            {
                Gizmos.color = Color.grey;
                foreach(var item in smokePositions)
                {
                    Gizmos.DrawCube(item, Vector3.one * smokeCellSize);
                }
            }
        }

        public enum GrenadeState
        {
            inactive,
            active,
            generating,
            doneGenerating,
            used
        }
    }
}
