using System;
using System.Collections;
using System.Collections.Generic;
using NOOD;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class HealthSystem : MonoBehaviour
    {
        public Action onOutOfHealth;
        public Action onDamage;

        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Health _health;
        private IBlockChainObject _blockchainObject;
        private float _maxHealth = 100f;
        private float _currentHealth;

        void Awake()
        {
            _blockchainObject = GetComponentInParent<IBlockChainObject>();
        }
        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.2f);
            _health = EntityManager.Instance.GetModel<Health>(_blockchainObject.hexCode, _blockchainObject.gameId);
            _health.OnUpdated.RemoveAllListeners();
            _health.OnUpdated.AddListener(() => UpdateData());
            if(_blockchainObject is Enemy)
            {
                _health.OnUpdated.AddListener(() => UpdateHealth());
            }
            _healthSlider.maxValue = _maxHealth;
            _currentHealth = _maxHealth;
            _healthSlider.value = _currentHealth;
        }
        void OnDisable()
        {
            if(_health)
                _health.OnUpdated.RemoveAllListeners();
        }

        public void UpdateHealth()
        {
            if(_currentHealth < _health.health)
            {
                // Heal
                NumberTextManager.Instance.SpawnText((_health.health - _currentHealth).ToString("0"), Color.green, this.transform.position);
                onDamage?.Invoke();
            }
            if(_currentHealth > _health.health)
            {
                // Damage
                NumberTextManager.Instance.SpawnText((_currentHealth - _health.health).ToString("0"), Color.red, this.transform.position);
            }
            _healthSlider.value = _health.health;
            _currentHealth = _health.health;
        }
        private void UpdateData()
        {
            if(_health.health <= 0)
            {
                onOutOfHealth?.Invoke();
                UpdateHealth();
            }
        }

        public float GetCurrentHealth()
        {
            return _currentHealth;
        }
    }

}
