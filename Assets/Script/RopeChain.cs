using UnityEngine;

public class RopeChain : MonoBehaviour
{
    [Header("Tùy Chỉnh Số Lượng & Màu Sắc")]
    [Range(2, 20)] public int numPoints = 5;  // Số điểm cho line (tăng để dây dài/zig-zag hơn, min 2)
    public Color ropeColor = Color.red;       // Màu cho đường dây (mặc định đỏ/hồng)

    [Header("Kích Thước")]
    public float lineLength = 3f;             // Chiều dài tổng dây (Y)
    public float connectorWidth = 0.2f;      // Độ dày đường (mỏng như dây)

    void Start()
    {
        GenerateRopeLine();
    }

    public void GenerateRopeLine()
    {
        if (numPoints < 2)
        {
            Debug.LogWarning("Số điểm phải >= 2 để tạo line!");
            numPoints = 2;  // Fallback
        }

        // Tạo LineRenderer (bỏ cube, chỉ line)
        LineRenderer lineRend = gameObject.AddComponent<LineRenderer>();
        Material ropeMat = new Material(Shader.Find("Sprites/Default"));  // Shader mỏng cho dây
        ropeMat.color = ropeColor;  // Set màu dây từ Inspector
        lineRend.material = ropeMat;
        lineRend.startWidth = connectorWidth;
        lineRend.endWidth = connectorWidth;
        lineRend.useWorldSpace = true;
        lineRend.positionCount = numPoints;  // Số điểm = numPoints (kéo dài line)

        // Set positions cho LineRenderer (zig-zag dọc Y, tổng chiều dài lineLength)
        float stepY = lineLength / (numPoints - 1);  // Khoảng cách Y giữa điểm
        for (int i = 0; i < numPoints; i++)
        {
            float zigZagX = Mathf.Sin(i * Mathf.PI / 3) * 0.1f;  // Zig-zag nhẹ theo X
            float zigZagZ = Mathf.Cos(i * Mathf.PI / 3) * 0.05f;  // Zig-zag theo Z
            Vector3 pos = new Vector3(zigZagX, i * stepY, zigZagZ);  // Từ dưới lên
            lineRend.SetPosition(i, pos);
        }

        // Bo góc mượt
        lineRend.numCapVertices = 3;

        Debug.Log($"Đã tạo sợi dây line với {numPoints} điểm (màu: {ropeColor}, dài: {lineLength})!");
    }
}