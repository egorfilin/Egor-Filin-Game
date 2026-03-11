using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoinSpritesheetAnimator : MonoBehaviour
{
    private Image _image;
    private Sprite[] _sprites;
    private Coroutine _coroutine;
    private float _frameRate = 24f;

    public void Initialize(Image image, Sprite[] sprites, float frameRate = 24f)
    {
        _image = image;
        _sprites = sprites;
        _frameRate = frameRate;
    }

    public void StartAnimation()
    {
        StopAnimation();
        _coroutine = StartCoroutine(Animate());
    }

    public void StopAnimation()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator Animate()
    {
        int frame = Random.Range(0, _sprites.Length);
        float interval = 1f / _frameRate;

        while (true)
        {
            if (_image != null && _sprites != null && _sprites.Length > 0)
            {
                _image.sprite = _sprites[frame % _sprites.Length];
                frame++;
            }
            yield return new WaitForSeconds(interval);
        }
    }

    private void OnDestroy()
    {
        StopAnimation();
    }
}