using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NoteType { Normal, Up, Down, Left, Right }

public class RhythmLaneController : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform laneRect;
    public RectTransform masterArrow; // 師匠
    public RectTransform playerArrow; // プレイヤー
    
    [Header("Prefab")]
    public GameObject notePrefab; // ノーツのプレハブ（Imageコンポーネントを持つUIオブジェクト）
    
    [Header("Sprites")]
    public Sprite normalSprite; // 白丸
    public Sprite upSprite;     // 上
    public Sprite downSprite;   // 下
    public Sprite leftSprite;   // 左
    public Sprite rightSprite;  // 右
    
    [Header("Game Settings")]
    public float totalDuration = 4f; // 1セットの合計時間（秒）
    public float gameSpeed = 1.0f;    // 再生速度
    public float bpm = 90f; // 曲のBPM
    public int beatsPerSet = 4; // 1セットあたりの拍数（基本は4拍＝1小節）

    private float laneWidth;
    private float phaseTime = 0f;     // 現在のフェーズの経過時間
    private int currentRepetition = 1; // 現在何回目のリピートか（1～5）
    private bool isPlayersTurn = false; // 現在プレイヤーの番かどうか

    private Image playerImage;
    private List<GameObject> activeNotes = new List<GameObject>(); // 生成されたノーツのリスト

    void Start()
    {
        // BPMから1セットの合計時間を自動で計算する
        totalDuration = (60f / bpm) * beatsPerSet;
        laneWidth = laneRect.rect.width;

        // プレイヤーのImageを取得して最初は隠す
        playerImage = playerArrow.GetComponent<Image>();
        if (playerImage != null)
        {
            playerImage.enabled = false;
        }

        // 第1回目のランダムノーツを生成
        GenerateRandomNotes();
        Debug.Log($"--- セット 1 / 5 開始 (師匠のターン) ---");
    }

    void Update()
    {
        // 5回すべて終了したら動かさない
        if (currentRepetition > 5) return;

        // 時間を進める
        phaseTime += Time.deltaTime * gameSpeed;

        if (!isPlayersTurn)
        {
            // 【師匠のターン】
            float masterProgress = Mathf.Clamp01(phaseTime / totalDuration);
            masterArrow.anchoredPosition = new Vector2(masterProgress * laneWidth, masterArrow.anchoredPosition.y);

            // 師匠がゴールに達したらプレイヤーのターンへ
            if (masterProgress >= 1.0f)
            {
                isPlayersTurn = true;
                phaseTime = 0f; // タイマーリセット

                if (playerImage != null)
                {
                    playerImage.enabled = true; // プレイヤーを表示
                }
                Debug.Log($"--- セット {currentRepetition} / 5 (プレイヤーのターン) ---");
            }
        }
        else
        {
            // 【プレイヤーのターン】
            float playerProgress = Mathf.Clamp01(phaseTime / totalDuration);
            playerArrow.anchoredPosition = new Vector2(playerProgress * laneWidth, playerArrow.anchoredPosition.y);

            // プレイヤーがゴールに達したら次の周へ
            if (playerProgress >= 1.0f)
            {
                currentRepetition++;
                phaseTime = 0f; // タイマーリセット
                isPlayersTurn = false; // 師匠のターンに戻す

                if (playerImage != null)
                {
                    playerImage.enabled = false; // プレイヤーを再び隠す
                }

                if (currentRepetition <= 5)
                {
                    // 次の周のノーツをランダム生成
                    GenerateRandomNotes();
                    Debug.Log($"--- セット {currentRepetition} / 5 開始 (師匠のターン) ---");
                }
                else
                {
                    Debug.Log("全5回のリピートがすべて終了しました！お疲れ様でした！");
                }
            }
        }
    }

    // リズム（8分割）に合わせてランダムにノーツを生成する関数
    void GenerateRandomNotes()
    {
        // 1. 前の周のノーツを綺麗に削除する
        foreach (var note in activeNotes)
        {
            if (note != null) Destroy(note);
        }
        activeNotes.Clear();

        // 各ステップ（2～8）にすでにノーツがあるかを記録するフラグ
        bool[] isStepOccupied = new bool[9]; 

        //ノーツを配置する確率を決定する
        for (int i = 0; i < 8; i++)
        {
            int step = i + 1; // 1つ目、2つ目...8つ目

            // 1つ目（スタート）にはノーツを配置しない
            if (step == 1 || step == 2) continue;

            // 節は確率を高め(90%)、それ以外は低め(10%)
            float spawnProbability = (step % 2 == 1) ? 0.90f : 0.10f;

            if (Random.value < spawnProbability)
            {
                SpawnSingleNote(i);
                isStepOccupied[step] = true;
            }
        }

        // 最低4つ以上表示
        // ノーツの合計数が4つ未満の間、空いている枠をランダムに選んで埋める
        while (activeNotes.Count < 4)
        {
            List<int> emptySteps = new List<int>();
            
            // 2つ目～8つ目の中で、まだノーツが置かれていないステップをリストアップ
            for (int step = 2; step <= 8; step++)
            {
                if (!isStepOccupied[step])
                {
                    emptySteps.Add(step);
                }
            }

            // 万が一、空き枠がなくなったらループを抜ける
            if (emptySteps.Count == 0) break;

            // 空いているステップからランダムに1つ選ぶ
            int chosenStep = emptySteps[Random.Range(0, emptySteps.Count)];
            int index = chosenStep - 1;

            // ノーツを追加生成
            SpawnSingleNote(index);
            isStepOccupied[chosenStep] = true; 
        }
    }

    // ノーツを1個生成して配置する共通処理
    void SpawnSingleNote(int index)
    {
        // 8等分グリッド上の位置（0.0 ～ 1.0 の間）を計算
        float progress = index / 8f; 

        NoteType randomType;
        float typeRoll = Random.value;
        
        // 白丸の確率 threshold (0.0 ～ 1.0)。低いほど矢印が出やすい。
        float normalNoteThreshold = 0.90f; 

        if (typeRoll < normalNoteThreshold)
        {
            randomType = NoteType.Normal;
        }
        else
        {
            randomType = (NoteType)Random.Range(1, 5);
        }

        // ノーツの生成
        GameObject noteObj = Instantiate(notePrefab, laneRect);
        RectTransform rect = noteObj.GetComponent<RectTransform>();
        
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(progress * laneWidth, 0);

        Image noteImage = noteObj.GetComponent<Image>();
        if (noteImage != null)
        {
            noteImage.sprite = GetNoteSprite(randomType);
        }

        noteObj.transform.SetAsFirstSibling();

        // リストに記録
        activeNotes.Add(noteObj);
    }

    Sprite GetNoteSprite(NoteType type)
    {
        return type switch
        {
            NoteType.Up => upSprite,
            NoteType.Down => downSprite,
            NoteType.Left => leftSprite,
            NoteType.Right => rightSprite,
            _ => normalSprite,
        };
    }
}