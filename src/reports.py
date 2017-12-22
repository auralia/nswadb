from enum import Enum
from src.db import Database


class OrderType(Enum):
    AUTHOR = 1
    TOTAL = 2
    ACTIVE_TOTAL = 3
    ACTIVE_NON_REPEALS_TOTAL = 4,
    ACTIVE_REPEALS_TOTAL = 5,
    REPEALED_TOTAL = 6


def generate_author_index(db: Database):
    bbcode = ""

    authors = db.authors[:]
    authors.extend(db.player_authors)
    authors.sort(key=lambda x: x.name)

    anchors = []
    for author in authors:
        if author.name[0] not in anchors:
            bbcode += "[anchor=index-" + author.name[0] + "][/anchor]"
            anchors.append(author.name[0])

        if author.is_player:
            bbcode += "[b][PLAYER] [nation]" + author.name + "[/nation][/b]\n"
        else:
            bbcode += "[b][nation]" + author.name + "[/nation][/b]\n"

        bbcode += "[list]"

        resolutions = author.authored_resolutions[:]
        resolutions.extend(author.coauthored_resolutions)
        resolutions.sort(key=lambda x: x.date)
        for resolution in resolutions:
            entry = "[url=http://www.nationstates.net/" \
                    "page=WA_past_resolutions/council=1/" \
                    "start=" + str(resolution.number - 1) + "]" \
                    + resolution.title + "[/url]"
            if resolution.repealed_by is not None:
                entry = "[strike]" + entry + "[/strike]"
            if resolution.coauthors:
                if author in resolution.coauthors:
                    entry += " (non-submitting co-author)"
                else:
                    entry += " (submitting co-author)"
            bbcode += "[*]" + entry + "\n"
        bbcode += "[/list]\n\n"

    print(bbcode)


def generate_author_table(db: Database, order_type: OrderType):
    authors = db.authors[:]
    authors.extend(db.player_authors)

    if order_type == OrderType.AUTHOR:
        authors.sort(key=lambda x: x.name)
    elif order_type == OrderType.TOTAL:
        authors.sort(key=lambda x: x.name)
        authors.reverse()
        authors.sort(key=lambda x: (len(x.authored_resolutions)
                                    + len(x.coauthored_resolutions)))
        authors.reverse()
    elif order_type == OrderType.ACTIVE_TOTAL:
        authors.sort(key=lambda x: x.name)
        authors.reverse()
        authors.sort(key=lambda x: (
            len(list(filter(lambda y: not y.repealed_by,
                            x.authored_resolutions)))
            + len(list(filter(lambda y: not y.repealed_by,
                              x.coauthored_resolutions)))))
        authors.reverse()
    elif order_type == OrderType.ACTIVE_NON_REPEALS_TOTAL:
        authors.sort(key=lambda x: x.name)
        authors.reverse()
        authors.sort(key=lambda x: (
            len(list(filter(lambda y: not y.repealed_by and not y.repeal,
                            x.authored_resolutions)))
            + len(list(filter(lambda y: not y.repealed_by and not y.repeal,
                              x.coauthored_resolutions)))))
        authors.reverse()
    elif order_type == OrderType.ACTIVE_REPEALS_TOTAL:
        authors.sort(key=lambda x: x.name)
        authors.reverse()
        authors.sort(key=lambda x: (
            len(list(filter(lambda y: not y.repealed_by and y.repeal,
                            x.authored_resolutions)))
            + len(list(filter(lambda y: not y.repealed_by and y.repeal,
                              x.coauthored_resolutions)))))
        authors.reverse()
    elif order_type == OrderType.REPEALED_TOTAL:
        authors.sort(key=lambda x: x.name)
        authors.reverse()
        authors.sort(key=lambda x: (
            len(list(filter(lambda y: y.repealed_by,
                            x.authored_resolutions)))
            + len(list(filter(lambda y: y.repealed_by,
                              x.coauthored_resolutions)))))
        authors.reverse()

    bbcode = "[table]"

    bbcode += "[tr]"
    bbcode += "[td][b]" + "Author" + "[/b][/td]"
    bbcode += "[td][b]" + "Active" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "Repealed" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "Total" + "[/b][/td]"
    bbcode += "[/tr]"

    bbcode += "[tr]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "Non-repeals" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "Repeals" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "Total" + "[/b][/td]"
    bbcode += "[td][b]" + "As author" + "[/b][/td]"
    bbcode += "[td][b]" + "As submitting co-author" + "[/b][/td]"
    bbcode += "[td][b]" + "As non-submitting co-author" + "[/b][/td]"
    bbcode += "[td][b]" + "Total" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[/tr]"

    bbcode += "[tr]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "As author" + "[/b][/td]"
    bbcode += "[td][b]" + "As submitting co-author" + "[/b][/td]"
    bbcode += "[td][b]" + "As non-submitting co-author" + "[/b][/td]"
    bbcode += "[td][b]" + "Total" + "[/b][/td]"
    bbcode += "[td][b]" + "As author" + "[/b][/td]"
    bbcode += "[td][b]" + "As submitting co-author" + "[/b][/td]"
    bbcode += "[td][b]" + "As non-submitting co-author" + "[/b][/td]"
    bbcode += "[td][b]" + "Total" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[td][b]" + "[/b][/td]"
    bbcode += "[/tr]"

    for author in authors:
        bbcode += "[tr]"
        if author.is_player:
            bbcode += "[td][PLAYER] [nation]" + author.name + "[/nation][/td]"
        else:
            bbcode += "[td][nation]" + author.name + "[/nation][/td]"

        active_non_repeal_author = len(list(filter(
            lambda y: not y.repealed_by and not y.coauthors and not y.repeal,
            author.authored_resolutions)))
        active_non_repeal_sub_coauthor = len(list(filter(
            lambda y: not y.repealed_by and y.coauthors and not y.repeal,
            author.authored_resolutions)))
        active_non_repeal_non_sub_coauthor = len(list(filter(
            lambda y: not y.repealed_by and not y.repeal,
            author.coauthored_resolutions)))
        active_non_repeal_total = (active_non_repeal_author
                                   + active_non_repeal_sub_coauthor
                                   + active_non_repeal_non_sub_coauthor)
        bbcode += "[td]" + str(active_non_repeal_author) + "[/td]"
        bbcode += "[td]" + str(active_non_repeal_sub_coauthor) + "[/td]"
        bbcode += "[td]" + str(active_non_repeal_non_sub_coauthor) + "[/td]"
        bbcode += "[td]" + str(active_non_repeal_total) + "[/td]"

        active_repeal_author = len(list(filter(
            lambda y: not y.coauthors and y.repeal,
            author.authored_resolutions)))
        active_repeal_sub_coauthor = len(list(filter(
            lambda y: y.coauthors and y.repeal,
            author.authored_resolutions)))
        active_repeal_non_sub_coauthor = len(list(filter(
            lambda y: y.repeal,
            author.coauthored_resolutions)))
        active_repeal_total = (active_repeal_author
                               + active_repeal_sub_coauthor
                               + active_repeal_non_sub_coauthor)
        bbcode += "[td]" + str(active_repeal_author) + "[/td]"
        bbcode += "[td]" + str(active_repeal_sub_coauthor) + "[/td]"
        bbcode += "[td]" + str(active_repeal_non_sub_coauthor) + "[/td]"
        bbcode += "[td]" + str(active_repeal_total) + "[/td]"

        active_total = active_non_repeal_total + active_repeal_total
        bbcode += "[td]" + str(active_total) + "[/td]"

        repealed_author = len(list(filter(
            lambda y: y.repealed_by and not y.coauthors,
            author.authored_resolutions)))
        repealed_sub_coauthor = len(list(filter(
            lambda y: y.repealed_by and y.coauthors,
            author.authored_resolutions)))
        repealed_non_sub_coauthor = len(list(filter(
            lambda y: y.repealed_by is not None,
            author.coauthored_resolutions)))
        repealed_total = (repealed_author + repealed_sub_coauthor
                          + repealed_non_sub_coauthor)
        bbcode += "[td]" + str(repealed_author) + "[/td]"
        bbcode += "[td]" + str(repealed_sub_coauthor) + "[/td]"
        bbcode += "[td]" + str(repealed_non_sub_coauthor) + "[/td]"
        bbcode += "[td]" + str(repealed_total) + "[/td]"

        bbcode += "[td]" + str(active_total + repealed_total) + "[/td]"
        bbcode += "[/tr]"

    bbcode += "[/table]"

    print(bbcode)


def generate_known_aliases(db: Database):
    bbcode = "[table]"

    bbcode += "[tr]"
    bbcode += "[td][b]" + "Player" + "[/b][/td]"
    bbcode += "[td][b]" + "Aliases" + "[/b][/td]"
    bbcode += "[/tr]"

    players = sorted(list(db.aliases.keys()))
    for player in players:
        bbcode += "[tr]"
        bbcode += "[td]" + player + "[/td]"
        aliases = ", ".join(["[nation]" + x + "[/nation]"
                             for x in sorted(db.aliases[player])])
        bbcode += "[td]" + aliases + "[/td]"
        bbcode += "[/tr]"

    bbcode += "[/table]"

    print(bbcode)


database = Database.create("../db/resolutions.csv", "../db/aliases.csv")
# generate_author_index(database)
# generate_author_table(database, OrderType.AUTHOR)
# generate_author_table(database, OrderType.TOTAL)
# generate_author_table(database, OrderType.ACTIVE_TOTAL)
# generate_author_table(database, OrderType.ACTIVE_NON_REPEALS_TOTAL)
# generate_author_table(database, OrderType.ACTIVE_REPEALS_TOTAL)
# generate_author_table(database, OrderType.REPEALED_TOTAL)
# generate_known_aliases(database)
