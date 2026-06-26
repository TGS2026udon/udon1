using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EvaluationManager : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private Image targetImage; // 表示するImageコンポーネント

    [Header("評価画像（Sprite）")]
    [SerializeField] private Sprite spriteGood; // 1: 良
    [SerializeField] private Sprite spriteOkay; // 2: 可
    [SerializeField] private Sprite spriteBad;  // 3: 不可

    [Header("演出設定")]
    [SerializeField] private float displayDuration = 1.0f; // 完全に消えるまでの時間

    private Coroutine fadeCoroutine; // フェード処理を管理するコルーチン

    void Start()
    {
        // 最初はImageを非表示（透明）にしておく
        if (targetImage != null)
        {
            SetAlpha(0);
        }
    }

    void Update()
    {
        // 1キー：良
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            ShowEvaluation(spriteGood);
        }
        // 2キー：可
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            ShowEvaluation(spriteOkay);
        }
        // 3キー：不可
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            ShowEvaluation(spriteBad);
        }
    }

    // 評価画像を表示する処理
    private void ShowEvaluation(Sprite newSprite)
    {
        if (targetImage == null || newSprite == null) return;

        // もしすでにフェード中の処理があれば強制終了（これですぐさま切り替わる）
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // 画像を切り替えて、不透明度を100%（1.0）にする
        targetImage.sprite = newSprite;
        SetAlpha(1.0f);

        // 徐々に消えるコルーチンを開始
        fadeCoroutine = StartCoroutine(FadeOutRoutine());
    }

    // 時間経過で徐々に消える処理
    private IEnumerator FadeOutRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < displayDuration)
        {
            elapsedTime += Time.deltaTime;
            // 残り時間に応じてアルファ値（透明度）を計算
            float alpha = Mathf.Clamp01(1.0f - (elapsedTime / displayDuration));
            SetAlpha(alpha);
            yield return null; // 1フレーム待つ
        }

        // 完全に透明にする
        SetAlpha(0);
    }

    // 不透明度（Alpha）を変更する便利メソッド
    private void SetAlpha(float alpha)
    {
        Color color = targetImage.color;
        color.a = alpha;
        targetImage.color = color;
    }
}