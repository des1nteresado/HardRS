using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardRS
{
    class Channel // настройки канала
    {
        public string Title { get; set; } //название канала
        public string Description { get; set; } //описание канала
        public string Link { get; set; } //ссылка на канал
        public string Copyright { get; set; } //копирайт

        public Channel()
        {
            Title = "";
            Description = "";
            Link = "";
            Copyright = "";
        }
    }
}
