using UnityEngine;
using UnityEngine.UI; // UIを使うために必要

public class ImageSwitcher : MonoBehaviour
{
    // インスペクターで画像を設定するための変数
    public Image targetImage;        // 画面に表示されている画像（UI）
    public Sprite firstSprite;       // 1枚目の画像
    public Sprite secondSprite;      // 2枚目の画像

    private bool isFirstImage = true; // 今どちらの画像か判断するフラグ

    void Update()
    {
        // スペースキーが押されたかチェック
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeImage();
        }
    }

    void ChangeImage()
    {
        // もし今の画像が1枚目なら2枚目に、そうでなければ1枚目にする
        if (isFirstImage)
        {
            targetImage.sprite = secondSprite;
        }
        else
        {
            targetImage.sprite = firstSprite;
        }

        // フラグを反転させる（trueならfalseへ、falseならtrueへ）
        isFirstImage = !isFirstImage;
    }
}