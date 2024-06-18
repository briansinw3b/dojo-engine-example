using System;
using System.Collections;
using System.Collections.Generic;
using NOOD;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(HealthSystem))]
    public class Enemy : MonoBehaviour, IBattler, IBlockChainObject
    {
        public string hexCode;
        private HealthSystem _healthSystem;
        private Animator _animator;
        private bool _isDead;

        string IBlockChainObject.hexCode { get => hexCode; set => hexCode = value; }
        public uint gameId { get; set; }

        void Awake()
        {
            _healthSystem = GetComponent<HealthSystem>();
            _animator = GetComponentInChildren<Animator>();
            _isDead = false;
        }

        void Start()
        {
            BattleManager.Instance.Prepare(BattleSide.Enemy, this);
            BattleManager.Instance.onEnemyTurn += EnemyTurn;
            _healthSystem.onOutOfHealth += OnOutOfHealth;
        }
        void OnDestroy()
        {
            _healthSystem.onOutOfHealth -= OnOutOfHealth;
            NoodyCustomCode.UnSubscribeAllEvent(BattleManager.Instance, this);
            if(BattleManager.Instance)
                BattleManager.Instance.Remove(BattleSide.Enemy, this);
        }

        private void EnemyTurn()
        {
            if(!_isDead)
                Attack(); 
        }
        private void OnOutOfHealth()
        {
            // Dead animation
            if(_animator)
                _animator.Play("Enemy Death");

            // EndTurn
            NoodyCustomCode.StartDelayFunction(() => BattleManager.Instance.EndBattle(BattleSide.Enemy), 1f);

            _isDead = true;
        }

        public void Attack()
        {
            // Attack animation
            if(_animator)
            {
                _animator.Play("Enemy Attack 1");
                ReturnToIdle_Delay(_animator.GetCurrentAnimatorStateInfo(0).length);
            }

            // Attack logic
            // GameManager.Instance.MonsterAttack(new SkillType.Attack());
            BattleManager.Instance.GetOpponent(BattleSide.Enemy)[0].Damage();
        }

        public void Skill()
        {
            Attack();
        }

        public void Damage()
        {
            // Animation
            if(_animator)
            {
                ReturnToIdle_Delay(_animator.GetCurrentAnimatorStateInfo(0).length);
            }

            // Logic
            _healthSystem.UpdateHealth();
        }
        private void ReturnToIdle_Delay(float delay)
        {
            NoodyCustomCode.StartDelayFunction(() =>
            {
                if (_isDead == true) return;
                
                if(_animator)
                    _animator.Play("Enemy Idle");
                // EndTurn
                BattleManager.Instance.EndTurn(BattleSide.Enemy);
            }, 0.9f);
        }
    }
}
