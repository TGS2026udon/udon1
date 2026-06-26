using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    // インスペクターから音源ファイルを割り当てるための変数
    public AudioClip spaceSound;

    private AudioSource audioSource;

    void Start()
    {
        // 自身にアタッチされているAudioSourceコンポーネントを取得
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // スペースキーが押された瞬間を検知
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 音を1回だけ再生（重なりを許容する再生方法）
            audioSource.PlayOneShot(spaceSound);
        }
    }
}