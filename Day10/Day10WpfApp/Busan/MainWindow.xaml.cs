using Busan.Logics;
using Busan.Models;
using CefSharp.DevTools.DOM;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using MySql.Data.MySqlClient;

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


        private async void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("부산 관광지", "관광지를 선택하세요.");
                return;
            }
            if (GrdResult.SelectedItems.Count > 1)
            {
                await Commons.ShowMessageAsync("부산 관광지", "관광지를 하나만 선택하세요.");
                return;
            }

            string sectorName = string.Empty;
            string content = string.Empty;
            string posterPath = string.Empty;
            if (GrdResult.SelectedItem is SectorItem)
            {
                var sector = GrdResult.SelectedItem as SectorItem;
                sectorName = sector.MAIN_TITLE;
                content = sector.ITEMCNTNTS.ToString();
                posterPath = sector.MAIN_IMG_NORMAL;
            }

            //movieName = (GrdResult.SelectedItem as MovieItem).Title;
            //await Commons.ShowMessageAsync("유튜브", $"예고편 볼 영화 {movieName}");
            var trailerWindow = new DetailWindow(sectorName,content,posterPath);
            trailerWindow.Owner = this;     // TrailerWindow의 부모는 MainWindow
            trailerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;    // 부모창의 정중앙에 위치
            //trailerWindow.Show();   // 모달리스로 창을 열면 부모창 접근 가능하기때문에 사용X
            trailerWindow.ShowDialog();
        }


        private async void SearchSector(string sectorName)
        {
            string Data_apiKey = "%2BSpE28mAXdWeJytRNqmFNB726xFOlifgTDbtsDCCIe7s6MMsNwUGHj9lz4R%2Bm6PaZDysLHw70T3PhiRzEPbjSA%3D%3D";
            string encoding_sectorName = HttpUtility.UrlEncode(sectorName, Encoding.UTF8);
            string openApiUri = $@"https://apis.data.go.kr/6260000/AttractionService/getAttractionKr?serviceKey={Data_apiKey}" +
                                    $"&pageNo=1&numOfRows=123&resultType=json";
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
            //var total = Convert.ToInt32(jsonResult["total_results"]);       // 전체 검색결과 수

            // await Commons.ShowMessageAsync("검색 결과", total.ToString());
            var items = jsonResult["getAttractionKr"]["item"];
            // items를 데이터그리드에 표시
            var json_array = items as JArray;

            var sectorItems = new List<SectorItem>();     // json에서 넘어온 배열을 담을 장소
            foreach (var val in json_array)
            {
                var SectorItem = new SectorItem()
                {
                    UC_SEQ = Convert.ToInt32(val["UC_SEQ"]),
                    MAIN_TITLE = Convert.ToString(val["MAIN_TITLE"]),
                    GUGUN_NM = Convert.ToString(val["GUGUN_NM"]),
                    LAT = Convert.ToDouble(val["LAT"]),
                    LNG = Convert.ToDouble(val["LNG"]),
                    PLACE = Convert.ToString(val["PLACE"]),
                    TITLE = Convert.ToString(val["TITLE"]),
                    SUBTITLE = Convert.ToString(val["SUBTITLE"]),
                    ADDR1 = Convert.ToString(val["ADDR1"]),
                    HOMEPAGE_URL = Convert.ToString(val["HOMEPAGE_URL"]),
                    TRFC_INFO = Convert.ToString(val["TRFC_INFO"]),
                    USAGE_DAY_WEEK_AND_TIME = Convert.ToString(val["USAGE_DAY_WEEK_AND_TIME"]),
                    USAGE_AMOUNT = Convert.ToString(val["USAGE_AMOUNT"]),
                    MIDDLE_SIZE_RM1 = Convert.ToString(val["MIDDLE_SIZE_RM1"]),
                    MAIN_IMG_NORMAL = Convert.ToString(val["MAIN_IMG_NORMAL"]),
                    ITEMCNTNTS = Convert.ToString(val["ITEMCNTNTS"]),
                };
                sectorItems.Add(SectorItem);

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
                            UC_SEQ = Convert.ToInt32(sensor["UC_SEQ"]),
                            MAIN_TITLE = Convert.ToString(sensor["MAIN_TITLE"]),
                            GUGUN_NM = Convert.ToString(sensor["GUGUN_NM"]),
                            LAT = Convert.ToDouble(sensor["LAT"]),
                            LNG = Convert.ToDouble(sensor["LNG"]),
                            PLACE = Convert.ToString(sensor["PLACE"]),
                            TITLE = Convert.ToString(sensor["TITLE"]),
                            SUBTITLE = Convert.ToString(sensor["SUBTITLE"]),
                            ADDR1 = Convert.ToString(sensor["ADDR1"]),
                            HOMEPAGE_URL = Convert.ToString(sensor["HOMEPAGE_URL"]),
                            TRFC_INFO = Convert.ToString(sensor["TRFC_INFO"]),
                            USAGE_DAY_WEEK_AND_TIME = Convert.ToString(sensor["USAGE_DAY_WEEK_AND_TIME"]),
                            USAGE_AMOUNT = Convert.ToString(sensor["USAGE_AMOUNT"]),
                            MIDDLE_SIZE_RM1 = Convert.ToString(sensor["MIDDLE_SIZE_RM1"]),
                            MAIN_IMG_NORMAL = Convert.ToString(sensor["MAIN_IMG_NORMAL"]),
                            ITEMCNTNTS = Convert.ToString(sensor["ITEMCNTNTS"]),

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

        private void GrdResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //    try
            //    {
            //        string posterPath = string.Empty;

            //        if (GrdResult.SelectedItem is SectorItem)    // openAPI로 검색된 영화의 포스터
            //        {
            //            var sector = GrdResult.SelectedItem as SectorItem;
            //            posterPath = sector.MAIN_IMG_PATH;
            //        }
            //        else if (GrdResult.SelectedItem is SectorItem)   // 즐겨찾기 DB에서 가져온 영화 포스터
            //        {
            //            var sector = GrdResult.SelectedItem as SectorItem;
            //            posterPath = sector.MAIN_IMG_PATH;
            //        }

            //        //var movie = GrdResult.SelectedItem as MovieItem;
            //        Debug.WriteLine(posterPath);

            //        if (string.IsNullOrEmpty(posterPath))    // 포스터 이미지가 없으면 No_Picture
            //        {
            //            ImgPoster.Source = new BitmapImage(new Uri("/No_Picture.png", UriKind.RelativeOrAbsolute));
            //        }
            //        else    // 이미포스터가 있으면
            //        {
            //            var base_url = "https://image.tmdb.org/t/p/w300_and_h450_bestv2";
            //            ImgPoster.Source = new BitmapImage(new Uri($"{base_url}{posterPath}", UriKind.RelativeOrAbsolute));
            //        }
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}
        }

        private void CboSectorLoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboSectorLoc.SelectedValue != null)
            {
                // MessageBox.Show(CboReqDate.SelectedValue.ToString());
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT 
	                                    UC_SEQ,
                                        GUGUN_NM,
                                        MAIN_TITLE,
                                        LAT,
                                        LNG,
                                        PLACE,
                                        TITLE,
                                        SUBTITLE,
                                        ADDR1,
                                        HOMEPAGE_URL,
                                        TRFC_INFO,
                                        USAGE_DAY_WEEK_AND_TIME,
                                        USAGE_AMOUNT,
                                        MIDDLE_SIZE_RM1,
                                        MAIN_IMG_NORMAL,
                                        ITEMCNTNTS
                                    FROM busan
                                   WHERE GUGUN_NM = @GUGUN_NM";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@GUGUN_NM", CboSectorLoc.SelectedValue.ToString());
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "busan");
                    List<SectorItem> dustSensors = new List<SectorItem>();

                    foreach (DataRow row in ds.Tables["busan"].Rows)
                    {
                        dustSensors.Add(new SectorItem
                        {
                            UC_SEQ = Convert.ToInt32(row["UC_SEQ"]),
                            MAIN_TITLE = Convert.ToString(row["MAIN_TITLE"]),
                            GUGUN_NM = Convert.ToString(row["GUGUN_NM"]),
                            LAT = Convert.ToDouble(row["LAT"]),
                            LNG = Convert.ToDouble(row["LNG"]),
                            PLACE = Convert.ToString(row["PLACE"]),
                            TITLE = Convert.ToString(row["TITLE"]),
                            SUBTITLE = Convert.ToString(row["SUBTITLE"]),
                            ADDR1 = Convert.ToString(row["ADDR1"]),
                            HOMEPAGE_URL = Convert.ToString(row["HOMEPAGE_URL"]),
                            TRFC_INFO = Convert.ToString(row["TRFC_INFO"]),
                            USAGE_DAY_WEEK_AND_TIME = Convert.ToString(row["USAGE_DAY_WEEK_AND_TIME"]),
                            USAGE_AMOUNT = Convert.ToString(row["USAGE_AMOUNT"]),
                            MIDDLE_SIZE_RM1 = Convert.ToString(row["MIDDLE_SIZE_RM1"]),
                            MAIN_IMG_NORMAL = Convert.ToString(row["MAIN_IMG_NORMAL"]),
                            ITEMCNTNTS = Convert.ToString(row["ITEMCNTNTS"]),
                        });
                    }

                    this.DataContext = dustSensors;
                    StsResult.Content = $"DB {dustSensors.Count} 건 조회완료";
                }
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 콤보박스에 들어갈 날짜 DB에서 불러와서
            // 저장한 뒤에도 콤보박스를 재조회해야 날짜전부 출력
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT GUGUN_NM AS Save_Date
                                 FROM busan
                                GROUP BY 1
                                ORDER BY 1 ";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                List<string> saveDateList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    saveDateList.Add(Convert.ToString(row["Save_Date"]));
                }

                CboSectorLoc.ItemsSource = saveDateList;
            }
        }

        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.Items.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "조회를하고 저장하세요");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();
                    var query = @"INSERT INTO busan
                                        (UC_SEQ,
                                        GUGUN_NM,
                                        MAIN_TITLE,
                                        LAT,
                                        LNG,
                                        PLACE,
                                        TITLE,
                                        SUBTITLE,
                                        ADDR1,
                                        HOMEPAGE_URL,
                                        TRFC_INFO,
                                        USAGE_DAY_WEEK_AND_TIME,
                                        USAGE_AMOUNT,
                                        MIDDLE_SIZE_RM1,
                                        MAIN_IMG_NORMAL,
                                        ITEMCNTNTS
                                        )
                                        VALUES
                                        (@UC_SEQ,
                                        @GUGUN_NM,
                                        @MAIN_TITLE,
                                        @LAT,
                                        @LNG,
                                        @PLACE,
                                        @TITLE,
                                        @SUBTITLE,
                                        @ADDR1,
                                        @HOMEPAGE_URL,
                                        @TRFC_INFO,
                                        @USAGE_DAY_WEEK_AND_TIME,
                                        @USAGE_AMOUNT,
                                        @MIDDLE_SIZE_RM1,
                                        @MAIN_IMG_NORMAL,
                                        @ITEMCNTNTS
                                        )";

                    var insRes = 0;
                    foreach (var temp in GrdResult.Items)
                    {
                        if (temp is SectorItem)
                        {
                            var item = temp as SectorItem;

                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("UC_SEQ", item.UC_SEQ);
                            cmd.Parameters.AddWithValue("GUGUN_NM", item.GUGUN_NM);
                            cmd.Parameters.AddWithValue("MAIN_TITLE", item.MAIN_TITLE);
                            cmd.Parameters.AddWithValue("LAT", item.LAT);
                            cmd.Parameters.AddWithValue("LNG", item.LNG);
                            cmd.Parameters.AddWithValue("PLACE", item.PLACE);
                            cmd.Parameters.AddWithValue("TITLE", item.TITLE);
                            cmd.Parameters.AddWithValue("SUBTITLE", item.SUBTITLE);
                            cmd.Parameters.AddWithValue("ADDR1", item.ADDR1);
                            cmd.Parameters.AddWithValue("HOMEPAGE_URL", item.HOMEPAGE_URL);
                            cmd.Parameters.AddWithValue("TRFC_INFO", item.TRFC_INFO);
                            cmd.Parameters.AddWithValue("USAGE_DAY_WEEK_AND_TIME", item.USAGE_DAY_WEEK_AND_TIME);
                            cmd.Parameters.AddWithValue("USAGE_AMOUNT", item.USAGE_AMOUNT);
                            cmd.Parameters.AddWithValue("MIDDLE_SIZE_RM1", item.MIDDLE_SIZE_RM1);
                            cmd.Parameters.AddWithValue("MAIN_IMG_NORMAL", item.MAIN_IMG_NORMAL);
                            cmd.Parameters.AddWithValue("ITEMCNTNTS", item.ITEMCNTNTS);


                            insRes += cmd.ExecuteNonQuery();
                        }
                    }

                    await Commons.ShowMessageAsync("저장", "DB저장 성공!");
                    StsResult.Content = $"DB저장 {insRes}건 성공";

                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB 저장오류! {ex.Message}");
            }

        }

        private async void BtnSearchSector_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selItem = GrdResult.SelectedItem as SectorItem;

                var mapWindow = new MapWindow(selItem.LNG, selItem.LAT);  // 부모창 위치값을 자식창으로 전달
                mapWindow.Owner = this;
                mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;    // 부모창 중간에 출력
                mapWindow.ShowDialog();
            }
            catch (Exception )
            {
                await Commons.ShowMessageAsync("오류", $"찾을 위치를 클릭해주세요!");

            }

        }
    }
}
