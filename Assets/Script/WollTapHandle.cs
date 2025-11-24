using System.Collections;
using UnityEngine;

public class WollTapHandle : MonoBehaviour
{
    private GridManager gridManager; 
  

    void Start()
    {
        // Tìm GridManager nếu cần (hoặc pass từ GenerateWoll)
        gridManager = FindObjectOfType<GridManager>();
    }

    private void OnMouseDown()
    {
        Transform woolTransform = this.transform;
        Debug.Log("Tap wool: " + gameObject.name);

        // Xác định hướng mũi tên dựa tên GO (Horizontal: phải +X, Vertical: xuống +Z)
        bool isHorizontal = gameObject.name.Contains("Horizontal");
        Vector3 direction = isHorizontal ? Vector3.right : Vector3.forward;  // Phải (+X) hoặc Xuống (+Z)

        // truy cập child của woll
        Transform prismChild = transform.Find("Prism");  // Tìm child "Prism"
        if (prismChild != null)
        {
            Debug.Log($"✅ Truy cập prismChild thành công trong {gameObject.name} (position: {woolTransform.localPosition})");

            // Ví dụ thao tác: Gán màu mới cho Prism khi tap
            Renderer prismRend = prismChild.GetComponent<Renderer>();
            if (prismRend != null)
            {
                Material newMat = new Material(Shader.Find("Standard"));
                //newMat.color = Color.green;  // Màu xanh khi tap
                prismRend.material = newMat;
                Debug.Log("Đã đổi màu Prism thành xanh!");
            }

        }
        else
        {
            Debug.LogWarning($"❌ Không tìm thấy child 'Prism' trong {gameObject.name} – Check tên prefab!");
        }

        Transform lineChild = transform.Find("Line");  // Tìm child "Prism"
        if (lineChild != null)
        {
            Debug.Log($"✅ Truy cập lineChild thành công trong {gameObject.name} (position: {lineChild.localPosition})");

            // Ví dụ thao tác: Gán màu mới cho Prism khi tap
            Renderer lineRend = lineChild.GetComponent<Renderer>(); 
            if (lineRend != null)
            {
                Material newMat = new Material(Shader.Find("Standard"));
                //newMat.color = Color.green;  // Màu xanh khi tap
                lineRend.material = newMat;

                LineRenderer lineRenderer = lineChild.GetComponent<LineRenderer>();
            }

         
        }
        else
        {
            Debug.LogWarning($"❌ Không tìm thấy child 'Line' trong {gameObject.name} – Check tên prefab!");
        }

        Vector3 directionWoll = gridManager.GetDirectionFromPrism(lineChild, prismChild);
        StartCoroutine(gridManager.MoveOutOfGrid(woolTransform, directionWoll));
    }

   
}

//public IEnumerator MoveOutOfGrid(Vector3 direction)
//{
//    Vector3 startPos = transform.position;
//    Vector3 targetPos = startPos + direction * outDistance;  // Di chuyển ra ngoài grid (2 units)

//    float elapsed = 0f;
//    while (elapsed < 1f)
//    {
//        elapsed += Time.deltaTime * moveSpeed;
//        transform.position = Vector3.Lerp(startPos, targetPos, elapsed);  // Di chuyển mượt
//        yield return null;
//    }

//    // Đến đích: Destroy wool
//    Debug.Log($"Wool {gameObject.name} đã đi ra ngoài grid theo hướng {direction}!");
//    Destroy(gameObject);
//}