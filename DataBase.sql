CREATE DATABASE `WeatherDatabase`;

USE `WeatherDatabase`;

DROP TABLE IF EXISTS `cities`;
CREATE TABLE `Cities` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` text NULL,
    PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `weathers`;
CREATE TABLE `Weathers` (
`Id` int NOT NULL AUTO_INCREMENT,
  `Date` datetime NOT NULL,
  `State` text,
  `Temperature` float NOT NULL,
  `WindSpeed` float NOT NULL,
  `ChancePrecipitation` int NOT NULL,
  `AirHumidity` int NOT NULL,
  `CityId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Weathers_CityId` (`CityId`),
  CONSTRAINT `FK_Weathers_Cities_CityId` FOREIGN KEY (`CityId`) REFERENCES `cities` (`Id`) ON DELETE RESTRICT
);