using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands.TubeClient;
using NOOD;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public class UIManager : MonoBehaviorInstance<UIManager>
    {
        public Action onPlayerSkill;
        public Action onPlayerAttack;
        public Action onPlayerHeal;

        [SerializeField] Button _attackBtn, _skillBtn, _healBtn;
        [SerializeField] private GameObject _endGamePanel;
        [SerializeField] private TextMeshProUGUI _endGameText;
        private bool _isShowBtn;

        void Start()
        {
            GameManager.Instance.onGameStart += () => OnGameStart();
            BattleManager.Instance.onPlayerTurn += () => 
            { 
                if(GameManager.Instance.IsPlaying())
                    ShowButtons();
            };
            GameManager.Instance.onEndGame += (isWin) => OnEndGame(isWin);

            _attackBtn.onClick.AddListener(() => 
            { 
                onPlayerAttack?.Invoke();
                HideButton();
            });
            _skillBtn.onClick.AddListener(() =>
            {
                onPlayerSkill?.Invoke();
                HideButton();
            });
            _healBtn.onClick.AddListener(() =>
            {
                onPlayerHeal?.Invoke();
                HideButton();
            });
        }
        void Update()
        {
            if (_isShowBtn == false) return;
            if(Input.GetKeyDown(KeyCode.A))
            {
                onPlayerAttack?.Invoke();
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                onPlayerSkill?.Invoke();
            }
            if(Input.GetKeyDown(KeyCode.D))
            {
                onPlayerHeal?.Invoke();
            }
        }

        private void OnEndGame(bool isWin)
        {
            _endGamePanel.gameObject.SetActive(true);
            _endGameText.text = isWin ? "YOU WIN" : "YOU LOSE";
            _endGameText.color = isWin ? NoodyCustomCode.HexToColor("#FEFF00") : Color.red;
        }

        private void OnGameStart()
        {
            ShowButtons();
        }

        public void ChoosePlayer(PlayerType playerType)
        {
            PlayerSpawner.Instance.SpawnPlayer(playerType);
        }

        private void ShowButtons()
        {
            _isShowBtn = true;
            _attackBtn.gameObject.SetActive(true);
            _skillBtn.gameObject.SetActive(true);
            _healBtn.gameObject.SetActive(true);
        }
        private void HideButton()
        {
            _isShowBtn = false;
            _attackBtn.gameObject.SetActive(false);
            _skillBtn.gameObject.SetActive(false);
            _healBtn.gameObject.SetActive(false);
        }
    }

}
