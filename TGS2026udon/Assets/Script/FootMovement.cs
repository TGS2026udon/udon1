using UnityEngine;

public class FootMovement : MonoBehaviour
{
    public float moveSpeed = 5f;       // 足を出すスピード
    public float returnSpeed = 8f;     // 足を戻すスピード（少し速めが自然です）
    public float maxDistance = 1.5f;   // 足が動ける最大距離（離れすぎ防止）

    public GameObject rightFoot;
    public GameObject leftFoot;

    // 足の初期位置（元の位置）を記憶する変数
    private Vector3 rightFootOrigin;
    private Vector3 leftFootOrigin;

    void Start()
    {
        // ゲーム開始時の足の位置を「元の位置」として記憶する
        // ※親オブジェクト（体）からの相対位置（localPosition）で記憶します
        if (rightFoot != null) rightFootOrigin = rightFoot.transform.localPosition;
        if (leftFoot != null) leftFootOrigin = leftFoot.transform.localPosition;
    }

    void Update()
    {
        // 入力の取得
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(moveX, moveY).normalized;

        // --- 1. 足を出す（移動）処理 ---
        if (movement != Vector2.zero)
        {
            // 「上（前）」または「右」に動くときは右足を出す
            if (moveY > 0 || moveX > 0)
            {
                rightFoot.transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
                // 出せる限界の距離を制限する（ローカル座標で制限）
                rightFoot.transform.localPosition = Vector3.ClampMagnitude(rightFoot.transform.localPosition - rightFootOrigin, maxDistance) + rightFootOrigin;
            }
            // 「下（後）」または「左」に動くときは左足を出す
            else if (moveY < 0 || moveX < 0)
            {
                leftFoot.transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
                // 出せる限界の距離を制限する
                leftFoot.transform.localPosition = Vector3.ClampMagnitude(leftFoot.transform.localPosition - leftFootOrigin, maxDistance) + leftFootOrigin;
            }
        }

        // --- 2. 元の位置に戻る処理 ---
        // キーが押されていない、または操作していない方の足を、元の位置へ滑らかに戻す（Vector3.Lerpを使用）
        
        // 右足の戻り処理（「前・右」に入力がない、または全く移動していないとき）
        if (!(moveY > 0 || moveX > 0) || movement == Vector2.zero)
        {
            rightFoot.transform.localPosition = Vector3.Lerp(
                rightFoot.transform.localPosition, 
                rightFootOrigin, 
                returnSpeed * Time.deltaTime
            );
        }

        // 左足の戻り処理（「後・左」に入力がない、または全く移動していないとき）
        if (!(moveY < 0 || moveX < 0) || movement == Vector2.zero)
        {
            leftFoot.transform.localPosition = Vector3.Lerp(
                leftFoot.transform.localPosition, 
                leftFootOrigin, 
                returnSpeed * Time.deltaTime
            );
        }
    }
}