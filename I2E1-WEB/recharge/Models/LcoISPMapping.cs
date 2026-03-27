using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using i2e1_basics.DynamoUtilities;
using i2e1_basics.Utilities;
using Newtonsoft.Json;

namespace recharge.Models
{

    [DynamoDBTable("LcoISPMapping")]

    public class LcoISPMapping
    {
        [DynamoDBHashKey]
        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        public LongIdInfo lcoAccountId {get; set;}


        [DynamoDBProperty(typeof(ISPMappingListConverter))]
        public List<ISP_ISPType_Mapping> mappings {get; set;}
    }

public class ISPMappingListConverter : IPropertyConverter
{
    public DynamoDBEntry ToEntry(object value)
    {
        if (value == null) return new DynamoDBNull();
        
        var list = value as List<ISP_ISPType_Mapping>;
        if (list == null) throw new ArgumentException("Expected a List<ISP_ISPType_Mapping>");

        // Serialize the list as JSON
        return JsonConvert.SerializeObject(list);
    }

public object FromEntry(DynamoDBEntry entry)
{
    if (entry == null || entry is DynamoDBNull) 
        return null;

    try
    {
        // Ensure entry is of type DynamoDBList
        if (entry is DynamoDBList entryList)
        {
            var mappings = entryList.Entries.Select(e =>
            {
                if (e is Document doc)
                {
                    return new ISP_ISPType_Mapping
                    {
                        ispType = (ISPTYPE)doc["ispType"].AsInt(),  // Convert stored number to Enum
                        ispName = doc["ispName"].AsString()         // Extract string value
                    };
                }
                return null;
            }).Where(m => m != null).ToList();

            return mappings;
        }

        Logger.GetInstance().Error("Entry is not a valid DynamoDBList");
        return null;
    }
    catch (Exception ex)
    {
        Logger.GetInstance().Error($"Error deserializing ISP_ISPType_Mapping: {ex.Message}");
        return null;
    }
}



}
}
