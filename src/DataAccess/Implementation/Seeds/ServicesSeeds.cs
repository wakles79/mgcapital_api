using MGCap.DataAccess.Implementation.Context;
using MGCap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MGCap.DataAccess.Implementation.Seeds
{
    public static class ServicesSeeds
    {
        public static List<Dictionary<string, string>> Services = new List<Dictionary<string, string>>
        {
   new Dictionary<string, string> {
    ["ServiceName"] = "Dry Carpet Cleaning",
    ["UnitPrice"] = "0.1",
    ["UnitFactor"] = "Sq.FT",
    ["MinPrice"] = "200"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Shampoo/Steam Cleaning",
    ["UnitPrice"] = "0.15",
    ["UnitFactor"] = "Sq.FT",
    ["MinPrice"] = "200"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Extraction Carpet Cleaning",
    ["UnitPrice"] = "0.1",
    ["UnitFactor"] = "Sq.FT",
    ["MinPrice"] = "200"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Spot Carpet Cleaning",
    ["UnitPrice"] = "12",
    ["UnitFactor"] = "Hour",
    ["MinPrice"] = "350"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Strip and Wax Floors",
    ["UnitPrice"] = "0.5",
    ["UnitFactor"] = "Sq.FT",
    ["MinPrice"] = "200"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Wash Bathroom",
    ["UnitPrice"] = "50",
    ["UnitFactor"] = "Unit",
    ["MinPrice"] = "50"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Wash Grout",
    ["UnitPrice"] = "0.5",
    ["UnitFactor"] = "Sq.FT",
    ["MinPrice"] = "200"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Stone Foors",
    ["UnitPrice"] = "0.15",
    ["UnitFactor"] = "Sq.FT",
    ["MinPrice"] = "200"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Power Wash",
    ["UnitPrice"] = "20",
    ["UnitFactor"] = "Hour",
    ["MinPrice"] = "200"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Painting",
    ["UnitPrice"] = "0.75",
    ["UnitFactor"] = "Sq.FT",
    ["MinPrice"] = "200"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Window Cleaning",
    ["UnitPrice"] = "75",
    ["UnitFactor"] = "Unit",
    ["MinPrice"] = "200"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Deep Cleaning",
    ["UnitPrice"] = "0.5",
    ["UnitFactor"] = "Sq.FT",
    ["MinPrice"] = "200"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Post Contruction Cleaning",
    ["UnitPrice"] = "1",
    ["UnitFactor"] = "Sq.FT",
    ["MinPrice"] = "200"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Clean Refrigerator",
    ["UnitPrice"] = "50",
    ["UnitFactor"] = "Unit",
    ["MinPrice"] = "100"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Clean Microwave",
    ["UnitPrice"] = "50",
    ["UnitFactor"] = "Unit",
    ["MinPrice"] = "100"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Toasters",
    ["UnitPrice"] = "50",
    ["UnitFactor"] = "Unit",
    ["MinPrice"] = "100"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Ice Makers",
    ["UnitPrice"] = "50",
    ["UnitFactor"] = "Unit",
    ["MinPrice"] = "100"
  },
   new Dictionary<string, string> {
    ["ServiceName"] = "Coffee Pots",
    ["UnitPrice"] = "50",
    ["UnitFactor"] = "Unit",
    ["MinPrice"] = "100"
  }
        };
        public static void AddOrUpdate(MGCapDbContext context, int count = 50)
        {
            if (context.Services.Any())
            {
                return;
            }

            var companiesIds = context.Companies
                          .Select(c => c.ID)
                          .ToList();
            foreach (var companyId in companiesIds)
            {
                foreach (var dic in Services)
                {
                    var s = new Service
                    {
                        CompanyId = companyId,
                        Guid = Guid.NewGuid(),
                        CreatedBy = "Seed",
                        UpdatedBy = "Seed",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        Name = dic["ServiceName"],
                        MinPrice = double.Parse(dic["MinPrice"]),
                        UnitFactor = dic["UnitFactor"],
                        UnitPrice = double.Parse(dic["UnitPrice"])
                    };
                    context.Services.Add(s);
                }
            }
        }
    }
}
