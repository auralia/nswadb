//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Auralia">
//     Copyright (C) 2014-2015 Auralia
// </copyright>
//-----------------------------------------------------------------------

namespace Auralia.NationStates.GaResolutionsDatabase
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.OleDb;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    
    /// <summary>
    /// The main class of the program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The path to the resolutions database.
        /// </summary>
        private string databasePath;

        /// <summary>
        /// The path to the BBCode output folder.
        /// </summary>
        private string outputPath;

        /// <summary>
        /// The database connection string.
        /// </summary>
        private string databaseConnectionString;

        /// <summary>
        /// The command to retrieve all of the resolutions from the database.
        /// </summary>
        private string databaseCommand;

        /// <summary>
        /// The data set used to store information from the database.
        /// </summary>
        private DataSet dataSet;

        /// <summary>
        /// A list of all General Assembly resolutions.
        /// </summary>
        private List<Resolution> resolutions;

        /// <summary>
        /// A list of all General Assembly resolution authors.
        /// </summary>
        private List<Author> authors;

        /// <summary>
        /// Initializes a new instance of the Program class.
        /// </summary>
        public Program()
        {
            this.databasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\..\\..\\database\\resolutions.mdb";
            this.outputPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\..\\out\\";
            this.databaseConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + this.databasePath;
            this.databaseCommand = "SELECT * FROM RESOLUTIONS";

            this.GetDataSet();
            this.GetResolutions();
            this.GetAuthors();
        }

        /// <summary>
        /// The order type of the resolutions table.
        /// </summary>
        private enum OrderType
        {
            /// <summary>
            /// Order the table by author name (alphabetical).
            /// </summary>
            Author = 1,

            /// <summary>
            /// Order the table by total number of resolutions passed.
            /// </summary>
            Total = 2,

            /// <summary>
            /// Order the table by total number of active resolutions passed.
            /// </summary>
            ActiveTotal = 3,

            /// <summary>
            /// Order the table by total number of repealed resolutions passed.
            /// </summary>
            RepealedTotal = 4
        }

        /// <summary>
        /// Runs the application.
        /// </summary>
        /// <param name="args">Command line arguments passed to the application.</param>
        public static void Main(string[] args)
        {
            var prog = new Program();
            prog.GenerateGeneralAssemblyAuthorIndex();

            prog.GenerateGeneralAssemblyAuthorTable(OrderType.Author);
            prog.GenerateGeneralAssemblyAuthorTable(OrderType.Total);
            prog.GenerateGeneralAssemblyAuthorTable(OrderType.ActiveTotal);
            prog.GenerateGeneralAssemblyAuthorTable(OrderType.RepealedTotal);

            Console.WriteLine("BBCode list and tables generation complete.");
            Console.ReadKey();
        }

        /// <summary>
        /// Gets the data set from the database.
        /// </summary>
        private void GetDataSet()
        {
            var connection = new OleDbConnection(this.databaseConnectionString);
            connection.Open();

            var command = new OleDbCommand(this.databaseCommand, connection);

            var dataAdapter = new OleDbDataAdapter(command);

            this.dataSet = new DataSet();
            dataAdapter.Fill(this.dataSet);

            connection.Close();
        }

        /// <summary>
        /// Gets the resolutions from the data set.
        /// </summary>
        private void GetResolutions()
        {
            this.resolutions = new List<Resolution>();

            foreach (DataRow row in this.dataSet.Tables[0].Rows)
            {
                object[] resolutionData = row.ItemArray;
                
                var resolution = new Resolution();
                resolution.Number = (int)resolutionData[0];
                resolution.Title = (string)resolutionData[1];
                resolution.Category = (string)resolutionData[2];
                resolution.Subcategory = (string)resolutionData[3];
                resolution.Author = (string)resolutionData[4];
                resolution.Coauthor = resolutionData[5] == DBNull.Value ? null : (string)resolutionData[5];
                resolution.PlayerAuthor = resolutionData[6] == DBNull.Value ? null : (string)resolutionData[6];
                resolution.PlayerCoauthor = resolutionData[7] == DBNull.Value ? null : (string)resolutionData[7];
                resolution.IsRepealed = (bool)resolutionData[8];
                resolution.VotesFor = (int)resolutionData[9];
                resolution.VotesAgainst = (int)resolutionData[10];
                resolution.DateImplemented = (DateTime)resolutionData[11];

                this.resolutions.Add(resolution);
            }

            this.resolutions = this.resolutions.OrderBy(o => o.Number).ToList();
        }

        /// <summary>
        /// Gets the authors from the resolutions.
        /// </summary>
        private void GetAuthors()
        {
            this.authors = new List<Author>();

            foreach (Resolution resolution in this.resolutions)
            {
                Author author = null;
                Author coauthor = null;
                Author playerAuthor = null;
                Author playerCoauthor = null;

                foreach (Author auth in this.authors)
                {
                    if (!auth.IsPlayer)
                    {
                        if (auth.Name.Equals(resolution.Author))
                        {
                            author = auth;
                        }

                        if (resolution.Coauthor != null && auth.Name.Equals(resolution.Coauthor))
                        {
                            coauthor = auth;
                        }
                    }
                    else
                    {
                        if (resolution.PlayerAuthor != null && auth.Name.Equals(resolution.PlayerAuthor))
                        {
                            playerAuthor = auth;
                        }

                        if (resolution.PlayerCoauthor != null && auth.Name.Equals(resolution.PlayerCoauthor))
                        {
                            playerCoauthor = auth;
                        }
                    }
                }

                if (author == null)
                {
                    author = new Author(resolution.Author);
                    this.authors.Add(author);
                }

                if (coauthor == null && resolution.Coauthor != null)
                {
                    coauthor = new Author(resolution.Coauthor);
                    this.authors.Add(coauthor);
                }

                if (playerAuthor == null && resolution.PlayerAuthor != null)
                {
                    playerAuthor = new Author(resolution.PlayerAuthor);
                    playerAuthor.IsPlayer = true;
                    this.authors.Add(playerAuthor);
                }

                if (playerCoauthor == null && resolution.PlayerCoauthor != null)
                {
                    playerCoauthor = new Author(resolution.PlayerCoauthor);
                    playerCoauthor.IsPlayer = true;
                    this.authors.Add(playerCoauthor);
                }

                author.Resolutions.Add(resolution);

                if (coauthor != null)
                {
                    coauthor.Resolutions.Add(resolution);
                }

                if (playerAuthor != null)
                {
                    playerAuthor.Resolutions.Add(resolution);
                }

                if (playerCoauthor != null)
                {
                    playerCoauthor.Resolutions.Add(resolution);
                }
            }

            foreach (Author author in this.authors)
            {
                foreach (Resolution resolution in author.Resolutions)
                {
                    if (author.IsPlayer)
                    {
                        if (resolution.IsRepealed)
                        {
                            if (resolution.PlayerAuthor != null && resolution.PlayerAuthor.Equals(author.Name) && resolution.PlayerCoauthor == null && resolution.Coauthor == null)
                            {
                                author.RepealedAuthor += 1;
                            }
                            else if (resolution.PlayerAuthor != null && resolution.PlayerAuthor.Equals(author.Name))
                            {
                                author.RepealedSubmittingCoauthor += 1;
                            }
                            else if (resolution.PlayerCoauthor.Equals(author.Name))
                            {
                                author.RepealedNonsubmittingCoauthor += 1;
                            }
                        }
                        else
                        {
                            if (resolution.PlayerAuthor != null && resolution.PlayerAuthor.Equals(author.Name) && resolution.PlayerCoauthor == null && resolution.Coauthor == null)
                            {
                                author.ActiveAuthor += 1;
                            }
                            else if (resolution.PlayerAuthor != null && resolution.PlayerAuthor.Equals(author.Name))
                            {
                                author.ActiveSubmittingCoauthor += 1;
                            }
                            else if (resolution.PlayerCoauthor.Equals(author.Name))
                            {
                                author.ActiveNonsubmittingCoauthor += 1;
                            }
                        }
                    }
                    else
                    {
                        if (resolution.IsRepealed)
                        {
                            if (resolution.Author.Equals(author.Name) && resolution.Coauthor == null)
                            {
                                author.RepealedAuthor += 1;
                            }
                            else if (resolution.Author.Equals(author.Name))
                            {
                                author.RepealedSubmittingCoauthor += 1;
                            }
                            else if (resolution.Coauthor.Equals(author.Name))
                            {
                                author.RepealedNonsubmittingCoauthor += 1;
                            }
                        }
                        else
                        {
                            if (resolution.Author.Equals(author.Name) && resolution.Coauthor == null)
                            {
                                author.ActiveAuthor += 1;
                            }
                            else if (resolution.Author.Equals(author.Name))
                            {
                                author.ActiveSubmittingCoauthor += 1;
                            }
                            else if (resolution.Coauthor.Equals(author.Name))
                            {
                                author.ActiveNonsubmittingCoauthor += 1;
                            }
                        }
                    }
                }
            }

            this.authors = this.authors.OrderBy(o => o.Name).ToList();
        }

        /// <summary>
        /// Generates the BBCode index for the authors.
        /// </summary>
        private void GenerateGeneralAssemblyAuthorIndex()
        {
            string bbcode = string.Empty;

            foreach (var author in this.authors)
            {
                if (author.IsPlayer)
                {
                    bbcode += "[b][PLAYER] [nation]" + author.Name + "[/nation][/b]" + Environment.NewLine;
                }
                else
                {
                    bbcode += "[b][nation]" + author.Name + "[/nation][/b]" + Environment.NewLine;
                }
                
                bbcode += "[list]";

                foreach (Resolution resolution in this.resolutions)
                {
                    if (author.IsPlayer)
                    {
                        if ((resolution.PlayerAuthor != null && resolution.PlayerAuthor.Equals(author.Name)) || (resolution.PlayerCoauthor != null && resolution.PlayerCoauthor.Equals(author.Name)))
                        {
                            string entry = "[url=http://www.nationstates.net/page=WA_past_resolutions/council=1/start=" + (resolution.Number - 1) + "]" + resolution.Title + "[/url]";

                            if (resolution.IsRepealed)
                            {
                                entry = "[strike]" + entry + "[/strike]";
                            }

                            if (resolution.PlayerCoauthor != null && resolution.PlayerCoauthor.Equals(author.Name))
                            {
                                entry += " (non-submitting co-author)";
                            }
                            else if (resolution.PlayerCoauthor != null || resolution.Coauthor != null)
                            {
                                entry += " (submitting co-author)";
                            }

                            bbcode += "[*]" + entry + Environment.NewLine;
                        }
                    }
                    else
                    {
                        if (resolution.Author.Equals(author.Name) || (resolution.Coauthor != null && resolution.Coauthor.Equals(author.Name)))
                        {
                            string entry = "[url=http://www.nationstates.net/page=WA_past_resolutions/council=1/start=" + (resolution.Number - 1) + "]" + resolution.Title + "[/url]";

                            if (resolution.IsRepealed)
                            {
                                entry = "[strike]" + entry + "[/strike]";
                            }

                            if (resolution.Coauthor != null && resolution.Coauthor.Equals(author.Name))
                            {
                                entry += " (non-submitting co-author)";
                            }
                            else if (resolution.Coauthor != null)
                            {
                                entry += " (submitting co-author)";
                            }

                            bbcode += "[*]" + entry + Environment.NewLine;
                        }
                    }
                }

                bbcode += "[/list]" + Environment.NewLine + Environment.NewLine;
            }

            if (!System.IO.Directory.Exists(this.outputPath))
            {
                System.IO.Directory.CreateDirectory(this.outputPath);
            }

            StreamWriter file = new StreamWriter(this.outputPath + "\\index.txt");
            file.Write(bbcode);
            file.Close();
        }

        /// <summary>
        /// Generates the BBCode tables for the resolutions database.
        /// </summary>
        /// <param name="orderType">The order type of the table.</param>
        private void GenerateGeneralAssemblyAuthorTable(OrderType orderType)
        {
            if (orderType == OrderType.Author)
            {
                this.authors = this.authors.OrderBy(o => o.Name).ToList();
            }
            else if (orderType == OrderType.Total)
            {
                this.authors = this.authors.OrderBy(o => o.Name).ToList();
                this.authors = this.authors.OrderByDescending(o => o.Total).ToList();
            }
            else if (orderType == OrderType.ActiveTotal)
            {
                this.authors = this.authors.OrderBy(o => o.Name).ToList();
                this.authors = this.authors.OrderByDescending(o => o.ActiveTotal).ToList();
            }
            else if (orderType == OrderType.RepealedTotal)
            {
                this.authors = this.authors.OrderBy(o => o.Name).ToList();
                this.authors = this.authors.OrderByDescending(o => o.RepealedTotal).ToList();
            }

            var bbcode = "[table]";

            bbcode += "[tr]";
            bbcode += "[td][b]" + "Author" + "[/b][/td]";
            bbcode += "[td][b]" + "Active" + "[/b][/td]";
            bbcode += "[td][b]" + "[/b][/td]";
            bbcode += "[td][b]" + "[/b][/td]";
            bbcode += "[td][b]" + "[/b][/td]";
            bbcode += "[td][b]" + "Repealed" + "[/b][/td]";
            bbcode += "[td][b]" + "[/b][/td]";
            bbcode += "[td][b]" + "[/b][/td]";
            bbcode += "[td][b]" + "[/b][/td]";
            bbcode += "[td][b]" + "Total" + "[/b][/td]";
            bbcode += "[/tr]";

            bbcode += "[tr]";
            bbcode += "[td][b]" + "[/b][/td]";
            bbcode += "[td][b]" + "As author" + "[/b][/td]";
            bbcode += "[td][b]" + "As submitting co-author" + "[/b][/td]";
            bbcode += "[td][b]" + "As non-submitting co-author" + "[/b][/td]";
            bbcode += "[td][b]" + "Total" + "[/b][/td]";
            bbcode += "[td][b]" + "As author" + "[/b][/td]";
            bbcode += "[td][b]" + "As submitting co-author" + "[/b][/td]";
            bbcode += "[td][b]" + "As non-submitting co-author" + "[/b][/td]";
            bbcode += "[td][b]" + "Total" + "[/b][/td]";
            bbcode += "[td][b]" + "[/b][/td]";
            bbcode += "[/tr]";

            foreach (Author author in this.authors)
            {
                bbcode += "[tr]";

                if (author.IsPlayer)
                {
                    bbcode += "[td][PLAYER] [nation]" + author.Name + "[/nation][/td]";
                }
                else
                {
                    bbcode += "[td][nation]" + author.Name + "[/nation][/td]";
                }

                bbcode += "[td]" + author.ActiveAuthor + "[/td]";
                bbcode += "[td]" + author.ActiveSubmittingCoauthor + "[/td]";
                bbcode += "[td]" + author.ActiveNonsubmittingCoauthor + "[/td]";
                bbcode += "[td]" + author.ActiveTotal + "[/td]";
                bbcode += "[td]" + author.RepealedAuthor + "[/td]";
                bbcode += "[td]" + author.RepealedSubmittingCoauthor + "[/td]";
                bbcode += "[td]" + author.RepealedNonsubmittingCoauthor + "[/td]";
                bbcode += "[td]" + author.RepealedTotal + "[/td]";
                bbcode += "[td]" + author.Total + "[/td]";
                bbcode += "[/tr]";
            }

            bbcode += "[/table]";

            StreamWriter file = new StreamWriter(this.outputPath + "\\table-" + (int)orderType + ".txt");
            file.Write(bbcode);
            file.Close();
        }
    }
}
