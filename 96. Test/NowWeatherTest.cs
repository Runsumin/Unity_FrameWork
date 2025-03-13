// C# ���� �ڵ�
using System;
using System.Net;
using System.Net.Http;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


namespace LOBS
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //  
    // NowWeatherTest
    // �ǽð����� ���� ������ ������ Ŭ���� 
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class NowWeatherTest : MonoBehaviour
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Nested Class
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Nested] Setting
        [Serializable]
        public class NSetting
        {
            public int Time;            // ����ð�
            public string Base_Data;    // yyyy.mm.dd
            public string Base_Time;    // ex) 1300 2300
            public string URL;          // �������������� url
            public string TimeZone;      // �ð���
            public string SerialKey;
        }
        public NSetting Setting = new NSetting();
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Variable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Variable] Base
        // 0100, 0200 -> 2300
        DateTime exception_base_time = DateTime.Now.AddDays(-1);
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 0. Base Methods
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Init] Awake
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Awake()
        {
            Setting.Time = int.Parse(DateTime.Now.ToString("HH"));
            Setting.Base_Data = DateTime.Now.ToString("yyyyMMdd");
        }
        #endregion

        #region [Init] Start
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Start()
        {
            Get_Base_Time(Setting.Time); //base_Time
            Get_TimeZone(Setting.Time);
            Geturl();

            StartCoroutine(LoadData());

            Debug.Log("Time:" + Setting.Time + "base_Date:" + Setting.Base_Data + "base_Time:" + Setting.Base_Time);
            Debug.Log("TimeZone:" + Setting.TimeZone);
        }
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. Enumrator
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Enumrator] LoadData;
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        IEnumerator LoadData()
        {
            UnityWebRequest www = UnityWebRequest.Get(Setting.URL);

            yield return www.SendWebRequest();
            if (www.error == null) 
            {
                Debug.Log(www.downloadHandler.text);
            }

            XmlDocument xml = new XmlDocument();
            xml.Load(Setting.URL);
            XmlNodeList xmResponse = xml.GetElementsByTagName("response");  
            XmlNodeList xmlList = xml.GetElementsByTagName("item");

            foreach (XmlNode node in xmResponse)
            {
                if (node["header"]["resultMsg"].InnerText.Equals("NORMAL_SERVICE")) 
                {
                    foreach (XmlNode node1 in xmlList)  
                    {
                        if (node1["fcstTime"].InnerText.Equals(Setting.TimeZone))
                        {
                            if (node1["category"].InnerText.Equals("SKY"))  // �ϴû���
                            {
                                switch (node1["fcstValue"].InnerText)
                                {
                                    case "1":
                                        Debug.Log("����");
                                        break;
                                    case "3":
                                        Debug.Log("��������");
                                        break;
                                    case "4":
                                        Debug.Log("�帲");
                                        break;
                                    default:
                                        Debug.Log("�ش��ϴ� �ڷᰡ ����");
                                        break;
                                }
                            }

                            if (node1["category"].InnerText.Equals("PTY"))  // ��������
                            {
                                switch (node1["fcstValue"].InnerText)
                                {
                                    case "0":
                                        Debug.Log("����");
                                        break;
                                    case "1":
                                        Debug.Log("��");
                                        break;
                                    case "2":
                                        //��/��/��������
                                        Debug.Log("��/��/��������");
                                        break;
                                    case "3":
                                        Debug.Log("��");
                                        break;
                                    case "4":
                                        Debug.Log("�ҳ���");
                                        break;
                                    default:
                                        Debug.Log("�ش��ϴ� �ڷᰡ �����ϴ�.");
                                        break;
                                }
                            }

                            if (node1["category"].InnerText.Equals("TMP"))  // ���� ��� �ҷ��� 
                            {
                                //Debug.Log("TMP ��"+ node1["fcstDate"].InnerText);
                                Debug.Log("���� TMP:" + node1["fcstValue"].InnerText);
                            }
                        }

                    }
                }
                else
                {
                    string apiErrorMsg = String.Empty;

                    // API ���� ���� �޼��� ����
                    apiErrorMsg = node["header"]["resultMsg"].InnerText switch
                    {
                        "APPLICATION_ERROR" => "���ø����̼� ����",
                        "DB_ERROR" => "�����ͺ��̽� ����",
                        "NODATA_ERROR" => "������ ����",
                        "HTTP_ERROR" => "HTTP ����",
                        "SERVICETIME_OUT" => "���� �������",
                        "INVALID_REQUEST_PARAMETER_ERROR" => "�߸��� ��û �Ķ����",
                        "NO_MANDATORY_REQUEST_PARAMETERS_ERROR" => "�ʼ���û �Ķ���Ͱ� ����",
                        "NO_OPENAPI_SERVICE_ERROR" => "�ش� ���� API���񽺰� ���ų� ����",
                        "SERVICE_ACCESS_DENIED_ERROR" => "���� ���� �ź�",
                        "TEMPORARILY_DISABLE_THE_SERVICEKEY_ERROR" => "�Ͻ������� ����� �� ���� ���� Ű",
                        "LIMITED_NUMBER_OF_SERVICE_REQUESTS_EXCEEDS_ERROR" => "���� ��û����Ƚ�� �ʰ�",
                        "SERVICE_KEY_IS_NOT_REGISTERED_ERROR" => "��ϵ��� ���� ���� Ű",
                        "DEADLINE_HAS_EXPIRED_ERROR" => "���� ����� ���� Ű",
                        "UNREGISTERED_IP_ERROR" => "��ϵ��� ���� IP",
                        "UNSIGNED_CALL_ERROR" => "������� ���� ȣ��",
                        "UNKNOWN_ERROR" => "��Ÿ����",
                        _ => "�ش��ϴ� ������ �������� ����",
                    };

                    // API ���� ���� �޼��� ���
                    Debug.Log(apiErrorMsg);
                }
            }
        }
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 99. Utill
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Utill] Get_Base_Time
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Get_Base_Time(int time)
        {
            switch (time)
            {
                case 3:
                case 4:
                case 5:
                    Setting.Base_Time = "0200";
                    break;
                case 6:
                case 7:
                case 8:
                    Setting.Base_Time = "0500";
                    break;
                case 9:
                case 10:
                case 11:
                    Setting.Base_Time = "0800";
                    break;
                case 12:
                case 13:
                case 14:
                    Setting.Base_Time = "1100";
                    break;
                case 15:
                case 16:
                case 17:
                    Setting.Base_Time = "1400";
                    break;
                case 18:
                case 19:
                case 20:
                    Setting.Base_Time = "1700";
                    break;
                case 21:
                case 22:
                case 23:
                    Setting.Base_Time = "2000";
                    break;
                case 24:
                case 1:
                case 2:
                    Setting.Base_Time = "2300";
                    Setting.Base_Data = exception_base_time.ToString("yyyyMMdd");
                    break;
                default:
                    Setting.Base_Time = "2300";
                    break;
            }
        }
        #endregion

        #region [Utill] Get_TimeZone
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Get_TimeZone(int time)
        {
            if(time < 10)
            {
                Setting.TimeZone = "0" + time.ToString() + "00";
            }
            else
            {
                Setting.TimeZone = time.ToString() + "00";
            }
        }
        #endregion

        #region [Utill] Geturl
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //url ���ϴ� �Լ� 
        public void Geturl()
        {
            Setting.URL = "http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getVilageFcst";
            Setting.URL += "?ServiceKey=" + Setting.SerialKey;
            Setting.URL += "&numOfRows=36";                     // �������� ��� ��(Default : 12)  
            Setting.URL += "&pageNo=1";                         // ������ ��ȣ(Default : 1)
            Setting.URL += "&dataType=XML";                     // ���� �ڷ�����(XML, JSON)
            Setting.URL += "&base_date=" + Setting.Base_Data;   // ��û ��¥(yyMMdd)
            Setting.URL += "&base_time=" + Setting.Base_Time;   // ��û �ð�(HHmm)
            Setting.URL += "&nx=55";                            // ��û ���� x��ǥ
            Setting.URL += "&ny=127";                           // ��û ���� y�·�

            Debug.Log(Setting.URL);
        }
        #endregion
    }
}

//// C# ���� �ڵ�
//using System;
//using System.Net;
//using System.Net.Http;
//using System.IO;
//using UnityEditor;
//using UnityEngine;

//namespace LOBS
//{
//    public class Program : MonoBehaviour
//    {
//        static HttpClient client = new HttpClient();

//        public void Start()
//        {
//            string url = "http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getVilageFcst"; // URL
//            url += "?ServiceKey=" + "�ø���Ű �Է��ϼ���"; // Service Key
//            url += "&pageNo=1";
//            url += "&numOfRows=1000";
//            url += "&dataType=JSON";
//            url += "&base_date=20250305";
//            url += "&base_time=0800";
//            url += "&nx=55";
//            url += "&ny=127";DDD

//            var request = (HttpWebRequest)WebRequest.Create(url);
//            request.Method = "GET";

//            string results = string.Empty;
//            HttpWebResponse response;
//            using (response = request.GetResponse() as HttpWebResponse)
//            {
//                StreamReader reader = new StreamReader(response.GetResponseStream());
//                results = reader.ReadToEnd();
//            }

//            Debug.Log(results);
//        }
//    }
//}