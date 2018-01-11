using System;
using System.Collections.Generic;
using System.Text;

namespace OwnedTypes
{
    public class Book: IAudit
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public EnumWrapper<Genre> Genre { get; set; }
        public EnumWrapper<Section> Section { get; set; }
        public EnumWrapper<Status> Status { get; set; }
        public Audit Audit { get; set; }

        public virtual Author Author { get; set; }
    }
}
