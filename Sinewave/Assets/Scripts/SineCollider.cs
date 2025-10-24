using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class SineCollider : MonoBehaviour
{
    LineRenderer lr;
    EdgeCollider2D edge;

    // 버퍼 재사용(할당 최소화)
    Vector3[] lineBuf;   // LineRenderer 점(3D)
    Vector2[] collBuf;   // EdgeCollider2D 점(2D)

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        edge = GetComponent<EdgeCollider2D>();

        // Rigidbody2D는 콜라이더 안정화를 위해 Kinematic 권장
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        edge.isTrigger = true;

        // 초기 버퍼 확보
        AllocateBuffers();
    }

    void OnValidate()
    {
        // 에디터에서 points 바뀌었을 수도 있으니 버퍼 재확보
        if (lr != null) AllocateBuffers();
    }

    void AllocateBuffers()
    {
        int n = Mathf.Max(2, lr.positionCount);
        if (lineBuf == null || lineBuf.Length != n) lineBuf = new Vector3[n];
        if (collBuf == null || collBuf.Length != n) collBuf = new Vector2[n];
    }

    void Update()
    {
        // 렌더러 점 수 변동 대비
        if (lr.positionCount != (lineBuf?.Length ?? 0))
            AllocateBuffers();
    }

    void FixedUpdate()
    {
        if (lr.positionCount < 2) return;

        // 1) 라인 실제 좌표 가져오기
        lr.GetPositions(lineBuf);

        // 2) EdgeCollider2D는 로컬좌표를 사용 → 변환 필요할 수 있음
        if (lr.useWorldSpace)
        {
            for (int i = 0; i < lineBuf.Length; i++)
                collBuf[i] = transform.InverseTransformPoint(lineBuf[i]);
        }
        else
        {
            for (int i = 0; i < lineBuf.Length; i++)
                collBuf[i] = new Vector2(lineBuf[i].x, lineBuf[i].y);
        }

        // 3) 콜라이더 동기화
        edge.points = collBuf;
    }

    // 요청대로: 충돌 시 Debug.Log만
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[SineCollider] Trigger with {other.name} at t={Time.time:0.000}");
    }

    // 트리거 대신 충돌 이벤트를 쓰고 싶으면 isTrigger=false로 바꾸고 이 콜백 사용
    void OnCollisionEnter2D(Collision2D other)
    {
        if (!edge.isTrigger)
            Debug.Log($"[SineCollider] Collision with {other.collider.name} at t={Time.time:0.000}");
    }
}
