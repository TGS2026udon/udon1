using UnityEngine;

public class CircleShaper : MonoBehaviour
{
    [Header("伸びるスピード")]
    public float growthSpeed = 2f;

    [Header("最大でどこまで伸ばせるか")]
    public float maxScale = 3f;

    [Header("各パーツのTransformを設定")]
    public Transform qTopLeft;
    public Transform wTop;
    public Transform eTopRight;
    public Transform aLeft;
    public Transform dRight;
    public Transform zBottomLeft;
    public Transform xBottom;
    public Transform cBottomRight;

    void Update()
    {
        // --- Qキー: 左上へ伸びる (Xマイナス、Yプラス) ---
        if (Input.GetKey(KeyCode.Q)) 
            GrowDiagonal(qTopLeft, -1f, 1f);

        // --- Wキー: 上へ伸びる (Yプラス) ---
        if (Input.GetKey(KeyCode.W)) 
            GrowVertical(wTop, 1f);

        // --- Eキー: 右上へ伸びる (Xプラス, Yプラス) ---
        if (Input.GetKey(KeyCode.E)) 
            GrowDiagonal(eTopRight, 1f, 1f);

        // --- Aキー: 左へ伸びる (Xプラス ※左側パーツの幅を広げる想定) ---
        if (Input.GetKey(KeyCode.A)) 
            GrowHorizontal(aLeft, 1f);

        // --- Sキー: 何もしない ---

        // --- Dキー: 右へ伸びる (Xプラス) ---
        if (Input.GetKey(KeyCode.D)) 
            GrowHorizontal(dRight, 1f);

        // --- Zキー: 左下へ伸びる (Xマイナス、Yマイナス) ---
        if (Input.GetKey(KeyCode.Z)) 
            GrowDiagonal(zBottomLeft, -1f, -1f);

        // --- Xキー: 下へ伸びる (Yマイナス ※下側パーツの縦を伸ばす想定) ---
        if (Input.GetKey(KeyCode.X)) 
            GrowVertical(xBottom, 1f);

        // --- Cキー: 右下へ伸びる (Xプラス、Yマイナス) ---
        if (Input.GetKey(KeyCode.C)) 
            GrowDiagonal(cBottomRight, 1f, -1f);
    }

    // 横方向に伸ばす処理
    void GrowHorizontal(Transform target, float directionX)
    {
        if (target == null) return;
        Vector3 scale = target.localScale;
        scale.x += directionX * growthSpeed * Time.deltaTime;
        scale.x = Mathf.Clamp(scale.x, 1f, maxScale); // 元のサイズ(1)から最大値の間に制限
        target.localScale = scale;
    }

    // 縦方向に伸ばす処理
    void GrowVertical(Transform target, float directionY)
    {
        if (target == null) return;
        Vector3 scale = target.localScale;
        scale.y += directionY * growthSpeed * Time.deltaTime;
        scale.y = Mathf.Clamp(scale.y, 1f, maxScale);
        target.localScale = scale;
    }

    // 斜め方向に伸ばす処理 (XとYを同時に外側へ広げる)
    void GrowDiagonal(Transform target, float directionX, float directionY)
    {
        if (target == null) return;
        Vector3 scale = target.localScale;
        
        // 符号を考慮して外側に大きくなるように計算
        scale.x += Mathf.Sign(scale.x) * growthSpeed * Time.deltaTime;
        scale.y += Mathf.Sign(scale.y) * growthSpeed * Time.deltaTime;
        
        // 限界値を超えないように制限
        scale.x = Mathf.Clamp(scale.x, 1f, maxScale);
        scale.y = Mathf.Clamp(scale.y, 1f, maxScale);
        
        target.localScale = scale;
    }
}