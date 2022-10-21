/*
I think that this script was used to as a part of what we called 
the "move-up" process. The move-up process was done once a year 
to every client's database in order to build the foundation of the 
new school year in the database. Customers could log into the software 
based on a school year (such as 2012-2013). The move-up process populated 
the database and added the new school year to the dropdownlist menu 
on the login screen. This was also used as a stagegate for licensing.
*/

CREATE TABLE noname.StuContactRefOther LIKE customer01database.StuContactRefOther;
CREATE TABLE noname.HealthNotes LIKE customer01database.HealthNotes;



INSERT INTO noname.StuContactRefOther
SELECT scro.*
FROM customer01database.StuContactRefOther scro
INNER JOIN moveup.1314_Students stu ON stu.StuRefNum=scro.StuRefNum
LEFT JOIN customer02database.0910_Students 09stu ON 09stu.StuRefNum=stu.StuRefNum
LEFT JOIN customer02database.1011_Students 10stu ON 10stu.StuRefNum=stu.StuRefNum
LEFT JOIN customer02database.1112_Students 11stu ON 11stu.StuRefNum=stu.StuRefNum
LEFT JOIN customer02database.1213_Students 12stu ON 12stu.StuRefNum=stu.StuRefNum
WHERE 09stu.StuRefNum IS NULL
AND 10stu.StuRefNum IS NULL
AND 11stu.StuRefNum IS NULL
AND 12stu.StuRefNum IS NULL;

INSERT INTO noname.HealthNotes
(
StuRefNum,
Date,
Narrative,
UpdateDate
)
SELECT hn.StuRefNum,
hn.Date,
hn.Narrative,
hn.UpdateDate
FROM customer01database.HealthNotes hn
INNER JOIN moveup.1314_Students stu ON stu.StuRefNum=hn.StuRefNum
LEFT JOIN customer02database.0910_Students 09stu ON 09stu.StuRefNum=stu.StuRefNum
LEFT JOIN customer02database.1011_Students 10stu ON 10stu.StuRefNum=stu.StuRefNum
LEFT JOIN customer02database.1112_Students 11stu ON 11stu.StuRefNum=stu.StuRefNum
LEFT JOIN customer02database.1213_Students 12stu ON 12stu.StuRefNum=stu.StuRefNum
WHERE 09stu.StuRefNum IS NULL
AND 10stu.StuRefNum IS NULL
AND 11stu.StuRefNum IS NULL
AND 12stu.StuRefNum IS NULL;


CREATE TABLE noname.TestScores LIKE customer01database.TestScores;
CREATE TABLE noname.Immunizations LIKE customer01database.Immunizations;
CREATE TABLE noname.HealthHistory LIKE customer01database.HealthHistory;


INSERT INTO noname.TestScores
(
StuRefNum,
Date,
IsUsed,
TestID,
Grade,
Score,
ScoreType,
TestScoresComment,
DateType,
Result,
Percentile,
Proficiency,
CreationDate
)
SELECT ts.StuRefNum,
IFNULL(ts.Date,'0000-00-00'),
IFNULL(ts.IsUsed,0),
IFNULL(ts.TestID,''),
IFNULL(ts.Grade,''),
IFNULL(ts.Score,''),
IFNULL(ts.ScoreType,0),
IFNULL(ts.TestScoresComment,''),
IFNULL(ts.DateType,0),
IFNULL(ts.Result,0),
IFNULL(ts.Percentile,0),
IFNULL(ts.Proficiency,''),
ts.CreationDate
FROM customer01database.TestScores ts
INNER JOIN customer01database.1213_Students stu
ON stu.StuRefNum=ts.StuRefNum
LEFT JOIN customer02database.TestScores ts2
ON ts2.StuRefNum=ts.StuRefNum
AND IFNULL(ts2.Date,'0000-00-00')=IFNULL(ts.Date,'0000-00-00')
AND IFNULL(ts2.TestID,'')=IFNULL(ts.TestID,'')
AND IFNULL(ts2.Grade,'')=IFNULL(ts.Grade,'')
AND IFNULL(ts2.Score,'')=IFNULL(ts.Score,'')
AND IFNULL(ts2.ScoreType,0)=IFNULL(ts.ScoreType,0)
AND IFNULL(ts2.DateType,0)=IFNULL(ts.DateType,0)
AND IFNULL(ts2.Result,0)=IFNULL(ts.Result,0)
AND IFNULL(ts2.Percentile,0)=IFNULL(ts.Percentile,0)
AND IFNULL(ts2.Proficiency,'')=IFNULL(ts.Proficiency,'')
WHERE ts2.StuRefNum IS NULL;


INSERT INTO noname.Immunizations
(
StuRefNum,
Date,
Type,
DateType,
IsAdministeredAtSchool,
IsValid,
Dose,
Exemption,
Source,
Comment,
UpdateDate
)
SELECT imm.StuRefNum,
IFNULL(imm.Date,'0000-00-00'),
IFNULL(imm.Type,0),
IFNULL(imm.DateType,0),
IFNULL(imm.IsAdministeredAtSchool,0),
IFNULL(imm.IsValid,0),
IFNULL(imm.Dose,0),
IFNULL(imm.Exemption,0),
IFNULL(imm.Source,''),
IFNULL(imm.Comment,''),
imm.UpdateDate
FROM customer01database.Immunizations imm
INNER JOIN customer01database.1213_Students stu
ON stu.StuRefNum=imm.StuRefNum
LEFT JOIN customer02database.Immunizations imm2
ON imm2.StuRefNum=imm.StuRefNum
AND IFNULL(imm2.Date,'0000-00-00')=IFNULL(imm.Date,'0000-00-00')
AND IFNULL(imm2.Type,0)=IFNULL(imm.Type,0)
AND IFNULL(imm2.DateType,0)=IFNULL(imm.DateType,0)
AND IFNULL(imm2.IsAdministeredAtSchool,0)=IFNULL(imm.IsAdministeredAtSchool,0)
AND IFNULL(imm2.IsValid,0)=IFNULL(imm.IsValid,0)
AND IFNULL(imm2.Dose,0)=IFNULL(imm.Dose,0)
AND IFNULL(imm2.Exemption,0)=IFNULL(imm.Exemption,0)
AND IFNULL(imm2.Source,'')=IFNULL(imm.Source,'')
WHERE imm2.StuRefNum IS NULL;


INSERT INTO noname.HealthHistory
(
StuRefNum,
RecordType,
Flag,
FieldA,
FieldB,
Date,
Grade,
Comment,
UpdateDate
)
SELECT hhist.StuRefNum,
IFNULL(hhist.RecordType,0),
IFNULL(hhist.Flag,0),
IFNULL(hhist.FieldA,''),
IFNULL(hhist.FieldB,''),
IFNULL(hhist.Date,'0000-00-00'),
IFNULL(hhist.Grade,''),
IFNULL(hhist.Comment,''),
hhist.UpdateDate
FROM customer01database.HealthHistory hhist
INNER JOIN customer01database.1213_Students stu
ON stu.StuRefNum=hhist.StuRefNum
LEFT JOIN customer02database.HealthHistory hhist2
ON hhist2.StuRefNum=hhist.StuRefNum
AND IFNULL(hhist2.RecordType,0)=IFNULL(hhist.RecordType,0)
AND IFNULL(hhist2.FieldA,'')=IFNULL(hhist.FieldA,'')
AND IFNULL(hhist2.FieldB,'')=IFNULL(hhist.FieldB,'')
AND IFNULL(hhist2.Date,'0000-00-00')=IFNULL(hhist.Date,'0000-00-00')
AND IFNULL(hhist2.Grade,'')=IFNULL(hhist.Grade,'')
WHERE hhist2.StuRefNum IS NULL;


