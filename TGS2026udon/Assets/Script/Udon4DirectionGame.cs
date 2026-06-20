using UnityEngine;

public class Udon4DirectionGame : MonoBehaviour
{
    public enum ShishoCommand { Free, Up, Down, Left, Right }

    [Header("4方向の生地オブジェクト")]
    public Transform kijiUp;
    public Transform kijiDown;
    public Transform kijiLeft;
    public Transform kijiRight;

    [Header("生地の設定")]
    public float expandSpeed = 0.2f;       // 踏んだ方向に直接伸びる量
    public float fatSpeed = 0.12f;         // 【重要】踏んだパーツが「横に太る」量（これを上げると十字にならなくなります）
    public float sideInfluence = 0.08f;     // 【重要】隣のパーツが引っ張られる量
    public float targetScale = 4.0f;       // 目標とする点線の円のスケール

    [Header("師匠の現在の指示")]
    public ShishoCommand currentCommand = ShishoCommand.Free;

    [Header("リズム設定")]
    public float bpm = 120f;
    private float beatInterval;
    private float beatTimer = 0f;

    private float totalRhythmScore = 0f;
    private float totalCommandScore = 0f;
    private int stepCount = 0;
    private int commandCheckCount = 0;
    private bool isGameEnded = false;

    void Start()
    {
        beatInterval = 60f / bpm;

        // kijiUp.localScale = Vector3.one;
        // kijiDown.localScale = Vector3.one;
        // kijiLeft.localScale = Vector3.one;
        // kijiRight.localScale = Vector3.one;
    }

    void Update()
    {
        if (isGameEnded) return;

        beatTimer += Time.deltaTime;
        if (beatTimer >= beatInterval) beatTimer -= beatInterval;

        // キー入力
        if (Input.GetKeyDown(KeyCode.W)) PlayerStep(KeyCode.W);
        if (Input.GetKeyDown(KeyCode.S)) PlayerStep(KeyCode.S);
        if (Input.GetKeyDown(KeyCode.A)) PlayerStep(KeyCode.A);
        if (Input.GetKeyDown(KeyCode.D)) PlayerStep(KeyCode.D);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isGameEnded = true;
            EvaluateFinalScore();
            enabled = false;
        }
    }

void PlayerStep(KeyCode pressedKey)
    {
        stepCount++;

        // --- リズム・コマンド判定はそのまま保持 ---
        float halfInterval = beatInterval / 2f;
        float diff = Mathf.Abs(beatTimer - (beatTimer > halfInterval ? beatInterval : 0f));
        float rhythmScore = Mathf.Clamp(100f - (diff * 250f), 0f, 100f);
        totalRhythmScore += rhythmScore;

        if (currentCommand != ShishoCommand.Free)
        {
            commandCheckCount++;
            bool isCorrect = false;
            if (currentCommand == ShishoCommand.Up && pressedKey == KeyCode.W) isCorrect = true;
            if (currentCommand == ShishoCommand.Down && pressedKey == KeyCode.S) isCorrect = true;
            if (currentCommand == ShishoCommand.Left && pressedKey == KeyCode.A) isCorrect = true;
            if (currentCommand == ShishoCommand.Right && pressedKey == KeyCode.D) isCorrect = true;
            if (isCorrect) totalCommandScore += 100f;
        }

        // --- 各パーツの(Xの増加量, Yの増加量)を細かく指定 ---
        Vector2 upGrow = Vector2.zero;
        Vector2 downGrow = Vector2.zero;
        Vector2 leftGrow = Vector2.zero;
        Vector2 rightGrow = Vector2.zero;

        // お隣さんが引っ張られる量
        float sideSpeed = expandSpeed * sideInfluence; 

        if (pressedKey == KeyCode.W) // 上を踏んだ
        {
            upGrow    = new Vector2(fatSpeed, expandSpeed); // 縦に伸び、横にも太る
            leftGrow  = new Vector2(0f, sideSpeed);         // 左パーツの上側を引き上げる
            rightGrow = new Vector2(0f, sideSpeed);         // 右パーツの上側を引き上げる
        }
        else if (pressedKey == KeyCode.S) // 下を踏んだ
        {
            downGrow  = new Vector2(fatSpeed, expandSpeed);
            leftGrow  = new Vector2(0f, sideSpeed);
            rightGrow = new Vector2(0f, sideSpeed);
        }
        else if (pressedKey == KeyCode.A) // 左を踏んだ
        {
            leftGrow  = new Vector2(expandSpeed, fatSpeed); // 横に伸び、縦にも太る
            upGrow    = new Vector2(sideSpeed, 0f);         // 上パーツの左側を引き出す
            downGrow  = new Vector2(sideSpeed, 0f);         // 下パーツの左側を引き出す
        }
        else if (pressedKey == KeyCode.D) // 右を踏んだ
        {
            rightGrow = new Vector2(expandSpeed, fatSpeed);
            upGrow    = new Vector2(sideSpeed, 0f);
            downGrow  = new Vector2(sideSpeed, 0f);
        }

        // 4つのパーツに新しい補正ロジックで適用
        ApplyComplexGrowth(kijiUp, Vector3.up, upGrow);
        ApplyComplexGrowth(kijiDown, Vector3.down, downGrow);
        ApplyComplexGrowth(kijiLeft, Vector3.left, leftGrow);
        ApplyComplexGrowth(kijiRight, Vector3.right, rightGrow);
    }

    // 新・サイズと位置の補正関数
    void ApplyComplexGrowth(Transform targetKiji, Vector3 mainDir, Vector2 growAmount)
    {
        Vector3 scale = targetKiji.localScale;
        Vector3 pos = targetKiji.localPosition;

        // 1. スケールを大きくする
        scale.x += growAmount.x;
        scale.y += growAmount.y;

        // 2. 【ここを修正】縦横両方の拡大に対して、根元が中心(0,0)からズレないように位置を補正する
        // メインの伸び方向だけでなく、太る方向（横幅）に対しても、そのパーツが属する側に半分だけズラします
        if (mainDir.y != 0) // 上下パーツの場合
        {
            pos.y += mainDir.y * (growAmount.y / 2f); // 縦に伸びた分、外側へ
            // 上下パーツが横に太るときは、中央固定で左右均等に広げてOKなのでpos.xは動かさない
        }
        else if (mainDir.x != 0) // 左右パーツの場合
        {
            pos.x += mainDir.x * (growAmount.x / 2f); // 横に伸びた分、外側へ
            // 左右パーツが縦に太るときは、中央固定で上下均等に広げてOKなのでpos.yは動かさない
        }

        targetKiji.localScale = scale;
        targetKiji.localPosition = pos;
    }

    void EvaluateFinalScore()
    {
        // 4方向の広がり具合の平均をとって形を評価
        float finalScoreX = (kijiLeft.localScale.x + kijiRight.localScale.x) / 2f;
        float finalScoreY = (kijiUp.localScale.y + kijiDown.localScale.y) / 2f;

        float diffX = Mathf.Abs(finalScoreX - targetScale);
        float diffY = Mathf.Abs(finalScoreY - targetScale);

        float sizeScoreX = Mathf.Clamp(100f - (diffX * 150f), 0f, 100f);
        float sizeScoreY = Mathf.Clamp(100f - (diffY * 150f), 0f, 100f);
        float finalShapeScore = (sizeScoreX + sizeScoreY) / 2f;

        float avgRhythm = stepCount > 0 ? totalRhythmScore / stepCount : 0f;
        float avgCommand = commandCheckCount > 0 ? totalCommandScore / commandCheckCount : 100f;
        float finalScore = (avgRhythm * 0.35f) + (finalShapeScore * 0.35f) + (avgCommand * 0.30f);

        Debug.Log($"======= 師匠の最終審判 =======");
        Debug.Log($"生地のぴったり度: {finalShapeScore:F1} 点");
        Debug.Log($"総合評価: {finalScore:F1} 点");
    }
}