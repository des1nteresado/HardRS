using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardRS
{
    class ImageOfChannel //рисунок канала
    {
        public string ImgTitle { get; set; } //название канала
        public string ImgLink { get; set; } //ссылка на сайт
        public string ImgURL { get; set; } //url картинки


        public ImageOfChannel()
        {
            ImgTitle = "";
            ImgLink = "";
            ImgURL = "";
        }
    }
}