from src.db import Database
from src.reports import (generate_author_index, generate_author_table,
                         generate_known_aliases, OrderType)

if '__name__' == '__main__':
    db = Database.create('../db/resolutions.csv', '../db/aliases.csv')
    generate_author_index(db)
    generate_author_table(db, OrderType.AUTHOR)
    generate_author_table(db, OrderType.TOTAL)
    generate_author_table(db, OrderType.ACTIVE_TOTAL)
    generate_author_table(db, OrderType.ACTIVE_NON_REPEALS_TOTAL)
    generate_author_table(db, OrderType.ACTIVE_REPEALS_TOTAL)
    generate_author_table(db, OrderType.REPEALED_TOTAL)
    generate_known_aliases(db)
