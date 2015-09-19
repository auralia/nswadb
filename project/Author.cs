//-----------------------------------------------------------------------
// <copyright file="Author.cs" company="Auralia">
//     Copyright (C) 2014-2015 Auralia
// </copyright>
//-----------------------------------------------------------------------

namespace Auralia.NationStates.GaResolutionsDatabase
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an author of a resolution.
    /// </summary>
    public class Author
    {
        /// <summary>
        /// Initializes a new instance of the Author class.
        /// </summary>
        /// <param name="name">The nation name of the author.</param>
        public Author(string name)
        {
            this.Name = name;
            this.Resolutions = new List<Resolution>();
            this.IsPlayer = false;
        }

        /// <summary>
        /// Gets or sets the nation name of the author.
        /// </summary>
        /// <value>The nation name of the author.</value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of resolution the nation has authored.
        /// </summary>
        /// <value>A list of resolution the nation has authored.</value>
        public List<Resolution> Resolutions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of active resolutions the nation has uniquely authored.
        /// </summary>
        /// <value>The number of active resolutions the nation has uniquely authored.</value>
        public int ActiveAuthor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of active resolutions for which the nation is a submitting coauthor.
        /// </summary>
        /// <value>The number of active resolutions for which the nation is a submitting coauthor.</value>
        public int ActiveSubmittingCoauthor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of active resolutions for which the nation is a non-submitting coauthor.
        /// </summary>
        /// <value>The number of active resolutions for which the nation is a non-submitting coauthor.</value>
        public int ActiveNonsubmittingCoauthor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the total number of active resolutions authored by this nation.
        /// </summary>
        /// <value>The total number of active resolutions authored by this nation.</value>
        public int ActiveTotal
        {
            get
            {
                return this.ActiveAuthor + this.ActiveSubmittingCoauthor + this.ActiveNonsubmittingCoauthor;
            }
        }

        /// <summary>
        /// Gets or sets the number of repealed resolutions the nation has uniquely authored.
        /// </summary>
        /// <value>The number of repealed resolutions the nation has uniquely authored.</value>
        public int RepealedAuthor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of repealed resolutions for which the nation is a submitting coauthor.
        /// </summary>
        /// <value>The number of repealed resolutions for which the nation is a submitting coauthor.</value>
        public int RepealedSubmittingCoauthor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of repealed resolutions for which the nation is a non-submitting coauthor.
        /// </summary>
        /// <value>The number of repealed resolutions for which the nation is a non-submitting coauthor.</value>
        public int RepealedNonsubmittingCoauthor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the total number of repealed resolutions authored by this nation.
        /// </summary>
        /// <value>The total number of repealed resolutions authored by this nation.</value>
        public int RepealedTotal
        {
            get
            {
                return this.RepealedAuthor + this.RepealedSubmittingCoauthor + this.RepealedNonsubmittingCoauthor;
            }
        }

        /// <summary>
        /// Gets the total number of resolutions authored by this nation.
        /// </summary>
        /// <value>The total number of resolutions authored by this nation.</value>
        public int Total
        {
            get
            {
                return this.ActiveTotal + this.RepealedTotal;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the author is an ordinary nation or a special "player" nation, which tracks resolutions written by multiple nations controlled by the same player.
        /// </summary>
        /// <value>Whether the author is an ordinary nation or a special "player" nation, which tracks resolutions written by multiple nations controlled by the same player.</value>
        public bool IsPlayer
        {
            get;
            set;
        }
    }
}
