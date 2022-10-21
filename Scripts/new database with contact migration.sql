/*
I'm not exactly sure what this script does anymore because it's 
been so long since I looked at it. It looks like it's creating a new 
database and populating it with student contact information (i.e. their guardian). 
This might have been used for building a database for a new client. 
Sometimes students moved between school districts and we gave schools 
(which were our clients) the option to migrate student contact information 
to the new databases. We also migrated student contact information between 
school buildings, which was a part of our standard procedure. 
For example, when Johnny graduated from Middle school to High school, 
his data should migrate along with him.
*/

DROP TABLE IF EXISTS noname.StuContactRef;
DROP TABLE IF EXISTS noname.Contacts;
DROP TABLE IF EXISTS noname.ContactsPhones;

DROP TABLE IF EXISTS noname.StuContactRef2;
DROP TABLE IF EXISTS noname.Contacts2;
DROP TABLE IF EXISTS noname.ContactsPhones2;

CREATE TABLE noname.StuContactRef LIKE customer01database.StuContactRef;
CREATE TABLE noname.Contacts LIKE customer01database.Contacts;
CREATE TABLE noname.ContactsPhones LIKE customer01database.ContactsPhones;

CREATE TABLE noname.StuContactRef2 LIKE customer01database.StuContactRef;
CREATE TABLE noname.Contacts2 LIKE customer01database.Contacts;
CREATE TABLE noname.ContactsPhones2 LIKE customer01database.ContactsPhones;


/* ## this MUST point to the database with the new students table */
INSERT INTO noname.StuContactRef2
SELECT DISTINCT oldscr.* FROM customer01database.StuContactRef oldscr
INNER JOIN customer02database.1314_Students newstu ON newstu.StuRefNum=oldscr.StuRefNum
LEFT JOIN customer02database.StuContactRef existscr ON existscr.StuRefNum=newstu.StuRefNum
WHERE existscr.StuRefNum IS NULL AND newstu.Grade!=98;

/* ## this MUST point to the database with the new students table */
INSERT INTO noname.StuContactRef
SELECT DISTINCT oldscr.* FROM customer01database.StuContactRef oldscr
INNER JOIN customer02database.1314_Students newstu ON newstu.StuRefNum=oldscr.StuRefNum
LEFT JOIN customer02database.StuContactRef existscr ON existscr.StuRefNum=newstu.StuRefNum
WHERE existscr.StuRefNum IS NULL AND newstu.Grade!=98;

/* ## this MUST point to the database with the new students table */
INSERT INTO noname.Contacts2
SELECT DISTINCT oldcon.* FROM customer01database.Contacts oldcon
INNER JOIN customer01database.StuContactRef oldscr ON oldscr.FamRefNum=oldcon.FamRefNum
INNER JOIN customer02database.1314_Students newstu ON newstu.StuRefNum=oldscr.StuRefNum
LEFT JOIN customer02database.StuContactRef existscr ON existscr.StuRefNum=newstu.StuRefNum
WHERE existscr.StuRefNum IS NULL AND newstu.Grade!=98;

/* ## this MUST point to the database with the new students table */
INSERT INTO noname.ContactsPhones2
SELECT DISTINCT oldcp.* FROM customer01database.ContactsPhones oldcp
INNER JOIN customer01database.Contacts oldcon ON oldcon.FamRefNum=oldcp.FamRefNum
INNER JOIN customer01database.StuContactRef oldscr ON oldscr.FamRefNum=oldcon.FamRefNum
INNER JOIN customer02database.1314_Students newstu ON newstu.StuRefNum=oldscr.StuRefNum
LEFT JOIN customer02database.StuContactRef existscr ON existscr.StuRefNum=newstu.StuRefNum
WHERE existscr.StuRefNum IS NULL AND newstu.Grade!=98;

/* check for FamRefNum dupes */
SELECT newcon.FamRefNum FROM noname.Contacts2 newcon
INNER JOIN customer02database.Contacts oldcon ON oldcon.FamRefNum=newcon.FamRefNum;


INSERT INTO noname.StuContactRef
SELECT scr.* FROM noname.StuContactRef2 scr WHERE StuRefNum NOT IN (
SELECT DISTINCT newscr.StuRefNum FROM noname.StuContactRef2 newscr
INNER JOIN noname.Contacts2 newcon ON newcon.FamRefNum=newscr.FamRefNum
INNER JOIN noname.ContactsPhones2 newconph ON newconph.FamRefNum=newcon.FamRefNum
INNER JOIN customer02database.Contacts oldcon ON oldcon.FirstName=newcon.FirstName AND oldcon.LastName=newcon.LastName
INNER JOIN customer02database.ContactsPhones oldconph ON oldconph.FamRefNum=oldcon.FamRefNum AND RIGHT(oldconph.Number,4)=RIGHT(newconph.Number,4)
);

INSERT INTO noname.Contacts
SELECT con.* FROM noname.Contacts2 con WHERE FamRefNum NOT IN (
SELECT DISTINCT newcon.FamRefNum FROM noname.Contacts2 newcon
INNER JOIN noname.ContactsPhones2 newconph ON newconph.FamRefNum=newcon.FamRefNum
INNER JOIN customer02database.Contacts oldcon ON oldcon.FirstName=newcon.FirstName AND oldcon.LastName=newcon.LastName
INNER JOIN customer02database.ContactsPhones oldconph ON oldconph.FamRefNum=oldcon.FamRefNum AND RIGHT(oldconph.Number,4)=RIGHT(newconph.Number,4)
);

INSERT INTO noname.ContactsPhones
SELECT cp.* FROM noname.ContactsPhones2 cp WHERE FamRefNum NOT IN (
SELECT DISTINCT newcon.FamRefNum FROM noname.Contacts2 newcon
INNER JOIN noname.ContactsPhones2 newconph ON newconph.FamRefNum=newcon.FamRefNum
INNER JOIN customer02database.Contacts oldcon ON oldcon.FirstName=newcon.FirstName AND oldcon.LastName=newcon.LastName
INNER JOIN customer02database.ContactsPhones oldconph ON oldconph.FamRefNum=oldcon.FamRefNum AND RIGHT(oldconph.Number,4)=RIGHT(newconph.Number,4)
);


UPDATE noname.StuContactRef scr
INNER JOIN (
SELECT * FROM (
SELECT
con.FamRefNum,
IFNULL(con.FirstName,'') AS FirstName,
IFNULL(con.LastName,'') AS LastName,
IFNULL(con.Address,'') AS Address,
RIGHT(conph.Number,4) AS Number
FROM noname.Contacts2 con
INNER JOIN noname.ContactsPhones2 conph ON conph.FamRefNum=con.FamRefNum
GROUP BY con.FirstName,con.LastName,RIGHT(conph.Number,4)
UNION ALL
SELECT
con.FamRefNum,
IFNULL(con.FirstName,'') AS FirstName,
IFNULL(con.LastName,'') AS LastName,
IFNULL(con.Address,'') AS Address,
RIGHT(conph.Number,4) AS Number
FROM customer02database.Contacts con
INNER JOIN customer02database.ContactsPhones conph ON conph.FamRefNum=con.FamRefNum
GROUP BY con.FirstName,con.LastName,RIGHT(conph.Number,4)
) tab
GROUP BY tab.FirstName,tab.LastName,RIGHT(tab.Number,4)
HAVING COUNT(tab.FirstName)>1
) dupesbydestination ON dupesbydestination.FamRefNum=scr.FamRefNum
INNER JOIN (
SELECT * FROM (
SELECT
con.FamRefNum,
IFNULL(con.FirstName,'') AS FirstName,
IFNULL(con.LastName,'') AS LastName,
IFNULL(con.Address,'') AS Address,
RIGHT(conph.Number,4) AS Number
FROM customer02database.Contacts con
INNER JOIN customer02database.ContactsPhones conph ON conph.FamRefNum=con.FamRefNum
GROUP BY con.FirstName,con.LastName,RIGHT(conph.Number,4)
UNION ALL
SELECT
con.FamRefNum,
IFNULL(con.FirstName,'') AS FirstName,
IFNULL(con.LastName,'') AS LastName,
IFNULL(con.Address,'') AS Address,
RIGHT(conph.Number,4) AS Number
FROM noname.Contacts2 con
INNER JOIN noname.ContactsPhones2 conph ON conph.FamRefNum=con.FamRefNum
GROUP BY con.FirstName,con.LastName,RIGHT(conph.Number,4)
) tab
GROUP BY tab.FirstName,tab.LastName,RIGHT(tab.Number,4)
HAVING COUNT(tab.FirstName)>1
) dupesbysource ON dupesbysource.FirstName=dupesbydestination.FirstName
AND dupesbysource.LastName=dupesbydestination.LastName
AND RIGHT(dupesbysource.FirstName,4)=RIGHT(dupesbydestination.FirstName,4)
SET scr.FamRefNum=dupesbysource.FamRefNum;


/* insert contact records that do not have duplicates */
INSERT INTO noname.Contacts
(
FamRefNum,
Title,
FirstName,
LastName,
Address,
Address2,
City,
State,
Zip,
Email,
Email2,
Email3,
Email4,
Email5,
Email6,
Email7,
Email8,
SupervisorPassword,
UpdateDate
)
SELECT
con.FamRefNum,
con.Title,
con.FirstName,
con.LastName,
con.Address,
con.Address2,
con.City,
con.State,
con.Zip,
con.Email,
con.Email2,
con.Email3,
con.Email4,
con.Email5,
con.Email6,
con.Email7,
con.Email8,
con.SupervisorPassword,
con.UpdateDate
FROM noname.Contacts2 con
LEFT JOIN (
SELECT * FROM (
SELECT
con.FamRefNum,
IFNULL(con.FirstName,'') AS FirstName,
IFNULL(con.LastName,'') AS LastName,
IFNULL(con.Address,'') AS Address,
RIGHT(conph.Number,4) AS Number
FROM noname.Contacts2 con
INNER JOIN noname.ContactsPhones2 conph ON conph.FamRefNum=con.FamRefNum
GROUP BY con.FirstName,con.LastName,RIGHT(conph.Number,4)
UNION ALL
SELECT
con.FamRefNum,
IFNULL(con.FirstName,'') AS FirstName,
IFNULL(con.LastName,'') AS LastName,
IFNULL(con.Address,'') AS Address,
RIGHT(conph.Number,4) AS Number
FROM customer02database.Contacts con
INNER JOIN customer02database.ContactsPhones conph ON conph.FamRefNum=con.FamRefNum
GROUP BY con.FirstName,con.LastName,RIGHT(conph.Number,4)
) tab GROUP BY tab.FirstName,tab.LastName,RIGHT(tab.Number,4)
HAVING COUNT(tab.FirstName)>1
) dupes ON dupes.FamRefNum=con.FamRefNum
WHERE dupes.FamRefNum IS NULL;


/* insert contact phone records that do not have duplicates */
/* ## can remove address */
INSERT INTO noname.ContactsPhones
(
FamRefNum,
ID,
Rank,
Type,
Number,
Description
)
SELECT
cp.FamRefNum,
cp.ID,
cp.Rank,
cp.Type,
cp.Number,
cp.Description
FROM noname.ContactsPhones2 cp
LEFT JOIN (
SELECT * FROM (
SELECT
con.FamRefNum,
IFNULL(con.FirstName,'') AS FirstName,
IFNULL(con.LastName,'') AS LastName,
IFNULL(con.Address,'') AS Address,
RIGHT(conph.Number,4) AS Number
FROM noname.Contacts2 con
INNER JOIN noname.ContactsPhones2 conph ON conph.FamRefNum=con.FamRefNum
GROUP BY con.FirstName,con.LastName,RIGHT(conph.Number,4)
UNION ALL
SELECT
con.FamRefNum,
IFNULL(con.FirstName,'') AS FirstName,
IFNULL(con.LastName,'') AS LastName,
IFNULL(con.Address,'') AS Address,
RIGHT(conph.Number,4) AS Number
FROM customer02database.Contacts con
INNER JOIN customer02database.ContactsPhones conph ON conph.FamRefNum=con.FamRefNum
GROUP BY con.FirstName,con.LastName,RIGHT(conph.Number,4)
) tab GROUP BY tab.FirstName,tab.LastName,RIGHT(tab.Number,4)
HAVING COUNT(tab.FirstName)>1
) dupes ON dupes.FamRefNum=cp.FamRefNum
WHERE dupes.FamRefNum IS NULL;


DROP TABLE noname.StuContactRef2;
DROP TABLE noname.Contacts2;
DROP TABLE noname.ContactsPhones2;
