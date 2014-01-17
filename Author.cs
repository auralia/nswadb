using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auralia.NationStates.ResolutionsDatabase
{
    public class Author
    {
        public string Name
        {
            get;
            set;
        }

        public List<Resolution> Resolutions
        {
            get;
            set;
        }

        public int activeAuthor;
        public int activeSubmittingCoauthor;
        public int activeNonsubmittingCoauthor;

        public int activeTotal
        {
            get
            {
                return activeAuthor + activeSubmittingCoauthor + activeNonsubmittingCoauthor;
            }
        }

        public int repealedAuthor;
        public int repealedSubmittingCoauthor;
        public int repealedNonsubmittingCoauthor;

        public int repealedTotal
        {
            get
            {
                return repealedAuthor + repealedSubmittingCoauthor + repealedNonsubmittingCoauthor;
            }
        }

        public int total
        {
            get
            {
                return activeTotal + repealedTotal;
            }
        }


        public Author(string name)
        {
            this.Name = name;
            this.Resolutions = new List<Resolution>();
        }
    }
}
