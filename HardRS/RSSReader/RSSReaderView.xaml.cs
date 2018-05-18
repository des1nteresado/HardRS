using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Xml;

namespace HardRS.RSSReader
{
    /// <summary>
    /// Логика взаимодействия для RSSReaderView.xaml
    /// </summary>
    public partial class RSSReaderView : UserControl
    {
        Image imageChannel = new Image(); // объект класса рисунка
        Item[] articles; // массив элементов item канала
        Channel channel = new Channel(); // объект класса Channel
        ChannelContext db;
        int ChannelId;
        const string link = "https://news.tut.by/rss/42/devices_tehno.rss";

        public RSSReaderView()
        {
            InitializeComponent();

            db = new ChannelContext();

            if (getNewArticles(link))
            {
                if (generateHtml())
                {
                    Browser.Navigate(Environment.CurrentDirectory + "/last_articles.html");
                }
            }
            else
            {
                Browser.Navigate(Environment.CurrentDirectory + "/last_articles.html");
                label1.Content = "Подключение отсутствует, данные не обновлены.";
            }

        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }

        //Метод принимает в качестве параметра ссылку на RSS-поток,
        //и возвращает либо true при успешном выполнении,
        //либо false при неудачной попытке.       
        bool getNewArticles(string fileSource)
        {
            //Весь код помещен в try...catch
            //для отслеживания  исключений
            // и вывода их в виде сообщения.
            try
            {
                //Для предотвращения ошибки 407 (Удаленный сервер возвратил ошибку: 
                //(407) Требуется проверка подлинности посредника)
                //в сетях с прокси сервером 
                //загрузка RSS ленты осуществляется через класс WebRequest
                //с указанием настроек прокси.
                WebRequest wr = WebRequest.Create(fileSource);

                //Указываем системные учетные данные приложения,
                //передаваемые прокси-серверу для выполнения проверки подлинности.
                wr.Proxy.Credentials = CredentialCache.DefaultCredentials;


                //Инициализируем класс XmlTextReader, который
                //обеспечивает прямой доступ (только для чтения) к потокам данных XML.
                //и передаем ему экземпляр класса System.IO.Stream(GetResponseStream)
                //для чтения данных из интернет-ресурса.
                XmlTextReader xtr = new XmlTextReader(wr.GetResponse().GetResponseStream());

                //Инициализируем класс "XmlDocument". Который представляет XML-документ
                //и включает в себя метод "Load",
                //предназначенный для загрузки документа 
                //с помощью объекта "XMLReader". 
                XmlDocument doc = new XmlDocument();
                doc.Load(xtr);


                //XmlNode root - содержит корневой элемент XML для 
                //загруженного элемента.
                XmlNode root = doc.DocumentElement;
                //Получаем количество элементов item в RSS-потоке,
                //используя SelectNodes() и
                //выражение XPath, которое позволяет это сделать.
                articles = new Item[root.SelectNodes("channel/item").Count];
                // Инициализируем класс System.Xml.XmlNodeList, 
                //содержащий все дочерние узлы данного потока (channel).
                XmlNodeList nodeList;
                nodeList = root.ChildNodes;

                //Индикатор числового типа,
                //для массива articles[].
                int count = 0;

                //Цикл для прохода по всем каналам в RSS-потоке.
                foreach (XmlNode chanel in nodeList)
                {
                    //Цикл для прохода по всем элементам cnannel.  
                    foreach (XmlNode chanel_item in chanel)
                    {
                        //Название канала RSS-потока.
                        if (chanel_item.Name == "title")
                        {
                            channel.Title = chanel_item.InnerText;
                        }
                        //Описание канала RSS-потока.
                        if (chanel_item.Name == "description")
                        {
                            channel.Description = chanel_item.InnerText;
                        }
                        if (chanel_item.Name == "copyright")
                        {
                            channel.Copyright = chanel_item.InnerText;
                        }
                        //Ссылка на сайт RSS-потока.
                        if (chanel_item.Name == "link")
                        {
                            channel.Link = chanel_item.InnerText;
                        }

                        //Получение изображения канала RSS-потока.
                        if (chanel_item.Name == "image")
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
                        //Обработка сообщения канала RSS-потока.
                        if (chanel_item.Name == "item")
                        {
                            XmlNodeList itemsList = chanel_item.ChildNodes;
                            articles[count] = new Item();

                            foreach (XmlNode item in itemsList)
                            {
                                //Заголовок сообщения.
                                if (item.Name == "title")
                                {
                                    articles[count].Title = item.InnerText;
                                }
                                //Ссылка на сообщение в интернете.
                                if (item.Name == "link")
                                {
                                    articles[count].Link = item.InnerText;
                                }
                                //Описание сообщения, по сути оно и является
                                //самим сообщением в формате HTML.
                                if (item.Name == "description")
                                {
                                    articles[count].Description = item.InnerText;
                                }
                                // Дата публикации сообщения.
                                if (item.Name == "pubDate")
                                {
                                    ConvertToDateTime(item.InnerText, articles[count]);
                                    //articles[count].PubDate = item.InnerText;
                                }
                                articles[count].ChannelId = channel.Id;
                            }

                            //Увеличение счетчика сообщений
                            //для массива articles.
                            count += 1;
                        }
                    }
                }
                //После выполнения этого метода, 
                //объекты классов, будут заполнены данными. 
                //В imageChanel содержатся все данные о рисунке (если он есть),
                //В channel - все параметры канала,
                //Массив articles - будет содержать все сообщения.  
                //И метод возвратит значение true.
                if (!hasEqualsChannel(channel))//проверяем на аналогичный канал
                {
                    db.Channels.Add(channel);
                    db.Images.Add(imageChannel);
                    Console.WriteLine("count articles" + articles.Count());
                    foreach (Item item in articles)
                    {
                        db.Items.Add(item);
                    }
                }
                else
                {
                    foreach (Item item in articles)
                    {
                        if (!hasEqualsItem(item))//проверяем на аналогичный item, если есть новый - запихиваем в конец
                        {
                            item.ChannelId = ChannelId;
                            db.Items.Add(item);
                        }
                    }
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                //Сообщение об ошибке при получении или сиснтаксическом анализе данных.
                MessageBox.Show("Ошибка получения данных :" + ex.Message);

                //И метод возвратит значение false.
                return false;
            }
        }

        bool hasEqualsChannel(Channel channel)
        {
            foreach (Channel ch in db.Channels.ToList())
            {
                if (channel.Title.Equals(ch.Title))
                {
                    ChannelId = ch.Id;
                    return true;

                }
            }
            return false;
        }
        bool hasEqualsItem(Item item)
        {
            foreach (Item it in db.Items.ToList())
            {
                if (item.Title.Equals(it.Title))
                    return true;
            }
            return false;
        }

        private static void ConvertToDateTime(string value, Item article)
        {
            DateTime convertedDate;
            try
            {
                convertedDate = Convert.ToDateTime(value);
                article.PubDate = convertedDate;
            }
            catch (FormatException)
            {
                MessageBox.Show("'{0}' is not in the proper format.", value);
            }
        }

        //Вывод полученных данных будет происходить
        //в элементе управления WebBrowser. Все данные из RSS-потока
        //будут сохранены в виде *.html файла, и последующей его загрузки. 
        bool generateHtml()
        {
            //Переменная для подсчета количества и нумерации сообщений.
            try
            {
                using (StreamWriter writer = new StreamWriter("last_articles.html", false, Encoding.Default))
                {
                    int countofel = 1;
                    List<Channel> channels = new List<Channel>(db.Channels.ToList());
                    List<Image> images = new List<Image>(db.Images.ToList());
                    List<Item> items = new List<Item>(db.Items.ToList().OrderByDescending(d => d.PubDate));
                    for (int i = 0; i < channels.Count; i++)
                    {
                        //Начало формирования HTML страницы.
                        writer.WriteLine("<html>");
                        writer.WriteLine("<head>");
                        writer.WriteLine("<meta http-equiv=\"content-type\" content=\"text / html; charset = \"utf - 8\">");
                        writer.WriteLine("<title>");
                        //Создание элемента
                        writer.WriteLine(channels[i].Title);
                        writer.WriteLine("</title>");
                        //Стили применяемые к странице.
                        writer.WriteLine("<style type=\"text/css\">");
                        writer.WriteLine("img{ width: 100 px; height: 100 px;}");
                        writer.WriteLine("A{color:#483D8B; text-decoration:none; font:Century Gothic; font-weight:bold;}");
                        writer.WriteLine("body{ background-color: beige;}");
                        writer.WriteLine("p.date {text-align: right; font:Century Gothic; font-size:15; margin-right: 20px;}");
                        writer.WriteLine("p.m {margin-left: 20px;}");
                        writer.WriteLine("background-color:#dfe2e5;padding-top:5pt;padding-left:5pt;");
                        writer.WriteLine("padding-bottom:5pt;border-top:1pt solid #87A5C3;");
                        writer.WriteLine("border-bottom:1pt solid #87A5C3;border-left:1pt solid #87A5C3;");
                        writer.WriteLine("border-right : 1pt solid #87A5C3;	text-align : left;}");
                        writer.WriteLine("</style>");
                        writer.WriteLine("</head>");
                        writer.WriteLine("<body>");
                        //Вставка изображения из сообщения.
                        writer.WriteLine("<font size=\"2\" face=\"Century Gothic\">");
                        //writer.WriteLine("<a href=\"" + images[i].ImgLink + "\">");
                        //writer.WriteLine("<img src=\"" + images[i].ImgURL + "\" border=0></a>  ");
                        //Вывод заголовка(гиперссылки) RSS-потока - источника.
                        writer.WriteLine("<h3>" + channels[i].Title + "." + "</h3></a><br>");

                        writer.WriteLine("<table width=\"80 % \" align=\"center\" border=1>");//border
                        foreach (Item article in items)
                        {
                            if (article.ChannelId == channels[i].Id)
                            {
                                writer.WriteLine("<tr>");
                                writer.WriteLine("<td>");
                                writer.WriteLine("<br><h3>  <a href=\"" + article.Link + "\"><b>" + countofel + ". " + article.Title + "." + "</b></a></br><br>");
                                writer.WriteLine("<table width=\"95 % \" align=\"center\" border=0>");
                                writer.WriteLine("<tr><td>");
                                writer.WriteLine(article.Description);
                                writer.WriteLine("</td></tr></table>");
                                writer.Write("<a href=\"" + article.Link + "\">");
                                writer.Write("<font size=\"2\"><p class=\"m\">Читать далее..</p></font></a>");
                                writer.Write("<p class=\"date\">" + article.PubDate + "</p>");
                                writer.Write("</td>");
                                writer.Write("</tr>");
                                countofel++;
                            }
                        }
                        writer.WriteLine("</table><br>");
                        writer.WriteLine("<p align=\"center\">");
                        writer.WriteLine("<a href=\"" + channels[i].Link + "\">" + channels[i].Copyright + "</a></p>");
                        writer.WriteLine("</font>");
                        writer.WriteLine("</body>");
                        writer.WriteLine("</html>");

                    }

                    //Вывод общего количества сообщений в label1.
                    label1.Content = "Общее кол.во статей: " + (countofel - 1);
                    //Если все выполнено успешно, метод возвратит true.
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
            if (getNewArticles(link))
            {
                if (generateHtml())
                {
                    Browser.Navigate(Environment.CurrentDirectory + "/last_articles.html");
                }
            }
            else
            {
                Browser.Navigate(Environment.CurrentDirectory + "/last_articles.html");
                label1.Content = "Подключение отсутствует, данные не обновлены.";
            }
        }

        private void SearchBut_Click(object sender, RoutedEventArgs e)
        {
            List<Item> items = new List<Item>(db.Items.ToList().OrderByDescending(d => d.PubDate));
            List<Item> findItems = new List<Item>();
            Regex regex = new Regex(@"" + SearchBox.Text + "(\\w*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (Item it in items)
            {
                if (regex.IsMatch(it.Title))
                {
                    findItems.Add(it);
                }
            }
            GenerateSearchHtml(findItems);
            Browser.Navigate(Environment.CurrentDirectory + "/search_articles.html");
        }
        private void GenerateSearchHtml(List<Item> items)
        {

            using (StreamWriter writer = new StreamWriter("search_articles.html", false, Encoding.Default))
            {
                int countofel = 1;
                foreach (Item it in items)
                {
                    //Начало формирования HTML страницы.
                    writer.WriteLine("<html>");
                    writer.WriteLine("<head>");
                    writer.WriteLine("<meta http-equiv=\"content-type\" content=\"text / html; charset = \"utf - 8\">");
                    writer.WriteLine("<style type=\"text/css\">");
                    writer.WriteLine("A{color:#483D8B; text-decoration:none; font:Verdana;}");
                    writer.WriteLine("body{ background-color: grey;}");
                    writer.WriteLine("pre{font-family:courier;color:#000000;");
                    writer.WriteLine("background-color:#dfe2e5;padding-top:5pt;padding-left:5pt;");
                    writer.WriteLine("padding-bottom:5pt;border-top:1pt solid #87A5C3;");
                    writer.WriteLine("border-bottom:1pt solid #87A5C3;border-left:1pt solid #87A5C3;");
                    writer.WriteLine("border-right : 1pt solid #87A5C3;	text-align : left;}");
                    writer.WriteLine("</style>");
                    writer.WriteLine("</head>");
                    writer.WriteLine("<body>");
                    //Переменная для подсчета количества и нумерации сообщений.
                    writer.WriteLine("<table width=\"80 % \" align=\"center\" border=1>");//border
                    writer.WriteLine("<tr>");
                    writer.WriteLine("<td>");
                    writer.WriteLine("<br>  <a href=\"" + it.Link + "\"><b>" + countofel + ". " + it.Title + "</b></a>");
                    writer.WriteLine(" (" + it.PubDate + ")<br><br>");
                    writer.WriteLine("<table width=\"95 % \" align=\"center\" border=0>");
                    writer.WriteLine("<tr><td>");
                    writer.WriteLine(it.Description);
                    writer.WriteLine("</td></tr></table>");
                    writer.WriteLine("<br>  <a href=\"" + it.Link + "\">");
                    writer.WriteLine("<font size=\"4\">Читать далее > > ></font></a><br><br>");
                    writer.WriteLine("</td>");
                    writer.WriteLine("</tr>");
                    writer.WriteLine("</table><br>");
                    writer.WriteLine("</font>");
                    writer.WriteLine("</body>");
                    writer.WriteLine("</html>");
                    countofel++;
                }
                label1.Content = "Кол.во найденных статей: " + (countofel - 1);
            }
        }
    }
}
