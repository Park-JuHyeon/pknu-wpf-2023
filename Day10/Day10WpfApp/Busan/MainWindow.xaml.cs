using Busan.Logics;
using Busan.Models;
using CefSharp.DevTools.DOM;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Busan
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void TxtSectorName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearchSector_Click(sender, e);
            }
        }

        private async void BtnSearchSector_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtSectorName.Text))
            {
                await Commons.ShowMessageAsync("검색", "검색할 영화명을 입력하세요.");
                return;
            }

            //if (TxtMovieName.Text.Length < 2)
            // {
            //     await Commons.ShowMessageAsync("검색", "검색어를 2자이상 입력하세요.");
            //     return;
            // }

            try
            {
                SearchSector(TxtSectorName.Text);
            }
            catch (Exception ex)
            {

                await Commons.ShowMessageAsync("오류", $"오류발생 : {ex.Message}");
            }
        }

        private async void SearchSector(string sectorName)
        {
            string Data_apiKey = "%2BSpE28mAXdWeJytRNqmFNB726xFOlifgTDbtsDCCIe7s6MMsNwUGHj9lz4R%2Bm6PaZDysLHw70T3PhiRzEPbjSA%3D%3D";
            string encoding_sectorName = HttpUtility.UrlEncode(sectorName, Encoding.UTF8);
            string openApiUri = $@"https://apis.data.go.kr/6260000/AttractionService/getAttractionKr?serviceKey={Data_apiKey}" +
                                    $"&pageNo=1&numOfRows=20&resultType=json";   
            string result = string.Empty;   // 결과값 초기화

            // api 실행할 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            // Data API 요청
            try
            {
                req = WebRequest.Create(openApiUri);    // URL을 넣어서 객체 생성
                res = await req.GetResponseAsync();    // 요청한 결과를 비동기 응답에 할당
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();    // json결과 텍스트로 저장

                Debug.WriteLine(result);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                res.Close();

            }

            // result를 json으로 변경
            var jsonResult = JObject.Parse(result);
            var total = Convert.ToInt32(jsonResult["total_results"]);       // 전체 검색결과 수

            // await Commons.ShowMessageAsync("검색 결과", total.ToString());
            var items = jsonResult["results"];
            // items를 데이터그리드에 표시
            var json_array = items as JArray;

            var sectorItems = new List<SectorItem>();     // json에서 넘어온 배열을 담을 장소
            foreach (var val in  json_array)
            {
                var SectorItem = new SectorItem() 
                {
                    UC_SEQ = Convert.ToInt32(val["uc_seq"]),
                    MAIN_TITLE = Convert.ToString(val["main_title"]),
                    GUGUN_NM = Convert.ToString(val["gugun_nm"]),
                    LAT = Convert.ToDouble(val["lat"]),
                    LNG = Convert.ToDouble(val["lng"]),
                    PLACE = Convert.ToString(val["place"]),
                    TITLE = Convert.ToString(val["title"]),
                    SUBTITLE = Convert.ToString(val["subtitle"]),
                    ADDR1 = Convert.ToString(val["addr1"]),
                    HOMEPAGE_URL = Convert.ToString(val["homepage_url"]),
                    TRFC_INFO = Convert.ToString(val["trfc_info"]),
                    USAGE_DAY_WEEK_AND_TIME = Convert.ToString(val["usage_day_week_and_time"]),
                    USAGE_AMOUNT = Convert.ToString(val["usage_amount"]),
                    MIDDLE_SIZE_RM1 = Convert.ToString(val["middle_size_rm1"]),
                    MAIN_IMG_PATH = Convert.ToString(val["main_img_path"]),
                    ITEMCNTNTS = Convert.ToString(val["itemcntnts"]),
                };
                sectorItems.Add(SectorItem) ;

            }

            this.DataContext = sectorItems;
            StsResult.Content = $"OpenAPI {sectorItems.Count}건 조회완료";

        }

        private async void BtnShowAll_Click(object sender, RoutedEventArgs e)
        {
            string Data_apiKey = "%2BSpE28mAXdWeJytRNqmFNB726xFOlifgTDbtsDCCIe7s6MMsNwUGHj9lz4R%2Bm6PaZDysLHw70T3PhiRzEPbjSA%3D%3D";

            string openApiUri = $@"https://apis.data.go.kr/6260000/AttractionService/getAttractionKr?serviceKey={Data_apiKey}" +
                                    $"&pageNo=1&numOfRows=123&resultType=json";
            string result = string.Empty;   // 결과값 초기화

            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"OpenAPI 조회오류 {ex.Message}");
            }

            var jsonResult = JObject.Parse(result);
            var code = Convert.ToString(jsonResult["getAttractionKr"]["header"]["code"]);

            try
            {
                if (code == "00")  // 정상(00)이면 데이터받아서 처리
                {
                    var data = jsonResult["getAttractionKr"]["item"];
                    var json_array = data as JArray;
                    Debug.WriteLine(data.ToString());

                    var dustSensors = new List<SectorItem>();
                    foreach (var sensor in json_array)
                    {

                        dustSensors.Add(new SectorItem
                        {
                            UC_SEQ = Convert.ToInt32(sensor["uc_seq"]),
                            MAIN_TITLE = Convert.ToString(sensor["main_title"]),
                            GUGUN_NM = Convert.ToString(sensor["gugun_nm"]),
                            LAT = Convert.ToDouble(sensor["lat"]),
                            LNG = Convert.ToDouble(sensor["lng"]),
                            PLACE = Convert.ToString(sensor["place"]),
                            TITLE = Convert.ToString(sensor["title"]),
                            SUBTITLE = Convert.ToString(sensor["subtitle"]),
                            ADDR1 = Convert.ToString(sensor["addr1"]),
                            HOMEPAGE_URL = Convert.ToString(sensor["homepage_url"]),
                            TRFC_INFO = Convert.ToString(sensor["trfc_info"]),
                            USAGE_DAY_WEEK_AND_TIME = Convert.ToString(sensor["usage_day_week_and_time"]),
                            USAGE_AMOUNT = Convert.ToString(sensor["usage_amount"]),
                            MIDDLE_SIZE_RM1 = Convert.ToString(sensor["middle_size_rm1"]),
                            MAIN_IMG_PATH = Convert.ToString(sensor["main_img_path"]),
                            ITEMCNTNTS = Convert.ToString(sensor["itemcntnts"]),

                        });
                    }
                    this.DataContext = dustSensors;
                    StsResult.Content = $"OpenAPi {dustSensors.Count}건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"Json 처리오류 {ex.Message}");

            }


        }
    }
}
