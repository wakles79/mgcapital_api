using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MGCap.DataAccess.Implementation.Seeds
{
    public static class BuildingEmployeeSeeds
    {
        public static List<Dictionary<string, string>> GabrielBuildings = new List<Dictionary<string, string>>
{
  new Dictionary<string, string> {
   ["Building"] = "Rexwoods II",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Carlos Benito",
   ["SupervisorPhone"] = "919 240 8406",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Rexwoods III",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Carlos Benito",
   ["SupervisorPhone"] = "919 240 8406",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Rexwoods IV",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Carlos Benito",
   ["SupervisorPhone"] = "919 240 8406",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Rexwoods V",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Carlos Benito",
   ["SupervisorPhone"] = "919 240 8406",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "4201 Lake Boone",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Jose Valiente",
   ["SupervisorPhone"] = "919 627 3370",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Midtown North",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "2841 Plaza Place",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "FCC",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Yahaira Frias",
   ["SupervisorPhone"] = "787 587 2832",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "North Hills/ FCB",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Yahaira Frias",
   ["SupervisorPhone"] = "787 587 2832",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "SDC",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Gabriel Luis",
   ["SupervisorPhone"] = "919 337 8401",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "FCB Raleigh Warehouse",
   ["Region"] = "Garner",
   ["SupervisorFullName"] = "Javier Leyva",
   ["SupervisorPhone"] = "919 210 6281",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "DAC",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Maria Alvarado",
   ["SupervisorPhone"] = "919 438 4230",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "10 lab I and II",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Ernesto Rivera",
   ["SupervisorPhone"] = "919 699 7106",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "West point",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Ernesto Rivera",
   ["SupervisorPhone"] = "919 699 7106",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "2000 CentreGreen",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Ernesto Rivera",
   ["SupervisorPhone"] = "919 699 7106",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "4000 CG",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Ernesto Rivera",
   ["SupervisorPhone"] = "919 699 7106",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "5000 CG",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Ernesto Rivera",
   ["SupervisorPhone"] = "919 699 7106",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "3000 CG",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Ernesto Rivera",
   ["SupervisorPhone"] = "919 699 7106",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "1000 CG",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Ernesto Rivera",
   ["SupervisorPhone"] = "919 699 7106",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Boyd Hall",
   ["Region"] = "Chapel Hill",
   ["SupervisorFullName"] = "Kim Youg",
   ["SupervisorPhone"] = "919 255 8143",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Europa",
   ["Region"] = "Chapel Hill",
   ["SupervisorFullName"] = "Nereyda Martinez",
   ["SupervisorPhone"] = "919 824 1373",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Exchange",
   ["Region"] = "Chapel Hill",
   ["SupervisorFullName"] = "Arnulfo Ayala",
   ["SupervisorPhone"] = "919 699 0029",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Franklin",
   ["Region"] = "Chapel Hill",
   ["SupervisorFullName"] = "Arnulfo Ayala",
   ["SupervisorPhone"] = "919 699 0029",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "McClamroch",
   ["Region"] = "Chapel Hill",
   ["SupervisorFullName"] = "Kim Youg",
   ["SupervisorPhone"] = "919 255 8143",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Carolina Square",
   ["Region"] = "Chapel Hill",
   ["SupervisorFullName"] = "Nereyda Martinez",
   ["SupervisorPhone"] = "919 824 1373",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Ribbon",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Xiomara Blanco",
   ["SupervisorPhone"] = "919 407 1926",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Credit Suisse",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Boni Polanco",
   ["SupervisorPhone"] = "919 599 0031",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Node 1 & 2",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Carlos Curz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "2605 Meridian",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Salvador Moran",
   ["SupervisorPhone"] = "919 621 5573",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "2500 Meridian",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Salvador Moran",
   ["SupervisorPhone"] = "919 621 5573",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "2505 Meridian",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Salvador Moran",
   ["SupervisorPhone"] = "919 621 5573",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "2510 Meridian",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Salvador Moran",
   ["SupervisorPhone"] = "919 621 5573",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "2520 Meridian",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Salvador Moran",
   ["SupervisorPhone"] = "919 621 5573",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "2525 Meridian",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Salvador Moran",
   ["SupervisorPhone"] = "919 621 5573",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "2645 Meridian",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Salvador Moran",
   ["SupervisorPhone"] = "919 621 5573",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "2700 Meridian",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Salvador Moran",
   ["SupervisorPhone"] = "919 621 5573",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "2800 Meridian",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Salvador Moran",
   ["SupervisorPhone"] = "919 621 5573",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "University Tower",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Jose Cruz",
   ["SupervisorPhone"] = "919 951 4768",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Valley View",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Gabriel Luis",
   ["SupervisorPhone"] = "919 337 8401",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "RTP",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "",
   ["SupervisorPhone"] = "",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Town Hall Commons",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Kim Youg",
   ["SupervisorPhone"] = "919 255 8143",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "430 Davis",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Carlos Curz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "530 Davis",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Carlos Curz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "630 Davis",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Carlos Curz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Venture I",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Osmel Lacasse",
   ["SupervisorPhone"] = "919 314 7707",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Venture II",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Osmel Lacasse",
   ["SupervisorPhone"] = "919 314 7707",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Venture III",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Osmel Lacasse",
   ["SupervisorPhone"] = "919 314 7707",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Venture IV",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Osmel Lacasse",
   ["SupervisorPhone"] = "919 314 7707",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Venture Place",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Osmel Lacasse",
   ["SupervisorPhone"] = "919 314 7707",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Alliance One",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Osmel Lacasse",
   ["SupervisorPhone"] = "919 314 7707",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Wells Fargo Tower",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Luis Cruz",
   ["SupervisorPhone"] = "919 730 7373",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "NC State Bar",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "3700 Glenwood",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Camden",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Caswell",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Chatham",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Cumberland",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Dare",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "McDowell",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Northampton",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "GlenLake 1",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Silvio Montesino",
   ["SupervisorPhone"] = "917 327 5350",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "GlenLake 4",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Silvio Montesino",
   ["SupervisorPhone"] = "917 327 5350",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "GlenLake 5",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Silvio Montesino",
   ["SupervisorPhone"] = "917 327 5350",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "GlenLake 6",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Silvio Montesino",
   ["SupervisorPhone"] = "917 327 5350",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "CBC Flex Lab",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Carolina Place",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Wycliff",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Sugar Bush",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Maria Amaya",
   ["SupervisorPhone"] = "919 757 9383",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Springfield",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Kim Youg",
   ["SupervisorPhone"] = "919 255 8143",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Landmark Center",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Kim Youg",
   ["SupervisorPhone"] = "919 255 8143",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "North Chase I",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Kim Youg",
   ["SupervisorPhone"] = "919 255 8143",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "North Chase II",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Kim Youg",
   ["SupervisorPhone"] = "919 255 8143",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Blue Ridge I",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Carlos Benito",
   ["SupervisorPhone"] = "919 240 8406",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Blue Ridge II",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Carlos Benito",
   ["SupervisorPhone"] = "919 240 8406",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "Rexwoods I",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 },
  new Dictionary<string, string> {
   ["Building"] = "2600 Meridian",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Salvador Moran",
   ["SupervisorPhone"] = "919 621 5573",
   ["OperationsManagerEmail"] = "gabriel@mgcapitalmain.com"
 }
};

        public static List<Dictionary<string, string>> WilsonBuildings = new List<Dictionary<string, string>>
{
   new Dictionary<string, string> {
   ["Building"] = "1100 Crescent Green",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Ricardo Rodriguez",
   ["SupervisorPhone"] = "917 500 8892",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "1200 Crescent Green",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Mateo Chavez",
   ["SupervisorPhone"] = "336 327 5224",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "1300 Crescent Green",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Mateo Chavez",
   ["SupervisorPhone"] = "336 327 5224",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "1400 Crescent Green",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Ricardo Rodriguez",
   ["SupervisorPhone"] = "917 500 8892",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "1225 Lakeside I",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Ricardo Rodriguez",
   ["SupervisorPhone"] = "917 500 8892",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "1255 Lakeside II",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Ricardo Rodriguez",
   ["SupervisorPhone"] = "917 500 8892",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "114 Edinburgh",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Wilson Bolaines",
   ["SupervisorPhone"] = "919 327 7134",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "500 Gregson",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Mateo Chavez",
   ["SupervisorPhone"] = "336 327 5224",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "305 Gregson",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Mateo Chavez",
   ["SupervisorPhone"] = "336 327 5224",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Bristol",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Wilson Bolaines",
   ["SupervisorPhone"] = "919 327 7134",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Chelsea",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Wilson Bolaines",
   ["SupervisorPhone"] = "919 327 7134",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Carlisle",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Wilson Bolaines",
   ["SupervisorPhone"] = "919 327 7134",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Nottingham",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Mateo Chavez",
   ["SupervisorPhone"] = "336 327 5224",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Oxford",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Wilson Bolaines",
   ["SupervisorPhone"] = "919 327 7134",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "IQVIA",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Juan Guardado",
   ["SupervisorPhone"] = "919 414 2651",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "DOT",
   ["Region"] = "DOT",
   ["SupervisorFullName"] = "Wilson Bolaines",
   ["SupervisorPhone"] = "919 327 7134",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Butterball",
   ["Region"] = "DOT",
   ["SupervisorFullName"] = "Wilson Bolaines",
   ["SupervisorPhone"] = "919 327 7134",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Methodist Curch",
   ["Region"] = "DOT",
   ["SupervisorFullName"] = "Wilson Bolaines",
   ["SupervisorPhone"] = "919 327 7134",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "1500 Perimeter",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Gilma Rodriguez",
   ["SupervisorPhone"] = "919 579 7781",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "1600 Perimeter",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Gilma Rodriguez",
   ["SupervisorPhone"] = "919 579 7781",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "2250 Perimeter",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Mateo Chavez",
   ["SupervisorPhone"] = "336 327 5224",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "2000 Perimeter",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Mateo Chavez",
   ["SupervisorPhone"] = "336 327 5224",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "3800 Paramount",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Ernesto Rivera",
   ["SupervisorPhone"] = "919 699 7106",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Perimeter Two",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Gilma Rodriguez",
   ["SupervisorPhone"] = "919 579 7781",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Perimeter Three",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Gilma Rodriguez",
   ["SupervisorPhone"] = "919 579 7781",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Perimeter Four",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Gilma Rodriguez",
   ["SupervisorPhone"] = "919 579 7781",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Forty540",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Mateo Chavez",
   ["SupervisorPhone"] = "336 327 5224",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Two Hannover",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Jose Bonilla",
   ["SupervisorPhone"] = "919 337 2097",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "All State",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "1001 Wade Ave",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Durant Medical",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Paragon",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "7208 Falls of Neuse",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "7200 Falls of Neuse",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "4700 Falls",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Wilson Bolaines",
   ["SupervisorPhone"] = "919 327 7134",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Green Road",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Jay Lee",
   ["SupervisorPhone"] = "919 291 4936",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Colonnade I",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Wilson Bolaines",
   ["SupervisorPhone"] = "919 327 7134",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Colonnade II",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Wilson Bolaines",
   ["SupervisorPhone"] = "919 327 7134",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Stonehenge I",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Maria Amaya",
   ["SupervisorPhone"] = "919 757 9383",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 },
   new Dictionary<string, string> {
   ["Building"] = "Stonehenge II",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Maria Amaya",
   ["SupervisorPhone"] = "919 757 9383",
   ["OperationsManagerEmail"] = "wilson@mgcapitalmain.com"
 }
};

        public static List<Dictionary<string, string>> JavierBuildings = new List<Dictionary<string, string>>
{
 new Dictionary<string, string> {
   ["Building"] = "Peak Way Market",
   ["Region"] = "Apex",
   ["SupervisorFullName"] = "Mateo Chavez",
   ["SupervisorPhone"] = "336 327 5224",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "11000 Regency",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Ramon Ruiz",
   ["SupervisorPhone"] = "919 758 4292",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "9000 Regency",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Ramon Ruiz",
   ["SupervisorPhone"] = "919 758 4292",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "12040 Regency Creek",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Ramon Ruiz",
   ["SupervisorPhone"] = "919 758 4292",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "100 Regency",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Ramon Ruiz",
   ["SupervisorPhone"] = "919 758 4292",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "200 Regency",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Ramon Ruiz",
   ["SupervisorPhone"] = "919 758 4292",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "400 Regency",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Ramon Ruiz",
   ["SupervisorPhone"] = "919 758 4292",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Crossroads I",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Victor Rodriguez",
   ["SupervisorPhone"] = "919 791 9993",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Crossroads II",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Victor Rodriguez",
   ["SupervisorPhone"] = "919 791 9993",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Crossroads III",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Victor Rodriguez",
   ["SupervisorPhone"] = "919 791 9993",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Crossroads IV",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Victor Rodriguez",
   ["SupervisorPhone"] = "919 791 9993",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Medfusion",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Victor Rodriguez",
   ["SupervisorPhone"] = "919 791 9993",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Epic Games",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Sheila Ramos",
   ["SupervisorPhone"] = "919 532 6576",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "1616 Evans",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Javier Leyva",
   ["SupervisorPhone"] = "919 210 6281",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "200 Lucent",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Wilson Bolaines",
   ["SupervisorPhone"] = "919 327 7134",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Cotton",
   ["Region"] = "Cary",
   ["SupervisorFullName"] = "Javier Leyva",
   ["SupervisorPhone"] = "919 210 6281",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Quadrangle II",
   ["Region"] = "Chapel Hill",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Quadrangle III",
   ["Region"] = "Chapel Hill",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Quadrangle IV",
   ["Region"] = "Chapel Hill",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Quadrangle V",
   ["Region"] = "Chapel Hill",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Durham Center",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Maria Sifuentes",
   ["SupervisorPhone"] = "919 291 6933",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "4101 Research Commons",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Xiomara Blanco",
   ["SupervisorPhone"] = "919 407 1926",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "4201 RC",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Xiomara Blanco",
   ["SupervisorPhone"] = "919 407 1926",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "4301 RC",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Xiomara Blanco",
   ["SupervisorPhone"] = "919 407 1926",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "4401 RC",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Xiomara Blanco",
   ["SupervisorPhone"] = "919 407 1926",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "4501 RC",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Xiomara Blanco",
   ["SupervisorPhone"] = "919 407 1926",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "DV I",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "DV II",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "DV III",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Crowe",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Fowler",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Reed",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Strickland",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Washington",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "ACPP",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Crowe-AU",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Strickland-AU",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Old Bull",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Noell",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Keystone with Hub",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "523 Davis (Apjet)",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Javier Leyva",
   ["SupervisorPhone"] = "919 210 6281",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "419 Davis (Infenion)",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Javier Leyva",
   ["SupervisorPhone"] = "919 210 6281",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "2100 Gateway",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Ernesto Rivera",
   ["SupervisorPhone"] = "919 699 7106",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "RDU I",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Ernesto Rivera",
   ["SupervisorPhone"] = "919 699 7106",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "RDU III",
   ["Region"] = "Morrisville",
   ["SupervisorFullName"] = "Ernesto Rivera",
   ["SupervisorPhone"] = "919 699 7106",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Red Hat Tower",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Hector Fuentes",
   ["SupervisorPhone"] = "919 381 0285",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Professional Building",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "3110 Edwords Mill",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Jose Valiente",
   ["SupervisorPhone"] = "919 627 3370",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Medical Plaza",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "David Ajqui",
   ["SupervisorPhone"] = "919 527 3866",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "2641 Sumner Blvd.",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Jose Moran",
   ["SupervisorPhone"] = "919 427 9268",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "West Chase I",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "West Chase II",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "West Chase III",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Palisades I",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Palisades II",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Trinity",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Carlos Cruz",
   ["SupervisorPhone"] = "919 410 2411",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "The Summit",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Javier Leyva",
   ["SupervisorPhone"] = "919 210 6281",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Brier Creek Medical",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Javier Leyva",
   ["SupervisorPhone"] = "919 210 6281",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "St. David School",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Javier Leyva",
   ["SupervisorPhone"] = "919 210 6281",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "FCC/ FCB",
   ["Region"] = "Raleigh",
   ["SupervisorFullName"] = "Yahaira Frias",
   ["SupervisorPhone"] = "787 587 2832",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 },
 new Dictionary<string, string> {
   ["Building"] = "Q Lab",
   ["Region"] = "Durham",
   ["SupervisorFullName"] = "Xiomara Blanco",
   ["SupervisorPhone"] = "919 407 1926",
   ["OperationsManagerEmail"] = "javier@mgcapitalmain.com"
 }
};
        public static void AddOrUpdate(MGCapDbContext context, int count = 50)
        {

            return;
            var buildings = context.Buildings.Where(b => b.CompanyId == 1);

            var employees = context.Employees
                                   .Include(e => e.Contact)
                                   .Where(e => e.CompanyId == 1);

            if (employees == null || employees.Count() == 0 || buildings == null || buildings.Count() == 0)
            {
                return;
            }

            var gabe = employees.FirstOrDefault(e => e.Email == "gabriel@mgcapitalmain.com");
            if (gabe != null)
            {
                UpdateBuildings(context, gabe, buildings, GabrielBuildings);
            }

            var javier = employees.FirstOrDefault(e => e.Email == "javier@mgcapitalmain.com");
            if (javier != null)
            {
                UpdateBuildings(context, gabe, buildings, JavierBuildings);
            }

            var wilson = employees.FirstOrDefault(e => e.Email == "wilson@mgcapitalmain.com");
            if (wilson != null)
            {
                UpdateBuildings(context, gabe, buildings, WilsonBuildings);
            }
        }

        public static void UpdateBuildings(MGCapDbContext context, Employee emp, IEnumerable<Building> buildings, List<Dictionary<string, string>> dics)
        {
            foreach (var bldg in dics)
            {
                var matched = buildings.Where(b => b.Name == bldg["Building"]);

                if (matched == null || matched.Count() == 0)
                {
                    continue;
                }

                foreach (var b in matched)
                {
                    //b.OperationsManagerId = emp.ID;
                    context.Buildings.Update(b);
                }
            }
        }
    }
}
