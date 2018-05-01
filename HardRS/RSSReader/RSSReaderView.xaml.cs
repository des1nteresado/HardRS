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
using System.Xml;

namespace HardRS.RSSReader
{
    /// <summary>
    /// Логика взаимодействия для RSSReaderView.xaml
    /// </summary>
    public partial class RSSReaderView : UserControl
    {
        public RSSReaderView()
        {
            InitializeComponent();
        }

        ImageOfChannel imageChannel = new ImageOfChannel(); // объект класса рисунка
        Items[] articles; // массив элементов item канала
        Channel channel = new Channel(); // объект класса Channel

        bool getNewArticles(string fileSource)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileSource);

                XmlNodeList nodeList;
                XmlNode root = doc.DocumentElement;
                articles = new Items[root.SelectNodes("channel/item").Count];
                nodeList = root.ChildNodes;
                int count = 0;

                foreach (XmlNode chanel in nodeList)
                {
                    foreach (XmlNode chanel_item in chanel)
                    {
                        if (chanel_item.Name == "title")
                        {
                            channel.Title = chanel_item.InnerText;
                        }
                        if (chanel_item.Name == "description")
                        {
                            channel.Description = chanel_item.InnerText;
                        }
                        if (chanel_item.Name == "copyright")
                        {
                            channel.Copyright = chanel_item.InnerText;
                        }
                        if (chanel_item.Name == "link")
                        {
                            channel.Link = chanel_item.InnerText;
                        }

                        if (chanel_item.Name == "img")
                        {
                            XmlNodeList imgList = chanel_item.ChildNodes;
                            foreach (XmlNode img_item in imgList)
                            {
                                if (img_item.Name == "url")
                                {
                                    imageChannel.ImgURL = img_item.InnerText;
                                }
                                if (img_item.Name == "link")
                                {
                                    imageChannel.ImgLink = img_item.InnerText;
                                }
                                if (img_item.Name == "title")
                                {
                                    imageChannel.ImgTitle = img_item.InnerText;
                                }
                            }
                        }

                        if (chanel_item.Name == "item")
                        {
                            XmlNodeList itemsList = chanel_item.ChildNodes;
                            articles[count] = new Items();

                            foreach (XmlNode item in itemsList)
                            {
                                if (item.Name == "title")
                                {
                                    articles[count].Title = item.InnerText;
                                }
                                if (item.Name == "link")
                                {
                                    articles[count].Link = item.InnerText;
                                }
                                if (item.Name == "description")
                                {
                                    articles[count].Description = item.InnerText;
                                }
                                if (item.Name == "pubDate")
                                {
                                    articles[count].PubDate = item.InnerText;
                                }
                            }
                            count += 1;
                        }


                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        bool generateHtml()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("last_articles.html", false, Encoding.Default))
                {
                    writer.WriteLine("<html>");
                    writer.WriteLine("<head>");
                    writer.WriteLine("<meta http-equiv=\"content-type\" content=\"text / html; charset = \"utf - 8\">");
                    writer.WriteLine("<title>");
                    writer.WriteLine(channel.Title);
                    writer.WriteLine("</title>");
                    writer.WriteLine("<style type=\"text/css\">");
                    writer.WriteLine("A{color:#483D8B; text-decoration:none; font:Verdana;}");
                    writer.WriteLine("pre{font-family:courier;color:#000000;");
                    writer.WriteLine("background-color:#dfe2e5;padding-top:5pt;padding-left:5pt;");
                    writer.WriteLine("padding-bottom:5pt;border-top:1pt solid #87A5C3;");
                    writer.WriteLine("border-bottom:1pt solid #87A5C3;border-left:1pt solid #87A5C3;");
                    writer.WriteLine("border-right : 1pt solid #87A5C3;	text-align : left;}");
                    writer.WriteLine("</style>");
                    writer.WriteLine("</head>");
                    writer.WriteLine("<body>");

                    writer.WriteLine("<font size=\"2\" face=\"Verdana\">");
                    writer.WriteLine("<a href=\"" + imageChannel.ImgLink + "\">");
                    writer.WriteLine("<img src=\"" + imageChannel.ImgURL + "\" border=0></a>  ");
                    writer.WriteLine("<h3>" + channel.Title + "</h3></a>");

                    writer.WriteLine("<table width=\"80 % \" align=\"center\" border=1>");
                    foreach (Items article in articles)
                    {
                        writer.WriteLine("<tr>");
                        writer.WriteLine("<td>");
                        writer.WriteLine("<br>  <a href=\"" + article.Link + "\"><b>" + article.Title + "</b></a>");
                        writer.WriteLine("& (" + article.PubDate + ")<br><br>");
                        writer.WriteLine("<table width=\"95 % \" align=\"center\" border=0>");
                        writer.WriteLine("<tr><td>");
                        writer.WriteLine(article.Description);
                        writer.WriteLine("</td></tr></table>");
                        writer.WriteLine("<br>  <a href=\"" + article.Link + "\">");
                        writer.WriteLine("<font size=\"1\">читать дальше</font></a><br><br>");
                        writer.WriteLine("</td>");
                        writer.WriteLine("</tr>");
                    }
                    writer.WriteLine("</table><br>");
                    writer.WriteLine("<p align=\"center\">");
                    writer.WriteLine("<a href=\"" + channel.Link + "\">" + channel.Copyright + "</a></p>");
                    writer.WriteLine("</font>");
                    writer.WriteLine("</body>");
                    writer.WriteLine("</html>");
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (getNewArticles("http://feeds.feedburner.com/applenewscomua") == true && generateHtml() == true)
            {
                Browser.Navigate(Environment.CurrentDirectory + "/last_articles.html");
            }
        }
    }
}
