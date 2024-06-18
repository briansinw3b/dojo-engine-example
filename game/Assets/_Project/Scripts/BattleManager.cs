using System;
using System.Collections;
using System.Collections.Generic;
using NOOD;
using UnityEngine;

namespace Game
{
    public enum BattleState
    {
        Prepare,
        Start,
        Battling,
        Win,
        Lose
    }

    public enum BattleSide
    {
        Player,
        Enemy
    }

    public class BattleManager : MonoBehaviorInstance<BattleManager>
    {
        public Action onPlayerTurn;
        public Action onEnemyTurn;

        private BattleState _state;
        private List<IBattler> _playerSideBattler = new List<IBattler>();
        private List<IBattler> _enemySideBattler = new List<IBattler>();

        void Awake()
        {
            _state = BattleState.Prepare;
            _playerSideBattler = new List<IBattler>();
            _enemySideBattler = new List<IBattler>();
        }

        void Start()
        {
            GameManager.Instance.onGameStart += () => OnGameStart();
        }

        public BattleState GetState()
        {
            return _state;
        }

        private void OnGameStart()
        {
            _state = BattleState.Prepare;
        }

        public void EndTurn(BattleSide battleSide)
        {
            switch (battleSide)
            {
                case BattleSide.Player:
                    NoodyCustomCode.StartDelayFunction(() =>
                    {
                        onEnemyTurn?.Invoke(); 
                    }, 1f);
                    break;
                case BattleSide.Enemy:
                    NoodyCustomCode.StartDelayFunction(() =>
                    {
                        onPlayerTurn?.Invoke();
                    }, 1f);
                    break;
            }
        }

        public void Prepare(BattleSide battleSide, IBattler battler)
        {
            switch (battleSide)
            {
                case BattleSide.Player:
                    _playerSideBattler.Add(battler);
                    break;
                case BattleSide.Enemy:
                    _enemySideBattler.Add(battler);
                    break;
            }
        }
        public void Remove(BattleSide battleSide, IBattler battler)
        {
            switch (battleSide)
            {
                case BattleSide.Player:
                    _playerSideBattler.Remove(battler);
                    break;
                case BattleSide.Enemy:
                    _enemySideBattler.Remove(battler);
                    break;
            }
        }

        public List<IBattler> GetOpponent(BattleSide battleSide)
        {
            switch (battleSide)
            {
                case BattleSide.Player:
                    return _enemySideBattler;
                case BattleSide.Enemy:
                    return _playerSideBattler;
            }
            return null;
        }

        public void EndBattle(BattleSide loseSide)
        {
            if (loseSide == BattleSide.Enemy)
                _state = BattleState.Win;
            else
                _state = BattleState.Lose;

            GameManager.Instance.CheckWin();
        }
    }
}