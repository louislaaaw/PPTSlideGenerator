using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace PPTSlideGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<Image> images = new List<Image>();
        public static List<CheckBox> checkBoxes = new List<CheckBox>(); // checkboxes for user to select images
        public MainWindow()
        {
            InitializeComponent();
        }
        public string AccessXamlFromRTB(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            MemoryStream stream = new MemoryStream();
            textRange.Save(stream, DataFormats.Xaml);
            string xamlText = Encoding.UTF8.GetString(stream.ToArray());
            return xamlText;
        }

        public List<string> ParseBoldElementsFromXaml(string xaml)
        {
            List<string> boldWords = new List<string>();
            Regex regex = new Regex(@"<Run FontWeight=" + "\"Bold\">" + @"(?<word>(\w+\s*)+)</Run>");
            MatchCollection matches = regex.Matches(xaml);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                boldWords.Add(groups["word"].Value);
            }
            return boldWords;
        }
        public async Task<List<string>> RequestGoogleImagesUrls()
        {
            List<string> boldWords = ParseBoldElementsFromXaml(AccessXamlFromRTB(TextArea));
            string queryContent = TitleArea.Text;
            foreach (string word in boldWords)
            {
                queryContent += "+" + word;
            }
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://customsearch.googleapis.com/customsearch/v1?cx=7d6333e7effbd7c4c&searchType=image&q=" + queryContent + "&key=AIzaSyBvp7kNcfFItkzQXbpcech9hYps8Evg0UU"); // I would hide my api key usually but this is for demo purposes
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject obj = JObject.Parse(responseBody);
            List<string> urls = new List<string>();
            for(int i = 0; i < (int)obj["queries"]["request"][0]["count"]; i++)
            {
                urls.Add((string)obj["items"][i]["link"]);
            }
            return urls;
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> imagesURLs = await RequestGoogleImagesUrls();
            for(int i = 0; i < imagesURLs.Count; i++)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Margin = new Thickness(5);
                checkBox.Name = "option" + i.ToString();
                checkBoxes.Add(checkBox);
                Image image = new Image();
                ImageSource imageSource = new BitmapImage(new Uri(imagesURLs[i]));
                image.Source = imageSource;
                image.Name = "img" + i.ToString();
                image.Width = 100;
                image.Height = 100;
                images.Add(image);
                ImageArea.Children.Add(checkBox);
                ImageArea.Children.Add(image);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            ResultWindow result = new ResultWindow();
            result.ResultTitle.Text = TitleArea.Text;
            TextArea.SelectAll();
            TextArea.Copy();
            result.ResultContent.Paste();
            for (int i = 0; i < checkBoxes.Count; i++)
            {
                if (checkBoxes[i].IsChecked == true)
                {
                    Image newImage = new Image();
                    newImage.Source = images[i].Source;
                    newImage.Width = 100;
                    newImage.Height = 100;
                    result.ResultImages.Children.Add(newImage);
                }
            }
            result.Show();
        }
    }
}
