using System.Collections;
using System.Collections.Generic;
using NOOD;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class ChoosePlayerUI : MonoBehaviour
    {
        [SerializeField] private Button _knightBtn;
        [SerializeField] private Button _wizardBtn;

        void Awake()
        {
            _knightBtn.onClick.AddListener(() =>
            {
                UIManager.Instance.ChoosePlayer(PlayerType.Knight);
                this.gameObject.SetActive(false);
                NoodyCustomCode.StartDelayFunction(() =>
                {
                    GameManager.Instance.PlayGame();
                }, 1f);
            });
            _wizardBtn.onClick.AddListener(() =>
            {
                UIManager.Instance.ChoosePlayer(PlayerType.Wizard);
                this.gameObject.SetActive(false);
                NoodyCustomCode.StartDelayFunction(() =>
                {
                    GameManager.Instance.PlayGame();
                },1f);
            });
        }
    }

}
