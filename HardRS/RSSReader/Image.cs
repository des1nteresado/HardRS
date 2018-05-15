using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardRS.RSSReader
{
    class Image //рисунок канала
    {
        [Key]
        [ForeignKey("Channel")]
        public int Id { get; set; }

        public string ImgTitle { get; set; } //название канала
        public string ImgLink { get; set; } //ссылка на сайт
        public string ImgURL { get; set; } //url картинки

        public Channel Channel { get; set; }

        public Image()
        {
            ImgTitle = "";  
            ImgLink = "";
            ImgURL = "";
        }
    }
}