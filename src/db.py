import csv
from datetime import datetime


class Database:
    def __init__(self):
        self.resolutions = []
        self.authors = []
        self.player_authors = []
        self.aliases = {}

    @staticmethod
    def create(resolutions_path, aliases_path):
        db = Database()
        db.parse_resolutions(resolutions_path)
        db.parse_aliases(aliases_path)
        return db

    def parse_resolutions(self, path):
        with open(path) as csv_file:
            next(csv_file)
            csv_reader = csv.reader(csv_file)
            for row in csv_reader:
                self.resolutions.append(Resolution(self, *row))

    def parse_aliases(self, path):
        with open(path) as csv_file:
            next(csv_file)
            csv_reader = csv.reader(csv_file)
            for row in csv_reader:
                player_name = row[0]
                aliases = row[1].split(",")
                aliases.append(player_name)
                self.aliases[player_name] = aliases

                player = Author(player_name, is_player=True)
                for resolution in self.resolutions:
                    for alias in aliases:
                        if alias == resolution.author.name:
                            player.authored_resolutions.append(resolution)
                            resolution.player_author = player
                        elif alias in [r.name for r in resolution.coauthors]:
                            player.coauthored_resolutions.append(resolution)
                            resolution.player_coauthors.append(player)
                self.player_authors.append(player)


class Author:
    def __init__(self, name, is_player=False):
        self.name = name
        self.is_player = is_player

        self.authored_resolutions = []
        self.coauthored_resolutions = []

    def __str__(self):
        return self.name


class Resolution:
    def __init__(self, db: Database, number: str, title: str, category: str,
                 subcategory: str, author_name: str, coauthor_names: str,
                 votes_for: str, votes_against: str, date: str):
        self.db = db
        self.number = int(number)
        self.title = title
        self.category = category
        self.subcategory = subcategory

        if self.category == "Repeal":
            repeal_number = int(self.subcategory)
            for resolution in db.resolutions:
                if resolution.number == int(self.subcategory):
                    self.repeal = resolution
                    resolution.repealed_by = self
                    break
            else:
                raise RuntimeError(f"Error processing resolution {number}:"
                                   f" no resolution found with number"
                                   f" {repeal_number}")
        else:
            self.repeal = None

        for author in db.authors:
            if author.name == author_name:
                self.author = author
                break
        else:
            self.author = Author(author_name)
            db.authors.append(self.author)
        self.author.authored_resolutions.append(self)

        self.coauthors = []
        coauthor_names = coauthor_names.split(",")
        for coauthor_name in coauthor_names:
            if coauthor_name == "":
                continue

            for author in db.authors:
                if author.name == coauthor_name:
                    coauthor = author
                    self.coauthors.append(author)
                    break
            else:
                coauthor = Author(coauthor_name)
                self.coauthors.append(coauthor)
                db.authors.append(coauthor)
            coauthor.coauthored_resolutions.append(self)

        self.votes_for = int(votes_for)
        self.votes_against = int(votes_against)
        self.date = datetime.strptime(date, "%Y-%m-%d")

        self.repealed_by = None
        self.player_author = None
        self.player_coauthors = []

    def __str__(self):
        return self.title
