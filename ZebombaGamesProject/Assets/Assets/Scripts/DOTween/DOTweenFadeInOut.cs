using DG.Tweening;
using TMPro;
using UnityEngine;

public class DOTweenFadeInOut : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float duration = 1f;

    private void Start()
    {
        text.DOFade(0, duration).SetLoops(-1, LoopType.Yoyo);
    }
}
