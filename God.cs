using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmiteBot
{
    public class God
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Roles { get; set; }
        public string Type { get; set; }
        public string Pantheon { get; set; }
        public string Lore { get; set; }
        public bool FreeRotation { get; set; }
        public string GodIcon { get; set; }
    }
}