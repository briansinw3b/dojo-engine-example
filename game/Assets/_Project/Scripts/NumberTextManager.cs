using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NOOD;
using TMPro;
using UnityEngine;

namespace Game
{
    public class NumberTextManager : MonoBehaviorInstance<NumberTextManager>
    {
        [SerializeField] private TextMeshPro _textMeshPro;

        public void SpawnText(string text, Color color, Vector3 position)
        {
            Transform transform = Instantiate(_textMeshPro, null).transform;
            transform.position = position;
            PlayAnimation(transform);
            TextMeshPro textMeshPro = transform.GetComponent<TextMeshPro>();
            textMeshPro.color = color;
            textMeshPro.text = text;
        }

        private void PlayAnimation(Transform transform)
        {
            transform.DOMoveY(this.transform.position.y + 0.5f, 2f);
            NoodyCustomCode.FadeOutTextMeshColor(transform.GetComponent<TextMeshPro>(), 0, 0.5f);
        }        
    }
}
