using ClassLibraryWeatherParser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfAppWeatherParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        public MainWindow()
        {
            InitializeComponent();

            var now = DateTime.Now;
            datePickerDate.DisplayDateStart = now.AddYears(-2);
            datePickerDate.DisplayDateEnd =now.AddDays(10);
        }

        private void buttonGetWeather_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = datePickerDate.SelectedDate;
            if (selectedDate.HasValue)
            {
                if ((selectedDate.Value - DateTime.Now.AddYears(-2).Date).TotalDays >= 0 && (selectedDate.Value - DateTime.Now.AddDays(10)).TotalMilliseconds <= 0)
                {
                    string formatted = selectedDate.Value.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);

                    var request = (HttpWebRequest)WebRequest.Create(textBoxApiUrl.Text + "api/GetWeather?name=" + textBoxCity.Text + "&date=" + formatted);
                    var response = (HttpWebResponse)request.GetResponse();
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    Weather weather = JsonConvert.DeserializeObject<Weather>(responseString);

                    if (weather != null)
                    {
                        textBoxWeather.Text = weather.State
                            + "\nТемпература: " + weather.Temperature + "°"
                            + "\nСкорость ветра: " + weather.WindSpeed + "м/с"
                            + "\nВлажность воздуха: " + weather.AirHumidity + "%";

                        if ((selectedDate.Value - DateTime.Now.Date).TotalDays >= 0) 
                            textBoxWeather.Text += "\nВероятность осадков: " + weather.ChancePrecipitation + "%";
                    }
                    else MessageBox.Show("Данные о погоде отсутствуют в базе данных.");

                }
                else MessageBox.Show("Ошибка, дата не корректна.");
            }
            else MessageBox.Show("Ошибка, дата не выбрана.");
        }

        private void buttonPars_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = datePickerDate.SelectedDate;
            if (selectedDate.HasValue)
            {
                if ((selectedDate.Value - DateTime.Now.AddYears(-2).Date).TotalDays >= 0 && (selectedDate.Value - DateTime.Now.AddDays(10)).TotalMilliseconds <= 0)
                {
                    string formatted = selectedDate.Value.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);

                    var request = (HttpWebRequest)WebRequest.Create(textBoxApiUrl.Text + "api/WeatherParser?name=" + textBoxCity.Text + "&date=" + formatted);
                    var response = (HttpWebResponse)request.GetResponse();
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    if (Convert.ToInt32(responseString) == 1) MessageBox.Show("Данные были успешно обновлены.");
                    else MessageBox.Show("При обновлении данных возникла ошибка.");
                }
                else MessageBox.Show("Ошибка, дата не корректна.");
            }
            else MessageBox.Show("Ошибка, дата не выбрана.");

        }
    }
}
