using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class CircleSpin : MonoBehaviour
    {
        public GameObject Prefab;
        public Transform CenterPoint;
        public Vector2 CircleSize = Vector2.one;
        public int CircleSegments = 5;
        public float AnimationSpeed = 1f;

        private Transform[] _instances;

        public void Start()
        {
            _instances = Enumerable.Range(0, CircleSegments)
                .Select(p => {
                    var instance = Instantiate(Prefab).transform;
                    instance.transform.parent = transform;
                    return instance;
                })
                .ToArray();

            if (CenterPoint == null)
                CenterPoint = transform;
        }

        public void Update()
        {
            var time = Time.time * AnimationSpeed;
            var segmentSize = Mathf.PI * 2 / CircleSegments;

            for (var i = 0; i < _instances.Length; i++)
            {
                var instance = _instances[i];
                var a = segmentSize * i + time;
                instance.transform.localPosition = new Vector3(
                    Mathf.Cos(a) * CircleSize.x,
                    0,
                    Mathf.Sin(a) * CircleSize.y
                );
            }
        }
    }
}