using System;
using System.ComponentModel.DataAnnotations;

namespace FineUI.Core.QuickStart
{
    public class Movie
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "名称不能为空！")]
        [Display(Name = "名称")]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "发布日期")]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "类型")]
        public string Genre { get; set; }

        [Display(Name = "价格")]
        public decimal Price { get; set; }

    }

}
