// C# 샘플 코드
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
    // 실시간으로 날씨 정보를 얻어오는 클래스 
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
            public int Time;            // 현재시간
            public string Base_Data;    // yyyy.mm.dd
            public string Base_Time;    // ex) 1300 2300
            public string URL;          // 공공데이터포털 url
            public string TimeZone;      // 시간대
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
                            if (node1["category"].InnerText.Equals("SKY"))  // 하늘상태
                            {
                                switch (node1["fcstValue"].InnerText)
                                {
                                    case "1":
                                        Debug.Log("맑음");
                                        break;
                                    case "3":
                                        Debug.Log("구름많음");
                                        break;
                                    case "4":
                                        Debug.Log("흐림");
                                        break;
                                    default:
                                        Debug.Log("해당하는 자료가 없음");
                                        break;
                                }
                            }

                            if (node1["category"].InnerText.Equals("PTY"))  // 강수형태
                            {
                                switch (node1["fcstValue"].InnerText)
                                {
                                    case "0":
                                        Debug.Log("없음");
                                        break;
                                    case "1":
                                        Debug.Log("비");
                                        break;
                                    case "2":
                                        //비/눈/진눈개비
                                        Debug.Log("비/눈/진눈개비");
                                        break;
                                    case "3":
                                        Debug.Log("눈");
                                        break;
                                    case "4":
                                        Debug.Log("소나기");
                                        break;
                                    default:
                                        Debug.Log("해당하는 자료가 없습니다.");
                                        break;
                                }
                            }

                            if (node1["category"].InnerText.Equals("TMP"))  // 현재 기온 불러옴 
                            {
                                //Debug.Log("TMP 들어감"+ node1["fcstDate"].InnerText);
                                Debug.Log("현재 TMP:" + node1["fcstValue"].InnerText);
                            }
                        }

                    }
                }
                else
                {
                    string apiErrorMsg = String.Empty;

                    // API 응답 에러 메세지 조사
                    apiErrorMsg = node["header"]["resultMsg"].InnerText switch
                    {
                        "APPLICATION_ERROR" => "어플리케이션 에러",
                        "DB_ERROR" => "데이터베이스 에러",
                        "NODATA_ERROR" => "데이터 없음",
                        "HTTP_ERROR" => "HTTP 에러",
                        "SERVICETIME_OUT" => "서비스 연결실패",
                        "INVALID_REQUEST_PARAMETER_ERROR" => "잘못된 요청 파라메터",
                        "NO_MANDATORY_REQUEST_PARAMETERS_ERROR" => "필수요청 파라메터가 없음",
                        "NO_OPENAPI_SERVICE_ERROR" => "해당 오픈 API서비스가 없거나 폐기됨",
                        "SERVICE_ACCESS_DENIED_ERROR" => "서비스 접근 거부",
                        "TEMPORARILY_DISABLE_THE_SERVICEKEY_ERROR" => "일시적으로 사용할 수 없는 서비스 키",
                        "LIMITED_NUMBER_OF_SERVICE_REQUESTS_EXCEEDS_ERROR" => "서비스 요청제한횟수 초과",
                        "SERVICE_KEY_IS_NOT_REGISTERED_ERROR" => "등록되지 않은 서비스 키",
                        "DEADLINE_HAS_EXPIRED_ERROR" => "기한 만료된 서비스 키",
                        "UNREGISTERED_IP_ERROR" => "등록되지 않은 IP",
                        "UNSIGNED_CALL_ERROR" => "서명되지 않은 호출",
                        "UNKNOWN_ERROR" => "기타에러",
                        _ => "해당하는 에러가 존재하지 않음",
                    };

                    // API 응답 에러 메세지 출력
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
        //url 구하는 함수 
        public void Geturl()
        {
            Setting.URL = "http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getVilageFcst";
            Setting.URL += "?ServiceKey=" + Setting.SerialKey;
            Setting.URL += "&numOfRows=36";                     // 한페이지 결과 수(Default : 12)  
            Setting.URL += "&pageNo=1";                         // 페이지 번호(Default : 1)
            Setting.URL += "&dataType=XML";                     // 받을 자료형식(XML, JSON)
            Setting.URL += "&base_date=" + Setting.Base_Data;   // 요청 날짜(yyMMdd)
            Setting.URL += "&base_time=" + Setting.Base_Time;   // 요청 시간(HHmm)
            Setting.URL += "&nx=55";                            // 요청 지역 x좌표
            Setting.URL += "&ny=127";                           // 요청 지역 y좌료

            Debug.Log(Setting.URL);
        }
        #endregion
    }
}

//// C# 샘플 코드
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
//            url += "?ServiceKey=" + "시리얼키 입력하세요"; // Service Key
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