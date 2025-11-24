using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class GridManager : MonoBehaviour
{
    public int width = 5;
    public int height = 4;
    public List<Color> listwollColor = new List<Color>();
    public Texture2D lockTexture; // Drag lock.png từ Assets/Image vào đây (Inspector)
    private Material pngMaterial; // Material cho PNG
    private Material lockMaterial; // Material từ PNG
    public class Woll { public Vector3 gridpos; public Color wollColor; public int segment; }
    private List<Woll> wools = new List<Woll>(); // List lưu tất cả Woll (cube data)
    public LineRenderer ropeLine;
    public Color ropeColor = Color.magenta; // Màu dây (hồng/magenta)
    public float ropeWidth = 01f;
    public int numLinePoints = 5;
    public int cubeIndex = 0;
    private float moveSpeed = 5f;
    private float outDistance = 2f;
    [SerializeField] public GameObject woolyarn;
    public int lineRowOrCol = 1; // Hàng (nếu ngang) hoặc cột (nếu dọc) để vẽ line (0-4)
    void Start()
    {
        // Tạo Material từ PNG được gán ở Inspector
        if (lockTexture != null)
        {
            lockMaterial = new Material(Shader.Find("Standard")); // Hoặc "Unlit/Texture" nếu không cần ánh sáng
            lockMaterial.mainTexture = lockTexture; // Gán PNG vào Material
            Debug.Log($"✅ Đã gán PNG '{lockTexture.name}' vào Material lock!");
        }
        else
        {
            Debug.LogWarning("❌ Chưa gán lockTexture ở Inspector! Sử dụng default cho tất cả.");
        }
        if (listwollColor.Count == 0)
        {
            listwollColor.Add(Color.magenta); // Hồng
            listwollColor.Add(Color.cyan); // Xanh ngọc
            listwollColor.Add(Color.yellow); // Vàng
            Debug.Log("Thêm màu mặc định vào listwollColor!");
        }
        GenerateBoard();
        GenerateWoll(0, 1, true, 1);   
        GenerateWoll(0, 3, true, 1);   
        GenerateWoll(0, 0, false, 1);  
        GenerateWoll(2, 0, false, 1); 
        GenerateWoll(4, 0, false, 1);  
    }
    public void GenerateBoard()
    {
        // Đếm cube toàn bộ từ 0 (thứ 1=0, thứ 3=2)
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                cube.name = $"Cube_{i}_{j}"; // Đặt tên dễ nhận biết (tùy chọn)
                cube.transform.position = new Vector3Int(i, 0, j);
                cube.transform.localScale = new Vector3(0.1f, 0.3f, 0.1f); // Scale để vừa khít
                cube.transform.SetParent(transform); // Parent để dễ quản lý
            }
        }
        // Hàng chọn (z=-2) - Tiếp tục đếm index
        for (int i = 0; i < width; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = $"SelectCube_{i}"; // Đặt tên dễ nhận biết
            cube.transform.position = new Vector3Int(i, 0, -2);
            cube.transform.localScale = new Vector3(0.95f, 0.3f, 0.95f);
            cube.transform.SetParent(transform);
            Renderer rend = cube.GetComponent<Renderer>();
            if (cubeIndex >= 2) // Từ cube thứ 3 (nhưng lúc này index đã >20, nên luôn true)
            {
                rend.material = lockMaterial; // Gán PNG
            }
            else
            {
            }
            cubeIndex++; // Tăng đếm
        }
        Debug.Log($"Đã tạo {cubeIndex} cube tổng cộng!"); // Log để kiểm tra
    }
    // Cập nhật Woll class (thay Vector3 bằng Vector3Int)
    // Hàm GenerateWoll sửa (sử dụng Vector3Int)
    public void GenerateWoll(int x, int y, bool isHorizontal, int count)
    {
        if (woolyarn == null)
        {
            Debug.Log($"<color=red>WOLL PREFAB IS NULL</color>");
            return;
        }
        int localIndex = 0; // Index local cho hàm này (để gán màu/lock độc lập)
        if (isHorizontal)
        {
            // Ngang: Tạo wool theo hàng y, từ cột x=0 đến width-1
            for (int i = 0; i < count; i++)
            {
                GameObject wool = Instantiate(woolyarn);
                wool.name = $"Wool_Horizontal_{y}_{i}"; // Tên rõ ràng (hàng y, cột i)
                Vector3Int gridPosInt = new Vector3Int(i, 1, y); // FIX: Vector3Int cho vị trí int
                wool.transform.position = new Vector3(gridPosInt.x, gridPosInt.y, gridPosInt.z); // Chuyển sang Vector3 cho transform
                wool.transform.localScale = new Vector3(1, 1, 1);
                wool.transform.rotation = Quaternion.identity;
                wool.transform.SetParent(transform);
                WollTapHandle wollTapHandle = wool.AddComponent<WollTapHandle>();
                if (wollTapHandle == null) Debug.Log("Taphandle is null");
                // Woll data với Vector3Int
                Woll woll = new Woll();
                woll.gridpos = gridPosInt; // Lưu int coords
                woll.segment = 0;
                woll.wollColor = listwollColor[localIndex % listwollColor.Count]; // Loop màu từ list
                wools.Add(woll);
                // Gán Material
                Renderer rend = wool.GetComponent<Renderer>();
                if (rend != null)
                {
                    if (localIndex >= 2 && lockMaterial != null) // Từ wool thứ 3
                    {
                        rend.material = lockMaterial; // PNG lock
                    }
                    else
                    {
                        Material woolMat = new Material(Shader.Find("Standard"));
                        woolMat.color = woll.wollColor; // Màu wool
                        rend.material = woolMat;
                    }
                }
                Debug.Log($"✅ vị trí woll là {woll.gridpos}."); // Log Vector3Int (sạch int)
                localIndex++; // Tăng index cho mỗi wool
            }
            Debug.Log($"✅ Tạo {width} wool ngang theo hàng {y} (bắt đầu từ cột 0).");
        }
        else
        {
            // Dọc: Tạo wool theo cột x, từ hàng y=0 đến height-1
            for (int j = 0; j < count; j++)
            {
                GameObject wool = Instantiate(woolyarn);
                wool.name = $"Wool_Vertical_{x}_{j}"; // Tên rõ ràng (cột x, hàng j)
                Vector3Int gridPosInt = new Vector3Int(x, 1, j); // FIX: Vector3Int cho vị trí int
                wool.transform.position = new Vector3(gridPosInt.x, gridPosInt.y, gridPosInt.z); // Chuyển sang Vector3
                wool.transform.localScale = new Vector3(1, 1, 1);
                wool.transform.rotation = Quaternion.identity;
                wool.transform.SetParent(transform);
                WollTapHandle wollTapHandle = wool.AddComponent<WollTapHandle>();
                if (wollTapHandle == null) Debug.Log("Taphandle is null");
                // Woll data với Vector3Int
                Woll woll = new Woll();
                woll.gridpos = gridPosInt; // Lưu int coords
                woll.segment = 0;
                woll.wollColor = listwollColor[localIndex % listwollColor.Count]; // Loop màu từ list
                wools.Add(woll);
                // Gán Material
                Renderer rend = wool.GetComponent<Renderer>();
                if (rend != null)
                {
                    if (localIndex >= 2 && lockMaterial != null) // Từ wool thứ 3
                    {
                        rend.material = lockMaterial; // PNG lock
                    }
                    else
                    {
                        Material woolMat = new Material(Shader.Find("Standard"));
                        woolMat.color = woll.wollColor; // Màu wool
                        rend.material = woolMat;
                    }
                }
                Debug.Log($"✅ vị trí woll là {woll.gridpos}."); // Log Vector3Int (sạch int)
                localIndex++; // Tăng index cho mỗi wool
            }
            Debug.Log($"✅ Tạo {height} wool dọc theo cột {x} (bắt đầu từ hàng 0).");
        }
    }
    // Hàm riêng truy cập child trong wool (theo tên)
    public Transform AccessWoolChild(GameObject woolRoot, string childName)
    {
        if (woolRoot == null)
        {
            Debug.LogWarning("Wool root GO null!");
            return null;
        }
        Transform child = woolRoot.transform.Find(childName); // Tìm child theo tên (case-sensitive)
        if (child == null)
        {
            Debug.LogWarning($"Không tìm thấy child '{childName}' trong wool {woolRoot.name}. Check tên trong prefab!");
            return null;
        }
        Debug.Log($"✅ Truy cập child '{childName}' thành công trong {woolRoot.name} (position: {child.localPosition}).");
        return child;
    }
    public Vector3 GetDirectionFromPrism(Transform line, Transform prism)
    {
        LineRenderer lr = line.GetComponent<LineRenderer>();
        Vector3 p0 = lr.GetPosition(0);
        Vector3 p1 = lr.GetPosition(1);
        Vector3 mid = (p0 + p1) / 2f;
        Vector3 prismPos = prism.position;
        float dx = Mathf.Abs(p1.x - p0.x);
        float dz = Mathf.Abs(p1.z - p0.z);
        // Horizontal
        if (dx > dz)
        {
            if (prismPos.x > mid.x)
            {
                Debug.Log("Line đang là horizonal Prism đang nằm bên phải line");
                return Vector3.right; // 👉 phải
            }
            else
            {
                Debug.Log("Line đang là horizonal Prism đang nằm bên trái line");
                return Vector3.left; // 👈 trái
            }

        }
        // Vertical
        else
        {
            if (prismPos.z > mid.z)
            {
                Debug.Log("Line đang là vertical Prism đang nằm trên line");
                return Vector3.forward; // ⬆ trên
            }
            else
            {
                Debug.Log("Line đang là vertical Prism đang nằm dưới line");
                return Vector3.back; // ⬇ dưới
            }
        }
    }
    public IEnumerator MoveOutOfGrid(Transform wollTranform, Vector3 direction)
    {
        Vector3 startPos = wollTranform.position;
        Vector3 targetPos = startPos + direction * outDistance; // Di chuyển ra ngoài grid (2 units)
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * moveSpeed;
            wollTranform.position = Vector3.Lerp(startPos, targetPos, elapsed); // Di chuyển mượt
            yield return null;
        }
        // Đến đích: Destroy wool
        Debug.Log($"Wool {gameObject.name} đã đi ra ngoài grid theo hướng {direction}!");
        Destroy(wollTranform.gameObject);
    }
    void Update()
    {
    }
}
