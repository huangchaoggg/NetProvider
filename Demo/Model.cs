using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class Model
    {
        public string toc_title { get; set; }
        public string tocHref { get; set; }
        public string href { get; set; }
        public string homepage { get; set; }
        public List<Model> children { get; set; }
    }
    public class Metadata
    {
        public string pdf_name { get; set; }
        public string pdf_absolute_path { get; set; }
    }
    public class Meta
    {
        public Metadata metadata { get; set; }
        public List<Model> items { get; set; }
    }
}
