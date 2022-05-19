using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Language_School.DB;

namespace Language_School
{
    class ClassHelper
    {
        public static Entities context { get; set; } = new Entities();
    }
}
