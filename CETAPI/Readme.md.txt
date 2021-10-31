Assumptions made: 
1. It is possible to have two updates happening at the same time for the same OrderId and insert into the database is allowed => TBD regarding concurrency 
2. It is possible to have an Insert action after an Update action for the same OrderId => OrderId 386548038 has Insert, Update, Update, Delete and repeat => this will not cause an error for the insert into the database
3. That a DELETE action affects only the very last preceding UPDATE or INSERT action with the same price and the same quantity, and not all of the preceding actions with the same price and quantity