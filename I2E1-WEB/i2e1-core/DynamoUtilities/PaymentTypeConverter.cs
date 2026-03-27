using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using i2e1_core.Models;
using System;

namespace i2e1_core.DynamoUtilities
{
    public class PaymentTypeConverter : IPropertyConverter
    {
        public DynamoDBEntry ToEntry(object value)
        {
            if (value == null)
                return new DynamoDBNull();

            if (value is PaymentType paymentType)
                return new Primitive(paymentType.ToString());

            throw new InvalidOperationException($"Unsupported type: {value.GetType()}");
        }

        public object FromEntry(DynamoDBEntry entry)
        {
            if (entry == null || entry is DynamoDBNull)
                return null;

            if (entry is Primitive primitive && primitive.Type == DynamoDBEntryType.Numeric)
            {
                if (Enum.IsDefined(typeof(PaymentType), primitive.AsInt()))
                    return (PaymentType)primitive.AsInt();
            }

            throw new InvalidOperationException($"Unsupported entry: {entry}");
        }
    }
}
