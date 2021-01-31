using System;
using System.Collections.Generic;

namespace Reconciliation.Models
{
    public class FrontMatter
    {
        public IList<string> Categories { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public IList<string> Images { get; set; }
        public string Location { get; set; }
    }
}
