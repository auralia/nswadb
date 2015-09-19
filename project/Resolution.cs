//-----------------------------------------------------------------------
// <copyright file="Resolution.cs" company="Auralia">
//     Copyright (C) 2014-2015 Auralia
// </copyright>
//-----------------------------------------------------------------------

namespace Auralia.NationStates.GaResolutionsDatabase
{
    using System;

    /// <summary>
    /// Represents a General Assembly resolution.
    /// </summary>
    public class Resolution
    {
        /// <summary>
        /// Gets or sets the number of the resolution.
        /// </summary>
        /// <value>The number of the resolution.</value>
        public int Number
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the title of the resolution.
        /// </summary>
        /// <value>The title of the resolution.</value>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the category of the resolution.
        /// </summary>
        /// <value>The category of the resolution.</value>
        public string Category
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the subcategory of the resolution (i.e. strength, area of effect, etc.).
        /// </summary>
        /// <value>The subcategory of the resolution (i.e. strength, area of effect, etc.).</value>
        public string Subcategory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the author (or submitting co-author) of the resolution.
        /// </summary>
        /// <value>The author (or submitting co-author) of the resolution.</value>
        public string Author
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the non-submitting co-author of the resolution, if any.
        /// </summary>
        /// <value>The non-submitting co-author of the resolution, if any.</value>
        public string Coauthor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the player author (or submitting co-author) of the resolution.
        /// </summary>
        /// <value>The player author (or submitting co-author) of the resolution.</value>
        public string PlayerAuthor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the player non-submitting co-author of the resolution, if any.
        /// </summary>
        /// <value>The player non-submitting co-author of the resolution, if any.</value>
        public string PlayerCoauthor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the resolution was repealed.
        /// </summary>
        /// <value>Whether the resolution was repealed.</value>
        public bool IsRepealed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of votes in favour of the resolution.
        /// </summary>
        /// <value>The number of votes in favour of the resolution.</value>
        public int VotesFor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of votes against the resolution.
        /// </summary>
        /// <value>The number of votes against the resolution.</value>
        public int VotesAgainst
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the date that the resolution was passed.
        /// </summary>
        /// <value>The date that the resolution was passed.</value>
        public DateTime DateImplemented
        {
            get;
            set;
        }
    }
}
