using System;
using System.Collections;
using System.Collections.Generic;
using NOOD;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(HealthSystem))]
    public class PlayerMain : MonoBehaviour, IBattler, IBlockChainObject
    {
        private HealthSystem _healthSystem;
        private Animator _animator;
        private bool _isDead;
        public string hexCode;

        string IBlockChainObject.hexCode { get => hexCode; set => hexCode = value; }
        public uint gameId { get; set; }

        void Awake()
        {
            _healthSystem = GetComponent<HealthSystem>();
            _animator = GetComponentInChildren<Animator>();
        }    

        void Start()
        {
            BattleManager.Instance.Prepare(BattleSide.Player, this);
            _healthSystem.onOutOfHealth += OnOutOfHealth;
            UIManager.Instance.onPlayerAttack += Attack;
            UIManager.Instance.onPlayerSkill += Skill;
            UIManager.Instance.onPlayerHeal += Heal;
        }


        void OnDestroy()
        {
            if(_healthSystem)
                _healthSystem.onOutOfHealth -= () => OnOutOfHealth();
            NoodyCustomCode.UnSubscribeAllEvent(UIManager.Instance, this);
            if(BattleManager.Instance)
                BattleManager.Instance.Remove(BattleSide.Player, this);
        }

        private void OnOutOfHealth()
        {
            // Animation
            if(_animator)
                _animator.Play("Death");

            // Logic
            _isDead = true;
            NoodyCustomCode.StartDelayFunction(() =>
            {
                BattleManager.Instance.EndBattle(BattleSide.Player);
            }, 1f);
        }

        private async void Heal()
        {
            // Animation
            if(_animator)
            {
                _animator.Play("Heal");
                PlayIdleAfterDelay(_animator.GetCurrentAnimatorStateInfo(0).length);
            }

            // Logic
            await GameManager.Instance.PlayerAction(new SkillType.Healing(), null);
            _healthSystem.UpdateHealth();

            // EndTurn
            BattleManager.Instance.EndTurn(BattleSide.Player);
        }

        public void Damage()
        {
            // Animation
            if(_animator)
            {
                _animator.Play("Hurt");
                PlayIdleAfterDelay(_animator.GetCurrentAnimatorStateInfo(0).length);
            }

            // Logic
            _healthSystem.UpdateHealth();
        }

        public async void Attack()
        {
            // Attack animation
            if(_animator)
            {
                _animator.Play("Attack");
                PlayIdleAfterDelay(_animator.GetCurrentAnimatorStateInfo(0).length);
            }

            // Attack logic
            IBattler target = BattleManager.Instance.GetOpponent(BattleSide.Player)[0];
            await GameManager.Instance.PlayerAction(new SkillType.Attack(), null);

            // EndTurn
            BattleManager.Instance.EndTurn(BattleSide.Player);
        }

        public async void Skill()
        {
            // Animation
            if(_animator)
            {
                _animator.Play("Skill");
                PlayIdleAfterDelay(_animator.GetCurrentAnimatorStateInfo(0).length);
            }

            // Logic
            IBattler target = BattleManager.Instance.GetOpponent(BattleSide.Player)[0];
            await GameManager.Instance.PlayerAction(new SkillType.StrongAttack(), null);

            // EndTurn
            BattleManager.Instance.EndTurn(BattleSide.Player);
        }
        
        private void PlayIdleAfterDelay(float delay)
        {
            NoodyCustomCode.StartDelayFunction(() =>
            {
                if(_isDead == true) return;

                if(_animator)
                    _animator.Play("Idle");
            }, delay);
        }
    }

}
