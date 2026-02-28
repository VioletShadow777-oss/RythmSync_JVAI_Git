using UnityEngine;
using DG.Tweening;

public class StageLight : MonoBehaviour
{
    [Header("Pivots")]
    public Transform yokePivot;
    public Transform headPivot;

    [Header("Speed")]
    public float yokeDuration = 3f;
    public float headDuration = 2f;

    Tween _yoke, _head;

    void Start()
    {
        DOTween.Init();

        if (!yokePivot || !headPivot)
        {
            Debug.LogError("StageLight: Assign both pivots in the Inspector!", this);
            return;
        }

        yokePivot.localRotation = Quaternion.identity;
        headPivot.localRotation = Quaternion.identity;

        _yoke = Spin(yokePivot, new Vector3(0f, 0f, 90f), yokeDuration);
        _head = Spin(headPivot, new Vector3(45f, 0f, 0f), headDuration);
    }

    Tween Spin(Transform target, Vector3 to, float duration)
    {
        return target
            .DOLocalRotate(to, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(target.gameObject);
    }

    void OnDisable() => KillTweens();
    void OnDestroy() => KillTweens();

    void KillTweens()
    {
        _yoke?.Kill();
        _head?.Kill();
    }
}