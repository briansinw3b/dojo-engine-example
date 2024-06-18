using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NOOD;
using UnityEngine;

namespace Game
{
    public enum PlayerType
    {
        Knight,
        Wizard
    }

    public class PlayerSpawner : MonoBehaviorInstance<PlayerSpawner>
    {
        [SerializeField] private PlayerMain _knight;
        [SerializeField] private PlayerMain _wizard;
        [SerializeField] private Transform _playerSpawnPoint;

        public async void SpawnPlayer(PlayerType playerType)
        {
            if(playerType == PlayerType.Knight)
            {
                await GameManager.Instance.actions.spawn(GameManager.Instance.masterAccount, new Character.Horseman());
            }
            else
            {
                await GameManager.Instance.actions.spawn(GameManager.Instance.masterAccount, new Character.Magician());
            }
        }

        public GameObject Spawn(Character character, string hexCode, uint gameId)
        {
            PlayerMain player;
            if(character.GetType() == typeof(Character.Horseman))
            {
                player = Instantiate(_knight);
            }
            else
            {
                player = Instantiate(_wizard);
            }
            player.gameObject.SetActive(true);
            player.transform.position = _playerSpawnPoint.transform.position;
            player.hexCode = hexCode;
            player.gameId = gameId;
            return player.gameObject;
        }
    }

}
