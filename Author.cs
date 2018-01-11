using System;
using System.Collections.Generic;
using System.Text;

namespace OwnedTypes
{
    public class Author: IAudit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EnumWrapper<Status> Status { get; set; }
        public Audit Audit { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new HashSet<Book>();
    }
}
