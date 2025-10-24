using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SineWave : MonoBehaviour
{
    public Vector2 pos = new Vector2(-10f, 0);

    public int points = 100;        // 점 개수 (해상도)
    public float amplitude = 1f;    // 진폭
    public float frequency = 1f;    // 주파수
    public float length = 10f;      // x축 길이
    public float speed = 1f;        // 애니메이션 속도

    // make it random parameter
    public float randomAmplitudeX = 0.5f;
    public float randomAmplitudeY = 1.3f;

    public float randomFrequencyX = 1.7f;
    public float randomFrequencyY = 2.3f;

    public float randomSpeedX = 0.7f;
    public float randomSpeedY = 1.3f;

    private float timer = 0f;
    private float changeTimeLimit = 2.5f;

    private LineRenderer lr;

    // 누적 스크롤 (위상)
    private float scrollX = 0f;

    // ====== 추가: 부드러운 전환을 위한 내부 상태 ======
    private Coroutine smoothCR;
    private const float rampDuration = 2f; // 전환(보간) 시간(초)

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = points;
    }

    void Update()
    {
        // 누적 스크롤: Time.time 대신 speed 적분
        scrollX += speed * Time.deltaTime;

        DrawSineWave();
        RandomShow();
    }

    void DrawSineWave()
    {
        for (int i = 0; i < points; i++)
        {
            float x = (i / (float)(points - 1)) * length;
            float y = amplitude * Mathf.Sin((x * frequency) + scrollX);
            lr.SetPosition(i, new Vector3(pos.x + x, pos.y + y, 0));
        }
    }

    void RandomShow()
    {
        timer += Time.deltaTime;

        if (timer >= changeTimeLimit)
        {
            timer = 0;

            float randomSpeed = Random.Range(randomSpeedX, randomSpeedY);
            float randomAmplitute = Random.Range(randomAmplitudeX, randomAmplitudeY);
          

            // ====== 변경: 즉시 치환 대신 부드럽게 올리기/내리기 ======
            if (smoothCR != null) StopCoroutine(smoothCR);
            smoothCR = StartCoroutine(SmoothToTargets(randomSpeed, randomAmplitute,rampDuration));
        }
    }

    // ====== 추가: 연속(continuous) 보간 코루틴 ======
    System.Collections.IEnumerator SmoothToTargets(float targetSpeed, float targetAmp, float duration)
    {
        float t = 0f;

        float startSpeed = speed;
        float startAmp = amplitude;

        // duration이 0이거나 아주 작아도 안전하게
        duration = Mathf.Max(0.0001f, duration);

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = t / duration;

            // 부드러운 보간(스무스스텝 느낌을 원하면 k*k*(3-2k) 등으로 바꿔도 됨)
            speed = Mathf.Lerp(startSpeed, targetSpeed, k);
            amplitude = Mathf.Lerp(startAmp, targetAmp, k);
  

            yield return null;
        }

        // 마지막 값 스냅 정렬
        speed = targetSpeed;
        amplitude = targetAmp;
  

        smoothCR = null;
    }
}
