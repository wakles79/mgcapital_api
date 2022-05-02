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
    public static class PropertyManagersSeeds
    {
        public static List<Dictionary<string, string>> Managers = new List<Dictionary<string, string>> {
 new Dictionary<string, string> {
   ["ManagerName"] = "Alissa Berkey",
   ["ManagerEmail"] = "abarkey@spectrumcos.com",
   ["MainPhone"] = "(919) 818 - 3141",

   ["AltPhone"] = "",

   ["Customer"] = "Spectrum Properties",
   ["BuildingNames"] = "Red Hat Tower",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Amy Mayer",
   ["ManagerEmail"] = "amayer@trinity-partners.com",
   ["MainPhone"] = "(919) 415 - 4401",

   ["AltPhone"] = "(919) 674 - 3687",

   ["Customer"] = "Trinity Partners",
   ["BuildingNames"] = "2250 Perimeter Park,3015 Perimeter Three,3025Perimeter Four",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Anne Rooks",
   ["ManagerEmail"] = "Anne.Rooks@avisonyoung.com",
   ["MainPhone"] = "(919) 866 - 4269",

   ["AltPhone"] = "",

   ["Customer"] = "Avison Young",
   ["BuildingNames"] = "Methodist Church,Methodist Church,Stonehenge I,Stonehenge II,Westpoint",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Annette Rallo",
   ["ManagerEmail"] = "annette.rallo@cushwake.com",
   ["MainPhone"] = "(919) 645 - 3398",

   ["AltPhone"] = "",

   ["Customer"] = "Cushman and Wakefield",
   ["BuildingNames"] = "1616 Evans ",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Barbara Armitage",
   ["ManagerEmail"] = "Barbara.Armitage@highwoods.com",
   ["MainPhone"] = "(919) 875 - 6689",

   ["AltPhone"] = "",

   ["Customer"] = "Highwoods",
   ["BuildingNames"] = "GlenLake 1,GlenLake 4,GlenLake 5,GlenLake 6",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Bryan Philip",
   ["ManagerEmail"] = "Bryan.Philip@foundrycommercial.com ",
   ["MainPhone"] = "(919) 987 - 1014",

   ["AltPhone"] = "",

   ["Customer"] = "Foundry Commercial",
   ["BuildingNames"] = "1002 Twin creek - Pitamillar",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Carrie Karcher",
   ["ManagerEmail"] = "ckarcher@trinity-partners.com",
   ["MainPhone"] = "(919) 674 - 3688",

   ["AltPhone"] = "",

   ["Customer"] = "Trinity Partners",
   ["BuildingNames"] = "4401 Bland (Somerset),4403 Bland (Somerset),4405 Bland (Somerset),4407 Bland (Somerset)",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Catherine Bartels",
   ["ManagerEmail"] = "cbartels@casso.com",
   ["MainPhone"] = "(919) 783 - 0981",

   ["AltPhone"] = "",

   ["Customer"] = "Capital Associates",
   ["BuildingNames"] = "Wycliff",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Catherine Winder",
   ["ManagerEmail"] = "cwinder@accessoservices.com",
   ["MainPhone"] = "(919) 544 - 2000",

   ["AltPhone"] = "(919) 274 - 5001",

   ["Customer"] = "American Real Estate Partners Mgt",
   ["BuildingNames"] = "2500 Meridian,2505 Meridian,2510 Meridian,2520 Meridian,2525 Meridian,2645 Meridian,2700 Meridian,2800 Meridian,430 Davis,530 Davis",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Celia McCauley",
   ["ManagerEmail"] = "cmccauley@TRIPROP.COM",
   ["MainPhone"] = "(919) 422 - 3430",

   ["AltPhone"] = "",

   ["Customer"] = "Tri Properties",
   ["BuildingNames"] = "Lumley Rd. (Briar Creek)",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Cheri Megan",
   ["ManagerEmail"] = "Cheri.Megan@foundrycommercial.com ",
   ["MainPhone"] = "(919) 987 - 1002",

   ["AltPhone"] = "",

   ["Customer"] = "Foundry Commercial",
   ["BuildingNames"] = "2605 Meridian,603 Keystone,Trinity",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Cheryl Cargill",
   ["ManagerEmail"] = "ccargill@casso.com",
   ["MainPhone"] = "(919) 839 - 8400",

   ["AltPhone"] = "",

   ["Customer"] = "Capital Associates",
   ["BuildingNames"] = "Two Hanover Square",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Christina Holloway",
   ["ManagerEmail"] = "christina.holloway@avisonyoung.com",
   ["MainPhone"] = "(919) 420 - 1577",

   ["AltPhone"] = "",

   ["Customer"] = "Avison Young",
   ["BuildingNames"] = "RDU I",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Clint Riley",
   ["ManagerEmail"] = "criley@capridgepartners.com",
   ["MainPhone"] = "(919) 737 - 5999",

   ["AltPhone"] = "",

   ["Customer"] = "Capridge Partners",
   ["BuildingNames"] = "Palisades I,Palisades II",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Craig Spivey",
   ["ManagerEmail"] = "CSpivey@cottoninc.com",
   ["MainPhone"] = "(919) 678 - 2257",

   ["AltPhone"] = "",

   ["Customer"] = "Cotton Corporation",
   ["BuildingNames"] = "Cotton Corp",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Crystal Hughs",
   ["ManagerEmail"] = "hughes@accessoservices.com",
   ["MainPhone"] = "(919) 544 - 2000",

   ["AltPhone"] = "(919) 274 - 5001",

   ["Customer"] = "American Real Estate Partners Mgt",
   ["BuildingNames"] = "2500 Meridian,2505 Meridian,2510 Meridian,2520 Meridian,2525 Meridian,2645 Meridian,2700 Meridian,2800 Meridian,430 Davis,530 Davis",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Crystal Thomas",
   ["ManagerEmail"] = "crystalt@thefocusproperties.com",
   ["MainPhone"] = "(919) 977 - 5577",

   ["AltPhone"] = "",

   ["Customer"] = "The Focus Properties,Inc.",
   ["BuildingNames"] = "Peak Way Market Sq.",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Danny Dean",
   ["ManagerEmail"] = "ddean@casso.com",
   ["MainPhone"] = "(919) 783 - 0981",

   ["AltPhone"] = "",

   ["Customer"] = "Capital Associates",
   ["BuildingNames"] = "Medical Plaza,The Summit,CBC Flex Lab",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "David Geese",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "(919) 669 - 9230",

   ["AltPhone"] = "(919) 782 - 3331",

   ["Customer"] = "St. David's School",
   ["BuildingNames"] = "St. David's School",
   ["InternalNotes"] = "This Property Manager has no associated Email"
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Deann Humble",
   ["ManagerEmail"] = "deann@urbencommercial.com",
   ["MainPhone"] = "(919) 615 - 1630",

   ["AltPhone"] = "",

   ["Customer"] = "Urben Commercial",
   ["BuildingNames"] = "7200 Falls",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Debbie Estrada",
   ["ManagerEmail"] = "Debbie.Estrada@CraigDavisProperties.com",
   ["MainPhone"] = "(919) 678 - 4215",

   ["AltPhone"] = "",

   ["Customer"] = "Craig Davis Properties",
   ["BuildingNames"] = "Colonnade II",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Denise Johston",
   ["ManagerEmail"] = "denise.johnston@foundrycommercial.com",
   ["MainPhone"] = "(919) 987 - 1001",

   ["AltPhone"] = "",

   ["Customer"] = "Foundry Commercial",
   ["BuildingNames"] = "DOT- Garner",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Dottie Miani",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "(919) 719 - 9268",

   ["AltPhone"] = "",

   ["Customer"] = "The North Carolina State Bar",
   ["BuildingNames"] = "NC State Bar",
   ["InternalNotes"] = "This Property Manager has no associated Email"
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Ed Boyle",
   ["ManagerEmail"] = "Ed.Boyle@highwoods.com",
   ["MainPhone"] = "(919) 875 - 6641",

   ["AltPhone"] = "",

   ["Customer"] = "Highwoods",
   ["BuildingNames"] = "Blue Ridge I,Blue Ridge II,Rexwoods I,Rexwoods II,Rexwoods III,Rexwoods IV,Rexwoods V,4201 Lake Boone",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Eddie Martinez",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "(919) 325 - 6522",

   ["AltPhone"] = "",

   ["Customer"] = "Epic Games",
   ["BuildingNames"] = "Epic Games",
   ["InternalNotes"] = "This Property Manager has no associated Email"
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Elizabeth  Swaringer",
   ["ManagerEmail"] = "ESwaringen@SpectrumCos.com",
   ["MainPhone"] = "(919) 857 - 3743",

   ["AltPhone"] = "",

   ["Customer"] = "Spectrum Properties",
   ["BuildingNames"] = "200 Lucent (200 Regency Woods),12040 Regency Pkwy,4700 Falls of Neuse",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Elizabeth Stewart",
   ["ManagerEmail"] = "'beth.stewart@cushwake.com'",
   ["MainPhone"] = "",

   ["AltPhone"] = "",

   ["Customer"] = "Cushman and Wakefield",
   ["BuildingNames"] = "Nottingham",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Erica Robinson",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "(919) 852 - 5248",

   ["AltPhone"] = "",

   ["Customer"] = "Spectrum Properties",
   ["BuildingNames"] = "Crossraods I,Crossraods II,Crossraods III,Crossraods IV",
   ["InternalNotes"] = "This Property Manager has no associated Email"
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Eve Pikington",
   ["ManagerEmail"] = "epilkington@trinity-partners.com ",
   ["MainPhone"] = "(919) 369 - 6229",

   ["AltPhone"] = "",

   ["Customer"] = "Trinity Partners",
   ["BuildingNames"] = "2100 Gateway",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Frank Sproviero",
   ["ManagerEmail"] = "Frank.Sproviero@SVN.com",
   ["MainPhone"] = "(704) 892 - 5653",

   ["AltPhone"] = "",

   ["Customer"] = "SVN Alliance Asset & Property Management",
   ["BuildingNames"] = "McClamroch Hall",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Gail Vaughan",
   ["ManagerEmail"] = "gvaughan@lfrep.com",
   ["MainPhone"] = "(919) 354 - 1332",

   ["AltPhone"] = "(919) 354 - 1334",

   ["Customer"] = "Longfellow Real Estate Partners",
   ["BuildingNames"] = "Keystone Techonology Park",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Greg Taylor",
   ["ManagerEmail"] = "gtaylor@TRIPROP.COM",
   ["MainPhone"] = "(919) 281 - 2314",

   ["AltPhone"] = "",

   ["Customer"] = "Tri Properties",
   ["BuildingNames"] = "Bristol Place,Chelsea,Oxford",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Gregory Mason",
   ["ManagerEmail"] = "greggmasonj@aol.com",
   ["MainPhone"] = "(919) 868 - 3369",

   ["AltPhone"] = "",

   ["Customer"] = "Mason Properties",
   ["BuildingNames"] = "Butterball",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Gwen Serreal",
   ["ManagerEmail"] = "GSarreal@capridgepartners.com",
   ["MainPhone"] = "(919) 737 - 5999",

   ["AltPhone"] = "",

   ["Customer"] = "Capridge Partners",
   ["BuildingNames"] = "Palisades I,Palisades II",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Heather Langdon",
   ["ManagerEmail"] = "Heather.Langdon@avisonyoung.com",
   ["MainPhone"] = "(919) 821 - 8023",

   ["AltPhone"] = "",

   ["Customer"] = "Avison Young",
   ["BuildingNames"] = "Professional Building",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Holly Doerner",
   ["ManagerEmail"] = "tenantservicesrdu@lincolnharris.com",
   ["MainPhone"] = "(919) 840 - 8040",

   ["AltPhone"] = "",

   ["Customer"] = "Epic Regency",
   ["BuildingNames"] = "400 Regency",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "J. Mills",
   ["ManagerEmail"] = "JMills@Spectrum-Properties.com",
   ["MainPhone"] = "(919) 852 - 5248",

   ["AltPhone"] = "",

   ["Customer"] = "Spectrum Properties",
   ["BuildingNames"] = "Wells Fargo Tower",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Jason Walden",
   ["ManagerEmail"] = "JWalden@cottoninc.com",
   ["MainPhone"] = "(919) 678 - 2257",

   ["AltPhone"] = "",

   ["Customer"] = "Cotton Corporation",
   ["BuildingNames"] = "Cotton Corp",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Jeff Muzzy",
   ["ManagerEmail"] = "jeff.muzzy@conduent.com",
   ["MainPhone"] = "(407) 926 - 2174",

   ["AltPhone"] = "",

   ["Customer"] = "Conduent",
   ["BuildingNames"] = "1200 Crescent,1300 Crescent,Green Road,2641 Sumner Blvd.",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Jennifer Bignall",
   ["ManagerEmail"] = "jbignall@genesiscres.com",
   ["MainPhone"] = "",

   ["AltPhone"] = "",

   ["Customer"] = "Genesis Commercial Real Estate Services",
   ["BuildingNames"] = "Midtown North",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Jennifer Nixon",
   ["ManagerEmail"] = "jnixon@casso.com",
   ["MainPhone"] = "(919) 422 - 1161",

   ["AltPhone"] = "",

   ["Customer"] = "Capital Associates",
   ["BuildingNames"] = "Durant Medical,Springfield,2841 Plaza Place,Madison 3",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Jennifer Staunton",
   ["ManagerEmail"] = "jstaunton@accessoservices.com",
   ["MainPhone"] = "(919) 544 - 2000",

   ["AltPhone"] = "(919) 274 - 5001",

   ["Customer"] = "American Real Estate Partners Mgt",
   ["BuildingNames"] = "2500 Meridian,2505 Meridian,2510 Meridian,2520 Meridian,2525 Meridian,2645 Meridian,2700 Meridian,2800 Meridian,430 Davis,530 Davis",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Joe Cerone",
   ["ManagerEmail"] = "icerone@ncbar.gov",
   ["MainPhone"] = "(919) 526 - 1207",

   ["AltPhone"] = "",

   ["Customer"] = "The North Carolina State Bar",
   ["BuildingNames"] = "NC State Bar",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "John Morris",
   ["ManagerEmail"] = "jmorris@cbc-raleigh.com",
   ["MainPhone"] = "(919) 433 - 4275",

   ["AltPhone"] = "",

   ["Customer"] = "Blackwell Street Management",
   ["BuildingNames"] = "Diamond View I,Diamond View II,Diamond View III,Hill,Old Bull,Noell,Strickland,Washington Bay,Reed,Crowe,Fowler,Power Plant",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Karen Riley",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "(919) 674 - 3684",

   ["AltPhone"] = "",

   ["Customer"] = "Trinity Partners",
   ["BuildingNames"] = "100 Regency Forest,200 Regenyc Forest",
   ["InternalNotes"] = "This Property Manager has no associated Email"
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Kathy Lewis",
   ["ManagerEmail"] = "klewis@casso.com",
   ["MainPhone"] = "(919) 929 - 2494",

   ["AltPhone"] = "",

   ["Customer"] = "Capital Associates",
   ["BuildingNames"] = "Exchange West,Franklin St. Trust,Building 1- 10 Lab Drive,Building 2-10 Lab Drive",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Kathy Powell",
   ["ManagerEmail"] = "kpowell@grubbventures.com ",
   ["MainPhone"] = "(919) 781 - 0079",

   ["AltPhone"] = "(919) 881 - 2070",

   ["Customer"] = "Grubb Ventures",
   ["BuildingNames"] = "Glenwood Place (Dare),Glenwood Place (Alleghany),Glenwood Place (Camden),Glenwood Place (Caswell),Glenwood Place (Chatham),Glenwood Place (Cumberland),Glenwood Place (McDowell),Glenwood Place (Northampton),3700 Glenwood ",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Kevin",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "(919) 415 - 4403",

   ["AltPhone"] = "",

   ["Customer"] = "Trinity Partners",
   ["BuildingNames"] = "1500 Perimeter Park,1600 Perimeter Park,3800 Paramount Parkway,2000 Perimeter",
   ["InternalNotes"] = "This Property Manager is missing a last name and has no associated Email"
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Kevin Park",
   ["ManagerEmail"] = "kpark@casso.com",
   ["MainPhone"] = "(919) 865 - 2243",

   ["AltPhone"] = "",

   ["Customer"] = "Capital Associates",
   ["BuildingNames"] = "1225 Lakeside,1255 Lakeside,500 Gregson,5501 Dillard (Medfusion)",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Kim Gandelman",
   ["ManagerEmail"] = "KGandelan@casso.com",
   ["MainPhone"] = "(919) 783 - 0981",

   ["AltPhone"] = "",

   ["Customer"] = "Capital Associates",
   ["BuildingNames"] = "Medical Plaza,The Summit,CBC Flex Lab",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Kim Ortenzio",
   ["ManagerEmail"] = "kim.ortenzio@avisonyoung.com",
   ["MainPhone"] = "(919) 866 - 4269",

   ["AltPhone"] = "",

   ["Customer"] = "Avison Young",
   ["BuildingNames"] = "114 Edinburgh Bldg",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Kim Wood",
   ["ManagerEmail"] = "kim.wood@cbre-raleigh.com",
   ["MainPhone"] = "919 829 8832",

   ["AltPhone"] = "",

   ["Customer"] = "Cushman and Wakefield",
   ["BuildingNames"] = "West Chase I,West Chase II,West Chase III",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Laura Bowser",
   ["ManagerEmail"] = "lbowser@dilweg.com",
   ["MainPhone"] = "(919) 313 - 2767",

   ["AltPhone"] = "(919) 356 - 0161",

   ["Customer"] = "Dilweg",
   ["BuildingNames"] = "9000 Regency,11000 Regency,RDU IIII,Univerity Tower",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Laura Homeyer",
   ["ManagerEmail"] = "lhomeyer@capridgepartners.com",
   ["MainPhone"] = "(919) 737 - 5999",

   ["AltPhone"] = "",

   ["Customer"] = "Capridge Partners",
   ["BuildingNames"] = "Palisades I,Palisades II",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Lindsay Barrier",
   ["ManagerEmail"] = "lindsay.barrier@cbre-raleigh.com",
   ["MainPhone"] = "(919) 831 - 8243",

   ["AltPhone"] = "",

   ["Customer"] = "CB Richard Ellis",
   ["BuildingNames"] = "Colonnade I,1100 Crescent,Forty540",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Lloyd White",
   ["ManagerEmail"] = "lwhite@casso.com",
   ["MainPhone"] = "(919) 422 - 1161",

   ["AltPhone"] = "",

   ["Customer"] = "Capital Associates",
   ["BuildingNames"] = "Durant Medical,Springfield,2841 Plaza Place,Madison 3",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Lowell Lovell",
   ["ManagerEmail"] = "lowell.lovell@avisonyoung.com",
   ["MainPhone"] = "(919) 866 - 4269",

   ["AltPhone"] = "",

   ["Customer"] = "Avison Young",
   ["BuildingNames"] = "114 Edinburgh Bldg,1400 Crescent -I",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Luis Prado",
   ["ManagerEmail"] = "luis.prado@us.abb.com ",
   ["MainPhone"] = "",

   ["AltPhone"] = "",

   ["Customer"] = "RUN Property Management",
   ["BuildingNames"] = "305 Gregson",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Manuel Salmon",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "",

   ["AltPhone"] = "",

   ["Customer"] = "RUN Property Management",
   ["BuildingNames"] = "305 Gregson",
   ["InternalNotes"] = "This Property Manager has no associated Email"
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Margie Bowles",
   ["ManagerEmail"] = "Mbowles@casso.com",
   ["MainPhone"] = "(919) 839 - 8400",

   ["AltPhone"] = "",

   ["Customer"] = "Capital Associates",
   ["BuildingNames"] = "Two Hanover Square",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Maria Moon",
   ["ManagerEmail"] = "MMoon@casso.com",
   ["MainPhone"] = "(919) 865 - 2243",

   ["AltPhone"] = "",

   ["Customer"] = "Capital Associates",
   ["BuildingNames"] = "1225 Lakeside,1255 Lakeside,500 Gregson,5501 Dillard (Medfusion)",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Maria Moon",
   ["ManagerEmail"] = "mmoon@casso.com",
   ["MainPhone"] = "",

   ["AltPhone"] = "",

   ["Customer"] = "Capital Associates",
   ["BuildingNames"] = "3110 Edwards Mill",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Matt Hinchey",
   ["ManagerEmail"] = "pm@trilandproperty.com",
   ["MainPhone"] = "",

   ["AltPhone"] = "",

   ["Customer"] = "TriLand Property Commercial",
   ["BuildingNames"] = "Valley View",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Melanie Rivera",
   ["ManagerEmail"] = "melanie.rivera@cbre-raleigh.com",
   ["MainPhone"] = "(919) 866 - 2763",

   ["AltPhone"] = "",

   ["Customer"] = "CB Richard Ellis",
   ["BuildingNames"] = "Carolina Square",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Melody Lemmon",
   ["ManagerEmail"] = "mlemmon@trademarkproperties.com",
   ["MainPhone"] = "(919) 645 - 1439",

   ["AltPhone"] = "",

   ["Customer"] = "TradeMark properties",
   ["BuildingNames"] = "Boyd Hall",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Michael Robbins",
   ["ManagerEmail"] = "mrobbins@stoltzusa.com",
   ["MainPhone"] = "(919) 249 - 6133",

   ["AltPhone"] = "",

   ["Customer"] = "Stoltz Management",
   ["BuildingNames"] = "All State",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Michele Lowery",
   ["ManagerEmail"] = "Michele.Lowery@highwoods.com",
   ["MainPhone"] = "",

   ["AltPhone"] = "",

   ["Customer"] = "Highwoods",
   ["BuildingNames"] = "2000 CentreGreen I,4000 CentreGreen II,5000 CentreGreen III,3000 CentreGreen IV,1000 CentreGreen V",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Michelle Deese",
   ["ManagerEmail"] = "mdeese@triprop.com",
   ["MainPhone"] = "(919) 415 - 4403",

   ["AltPhone"] = "",

   ["Customer"] = "Tri Properties",
   ["BuildingNames"] = "Town Hall Commons",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Michelle Deese",
   ["ManagerEmail"] = "mdeese@triprop.com",
   ["MainPhone"] = "(919) 281 - 2315",

   ["AltPhone"] = "",

   ["Customer"] = "Tri Properties",
   ["BuildingNames"] = "Carlisle,Town Hall Commons",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Nancy Burns",
   ["ManagerEmail"] = "nburns@trinity-partners.com",
   ["MainPhone"] = "(919) 415 - 4402",

   ["AltPhone"] = "",

   ["Customer"] = "Trinity Partners",
   ["BuildingNames"] = "2301 Sugar Bush,3020 Perimeter Two",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Nancy Lowery",
   ["ManagerEmail"] = "nancy.lowery@cbre-raleigh.com",
   ["MainPhone"] = "(919) 369 - 3513",

   ["AltPhone"] = "",

   ["Customer"] = "CB Richard Ellis",
   ["BuildingNames"] = "4101 Research Commons,4201 Research Commons,4301 Research Commons,4401 Research Commons,4501 Research Commons",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Reba Gallant",
   ["ManagerEmail"] = "reba.gallant@cbre-raleigh.com",
   ["MainPhone"] = "(919) 829 - 8832",

   ["AltPhone"] = "",

   ["Customer"] = "Cushman and Wakefield",
   ["BuildingNames"] = "West Chase I,West Chase II,West Chase III",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Rene LaReau",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "(919) 863 - 0331",

   ["AltPhone"] = "",

   ["Customer"] = "CB Richard Ellis",
   ["BuildingNames"] = "Venture l,Venture ll,Venture lll,Venture IV,Venture Place",
   ["InternalNotes"] = "This Property Manager has no associated Email"
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Rich McLeod",
   ["ManagerEmail"] = "rmacleod@cbc-raleigh.com",
   ["MainPhone"] = "(919) 433 - 4275",

   ["AltPhone"] = "",

   ["Customer"] = "Blackwell Street Management",
   ["BuildingNames"] = "Diamond View I,Diamond View II,Diamond View III,Hill,Old Bull,Noell,Strickland,Washington Bay,Reed,Crowe,Fowler,Power Plant",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Robert Barton",
   ["ManagerEmail"] = "bob.barton@cbre-raleigh.com",
   ["MainPhone"] = "(919) 863 - 0331",

   ["AltPhone"] = "",

   ["Customer"] = "CB Richard Ellis",
   ["BuildingNames"] = "Venture l,Venture ll,Venture lll,Venture IV,Venture Place",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Robert Bijeau",
   ["ManagerEmail"] = "rbijeau@casso.com",
   ["MainPhone"] = "(919) 865 - 2243",

   ["AltPhone"] = "",

   ["Customer"] = "Capital Associates",
   ["BuildingNames"] = "1225 Lakeside,1255 Lakeside,500 Gregson,5501 Dillard (Medfusion)",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Rodney Collier",
   ["ManagerEmail"] = "rcollier@cbc-raleigh.com",
   ["MainPhone"] = "(919) 433 - 4275",

   ["AltPhone"] = "",

   ["Customer"] = "Blackwell Street Management",
   ["BuildingNames"] = "Diamond View I,Diamond View II,Diamond View III,Hill,Old Bull,Noell,Strickland,Washington Bay,Reed,Crowe,Fowler,Power Plant",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Sarah Warren",
   ["ManagerEmail"] = "swarren@dilweg.com ",
   ["MainPhone"] = "(919) 313 - 7387",

   ["AltPhone"] = "",

   ["Customer"] = "Dilweg",
   ["BuildingNames"] = "RDU IIII,RDU III,Quadrangle II,Quadrangle III,Quadrangle IV,Quadrangle V",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Scott Schuler",
   ["ManagerEmail"] = "sschuler@griffinpartners.com ",
   ["MainPhone"] = "(919) 274 - 5001",

   ["AltPhone"] = "",

   ["Customer"] = "Griffin Partner's",
   ["BuildingNames"] = "Carolina Place",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Sherry AlteCook",
   ["ManagerEmail"] = "Tracy.Tobin@avisonyoung.com",
   ["MainPhone"] = "",

   ["AltPhone"] = "",

   ["Customer"] = "Avison Young",
   ["BuildingNames"] = "Europa Center",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Steve Stryker",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "(919) 274 - 5001",

   ["AltPhone"] = "",

   ["Customer"] = "Griffin Partner's",
   ["BuildingNames"] = "Carolina Place",
   ["InternalNotes"] = "This Property Manager has no associated Email"
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Tal Woods",
   ["ManagerEmail"] = "Tal.Woods@epicgames.com",
   ["MainPhone"] = "(919) 325 - 6522",

   ["AltPhone"] = "",

   ["Customer"] = "Epic Games",
   ["BuildingNames"] = "Epic Games",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Ted Rabalais",
   ["ManagerEmail"] = "TRabalais@casso.com",
   ["MainPhone"] = "(919) 929 - 2494",

   ["AltPhone"] = "",

   ["Customer"] = "Capital Associates",
   ["BuildingNames"] = "Exchange West,Franklin St. Trust,Building 1- 10 Lab Drive,Building 2-10 Lab Drive",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Tim Schell",
   ["ManagerEmail"] = "tschell@casso.com",
   ["MainPhone"] = "(919) 783 - 0981",

   ["AltPhone"] = "",

   ["Customer"] = "Capital Associates",
   ["BuildingNames"] = "Wycliff",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Tracy Tobin",
   ["ManagerEmail"] = "sherry.alte-cook@avisonyoung.com",
   ["MainPhone"] = "(919) 968 - 4017",

   ["AltPhone"] = "",

   ["Customer"] = "Avison Young",
   ["BuildingNames"] = "Europa Center",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Virginia Gordon",
   ["ManagerEmail"] = "virginia.gordon@avisonyoung.com",
   ["MainPhone"] = "(919) 420 - 1577",

   ["AltPhone"] = "",

   ["Customer"] = "Avison Young",
   ["BuildingNames"] = "RDU I,Landmark Center -I-II",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Wayne Singleton",
   ["ManagerEmail"] = "wsingleton@casso.com",
   ["MainPhone"] = "(919) 677 - 8308",

   ["AltPhone"] = "",

   ["Customer"] = "Capital Associates",
   ["BuildingNames"] = "Durham Center",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Wes Smith",
   ["ManagerEmail"] = "wes@rhynemanagement.com",
   ["MainPhone"] = "(919) 787 - 9375",

   ["AltPhone"] = "",

   ["Customer"] = "Rhyne management Associate",
   ["BuildingNames"] = "Dale St.",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "William Grant",
   ["ManagerEmail"] = "Bobby.Grant@Q2LabSolutions.com",
   ["MainPhone"] = "(919) 376 - 5801",

   ["AltPhone"] = "",

   ["Customer"] = "Q2 Solutions Lab",
   ["BuildingNames"] = "Lab 2 (Q2 solution)",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "Winter Lofaro",
   ["ManagerEmail"] = "wlofaro@triprop.com",
   ["MainPhone"] = "(919) 281 - 2326",

   ["AltPhone"] = "",

   ["Customer"] = "Tri Properties",
   ["BuildingNames"] = "North Chase I,North Chase II",
   ["InternalNotes"] = ""
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "",

   ["AltPhone"] = "",

   ["Customer"] = "CB Richard Ellis",
   ["BuildingNames"] = "Parmer RTP",
   ["InternalNotes"] = "This building has no associated property manager"
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "",

   ["AltPhone"] = "",

   ["Customer"] = "First Citizens Bank",
   ["BuildingNames"] = "Data Center",
   ["InternalNotes"] = "This building has no associated property manager"
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "",

   ["AltPhone"] = "",

   ["Customer"] = "First Citizens Bank",
   ["BuildingNames"] = "FCC",
   ["InternalNotes"] = "This building has no associated property manager"
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "",

   ["AltPhone"] = "",

   ["Customer"] = "First Citizens Bank",
   ["BuildingNames"] = "North Hills",
   ["InternalNotes"] = "This building has no associated property manager"
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "",

   ["AltPhone"] = "",

   ["Customer"] = "TradeMark properties",
   ["BuildingNames"] = "1001 Wade Ave",
   ["InternalNotes"] = "This building has no associated property manager"
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "",

   ["AltPhone"] = "",

   ["Customer"] = "Tri Properties",
   ["BuildingNames"] = "Berrington",
   ["InternalNotes"] = "This building has no associated property manager"
 },
 new Dictionary<string, string> {
   ["ManagerName"] = "",
   ["ManagerEmail"] = "",
   ["MainPhone"] = "",

   ["AltPhone"] = "",

   ["Customer"] = "",
   ["BuildingNames"] = "IQVIA (Quintiles)",
   ["InternalNotes"] = "This building has no associated property manager or customer"
 }
};
        /// <summary>
        ///     Main methods for executes the seed operations.
        /// </summary>
        /// <param name="context">The DbContext for modifiyng the data</param>
        /// <param name="count">The numbers of elements to add.</param>
        public static void AddOrUpdate(MGCapDbContext context, int count = 50)
        {
            if (context.BuildingContacts.Any())
            {
                return;
            }

            var companiesIds = context.Companies
                          .Select(c => c.ID)
                          .ToList();

            foreach (var companyId in companiesIds)
            {
                // All customers given the current company
                var customers = context.Customers
                                       .Where(c => c.CompanyId == companyId)
                                       ?.ToList();

                var buildings = context.Buildings
                                        .Where(c => c.CompanyId == companyId)
                                       ?.ToList();

                if (buildings == null || buildings.Count == 0)
                {
                    continue;
                }

                foreach (var manager in Managers)
                {
                    var buildingContactSeen = new List<BuildingContact>();
                    var customer = customers.FirstOrDefault(c => c.Name == manager["Customer"]);
                    var buildingNames = manager["BuildingNames"].Split(',');

                    var names = manager["ManagerName"].Split();
                    var email = manager["ManagerEmail"];
                    var mainPhone = manager["MainPhone"];
                    var altPhone = manager["AltPhone"];

                    var contact = new Contact
                    {
                        FirstName = names[0],
                        LastName = names.Length > 1 ? names[1] : "",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        CreatedBy = "Seed",
                        UpdatedBy = "Seed",
                        CompanyId = companyId,
                        CompanyName = manager["Customer"],
                    };
                    context.Contacts.Add(contact);

                    // Creates email if exists
                    if (!string.IsNullOrEmpty(email))
                    {
                        var contactEmail = new ContactEmail
                        {
                            ContactId = contact.ID,
                            Default = true,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedDate = DateTime.UtcNow,
                            CreatedBy = "Seed",
                            UpdatedBy = "Seed",
                            Email = email,
                            Type = "Main Email"
                        };
                        context.ContactEmails.Add(contactEmail);
                    }

                    // Creates Main Phone If Exists
                    if (!string.IsNullOrEmpty(mainPhone))
                    {
                        var contactMainPhone = new ContactPhone
                        {
                            CreatedDate = DateTime.UtcNow,
                            UpdatedDate = DateTime.UtcNow,
                            CreatedBy = "Seed",
                            UpdatedBy = "Seed",
                            ContactId = contact.ID,
                            Default = true,
                            Type = "Main Phone",
                            Phone = new String(mainPhone.Where(Char.IsDigit).ToArray())
                        };
                        context.ContactPhones.Add(contactMainPhone);
                    }

                    // Creates Alt Phone If Exists
                    if (!string.IsNullOrEmpty(altPhone))
                    {
                        var contactAltPhone = new ContactPhone
                        {
                            CreatedDate = DateTime.UtcNow,
                            UpdatedDate = DateTime.UtcNow,
                            CreatedBy = "Seed",
                            UpdatedBy = "Seed",
                            ContactId = contact.ID,
                            Default = false,
                            Type = "Alt Phone",
                            Phone = new String(altPhone.Where(Char.IsDigit).ToArray())
                        };
                        context.ContactPhones.Add(contactAltPhone);
                    }


                    // Creates relationship between customer and contact
                    if (customer != null)
                    {
                        var customerContact = new CustomerContact
                        {
                            CustomerId = customer.ID,
                            ContactId = contact.ID,
                            Type = "Property Manager"
                        };
                        context.CustomerContacts.Add(customerContact);
                    }

                    // Creates Relationship betewwn building and contact
                    if (buildingNames.Length > 0)
                    {
                        foreach (var bldg in buildingNames)
                        {
                            var building = buildings.FirstOrDefault(b => b.Name == bldg);
                            if (building == null)
                            {
                                continue;
                            }

                            if (buildingContactSeen.Any(bc => bc.ContactId == contact.ID && bc.BuildingId == building.ID))
                            {
                                continue;
                            }
                            var buildingContact = new BuildingContact
                            {
                                BuildingId = building.ID,
                                ContactId = contact.ID,
                                Type = "Property Manager"
                            };
                            buildingContactSeen.Add(buildingContact);
                            context.BuildingContacts.Add(buildingContact);
                        }
                    }
                }
            }
        }
    }
}
