# Yuvo-Project

##API DOCUMENTATION

###Create initial table:

create table TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER (
  NETWORK_SID INT PRIMARY KEY,
  DATETIME_KEY DATETIME,	
  NEID FLOAT,
  "OBJECT" VARCHAR,
  "TIME" DATETIME,
  "INTERVAL" INT,
  DIRECTION VARCHAR,
  NEALIAS	VARCHAR,
  NETYPE	VARCHAR,
  RXLEVELBELOWTS1	INT,
  RXLEVELBELOWTS2	INT,
  MINRXLEVEL FLOAT,
  MAXRXLEVEL FLOAT,
  TXLEVELABOVETS1	INT,
  MINTXLEVEL FLOAT,
  MAXTXLEVEL FLOAT,
  IDLOGNUM INT,
  FAILUREDESCRIPTION VARCHAR,
  LINK VARCHAR,
  TID VARCHAR ,
  FARENDTID VARCHAR,
  SLOT VARCHAR,
  PORT VARCHAR
);

###Create daily/hourly aggregation tables:

create table TRANS_MW_AGG_SLOT_HOURLY (
  "TIME" DATETIME,
  LINK VARCHAR,
  SLOT VARCHAR,
  NEALIAS	VARCHAR,
  NETYPE	VARCHAR,
  MAX_RX_LEVEL FLOAT,
  MAX_TX_LEVEL FLOAT,
  RSL_DEVIATION FLOAT
);

create table TRANS_MW_AGG_SLOT_DAILY (
  "TIME" DATETIME,
  LINK VARCHAR,
  SLOT VARCHAR,
  NEALIAS	VARCHAR,
  NETYPE	VARCHAR,
  MAX_RX_LEVEL FLOAT,
  MAX_TX_LEVEL FLOAT,
  RSL_DEVIATION FLOAT
);

###Create tables to check for already parsed and loaded files.

create table parsed_files(
fileName VARCHAR
); 

create table loaded_files(
fileName VARCHAR
); 

Create folders for the parser files, and another for loader files. 
Change the paths of the folders in appsettings.json according to your local device.
