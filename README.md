Refactored Web Api from HighloadCup competition (https://highloadcup.ru/)
You were supposed to write web api service in any programming language using any technologies you want.
The only restriction was that your implementation should fit into a single docker container with 10gb of disk space.
Web Api represents fictitious traveling service which has 3 types of entities: Users, Locations, Visits. 
Each visit entity represents user visiting specific location at some point in time. Every visit also has a score assigned to it by the user.
The task was to implement http web api to query each of 3 entities. Each entity can be created, updated or deleted.
Also there are 2 types of queries which would use all 3 entities: 
-- locations has a query to find average score for specific location, with optional filtering parameters (visit date range, user age range, gender)
-- users has a query to return list of visits by this user, which also has a list of optional filtering parameters (visit date range, country, max distance to nearest city)
When you submit your solution, the ranking system would flood your web api with 170k get/post requests. the faster you serve valid responses the higher rank you will get.