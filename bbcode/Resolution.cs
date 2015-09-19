using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auralia.NationStates.ResolutionsDatabase
{
    public class Resolution
    {
        public int Number
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Category
        {
            get;
            set;
        }

        public string Subcategory
        {
            get;
            set;
        }

        public string Author
        {
            get;
            set;
        }

        public string Coauthor
        {
            get;
            set;
        }

        public string PlayerAuthor
        {
            get;
            set;
        }

        public string PlayerCoauthor
        {
            get;
            set;
        }

        public bool IsRepealed
        {
            get;
            set;
        }

        public int VotesFor
        {
            get;
            set;
        }

        public int VotesAgainst
        {
            get;
            set;
        }

        public DateTime DateImplemented
        {
            get;
            set;
        }
    }
}
