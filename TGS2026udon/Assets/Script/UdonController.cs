using UnityEngine;

public class UdonController : MonoBehaviour
{
    [System.Serializable]
    public struct UdonNodeData
    {
        public Rigidbody2D nodeRb;       // 動かすノード（生地の端の物体）
        public Transform targetTrans;    // 目指すターゲットの座標（外側に向かう方向）
        public KeyCode key;              // 対応するキーボードのキー
    }

    public UdonNodeData[] udonNodes = new UdonNodeData[8];
    public float basePullForce = 50f;    // 引っ張る基本の力（適宜調整してください）

    void Update()
    {
        // 8方向のキー入力をチェック
        for (int i = 0; i < udonNodes.Length; i++)
        {
            if (Input.GetKeyDown(udonNodes[i].key))
            {
                // タイミング判定（1.0 = Perfect など）
                float timingScore = GetTimingScore(); 
                
                StretchUdon(i, timingScore);
            }
        }
    }

    // ダミーのタイミング判定
    float GetTimingScore() => 1.0f; 

    void StretchUdon(int index, float score)
    {
        UdonNodeData data = udonNodes[index];
        if (data.nodeRb == null || data.targetTrans == null) return;

        // 1. ターゲットへの方向を計算（例：上にあるターゲットなら真上のベクトルになる）
        Vector2 direction = (data.targetTrans.position - data.nodeRb.transform.position).normalized;

        // 2. タイミングの評価（score）に合わせて、その方向にだけ力を加える
        float finalForce = basePullForce * score;
        data.nodeRb.AddForce(direction * finalForce, ForceMode2D.Impulse); 
        // ※一瞬だけドンと動かしたいので「ForceMode2D.Impulse」にしています。通常のForceが良い場合は消してください。
    }
}