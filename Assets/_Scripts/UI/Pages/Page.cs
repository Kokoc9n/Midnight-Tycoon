using UnityEngine;

public abstract class Page : MonoBehaviour
{
    private Vector3 sScale;
    private Vector3 sPos;
    private void Awake()
    {
        sScale = transform.localScale;
        sPos = transform.position;
        Debug.Log(transform.name + " sPos:" + sPos);
    }
    public virtual void OnAppear()
    {
        gameObject.SetActive(true);
        //transform.DOMoveX(sPos.x, 0.5f).SetEase(Ease.OutSine);

        //transform.position = new Vector2(10f, sPos.y);
        //_ = transform.MoveToAsync(new Vector3(sPos.x,
        //                                    transform.position.y,
        //                                    transform.position.z), 0.5f, Tweener.Ease.EaseOutQuad);
    }
    public virtual async void OnDisappear()
    {
        //transform.DOMoveX(10f, 0.5f).SetEase(Ease.OutSine)
        //    .OnComplete(() => gameObject.SetActive(false));

        //await transform.MoveToAsync(new Vector3(10f,
        //                                    transform.position.y,
        //                                    transform.position.z), 0.5f, Tweener.Ease.EaseOutQuad);
        gameObject.SetActive(false);
    }
}