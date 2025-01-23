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

        // �ڽ� ������Ʈ�� �迭�� ������ �̸����� ����
        Transform[] children = new Transform[parent.childCount];
        for (int i = 0; i < parent.childCount; i++)
        {
            children[i] = parent.GetChild(i);
        }

        System.Array.Sort(children, (a, b) => string.Compare(a.name, b.name));

        // ���ĵ� ������� ������
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetSiblingIndex(i);
        }

        // ��� ȣ��
        foreach (Transform child in children)
        {
            SortChildrenByName(child);
        }
    }
}
