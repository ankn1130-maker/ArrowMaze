using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 5;
    public int height = 4;
    public List<Color> listwollColor = new List<Color>();
    public Texture2D lockTexture;  // Drag lock.png từ Assets/Image vào đây (Inspector)
    private Material pngMaterial;  // Material cho PNG
    private Material lockMaterial;  // Material từ PNG

    public class Woll { public Vector3 gridpos ; public Color wollColor; public int segment; }
    private List<Woll> wools = new List<Woll>();  // List lưu tất cả Woll (cube data)
    public LineRenderer ropeLine;
    public Color ropeColor = Color.magenta;       // Màu dây (hồng/magenta)
    public float ropeWidth = 01f;
    public int numLinePoints = 5;
    public int cubeIndex = 0;
    [SerializeField] public GameObject woolyarn;

    public int lineRowOrCol = 1;      // Hàng (nếu ngang) hoặc cột (nếu dọc) để vẽ line (0-4)
    void Start()
    {
        // Tạo Material từ PNG được gán ở Inspector
        if (lockTexture != null)
        {
            lockMaterial = new Material(Shader.Find("Standard"));  // Hoặc "Unlit/Texture" nếu không cần ánh sáng
            lockMaterial.mainTexture = lockTexture;  // Gán PNG vào Material
            Debug.Log($"✅ Đã gán PNG '{lockTexture.name}' vào Material lock!");
        }
        else
        {
            Debug.LogWarning("❌ Chưa gán lockTexture ở Inspector! Sử dụng default cho tất cả.");
           
        }

        if (listwollColor.Count == 0)
        {
            listwollColor.Add(Color.magenta);  // Hồng
            listwollColor.Add(Color.cyan);     // Xanh ngọc
            listwollColor.Add(Color.yellow);   // Vàng
            Debug.Log("Thêm màu mặc định vào listwollColor!");
        }

        GenerateBoard();
        GenerateWoll(2,0,true);
        //GenerateWoll(3, 5, false);
    }

    public void GenerateBoard()
    {
        // Đếm cube toàn bộ từ 0 (thứ 1=0, thứ 3=2)
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                cube.name = $"Cube_{i}_{j}";  // Đặt tên dễ nhận biết (tùy chọn)
                cube.transform.position = new Vector3(i, 0, j);
                cube.transform.localScale = new Vector3(0.1f, 0.3f, 0.1f);  // Scale để vừa khít
                cube.transform.SetParent(transform);  // Parent để dễ quản lý
            }
        }
        // Hàng chọn (z=-2) - Tiếp tục đếm index
        for (int i = 0; i < width; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = $"SelectCube_{i}";  // Đặt tên dễ nhận biết
            cube.transform.position = new Vector3(i, 0, -2);
            cube.transform.localScale = new Vector3(0.95f, 0.3f, 0.95f);
            cube.transform.SetParent(transform);

            Renderer rend = cube.GetComponent<Renderer>();
            if (cubeIndex >= 2)  // Từ cube thứ 3 (nhưng lúc này index đã >20, nên luôn true)
            {
                rend.material = lockMaterial;  // Gán PNG
            }
            else
            {
                
            }

            cubeIndex++;  // Tăng đếm
        }
        Debug.Log($"Đã tạo {cubeIndex} cube tổng cộng!");  // Log để kiểm tra
    }

    public void GenerateWoll(int x, int y, bool isHorizontal)
    {
        if (woolyarn == null)
        {
            Debug.Log($"<color=red>WOLL PREFAB IS NULL</color>");
            return;
        }

        int localIndex = 0;  // Index local cho hàm này (để gán màu/lock độc lập)

        if (isHorizontal)
        {
            // Ngang: Tạo wool theo hàng y, từ cột x=0 đến width-1 (bắt đầu từ vị trí x gốc nếu cần điều chỉnh)
            for (int i = 0; i < width; i++)
            {
                GameObject wool = Instantiate(woolyarn);
                wool.name = $"Wool_Horizontal_{y}_{i}";  // Tên rõ ràng (hàng y, cột i)
                wool.transform.position = new Vector3(i, 0, y);  // Vị trí ngang (x=i, z=y cố định)
                wool.transform.localScale = new Vector3(1, 1, 1);
                wool.transform.rotation = Quaternion.identity;
                wool.transform.SetParent(transform);

                // Woll data
                Woll woll = new Woll();
                woll.gridpos = new Vector3(i, 0, y);
                woll.segment = 0;
                woll.wollColor = listwollColor[localIndex % listwollColor.Count];  // Loop màu từ list
                wools.Add(woll);

                // Gán Material
                Renderer rend = wool.GetComponent<Renderer>();
                if (rend != null)
                {
                    if (localIndex >= 2 && lockMaterial != null)  // Từ wool thứ 3
                    {
                        rend.material = lockMaterial;  // PNG lock
                    }
                    else
                    {
                        Material woolMat = new Material(Shader.Find("Standard"));
                        woolMat.color = woll.wollColor;  // Màu wool
                        rend.material = woolMat;
                    }
                }

                localIndex++;  // Tăng index cho mỗi wool
            }
            Debug.Log($"✅ Tạo {width} wool ngang theo hàng {y} (bắt đầu từ cột 0).");
        }
        else
        {
            // Dọc: Tạo wool theo cột x, từ hàng y=0 đến height-1 (bắt đầu từ vị trí y gốc nếu cần điều chỉnh)
            for (int j = 0; j < height; j++)
            {
                GameObject wool = Instantiate(woolyarn);
                wool.name = $"Wool_Vertical_{x}_{j}";  // Tên rõ ràng (cột x, hàng j)
                wool.transform.position = new Vector3(x, 0, j);  // Vị trí dọc (x cố định, z=j)
                wool.transform.localScale = new Vector3(1, 1, 1);
                wool.transform.SetParent(transform);

                // Woll data
                Woll woll = new Woll();
                woll.gridpos = new Vector3(x, 0, j);
                woll.segment = 0;
                woll.wollColor = listwollColor[localIndex % listwollColor.Count];  // Loop màu từ list
                wools.Add(woll);

                // Gán Material
                Renderer rend = wool.GetComponent<Renderer>();
                if (rend != null)
                {
                    if (localIndex >= 2 && lockMaterial != null)  // Từ wool thứ 3
                    {
                        rend.material = lockMaterial;  // PNG lock
                    }
                    else
                    {
                        Material woolMat = new Material(Shader.Find("Standard"));
                        woolMat.color = woll.wollColor;  // Màu wool
                        rend.material = woolMat;
                    }
                }

                localIndex++;  // Tăng index cho mỗi wool
            }
            Debug.Log($"✅ Tạo {height} wool dọc theo cột {x} (bắt đầu từ hàng 0).");
        }
    }

    //public void GenerateRopeChainOnGrid( bool isHorizontal , int rowCol)
    //{

    //    int segment = numLinePoints - 3;
    //    GameObject lineGO = new GameObject("RopeLine_" + (isHorizontal ? "Horizontal" : "Vertical") + "_" + rowCol);
    //    lineGO.transform.SetParent(transform);  // Parent với GridManager
    //    lineGO.transform.localPosition = Vector3.zero;  // Vị trí tương đối

    //    ropeLine = lineGO.AddComponent<LineRenderer>();
    //    Material ropeMat = new Material(Shader.Find("Sprites/Default"));  // Shader mỏng cho dây
    //    ropeMat.color = ropeColor;
    //    ropeLine.material = ropeMat;
    //    ropeLine.startWidth = ropeWidth;
    //    ropeLine.endWidth = ropeWidth;
    //    ropeLine.useWorldSpace = true;
    //    ropeLine.positionCount = segment;

    //    if (isHorizontal)
    //    {
    //        // Ngang: Theo hàng lineRowOrCol (z = lineRowOrCol + 0.5f), x từ 0.5 đến 4.5
    //        float zPos = lineRowOrCol ;  // Giữa khe hàng (ví dụ: hàng 1 → z=1.5)
    //        float stepX = (width - 1f) / (segment - 1);  // Khoảng cách X
    //        for (int i = 0; i < segment; i++)
    //        {
    //            float x =  i * stepX;  // X giữa khe (0.5, 1.5, ..., 4.5)
    //            float zigZagY = Mathf.Sin(i * Mathf.PI / segment) * 0.05f;  // Zig nhẹ theo Y nếu muốn
    //            Vector3 pos = new Vector3(x, zigZagY, zPos);  // Y=0 mặc định, z giữa hàng
    //            ropeLine.SetPosition(i, pos);
    //        }
    //    }
    //    else
    //    {
    //        // Dọc: Theo cột lineRowOrCol (x = lineRowOrCol + 0.5f), z từ 0.5 đến 3.5
    //        float xPos = lineRowOrCol ;  // Giữa khe cột (ví dụ: cột 2 → x=2.5)
    //        float stepZ = (height - 1f) / (segment - 1);  // Khoảng cách Z
    //        for (int i = 0; i < segment; i++)
    //        {
    //            float z = i * stepZ;  // Z giữa khe (0.5, 1.5, ..., 3.5)
    //            float zigZagY = Mathf.Sin(i * Mathf.PI / segment) * 0.05f;  // Zig nhẹ theo Y
    //            Vector3 pos = new Vector3(xPos, zigZagY, z);  // X giữa cột, y=0
    //            ropeLine.SetPosition(i, pos);
    //        }
    //    }

    //    ropeLine.numCapVertices = 3;  // Bo góc mượt
    //}

    void Update()
    {

    }
}