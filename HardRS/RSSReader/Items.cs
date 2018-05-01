using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardRS.RSSReader
{
    class Items //статьи
    {
        public string Title { get; set; } //название статьи
        public string Link { get; set; } //ссылка
        public string Description { get; set; } //описание
        public string PubDate { get; set; } //дата публикации

        public Items()
        {
            Title = "";
            Link = "";
            Description = "";
            PubDate = "";
        }
    }
}
