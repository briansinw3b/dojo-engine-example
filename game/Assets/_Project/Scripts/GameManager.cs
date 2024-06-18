using System;
using System.Collections.Generic;
using System.Linq;
using Dojo;
using Dojo.Starknet;
using UnityEngine;
using NOOD;
using System.Collections;
using dojo_bindings;
using System.Threading.Tasks;

namespace Game
{
    public class GameManager : MonoBehaviorInstance<GameManager>
    {
        [SerializeField] private WorldManager _worldManager;
        [SerializeField] private WorldManagerData _worldManagerData;
        [SerializeField] private GameManagerData _gameManagerData;
        public Actions actions;
        public Account masterAccount;

        private Dictionary<FieldElement, string> _spawnedAccounts = new Dictionary<FieldElement, string>();

        public BurnerManager burnerManager;
        public JsonRpcClient provider;

        public Action<bool> onEndGame;

        public Action onGameStart;

        private bool _isPlaying;
        private uint _gameId;
        private List<GameObject> _spawnedObject = new List<GameObject>();

        void Awake()
        {
            _isPlaying = false;
        }

        IEnumerator Start()
        {
            provider = new JsonRpcClient(_worldManagerData.rpcUrl);
            masterAccount = new Account(provider, new SigningKey(_gameManagerData.privateKey), new FieldElement(_gameManagerData.accountAddress));
            burnerManager = new BurnerManager(provider, masterAccount);

            _worldManager.synchronizationMaster.OnEntitySpawned.AddListener(InitEntity);
            foreach (var entity in _worldManager.Entities())
            {
                InitEntity(entity);
            }

            yield return new WaitForSeconds(1f);
            // GetGameID();
            // SpawnPlayerAndMonster();
            _isPlaying = true;
        }

        public void CheckWin()
        {
            if (BattleManager.Instance.GetState() == BattleState.Win)
            {
                _isPlaying = false;
                onEndGame?.Invoke(true);
            }
            else
            {
                onEndGame?.Invoke(false);
                _isPlaying = false;
            }
        }

        #region Actions
        public async Task PlayerAction(SkillType skillType, Action onComplete)
        {
            try
            {
                await actions.action(masterAccount, skillType);
                onComplete?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            
        }
        public void MonsterAttack(SkillType skillType)
        {

        }
        public async Task SpawnAsync(PlayerType playerType)
        {
            if(playerType == PlayerType.Knight)
            {
                FieldElement txtHash = await GameManager.Instance.actions.spawn(GameManager.Instance.masterAccount, new Character.Horseman());
            }
            else
            {
                FieldElement txtHash = await GameManager.Instance.actions.spawn(GameManager.Instance.masterAccount, new Character.Magician());
            }


        }
        #endregion

        public void PlayGame()
        {
            _isPlaying = true;
            onGameStart?.Invoke();
        }

        public bool IsPlaying()
        {
            return _isPlaying;
        }

        public void InitEntity(GameObject entity)
        {
            Debug.Log("SpawnAccount: " + entity.name);
            foreach (var accountPair in _spawnedAccounts)
            {
                Debug.Log("Account: " + accountPair);
                if (accountPair.Value == null)
                {
                    _spawnedAccounts[accountPair.Key] = entity.name;
                    break;
                }
            }

            GetGameID();
            SpawnPlayerAndMonster();
        }

        private void GetGameID()
        {
            Debug.Log("------------------------");
            Debug.Log("Get Game Id");
            foreach(var entity in _worldManager.Entities())
            {
                if(entity.TryGetComponent<Counter>(out Counter counter))
                {
                    _gameId = counter.counter;
                }
            }
        }

        private void SpawnPlayerAndMonster()
        {
            Debug.Log("------------------------");
            Debug.Log("Spawn Player and Enemy");
            foreach(var g in _spawnedObject)
            {
                Destroy(g);
            }
            _spawnedObject.Clear();
            
            foreach(var entity in _worldManager.Entities())
            {
                if(entity.TryGetComponent<Health>(out Health health))
                {
                    if(NoodyCustomCode.CompareHexStrings(health.entityId.Hex(), "0x676f626c696e") && health.gameId == _gameId)
                    {
                        _spawnedObject.Add(EnemySpawner.Instance.SpawnEnemy(_gameId));
                    }
                }
                if(entity.TryGetComponent<Player>(out Player player))
                {
                    _spawnedObject.Add(PlayerSpawner.Instance.Spawn(player.character, player.player.Hex(), _gameId));
                }
            }
        }
    }
}
