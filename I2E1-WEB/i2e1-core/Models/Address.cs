using i2e1_basics.Database;
using i2e1_basics.Utilities;
using i2e1_core.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;

namespace i2e1_core.Models
{
    public enum AddressType
    {
        GOOGLE = 0,
        CUSTOM = 1
    }

    public class Address
    {
        public LongIdInfo id { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string home { get; set; }
        public string street { get; set; }
        public string address { get; set; }
        public string locality { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string pincode { get; set; }
        public AddressType type { get; set; }
        public DateTime addedTime { get; set; }

        public static Address GoogleReverseLocation(double lat, double lng)
        {
            try
            {
                string baseUri = "https://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&key=AIzaSyAjVBw0c7ZtGMewBvBRLq_4iGW1LIZbggM";
                string requestUri = string.Format(baseUri, lat, lng);

                using (WebClient wc = new WebClient())
                {
                    string response = wc.DownloadString(new Uri(requestUri));
                    var jObject = JsonConvert.DeserializeObject<JObject>(response);

                    if (jObject["status"].ToString() == "OK")
                    {
                        var addressSet = GetValidAddress((JArray)jObject["results"]);
                        var res = addressSet["formatted_address"].ToString();
                        var add = res.Split(',');
                        var count = add.Length;
                        var address = string.Empty;
                        var y = add[count - 2];
                        var pin = y.Substring(y.Length - 6);
                        var state = y.Length - 7 >= 0 ? y.Substring(0, y.Length - 7) : string.Empty;
                        var city = add[count - 3];
                        var subLocalityList = addressSet["address_components"].Where(m => {
                            return m["types"].FirstOrDefault(nd => nd.ToString() == "sublocality") != null;
                            }).ToList();

                        string subLocalityText = string.Empty; ;
                        if (subLocalityList != null)
                        {
                            for (int j = 0; j < subLocalityList.Count; ++j)
                            {
                                subLocalityText += subLocalityList[j]["short_name"];
                                if (j < subLocalityList.Count - 1)
                                    subLocalityText += ", ";
                            }
                        }
                        int i = 0;
                        for (i = 0; i <= count - 4; ++i)
                        {
                            if (subLocalityList == null || !subLocalityList.Contains(add[i].Trim()))
                            {
                                address += add[i] + ",";
                            }
                        }

                        return new Address()
                        {
                            city = city,
                            state = state,
                            pincode = pin,
                            address = address == string.Empty ? string.Empty : address.Substring(0, address.Length - 1),
                            locality = subLocalityText,
                            type = AddressType.GOOGLE,
                            lat = lat,
                            lng = lng
                        };

                    }
                    else
                    {
                        Logger.GetInstance().Error($"No Address Found for lat:{lat}, lng:{lng}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error("Address.GoogleReverseLocation -> " + ex.ToString());
            }
            return null;
        }

        private static JObject GetValidAddress(JArray addressSet)
        {
            var address = (JObject)addressSet.FirstOrDefault(address => !address["formatted_address"].ToString().Contains('+'));
            if (address == null)
                address = (JObject)addressSet[0];
            return address;
        }

        public LongIdInfo Save(long shardId, string updatedBy, string updatedByEntity)
        {
            return new ShardQueryExecutor<LongIdInfo>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(@"DECLARE @address_id int
                    SET @address_id = (SELECT top 1 id FROM t_address where lat = @lat and lng = @lng order by id desc)
                    IF @address_id is NOT NULL
                    BEGIN
	                    UPDATE t_address SET
	                    address = COALESCE(@address, address),
	                    locality = COALESCE(@locality, locality),
	                    city = COALESCE(@city, city),
	                    pincode = COALESCE(@pincode, pincode),
	                    state = COALESCE(@state, state),
	                    type = @type,
	                    extra_data = COALESCE(@extra_data, extra_data)
	                    WHERE id = @address_id

                        SELECT @address_id as id  
                    END
                    ELSE
                    BEGIN
	                    INSERT INTO t_address(lat, lng, address, locality, city, pincode, state, type, extra_data)
	                    VALUES(@lat, @lng, @address, @locality, @city, @pincode, @state, @type, @extra_data)
                        SELECT CAST(SCOPE_IDENTITY() AS int) AS id 
                    END");

                cmd.Parameters.Add(new SqlParameter("@lat", lat));
                cmd.Parameters.Add(new SqlParameter("@lng", lng));
                cmd.Parameters.Add(new SqlParameter("@address", CoreUtil.ToSafeDbObject(address)));
                cmd.Parameters.Add(new SqlParameter("@locality", CoreUtil.ToSafeDbObject(locality)));
                cmd.Parameters.Add(new SqlParameter("@city", CoreUtil.ToSafeDbObject(city)));
                cmd.Parameters.Add(new SqlParameter("@pincode", CoreUtil.ToSafeDbObject(pincode)));
                cmd.Parameters.Add(new SqlParameter("@state", CoreUtil.ToSafeDbObject(state)));
                cmd.Parameters.Add(new SqlParameter("@type", type));
                cmd.Parameters.Add(new SqlParameter("@extra_data", JsonConvert.SerializeObject(new { updatedBy, updatedByEntity })));

                res = ResponseType.READER;
                return cmd;
            }),shardId,
            new ResponseHandler<LongIdInfo>((reader) => {
                if (reader.Read())
                    return new LongIdInfo(shardId, DBObjectType.ADDRESS_TYPE, Convert.ToInt64(reader["id"]));
                return null;
            })).Execute();
        }

        public static Address GetAddress(LongIdInfo id)
        {
            if (id == null)
                return null;
            return new ShardQueryExecutor<Address>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(@"SELECT * FROM t_address where id = @id");

                cmd.Parameters.Add(new SqlParameter("@id", id.ToSafeDbObject()));

                res = ResponseType.READER;
                return cmd;
            }), id.shard_id,
            new ResponseHandler<Address>((reader) => {
                if (reader.Read())
                {
                    return new Address()
                    {
                        id = id,
                        lat = (double)reader["lat"],
                        lng = (double)reader["lng"],
                        address = reader["address"].ToString(),
                        locality = reader["locality"].ToString(),
                        city = reader["city"].ToString(),
                        pincode = reader["pincode"].ToString(),
                        state = reader["state"].ToString(),
                        type =  (AddressType)Convert.ToInt32(reader["type"]),
                        addedTime = (DateTime)reader["added_time"]
                    };
                }
                return null;
            })).Execute();
        }

        public static Address parseJson(string json) {
            try
            {
                var addrs = JsonConvert.DeserializeObject<List<Address>>(json);
                if (addrs != null && addrs.Count > 0)
                    return addrs[0];
            }
            catch (Exception ex) 
            {
                Logger.GetInstance().Info("Not able to parse address json: " + json);
            }
            return null;
        }
    }
}
