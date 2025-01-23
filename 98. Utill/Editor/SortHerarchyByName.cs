using UnityEditor;
using UnityEngine;

public class SortHerarchyByName : MonoBehaviour
{
    [MenuItem("LOBS/Tools/Sort Hierarchy By Name")]
    private static void SortHierarchy()
    {
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject root in rootObjects)
        {
            SortChildrenByName(root.transform);
        }
        Debug.Log("Hierarchy sorted by name!");
    }

    private static void SortChildrenByName(Transform parent)
    {
        if (parent.childCount <= 1) return;

        // 자식 오브젝트를 배열로 가져와 이름으로 정렬
        Transform[] children = new Transform[parent.childCount];
        for (int i = 0; i < parent.childCount; i++)
        {
            children[i] = parent.GetChild(i);
        }

        System.Array.Sort(children, (a, b) => string.Compare(a.name, b.name));

        // 정렬된 순서대로 재정렬
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetSiblingIndex(i);
        }

        // 재귀 호출
        foreach (Transform child in children)
        {
            SortChildrenByName(child);
        }
    }
}
