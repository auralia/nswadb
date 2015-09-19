using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Auralia.NationStates.ResolutionsDatabase
{
    class BBCodeGenerator
    {
        public string DATABASE_PATH;
        public string OUTPUT_PATH;
        public string DATABASE_CONNECTION_STRING;
        public string DATABASE_COMMAND;

        DataSet dataSet;
        List<Resolution> resolutions;
        List<Author> authors;

        public BBCodeGenerator()
        {
            DATABASE_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\..\\..\\database\\resolutions.mdb";
            OUTPUT_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\..\\out\\";
            DATABASE_CONNECTION_STRING = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DATABASE_PATH;
            DATABASE_COMMAND = "SELECT * FROM RESOLUTIONS";

            getDataSet();
            getResolutions();
            getAuthors();
        }

        public static void Main(string[] args)
        {
            var prog = new BBCodeGenerator();
            prog.generateGeneralAssemblyAuthorIndex();

            prog.generateGeneralAssemblyAuthorTable(OrderType.Author);
            prog.generateGeneralAssemblyAuthorTable(OrderType.Total);
            prog.generateGeneralAssemblyAuthorTable(OrderType.ActiveTotal);
            prog.generateGeneralAssemblyAuthorTable(OrderType.RepealedTotal);

            Console.WriteLine("BBCode list and tables generation complete.");
            Console.ReadKey();
        }

        public void getDataSet()
        {
            var connection = new OleDbConnection(DATABASE_CONNECTION_STRING);
            connection.Open();

            var command = new OleDbCommand(DATABASE_COMMAND, connection);

            var dataAdapter = new OleDbDataAdapter(command);

            dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            connection.Close();
        }

        public void getResolutions()
        {
            resolutions = new List<Resolution>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
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

                resolutions.Add(resolution);
            }

            resolutions = resolutions.OrderBy(o => o.Number).ToList();
        }

        public void getAuthors()
        {
            authors = new List<Author>();

            foreach (Resolution resolution in resolutions)
            {
                Author author = null;
                Author coauthor = null;
                Author playerAuthor = null;
                Author playerCoauthor = null;

                foreach (Author auth in authors)
                {
                    if (!auth.isPlayer)
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
                    authors.Add(author);
                }
                if (coauthor == null && resolution.Coauthor != null)
                {
                    coauthor = new Author(resolution.Coauthor);
                    authors.Add(coauthor);
                }
                if (playerAuthor == null && resolution.PlayerAuthor != null)
                {
                    playerAuthor = new Author(resolution.PlayerAuthor);
                    playerAuthor.isPlayer = true;
                    authors.Add(playerAuthor);
                }
                if (playerCoauthor == null && resolution.PlayerCoauthor != null)
                {
                    playerCoauthor = new Author(resolution.PlayerCoauthor);
                    playerCoauthor.isPlayer = true;
                    authors.Add(playerCoauthor);
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

            foreach (Author author in authors)
            {
                foreach (Resolution resolution in author.Resolutions)
                {
                    if (author.isPlayer)
                    {
                        if (resolution.IsRepealed)
                        {
                            if (resolution.PlayerAuthor.Equals(author.Name) && resolution.PlayerCoauthor == null && resolution.Coauthor == null)
                            {
                                author.repealedAuthor += 1;
                            }
                            else if (resolution.PlayerAuthor.Equals(author.Name))
                            {
                                author.repealedSubmittingCoauthor += 1;
                            }
                            else if (resolution.PlayerCoauthor.Equals(author.Name))
                            {
                                author.repealedNonsubmittingCoauthor += 1;
                            }
                        }
                        else
                        {
                            if (resolution.PlayerAuthor.Equals(author.Name) && resolution.PlayerCoauthor == null && resolution.Coauthor == null)
                            {
                                author.activeAuthor += 1;
                            }
                            else if (resolution.PlayerAuthor.Equals(author.Name))
                            {
                                author.activeSubmittingCoauthor += 1;
                            }
                            else if (resolution.PlayerCoauthor.Equals(author.Name))
                            {
                                author.activeNonsubmittingCoauthor += 1;
                            }
                        }
                    }
                    else
                    {
                        if (resolution.IsRepealed)
                        {
                            if (resolution.Author.Equals(author.Name) && resolution.Coauthor == null)
                            {
                                author.repealedAuthor += 1;
                            }
                            else if (resolution.Author.Equals(author.Name))
                            {
                                author.repealedSubmittingCoauthor += 1;
                            }
                            else if (resolution.Coauthor.Equals(author.Name))
                            {
                                author.repealedNonsubmittingCoauthor += 1;
                            }
                        }
                        else
                        {
                            if (resolution.Author.Equals(author.Name) && resolution.Coauthor == null)
                            {
                                author.activeAuthor += 1;
                            }
                            else if (resolution.Author.Equals(author.Name))
                            {
                                author.activeSubmittingCoauthor += 1;
                            }
                            else if (resolution.Coauthor.Equals(author.Name))
                            {
                                author.activeNonsubmittingCoauthor += 1;
                            }
                        }
                    }
                }
            }

            authors = authors.OrderBy(o => o.Name).ToList();
        }

        public void generateGeneralAssemblyAuthorIndex()
        {
            string bbcode = "";

            foreach (var author in authors)
            {
                if (author.isPlayer)
                {
                    bbcode += "[b][PLAYER] [nation]" + author.Name + "[/nation][/b]" + Environment.NewLine;
                }
                else
                {
                    bbcode += "[b][nation]" + author.Name + "[/nation][/b]" + Environment.NewLine;
                }

                
                bbcode += "[list]";

                foreach (Resolution resolution in resolutions)
                {
                    if (author.isPlayer)
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

            if (!System.IO.Directory.Exists(OUTPUT_PATH))
            {
                System.IO.Directory.CreateDirectory(OUTPUT_PATH);
            }

            StreamWriter file = new StreamWriter(OUTPUT_PATH + "\\index.txt");
            file.Write(bbcode);
            file.Close();
        }

        public enum OrderType
        {
            Author = 1,
            Total = 2,
            ActiveTotal = 3,
            RepealedTotal = 4
        }

        public void generateGeneralAssemblyAuthorTable(OrderType orderType)
        {
            if (orderType == OrderType.Author)
            {
                authors = authors.OrderBy(o => o.Name).ToList();
            }
            else if (orderType == OrderType.Total)
            {
                authors = authors.OrderBy(o => o.Name).ToList();
                authors = authors.OrderByDescending(o => o.total).ToList();
            }
            else if (orderType == OrderType.ActiveTotal)
            {
                authors = authors.OrderBy(o => o.Name).ToList();
                authors = authors.OrderByDescending(o => o.activeTotal).ToList();
            }
            else if (orderType == OrderType.RepealedTotal)
            {
                authors = authors.OrderBy(o => o.Name).ToList();
                authors = authors.OrderByDescending(o => o.repealedTotal).ToList();
            }

            var bbcode = "[table]";

            bbcode += "[tr]";
            bbcode += "[td][b]" + "Author" + "[/b][/td]";
            bbcode += "[td][b]" + "Active" + "[/b][/td]";
            bbcode += "[td][b]" + "" + "[/b][/td]";
            bbcode += "[td][b]" + "" + "[/b][/td]";
            bbcode += "[td][b]" + "" + "[/b][/td]";
            bbcode += "[td][b]" + "Repealed" + "[/b][/td]";
            bbcode += "[td][b]" + "" + "[/b][/td]";
            bbcode += "[td][b]" + "" + "[/b][/td]";
            bbcode += "[td][b]" + "" + "[/b][/td]";
            bbcode += "[td][b]" + "Total" + "[/b][/td]";
            bbcode += "[/tr]";

            bbcode += "[tr]";
            bbcode += "[td][b]" + "" + "[/b][/td]";
            bbcode += "[td][b]" + "As author" + "[/b][/td]";
            bbcode += "[td][b]" + "As submitting co-author" + "[/b][/td]";
            bbcode += "[td][b]" + "As non-submitting co-author" + "[/b][/td]";
            bbcode += "[td][b]" + "Total" + "[/b][/td]";
            bbcode += "[td][b]" + "As author" + "[/b][/td]";
            bbcode += "[td][b]" + "As submitting co-author" + "[/b][/td]";
            bbcode += "[td][b]" + "As non-submitting co-author" + "[/b][/td]";
            bbcode += "[td][b]" + "Total" + "[/b][/td]";
            bbcode += "[td][b]" + "" + "[/b][/td]";
            bbcode += "[/tr]";

            foreach (Author author in authors)
            {
                bbcode += "[tr]";
                if (author.isPlayer)
                {
                    bbcode += "[td][PLAYER] [nation]" + author.Name + "[/nation][/td]";
                }
                else
                {
                    bbcode += "[td][nation]" + author.Name + "[/nation][/td]";
                }
                bbcode += "[td]" + author.activeAuthor + "[/td]";
                bbcode += "[td]" + author.activeSubmittingCoauthor + "[/td]";
                bbcode += "[td]" + author.activeNonsubmittingCoauthor + "[/td]";
                bbcode += "[td]" + author.activeTotal + "[/td]";
                bbcode += "[td]" + author.repealedAuthor + "[/td]";
                bbcode += "[td]" + author.repealedSubmittingCoauthor + "[/td]";
                bbcode += "[td]" + author.repealedNonsubmittingCoauthor + "[/td]";
                bbcode += "[td]" + author.repealedTotal + "[/td]";
                bbcode += "[td]" + author.total + "[/td]";
                bbcode += "[/tr]";
            }
            bbcode += "[/table]";

            StreamWriter file = new StreamWriter(OUTPUT_PATH + "\\table-" + (int)orderType + ".txt");
            file.Write(bbcode);
            file.Close();
        }
    }
}
