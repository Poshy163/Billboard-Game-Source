#pragma warning disable 618
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using BsonReader = Newtonsoft.Json.Bson.BsonReader;
using JsonConvert = Newtonsoft.Json.JsonConvert;

// ReSharper disable PossibleNullReferenceException
// ReSharper disable ParameterHidesMember
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

namespace Saving
{
    public class Saving:MonoBehaviour
    {
        private const string MongoLogin =
            "mongodb+srv://User:User@time.ejfbr.mongodb.net/test?authSource=admin&replicaSet=atlas-hqix16-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true";

        public static void CheckLevelTime ( string name,double time,short level )
        {
            if(!(double.Parse(GetData(name,time,level)) > time))
            {
                return;
            }

            DeleteDatabaseEntry(name,level);
            SendToDatabase(name,time,level);
        }

        public static void DeleteUser ( string name )
        {
            BsonDocument filter = new BsonDocument { { "Name",name } };
            MongoClient client = new MongoClient(MongoLogin);
            IMongoDatabase database = client.GetDatabase("UserDetails");
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("Login Details");
            List<BsonDocument> documents = collection.Find(filter).ToList();
            collection.DeleteMany(documents[0]);
        }

        public static bool Login ( string name,string password )
        {
            try
            {
                BsonDocument filter = new BsonDocument { { "Name",name } };
                MongoClient client = new MongoClient(MongoLogin);
                IMongoDatabase database = client.GetDatabase("UserDetails");
                IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("Login Details");
                List<BsonDocument> documents = collection.Find(filter).ToList();
                dynamic jsonFile = JsonConvert.DeserializeObject(ToJson(documents[0]));
                return jsonFile["Password"] == Sha256Hash(password);
            }
            catch
            {
                return false;
            }
        }

        public static bool SignUp ( string name,string password )
        {
            try
            {
                BsonDocument filter = new BsonDocument { { "Name",name } };
                MongoClient client = new MongoClient(MongoLogin);
                IMongoDatabase database = client.GetDatabase("UserDetails");
                IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("Login Details");
                List<BsonDocument> documents = collection.Find(filter).ToList();
                JsonConvert.DeserializeObject(ToJson(documents[0]));
                return false;
            }
            catch
            {
                MongoClient client = new MongoClient(MongoLogin);
                IMongoDatabase database = client.GetDatabase("UserDetails");
                IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("Login Details");

                BsonDocument document = new BsonDocument
                {
                    {"Name", name},
                    {"Password", Sha256Hash(password)}
                };
                collection.InsertOne(document);
                return true;
            }
        }

        public static List<KeyValuePair<string,float>> GetTopTimes ( short level )
        {
            MongoClient client = new MongoClient(MongoLogin);
            IMongoDatabase database = client.GetDatabase("Time");
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>($"Level {level}");
            List<BsonDocument> documents = collection.Find(new BsonDocument()).ToList();
            Dictionary<string,float> topTime = documents.Select(doc => JsonConvert.DeserializeObject(ToJson(doc)))
                .ToDictionary<object,string,float>
                (jsonFile => ((dynamic)jsonFile)["Name"].ToString(),
                    jsonFile => float.Parse(((dynamic)jsonFile)["Time"].ToString()));
            List<KeyValuePair<string,float>> myList = topTime.ToList();
            myList.Sort(( pair1,pair2 ) => pair1.Value.CompareTo(pair2.Value));
            return myList;
        }

        public static void DeleteDatabaseEntry ( string name,short level )
        {
            BsonDocument filter = new BsonDocument { { "Name",name } };
            MongoClient client = new MongoClient(MongoLogin);
            IMongoDatabase database = client.GetDatabase("Time");
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>($"Level {level}");
            List<BsonDocument> documents = collection.Find(filter).ToList();
            collection.DeleteMany(documents[0]);
        }

        private static string GetData ( string name,double time,short level )
        {
            try
            {
                BsonDocument filter = new BsonDocument { { "Name",name } };
                MongoClient client = new MongoClient(MongoLogin);
                IMongoDatabase database = client.GetDatabase("Time");
                IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>($"Level {level}");
                List<BsonDocument> documents = collection.Find(filter).ToList();
                dynamic jsonFile = JsonConvert.DeserializeObject(ToJson(documents[0]));
                return jsonFile["Time"];
            }
            catch
            {
                SendToDatabase(name,time,level);
                return short.MaxValue.ToString();
            }
        }


        private static void SendToDatabase ( string name,double time,short level = 0 )
        {
            MongoClient client = new MongoClient(MongoLogin);
            IMongoDatabase database = client.GetDatabase("Time");
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>($"Level {level}");
            BsonDocument document = new BsonDocument
            {
                {"Name", name},
                {"Time", time}
            };
            collection.InsertOne(document);
        }

        private static string ToJson ( BsonDocument bson )
        {
            MemoryStream stream = new MemoryStream();
            using(BsonBinaryWriter writer = new BsonBinaryWriter(stream))
            {
                BsonSerializer.Serialize(writer,typeof(BsonDocument),bson);
            }

            stream.Seek(0,SeekOrigin.Begin);
            BsonReader reader = new BsonReader(stream);
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using(JsonTextWriter jWriter = new JsonTextWriter(sw))
            {
                jWriter.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                jWriter.WriteToken(reader);
            }

            return sb.ToString();
        }

        private static string Sha256Hash ( string rawData )
        {
            SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            foreach(byte t in bytes)
            {
                builder.Append(t.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}