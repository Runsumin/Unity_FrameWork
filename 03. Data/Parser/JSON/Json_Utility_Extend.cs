using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace LOBS
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // Json_Utility_Extend
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static class Json_Utility_Extend
    {
        [Serializable]
        private class JsonWrapper<T>
        {
            public List<T> datas;
        }

        /// <summary>
        /// ���� ����
        /// </summary>
        /// <typeparam name="T">Ŭ���� Ÿ��</typeparam>
        /// <param name="data">������</param>
        /// <param name="path">���</param>
        public static void FileSave<T>(T data, string path)
        {
            JsonWrapper<T> wrapper = new JsonWrapper<T>();
            wrapper.datas = new List<T> { data };
            string json = JsonUtility.ToJson(wrapper);
            json = PrettyPrintJson(json);
            if (!path.StartsWith('/')) path = "/" + path;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            File.WriteAllText(Application.streamingAssetsPath + path, json, Encoding.UTF8);
#elif UNITY_ANDROID
            File.WriteAllText(Application.persistentDataPath + path, json, Encoding.UTF8);
#endif
            //AssetDatabase.Refresh();
        }

        /// <summary>
        /// ���� �ҷ�����
        /// </summary>
        /// <typeparam name="T">Ŭ���� Ÿ��</typeparam>
        /// <param name="path">���</param>
        /// <returns>������ ��ȯ</returns>
        public static T FileLoad<T>(string path)
        {
            if (!path.StartsWith('/')) path = "/" + path;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            string json = File.ReadAllText(Application.streamingAssetsPath + path, Encoding.UTF8);
#elif UNITY_ANDROID
            string json = File.ReadAllText(Application.persistentDataPath + path, Encoding.UTF8);
#endif
            JsonWrapper<T> wrapper = JsonUtility.FromJson<JsonWrapper<T>>(json);
            return wrapper.datas[0];
        }

        /// <summary>
        /// ����Ʈ Ÿ�� ����
        /// </summary>
        /// <typeparam name="T">Ŭ���� Ÿ��</typeparam>
        /// <param name="datas">������ ����Ʈ</param>
        /// <param name="path">���</param>
        public static void FileSaveList<T>(List<T> datas, string path)
        {
            JsonWrapper<T> wrapper = new JsonWrapper<T>();
            wrapper.datas = datas;
            string json = JsonUtility.ToJson(wrapper);
            json = PrettyPrintJson(json);
            if (!path.StartsWith('/')) path = "/" + path;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            File.WriteAllText(Application.streamingAssetsPath + path, json, Encoding.UTF8);
#elif UNITY_ANDROID
            File.WriteAllText(Application.persistentDataPath + path, json, Encoding.UTF8);
#endif
            //AssetDatabase.Refresh();
        }

        /// <summary>
        /// ����Ʈ Ÿ�� �ҷ�����
        /// </summary>
        /// <typeparam name="T">Ŭ���� Ÿ��</typeparam>
        /// <param name="path">���</param>
        /// <returns>������ ����Ʈ ��ȯ</returns>
        //        public static List<T> FileLoadList<T>(string path)
        //        {
        //            //            if (!path.StartsWith('/')) path = "/" + path;
        //            //#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        //            //            string json = File.ReadAllText(Application.streamingAssetsPath + path, Encoding.UTF8);
        //            //#elif UNITY_ANDROID
        //            //            string json = File.ReadAllText(Application.persistentDataPath + path, Encoding.UTF8);
        //            //#endif
        //            //            JsonWrapper<T> wrapper = JsonUtility.FromJson<JsonWrapper<T>>(json);
        //            //            return wrapper.datas;

        //            string filePath;
        //#if UNITY_EDITOR || UNITY_STANDALONE
        //            filePath = Path.Combine(Application.streamingAssetsPath, path);
        //#elif UNITY_ANDROID
        //    filePath = Path.Combine("jar:file://" + Application.dataPath + "!/assets/", path + ".Json");
        //#elif UNITY_IOS
        //    filePath = Path.Combine(Application.streamingAssetsPath, path);
        //#else
        //    filePath = null;
        //#endif

        //            string jsonData;
        //#if UNITY_ANDROID && !UNITY_EDITOR
        //    if (filePath.StartsWith("jar:"))
        //    {
        //        UnityWebRequest www = UnityWebRequest.Get(filePath);
        //        www.SendWebRequest();
        //        while (!www.isDone) { }
        //        jsonData = www.downloadHandler.text;
        //    }
        //    else
        //    {
        //        jsonData = File.ReadAllText(filePath);
        //    }
        //#else
        //            jsonData = File.ReadAllText(filePath);
        //#endif

        //            JsonWrapper<T> wrapper = JsonUtility.FromJson<JsonWrapper<T>>(jsonData);
        //            return wrapper.datas;
        //        }

        public static List<T> FileLoadList<T>(string path)
        {
            if (path.StartsWith("/")) path = path.Substring(1);

            string json = "";
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            string fullPath = Path.Combine(Application.streamingAssetsPath, path);
            if (!File.Exists(fullPath))
            {
                Debug.LogError("������ �������� �ʽ��ϴ�: " + fullPath);
                return null;
            }
            json = File.ReadAllText(fullPath, Encoding.UTF8);
#elif UNITY_ANDROID
    string filePath = Path.Combine(Application.streamingAssetsPath, path);
    using (UnityWebRequest www = UnityWebRequest.Get(filePath))
    {
        www.SendWebRequest();
        while (!www.isDone) { } // ���������� ��ٸ�
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("������ ���� �� �����ϴ�: " + www.error);
            return null;
        }
        json = www.downloadHandler.text;
    }
#endif
            JsonWrapper<T> wrapper = JsonUtility.FromJson<JsonWrapper<T>>(json);
            return wrapper.datas;
        }

        /// <summary>
        /// Json ������
        /// </summary>  
        /// <param name="json">json ������ �ؽ�Ʈ</param>
        /// <returns>������ json �ؽ�Ʈ</returns>
        private static string PrettyPrintJson(string json)
        {
            const int indentSpaces = 4;
            int indent = 0;
            bool quoted = false;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < json.Length; i++)
            {
                char ch = json[i];

                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            sb.Append(new string(' ', ++indent * indentSpaces));
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            sb.Append(new string(' ', --indent * indentSpaces));
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        int index = i;
                        while (index > 0 && json[--index] == '\\')
                        {
                            escaped = !escaped;
                        }
                        if (!escaped)
                        {
                            quoted = !quoted;
                        }
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            sb.Append(new string(' ', indent * indentSpaces));
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.Append(" ");
                        }
                        break;
                    default:
                        if (quoted || !char.IsWhiteSpace(ch))
                        {
                            sb.Append(ch);
                        }
                        break;
                }
            }

            return sb.ToString();
        }
    }

}
