using System.Collections;
using System.Collections.Generic;
using NOOD;
using UnityEngine;

namespace Game
{
    public class EnemySpawner : MonoBehaviorInstance<EnemySpawner>
    {
        [SerializeField] private Transform _enemySpawnPos;
        [SerializeField] private Enemy _enemyPref;

        public GameObject SpawnEnemy(uint gameId)
        {
            Enemy enemy = Instantiate(_enemyPref, null);
            enemy.transform.position = _enemySpawnPos.transform.position;
            enemy.gameObject.SetActive(true);
            enemy.gameId = gameId;
            return enemy.gameObject;
        }
    }
}
