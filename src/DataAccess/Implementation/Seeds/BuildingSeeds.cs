using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MGCap.DataAccess.Implementation.Seeds
{

    /// <summary>
    ///     Contains the operation for seeding the
    ///     Building and Location tables
    /// </summary>
    public static class BuildingSeeds
    {
        public static List<Dictionary<string, string>> Buildings = new List<Dictionary<string, string>>
{
 new Dictionary<string, string> {
   ["BuildingName"] = "Rdu I",
   ["CustomerName"] = "Avison Young",
   ["BuildingAddress"] = "3000 Rdu Center Drive",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "114 Edinburgh Bldg",
   ["CustomerName"] = "Avison Young",
   ["BuildingAddress"] = "114 Edinburgh South Drive",
   ["City"] = "Cary",
   ["ZipCode"]= "27511",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Methodist Church",
   ["CustomerName"] = "Avison Young",
   ["BuildingAddress"] = "700 Waterfield Ridge Pl.",
   ["City"] = "Garner",
   ["ZipCode"]= "27529",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Stonehenge I ",
   ["CustomerName"] = "Avison Young",
   ["BuildingAddress"] = "7101 Creedmoor Rd",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27613",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Stonehenge Ii",
   ["CustomerName"] = "Avison Young",
   ["BuildingAddress"] = "7201 Creedmoor Rd",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27613",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "1400 Crescent -I",
   ["CustomerName"] = "Avison Young",
   ["BuildingAddress"] = "1400 Crescent Green",
   ["City"] = "Cary",
   ["ZipCode"]= "27511",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Landmark Center -I-ii",
   ["CustomerName"] = "Avison Young",
   ["BuildingAddress"] = "4601 Forks Rd. ",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Europa Center",
   ["CustomerName"] = "Avison Young",
   ["BuildingAddress"] = "100 Europa Drive",
   ["City"] = "Chapel Hill",
   ["ZipCode"]= "27517",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Westpoint",
   ["CustomerName"] = "Avison Young",
   ["BuildingAddress"] = "8706 Nc Highway 751",
   ["City"] = "Durham",
   ["ZipCode"]= "27713",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Professional Building",
   ["CustomerName"] = "Avison Young",
   ["BuildingAddress"] = "927 West Hagett St.",
   ["City"] = "",
   ["ZipCode"] = "",
   ["InternalNotes"] = "Is This The Correct Address? Correct Spelling? "
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2500 Meridian",
   ["CustomerName"] = "American Real Estate Partners Mgt",
   ["BuildingAddress"] = "2500 Meridian Pkwy",
   ["City"] = "Durham",
   ["ZipCode"]= "27713",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2505 Meridian",
   ["CustomerName"] = "American Real Estate Partners Mgt",
   ["BuildingAddress"] = "2505 Meridian Pkwy",
   ["City"] = "Durham",
   ["ZipCode"]= "27713",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2510 Meridian",
   ["CustomerName"] = "American Real Estate Partners Mgt",
   ["BuildingAddress"] = "2510 Meridian Pkwy",
   ["City"] = "Durham",
   ["ZipCode"]= "27713",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2520 Meridian",
   ["CustomerName"] = "American Real Estate Partners Mgt",
   ["BuildingAddress"] = "2520 Meridian Pkwy",
   ["City"] = "Durham",
   ["ZipCode"]= "27713",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2525 Meridian",
   ["CustomerName"] = "American Real Estate Partners Mgt",
   ["BuildingAddress"] = "2525 Meridian Pkwy",
   ["City"] = "Durham",
   ["ZipCode"]= "27713",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2645 Meridian",
   ["CustomerName"] = "American Real Estate Partners Mgt",
   ["BuildingAddress"] = "2645 Meridian Pkwy",
   ["City"] = "Durham",
   ["ZipCode"]= "27713",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2700 Meridian",
   ["CustomerName"] = "American Real Estate Partners Mgt",
   ["BuildingAddress"] = "2700 Meridian Pkwy",
   ["City"] = "Durham",
   ["ZipCode"]= "27713",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2800 Meridian",
   ["CustomerName"] = "American Real Estate Partners Mgt",
   ["BuildingAddress"] = "2800 Meridian Pkwy",
   ["City"] = "Durham",
   ["ZipCode"]= "27713",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "430 Davis",
   ["CustomerName"] = "American Real Estate Partners Mgt",
   ["BuildingAddress"] = "430 Davis Dr.",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "530 Davis",
   ["CustomerName"] = "American Real Estate Partners Mgt",
   ["BuildingAddress"] = "530 Davis Dr.",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Diamond View I",
   ["CustomerName"] = "Blackwell Street Management",
   ["BuildingAddress"] = "512 S. Mangum St.",
   ["City"] = "Durham",
   ["ZipCode"]= "27701",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Diamond View Ii",
   ["CustomerName"] = "Blackwell Street Management",
   ["BuildingAddress"] = "280 S. Mangum St.",
   ["City"] = "Durham",
   ["ZipCode"]= "27701",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Diamond View Iii",
   ["CustomerName"] = "Blackwell Street Management",
   ["BuildingAddress"] = "359 Blackwell St.",
   ["City"] = "Durham",
   ["ZipCode"]= "27701",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Hill",
   ["CustomerName"] = "Blackwell Street Management",
   ["BuildingAddress"] = "210 W. Pettigrew St.",
   ["City"] = "Durham",
   ["ZipCode"]= "27701",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Old Bull",
   ["CustomerName"] = "Blackwell Street Management",
   ["BuildingAddress"] = "300 Blackwell St.",
   ["City"] = "Durham",
   ["ZipCode"]= "27701",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Noell",
   ["CustomerName"] = "Blackwell Street Management",
   ["BuildingAddress"] = "312 Blackwell St.",
   ["City"] = "Durham",
   ["ZipCode"]= "27701",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Strickland",
   ["CustomerName"] = "Blackwell Street Management",
   ["BuildingAddress"] = "334 Blackwell Street",
   ["City"] = "Durham",
   ["ZipCode"]= "27701",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Washington Bay",
   ["CustomerName"] = "Blackwell Street Management",
   ["BuildingAddress"] = "324 Blackwell Street",
   ["City"] = "Durham",
   ["ZipCode"]= "27701",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Reed",
   ["CustomerName"] = "Blackwell Street Management",
   ["BuildingAddress"] = "318 Blackwell St.",
   ["City"] = "Durham",
   ["ZipCode"]= "27701",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Crowe",
   ["CustomerName"] = "Blackwell Street Management",
   ["BuildingAddress"] = "406 Blackwell St.",
   ["City"] = "Durham",
   ["ZipCode"]= "27701",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Fowler",
   ["CustomerName"] = "Blackwell Street Management",
   ["BuildingAddress"] = "410 Blackwell St.",
   ["City"] = "Durham",
   ["ZipCode"]= "27701",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Power Plant",
   ["CustomerName"] = "Blackwell Street Management",
   ["BuildingAddress"] = "320 Blackwell St.",
   ["City"] = "Durham",
   ["ZipCode"]= "27701",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Two Hanover Square",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "434 Fayetteville Street",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27601",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Durant Medical",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "10880 Durant Road",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27614",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Springfield",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "700 Spring Forest Road",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2841 Plaza Place",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "2841 Plaza Place",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "1225 Lakeside",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "1225 Crescent Green",
   ["City"] = "Cary",
   ["ZipCode"]= "27511",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "1255 Lakeside",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "1255 Crescent Green ",
   ["City"] = "Cary",
   ["ZipCode"]= "27511",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "500 Gregson",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "500 Gregson Drive, Suite 150",
   ["City"] = "Cary",
   ["ZipCode"]= "27511",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "5501 Dillard (Medfusion)",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "5501 Dillard Drive",
   ["City"] = "Cary",
   ["ZipCode"]= "27518",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Exchange West",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "1414 Raleigh Road",
   ["City"] = "Chapel Hill",
   ["ZipCode"]= "27517",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Franklin St. Trust",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "1450 Raleigh Road",
   ["City"] = "Chapel Hill",
   ["ZipCode"]= "27517",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Building 1- 10 Lab Drive",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "10 Laboratory Dr.",
   ["City"] = "Durham",
   ["ZipCode"]= "27709",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Building 2-10 Lab Drive",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "10 Laboratory Dr.",
   ["City"] = "Durham",
   ["ZipCode"]= "27709",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Wycliff",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "4414 Lake Boone Trail",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27606",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Medical Plaza",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "2610 Wycliff Road",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27607",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "The Summit",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "4101 Lake Boone Trail",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27607",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Cbc Flex Lab",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "1001 William Moore Drive",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27606",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Durham Center",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "300 West Morgan Street",
   ["City"] = "Durham",
   ["ZipCode"]= "27701",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "3110 Edwards Mill",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "3110 Edwards Mill Rd.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Palisades I ",
   ["CustomerName"] = "Capridge Partners",
   ["BuildingAddress"] = "5400 Trinity Rd",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27607",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Palisades Ii ",
   ["CustomerName"] = "Capridge Partners",
   ["BuildingAddress"] = "5410 Trinity Rd",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27607",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Cotton Corp",
   ["CustomerName"] = "Cotton Corporation",
   ["BuildingAddress"] = "6399 Weston Pkwy",
   ["City"] = "Cary",
   ["ZipCode"]= "27513",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "West Chase I",
   ["CustomerName"] = "Cushman And Wakefield",
   ["BuildingAddress"] = "4020 Westchase Blvd",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27607",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "West Chase Ii",
   ["CustomerName"] = "Cushman And Wakefield",
   ["BuildingAddress"] = "4000 Westchase Blvd",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27607",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "West Chase Iii",
   ["CustomerName"] = "Cushman And Wakefield",
   ["BuildingAddress"] = "4011westchase Blvd",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27607",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "1616 Evans ",
   ["CustomerName"] = "Cushman And Wakefield",
   ["BuildingAddress"] = "1616 Evans Road",
   ["City"] = "Cary",
   ["ZipCode"]= "27513",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Nottingham",
   ["CustomerName"] = "Cushman And Wakefield",
   ["BuildingAddress"] = "6601 Six Forks Road",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Colonnade I",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "8540 Six Forks Road",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "1100 Crescent",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "1100 Crescent Green",
   ["City"] = "Cary",
   ["ZipCode"]= "27511",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Forty540",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "3030 Slater Rd.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Venture L",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "940 Main Campus Drive",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27606",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Venture Ll",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "920 Main Campus Drive",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27606",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Venture Lll",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "900 Main Campus Drive",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27606",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Venture Iv",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "930 Main Campus Drive",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27606",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Venture Place",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "1730 Varsity Drive",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27606",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "4101 Research Commons",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "79 Tw Alexander Drive",
   ["City"] = "Rtp",
   ["ZipCode"]= "27713",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "4201 Research Commons",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "79 Tw Alexander Drive",
   ["City"] = "Rtp",
   ["ZipCode"]= "27713",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "4301 Research Commons",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "79 Tw Alexander Drive",
   ["City"] = "Rtp",
   ["ZipCode"]= "27713",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "4401 Research Commons",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "79 Tw Alexander Drive",
   ["City"] = "Rtp",
   ["ZipCode"]= "27713",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "4501 Research Commons",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "79 Tw Alexander Drive",
   ["City"] = "Rtp",
   ["ZipCode"]= "27713",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Parmer Rtp",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "5 Moore Dr. ",
   ["City"] = "Durham",
   ["ZipCode"]= "27709",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Carolina Square",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "5922 Six Forks Road, ",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Colonnade Ii",
   ["CustomerName"] = "Cb Richard Ellis",
   ["BuildingAddress"] = "8520 Six Forks Rd",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Alliance One",
   ["CustomerName"] = "Craig Davis Properties",
   ["BuildingAddress"] = "901 Main Campus Dr.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27606",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "1200 Crescent",
   ["CustomerName"] = "Conduent",
   ["BuildingAddress"] = "1200 Crescent Green",
   ["City"] = "Cary",
   ["ZipCode"]= "27511",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "1300 Crescent",
   ["CustomerName"] = "Conduent",
   ["BuildingAddress"] = "1300 Crescent Green",
   ["City"] = "Cary",
   ["ZipCode"]= "27511",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Green Road",
   ["CustomerName"] = "Conduent",
   ["BuildingAddress"] = "4924 Green Road",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27616",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2641 Sumner Blvd.",
   ["CustomerName"] = "Conduent",
   ["BuildingAddress"] = "2641 Sumner Blvd.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "9000 Regency",
   ["CustomerName"] = "Dilweg",
   ["BuildingAddress"] = "9000 Regency Parkway",
   ["City"] = "Cary",
   ["ZipCode"]= "27518",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "11000 Regency",
   ["CustomerName"] = "Dilweg",
   ["BuildingAddress"] = "11000 Regency Parkway",
   ["City"] = "Cary",
   ["ZipCode"]= "27518",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Rdu Iiii",
   ["CustomerName"] = "Dilweg",
   ["BuildingAddress"] = "2121 Rdu Center Drive",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Univerity Tower",
   ["CustomerName"] = "Dilweg",
   ["BuildingAddress"] = "3100 Tower Blvd.",
   ["City"] = "Durham",
   ["ZipCode"]= "27707",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Rdu Iii",
   ["CustomerName"] = "Dilweg",
   ["BuildingAddress"] = "2121 Durham Center Dr",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Quadrangle Ii",
   ["CustomerName"] = "Dilweg",
   ["BuildingAddress"] = "6320 Quadrangle Drive",
   ["City"] = "Chapel Hill",
   ["ZipCode"]= "27517",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Quadrangle Iii",
   ["CustomerName"] = "Dilweg",
   ["BuildingAddress"] = "6350 Quadrangle Drive",
   ["City"] = "Chapel Hill",
   ["ZipCode"]= "27517",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Quadrangle Iv",
   ["CustomerName"] = "Dilweg",
   ["BuildingAddress"] = "6340 Quadrangle Drive",
   ["City"] = "Chapel Hill",
   ["ZipCode"]= "27517",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Quadrangle V",
   ["CustomerName"] = "Dilweg",
   ["BuildingAddress"] = "6330 Quadrangle Drive",
   ["City"] = "Chapel Hill",
   ["ZipCode"]= "27517",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "400 Regency",
   ["CustomerName"] = "Epic Regency",
   ["BuildingAddress"] = "400 Regency Forest Street",
   ["City"] = "Cary",
   ["ZipCode"]= "27518",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Epic Games",
   ["CustomerName"] = "Epic Games",
   ["BuildingAddress"] = "620 Cross Roads",
   ["City"] = "Cary",
   ["ZipCode"]= "27618",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Data Center",
   ["CustomerName"] = "First Citizens Bank",
   ["BuildingAddress"] = "100 Tryon Rd",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27603",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Fcc",
   ["CustomerName"] = "First Citizens Bank",
   ["BuildingAddress"] = "4300 Six Forks Rd",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "North Hills",
   ["CustomerName"] = "First Citizens Bank",
   ["BuildingAddress"] = "4400 Six Forks Rd",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Dot- Garner",
   ["CustomerName"] = "Foundry Commercial",
   ["BuildingAddress"] = "750 Greenfield Parkway",
   ["City"] = "Garner",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "1002 Twin Creek - Pitamillar",
   ["CustomerName"] = "Foundry Commercial",
   ["BuildingAddress"] = "1002 Twin Creek",
   ["City"] = "Durham",
   ["ZipCode"]= "27703",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2605 Meridian",
   ["CustomerName"] = "Foundry Commercial",
   ["BuildingAddress"] = "4407 Bland Road",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "603 Keystone",
   ["CustomerName"] = "Foundry Commercial",
   ["BuildingAddress"] = "630 Davis Dr.",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Trinity",
   ["CustomerName"] = "Foundry Commercial",
   ["BuildingAddress"] = "1201 Edwards Mill Rd",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27607",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Midtown North",
   ["CustomerName"] = "Genesis Commercial Real Estate Services",
   ["BuildingAddress"] = "6026 Six Forks Rd",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Carolina Place",
   ["CustomerName"] = "Griffin Partner's",
   ["BuildingAddress"] = "2626 Glenwoods Ave.",
   ["City"] = "Durham",
   ["ZipCode"]= "27705",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Glenwood Place (Dare)",
   ["CustomerName"] = "Grubb Ventures",
   ["BuildingAddress"] = "3733 National Dr.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27606",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Glenwood Place (Alleghany)",
   ["CustomerName"] = "Grubb Ventures",
   ["BuildingAddress"] = "3701 National Drive",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Glenwood Place (Camden)",
   ["CustomerName"] = "Grubb Ventures",
   ["BuildingAddress"] = "3724 National Drive",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Glenwood Place (Caswell)",
   ["CustomerName"] = "Grubb Ventures",
   ["BuildingAddress"] = "3700 National Drive",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Glenwood Place (Chatham)",
   ["CustomerName"] = "Grubb Ventures",
   ["BuildingAddress"] = "3716 National Drive",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Glenwood Place (Cumberland)",
   ["CustomerName"] = "Grubb Ventures",
   ["BuildingAddress"] = "3739 National Drive",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Glenwood Place (Mcdowell)",
   ["CustomerName"] = "Grubb Ventures",
   ["BuildingAddress"] = "3717 National Drive",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Glenwood Place (Northampton)",
   ["CustomerName"] = "Grubb Ventures",
   ["BuildingAddress"] = "3725 National Drive",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "3700 Glenwood ",
   ["CustomerName"] = "Grubb Ventures",
   ["BuildingAddress"] = "3700 National Drive",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Blue Ridge I",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "2500 Blue Ridge Rd.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27067",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Blue Ridge Ii",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "2418 Blue Ridge Rd.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27067",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Rexwoods I",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "4301 Lake Boone Trail",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27067",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Rexwoods Ii",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "4207 Lake Boone Trail",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27067",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Rexwoods Iii",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "2301 Rexwoods Dr.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27067",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Rexwoods Iv",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "4325 Lake Boone Trail",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27067",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Rexwoods V",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "2300 Rexwoods Dr.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27067",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "4201 Lake Boone",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "4201 Lake Boone Trail",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27607",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Glenlake 1",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "4140 Parklake Ave.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Glenlake 4",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "4141 Parklake Ave.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Glenlake 5",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "4131 Parklake Ave.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Glenlake 6",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "4130 Parklake Ave.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2000 Centregreen I",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "2000 Centregreen Way",
   ["City"] = "Cary",
   ["ZipCode"]= "27513",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "4000 Centregreen Ii",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "4000 Centregreen Way",
   ["City"] = "Cary",
   ["ZipCode"]= "27513",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "5000 Centregreen Iii",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "5000 Centregreen Way",
   ["City"] = "Cary",
   ["ZipCode"]= "27513",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "3000 Centregreen Iv",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "3000 Centregreen Way",
   ["City"] = "Cary",
   ["ZipCode"]= "27513",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "1000 Centregreen V",
   ["CustomerName"] = "Highwoods",
   ["BuildingAddress"] = "1000 Centregreen Way",
   ["City"] = "Cary",
   ["ZipCode"]= "27513",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Keystone Techonology Park",
   ["CustomerName"] = "Longfellow Real Estate Partners",
   ["BuildingAddress"] = "627 Davis Dr. ",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Butterball",
   ["CustomerName"] = "Mason Properties",
   ["BuildingAddress"] = "700 Greenfield Parkway",
   ["City"] = "Garner",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Lab 2 (Q2 Solution)",
   ["CustomerName"] = "Q2 Solutions Lab",
   ["BuildingAddress"] = "4820 Page Road",
   ["City"] = "Durham",
   ["ZipCode"]= "27703",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "305 Gregson",
   ["CustomerName"] = "Run Property Management",
   ["BuildingAddress"] = "305 Gregson Dr",
   ["City"] = "Cary",
   ["ZipCode"]= "27511",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Dale St.",
   ["CustomerName"] = "Rhyne Management Associate",
   ["BuildingAddress"] = "",
   ["City"] = "",
   ["ZipCode"] = "",
   ["InternalNotes"] = "We Have No Address For This Building"
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "All State",
   ["CustomerName"] = "Stoltz Management",
   ["BuildingAddress"] = "3150 Spring Forest Rd. ",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Crossraods I",
   ["CustomerName"] = "Spectrum Properties",
   ["BuildingAddress"] = "5625 Dillard Dr.",
   ["City"] = "Cary",
   ["ZipCode"]= "27518",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Crossraods Ii",
   ["CustomerName"] = "Spectrum Properties",
   ["BuildingAddress"] = "110 Corning Rd.",
   ["City"] = "Cary",
   ["ZipCode"]= "27518",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Crossraods Iii",
   ["CustomerName"] = "Spectrum Properties",
   ["BuildingAddress"] = "111 Corning Rd.",
   ["City"] = "Cary",
   ["ZipCode"]= "27518",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Crossraods Iv",
   ["CustomerName"] = "Spectrum Properties",
   ["BuildingAddress"] = "5520 Dillard Drive",
   ["City"] = "Cary",
   ["ZipCode"]= "27518",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Wells Fargo Tower",
   ["CustomerName"] = "Spectrum Properties",
   ["BuildingAddress"] = "150 Fayetteville St.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27601",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "200 Lucent (200 Regency Woods)",
   ["CustomerName"] = "Spectrum Properties",
   ["BuildingAddress"] = "200 Lucent Dr.",
   ["City"] = "Cary",
   ["ZipCode"]= "27518",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "12040 Regency Pkwy",
   ["CustomerName"] = "Spectrum Properties",
   ["BuildingAddress"] = "12040 Regency Parkway",
   ["City"] = "Cary",
   ["ZipCode"]= "27518",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "4700 Falls Of Neuse",
   ["CustomerName"] = "Spectrum Properties",
   ["BuildingAddress"] = "4700 Falls Of The Neuse",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Red Hat Tower",
   ["CustomerName"] = "Spectrum Properties",
   ["BuildingAddress"] = "405 S. Wilmington St.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27601",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "St. David's School",
   ["CustomerName"] = "St. David's School",
   ["BuildingAddress"] = "100 E. Davie St.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27601",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Mcclamroch Hall",
   ["CustomerName"] = "Svn Alliance Asset & Property Management",
   ["BuildingAddress"] = "88 Vilcom Circle",
   ["City"] = "Chapel Hill",
   ["ZipCode"]= "27514",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "4401 Bland (Somerset)",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "3400 White Oak Road",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "4403 Bland (Somerset)",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "4401 Bland Road",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "4405 Bland (Somerset)",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "4403 Bland Road",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "4407 Bland (Somerset)",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "4405 Bland Road",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2100 Gateway",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "2100 Gateway Center Blvd",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "100 Regency Forest",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "100 Regency Forest Dr.",
   ["City"] = "Cary",
   ["ZipCode"]= "27518",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "200 Regenyc Forest",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "200 Regency Forest Dr.",
   ["City"] = "Cary",
   ["ZipCode"]= "27518",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "1500 Perimeter Park",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "1500 Perimeter Park Drive",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "1600 Perimeter Park",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "1600 Perimeter Park Drive",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "3800 Paramount Parkway",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "3800 Paramount Parkway",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2000 Perimeter",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "2000 Perimeter Park Dr.",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2250 Perimeter Park",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "2250 Perimeter Park Drive",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "3015 Perimeter Three",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "3015 Carrington Mill Blvd.",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "3025perimeter Four",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "3025 Carrington Mill Blvd.",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "2301 Sugar Bush",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "2301 Sugar Bush Rd.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "3020 Perimeter Two",
   ["CustomerName"] = "Trinity Partners",
   ["BuildingAddress"] = "3020 Carrington Mill Blvd.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27612",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Boyd Hall",
   ["CustomerName"] = "Trademark Properties",
   ["BuildingAddress"] = "55 Vilcome Circle",
   ["City"] = "Chapel Hill",
   ["ZipCode"]= "27514",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "1001 Wade Ave",
   ["CustomerName"] = "Trademark Properties",
   ["BuildingAddress"] = "1001 Wade Ave",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27604",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Town Hall Commons",
   ["CustomerName"] = "Tri Properties",
   ["BuildingAddress"] = "101 J. Moris Commons Lane",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Lumley Rd. (Briar Creek)",
   ["CustomerName"] = "Tri Properties",
   ["BuildingAddress"] = "3128 Highwoods Blvd.",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27604",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Bristol Place",
   ["CustomerName"] = "Tri Properties",
   ["BuildingAddress"] = "10321 Lumley Road",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27607",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Chelsea",
   ["CustomerName"] = "Tri Properties",
   ["BuildingAddress"] = "4819 Emperor Blvd",
   ["City"] = "Durham",
   ["ZipCode"]= "27703",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Oxford",
   ["CustomerName"] = "Tri Properties",
   ["BuildingAddress"] = "1007 Slater Road",
   ["City"] = "Durham",
   ["ZipCode"]= "27702",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "North Chase I",
   ["CustomerName"] = "Tri Properties",
   ["BuildingAddress"] = "1005 Slater Road",
   ["City"] = "Durham",
   ["ZipCode"]= "27703",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "North Chase Ii",
   ["CustomerName"] = "Tri Properties",
   ["BuildingAddress"] = "6501 Six Forks Road",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27609",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Iqvia (Quintiles)",
   ["CustomerName"] = "Tri Properties",
   ["BuildingAddress"] = "4820 Emperor Blvd",
   ["City"] = "Durham",
   ["ZipCode"]= "27703",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Carlisle",
   ["CustomerName"] = "Tri Properties",
   ["BuildingAddress"] = "4721 Emperor Blvd",
   ["City"] = "Durham",
   ["ZipCode"]= "27703",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Town Hall Commons",
   ["CustomerName"] = "Tri Properties",
   ["BuildingAddress"] = "101 J Morris Commons Lane",
   ["City"] = "Morrisville",
   ["ZipCode"]= "27560",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Berrington",
   ["CustomerName"] = "Tri Properties",
   ["BuildingAddress"] = "",
   ["City"] = "",
   ["ZipCode"] = "",
   ["InternalNotes"] = "We Have No Address For This Building"
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Peak Way Market Sq.",
   ["CustomerName"] = "The Focus Properties, Inc.",
   ["BuildingAddress"] = "2605 Meridian Pkwy",
   ["City"] = "Durham",
   ["ZipCode"]= "27713",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Nc State Bar",
   ["CustomerName"] = "The North Carolina State Bar",
   ["BuildingAddress"] = "800 Hwy 55",
   ["City"] = "Apex",
   ["ZipCode"]= "27502",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Valley View",
   ["CustomerName"] = "Triland Property Commercial",
   ["BuildingAddress"] = "3511 Shannon Rd",
   ["City"] = "Durham",
   ["ZipCode"]= "27707",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "7200 Falls",
   ["CustomerName"] = "Urben Commercial",
   ["BuildingAddress"] = "7200 Falls Of Neuse",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27615",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["BuildingName"] = "Madison 3",
   ["CustomerName"] = "Capital Associates",
   ["BuildingAddress"] = "3191 Anson Way",
   ["City"] = "Raleigh",
   ["ZipCode"]= "27615",
   ["InternalNotes"] = ""
 }
};
        /// <summary>
        ///     Main methods for executes the seed operations.
        /// </summary>
        /// <param name="context">The DbContext for modifiyng the data</param>
        /// <param name="count">The numbers of elements to add.</param>
        public static void AddOrUpdate(MGCapDbContext context, int count = 50)
        {
            if (context.Buildings.Any())
            {
                return;
            }

            var companiesIds = context.Companies
                          .Select(c => c.ID)
                          .ToList();

            // For random customer selection
            var rand = new Random();

            foreach (var companyId in companiesIds)
            {
                // All customers given the current company
                var customers = context.Customers
                                       .Where(c => c.CompanyId == companyId)
                                       ?.ToList();

                if (customers == null || customers.Count == 0)
                {
                    continue;
                }

                foreach (var bldg in Buildings)
                {
                    var customer = customers.FirstOrDefault(c => c.Name == bldg["CustomerName"]);
                    if (customer == null)
                    {
                        continue;
                    }

                    var address = new Address
                    {
                        AddressLine1 = bldg["BuildingAddress"],
                        City = bldg["City"],
                        State = "",
                        ZipCode = bldg["ZipCode"],
                        CountryCode = "US",
                        CreatedBy = "Seed",
                        UpdatedBy = "Seed",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    };
                    context.Addresses.Add(address);

                    var building = new Building
                    {
                        CompanyId = companyId,
                        CreatedBy = "Seed",
                        UpdatedBy = "Seed",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        Name = bldg["BuildingName"],
                        AddressId = address.ID,
                        //CustomerId = customer.ID
                    };
                    context.Buildings.Add(building);

                    // Create a contact for 'ContactSigner'
                    var contact = new Contact
                    {
                        CompanyId = companyId,
                        CompanyName = "Axzes",
                        FirstName = "Axzes",
                        LastName = "LLC",
                        CreatedBy = "Seed",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedBy = "Seed",
                        UpdatedDate = DateTime.UtcNow
                    };
                    context.Contacts.Add(contact);

                    var contactEmail = new ContactEmail
                    {
                        ContactId = contact.ID,
                        Email = "axzesllc@gmail.com",
                        Type = "Main",
                        Default = true,
                        CreatedBy = "Seed",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedBy = "Seed",
                        UpdatedDate = DateTime.UtcNow
                    };
                    context.ContactEmails.Add(contactEmail);

                    // Associate the created contact with the existing customer
                    var contactSigner = new CustomerContact
                    {
                        CustomerId = customer.ID,
                        ContactId = contact.ID,
                        Type = "CEO",
                        Default = true,
                        SelectedForMarketing = true,
                    };
                    context.CustomerContacts.Add(contactSigner);

                    // Creates a contract
                    var contract = new Contract
                    {
                        ContractNumber = rand.Next(100, 500).ToString(),
                        Area = rand.Next(1000, 2000),
                        NumberOfPeople = rand.Next(5, 20),
                        BuildingId = building.ID,
                        CustomerId = customer.ID,
                        ContactSignerId = contact.ID,
                        Status = 1,
                        CompanyId = companyId,
                        CreatedBy = "Seed",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedBy = "Seed",
                        UpdatedDate = DateTime.UtcNow
                    };
                    context.Contracts.Add(contract);

                    // Creates two customers (requester and property manager) for the building
                    //var requester = new Contact
                    //{
                    //    FirstName = "Ewan",
                    //    LastName = $"McGregor {i + 1}",
                    //    CreatedDate = DateTime.UtcNow,
                    //    UpdatedDate = DateTime.UtcNow,
                    //    CreatedBy = "Seed",
                    //    UpdatedBy = "Seed",
                    //    CompanyId = companyId,
                    //    CompanyName = "Trainspotting",
                    //};
                    //context.Contacts.Add(requester);

                    //var manager = new Contact
                    //{
                    //    FirstName = "Liam",
                    //    LastName = $"Neeson {i + 1}",
                    //    CreatedDate = DateTime.UtcNow,
                    //    UpdatedDate = DateTime.UtcNow,
                    //    CreatedBy = "Seed",
                    //    UpdatedBy = "Seed",
                    //    CompanyId = companyId,
                    //    CompanyName = "I will find you, LLC",
                    //};
                    //context.Contacts.Add(manager);

                    //var bldgContact1 = new BuildingContact
                    //{
                    //    BuildingId = building.ID,
                    //    ContactId = requester.ID,
                    //    Type = "Requester"
                    //};
                    //context.BuildingContacts.Add(bldgContact1);

                    //var bldgContact2 = new BuildingContact
                    //{
                    //    BuildingId = building.ID,
                    //    ContactId = manager.ID,
                    //    Type = "Property Manager"
                    //};
                    //context.BuildingContacts.Add(bldgContact2);
                }
            }
        }
    }
}
