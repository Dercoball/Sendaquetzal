/*create the table for get data from sensors*/
CREATE TABLE dbo.Devices_CollectedValues (
  ID VARCHAR(37) NOT NULL DEFAULT newid(),
  SerialNumer VARCHAR(45) NOT NULL,
  BateryValue DECIMAL(18,2) NULL,
  Channel INT NULL,
  Value DECIMAL(18,2) NULL,
  PRIMARY KEY (ID));

/*delete the dummy data, was only for test purpouse*/
DROP TABLE dummy;