CREATE TABLE IF NOT EXISTS bk2interview_infos
SELECT * FROM interview_infos;

CREATE TABLE IF NOT EXISTS bkinterview_infos_1902
SELECT * FROM interview_infos;

CREATE TABLE IF NOT EXISTS bkanswers_1902
SELECT * FROM answers;

CREATE TABLE IF NOT EXISTS bkopen_endeds_1902
SELECT * FROM open_endeds;


CREATE TABLE IF NOT EXISTS bkinterview_infos_2011
SELECT * FROM interview_infos WHERE project_id=23817;

CREATE TABLE IF NOT EXISTS bkanswers_2011
SELECT * FROM answers WHERE project_id=23817;

CREATE TABLE IF NOT EXISTS bkopen_endeds_2011
SELECT * FROM open_endeds WHERE project_id=23817;


DELETE FROM `answers` WHERE `project_id`=23817;
DELETE FROM `interview_infos` WHERE `project_id`=23817;
DELETE FROM `open_endeds` WHERE `project_id`=23817;