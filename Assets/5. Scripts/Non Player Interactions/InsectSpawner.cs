using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace NP_Interactions
{

    [RequireComponent(typeof(Collider))]
    public class InsectSpawner : MonoBehaviour
    {

        [SerializeField] private float spawnerRadius;
        [SerializeField] private int insectCount = 2;

        [Space(10), SerializeField] private GameObject insectPrefab;

        private List<PlayerAvoidingInsect> _insects = new List<PlayerAvoidingInsect>();
        private bool _playerIsInTrigger;
        private Transform _target;

        #region Edit

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (Application.isPlaying) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, spawnerRadius);

            if(visualiseSpawnPositions.Count > 0)
            {
                foreach(var point in visualiseSpawnPositions)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(point, 0.2f);
                }
            }
        }

        [Button("Test Spawn Position")]
        public void VisualiseSpawnPositions()
        {
            print("hello");
            visualiseSpawnPositions = new List<Vector3>();
            for(int i =0; i<insectCount; i++)
            {
                visualiseSpawnPositions.Add(GetSpawnPosition());
            }
        }

        private List<Vector3> visualiseSpawnPositions = new List<Vector3>();

#endif

        #endregion



        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player_Main") || other.CompareTag("Player"))
            {
                _playerIsInTrigger = true;
                _target = other.gameObject.transform;

            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player_Main") || other.CompareTag("Player"))
            {
                _playerIsInTrigger = false;
            }
        }


        private void Start()
        {

            for (int i = 0; i < insectCount; i++)
            {
                var obj = Instantiate(insectPrefab, GetSpawnPosition(), Quaternion.Euler(0, Random.Range(0,180), 0), transform);
                _insects.Add(obj.GetComponent<PlayerAvoidingInsect>());
            }

        }


        private void Update()
        {
            if (!_playerIsInTrigger) return;

            var targetPostion = _target.position;

            foreach (var insect in _insects)
            {
                insect.UpdateInsect(targetPostion);
            }
        }

        public Vector3 GetSpawnPosition()
        {
            var rad = Random.Range(0, spawnerRadius);
            var theta = Random.Range(0, 360);

            var spawnPos = Vector3.zero;

            spawnPos.y = 0;


            spawnPos.x = rad * Mathf.Cos(theta);
            spawnPos.z = rad * Mathf.Sin(theta);


            return transform.position + spawnPos;

        }

    }
}
