using Amazon.DynamoDBv2.Model;
using i2e1_basics.Utilities;
using i2e1_core.Models;
using i2e1_core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace i2e1_core.DynamoUtilities
{
    public class BaseDynamoDbHelper : i2e1_basics.DynamoUtilities.BaseDynamoDbHelper
    {
        public static AttributeValue ConvertToAttributeValue(object value)
        {
            var attributeValue = new AttributeValue();
            if (value == null)
            {
                attributeValue.NULL = true;
            }
            else
            {
                if (value is string stringValue)
                {
                    attributeValue.S = stringValue;
                }
                else if (value is int intValue)
                {
                    attributeValue.N = intValue.ToString();
                }
                else if (value is double doubleValue)
                {
                    attributeValue.N = doubleValue.ToString();
                }
                else if (value is DateTime dateTime)
                {
                    //DateTime dateTime1 = DateTime.ParseExact(dateTime.ToString(), "dd-MM-yyyy HH:mm:ss", null);
                    attributeValue.S = dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                }
                else if (value is long longValue)
                {
                    attributeValue.N = longValue.ToString();
                }
                else if (value is byte byteValue)
                {
                    attributeValue.N = byteValue.ToString();
                }
                else if (value is bool boolValue)
                {
                    attributeValue.BOOL = (bool)value;
                }
                else if (value is LongIdInfo longIdValue)
                {
                    attributeValue.N = longIdValue.GetLongId().ToString();
                }
                else if (value is List<string> listString)
                {
                    attributeValue.SS = listString;
                }
                else if (value is List<int> listInt)
                {
                    attributeValue.NS = listInt.Select((item) => item.ToString()).ToList();
                }
                else if (value is PaymentType payEnum)
                {
                    attributeValue.N = ((int)payEnum).ToString();
                }
            }
            return attributeValue;
        }
    }
}
