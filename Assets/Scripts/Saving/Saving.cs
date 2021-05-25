using System.IO;
using System.Text;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
// ReSharper disable PossibleNullReferenceException
#pragma warning disable 618

namespace Saving
{
    public class Saving : MonoBehaviour
    {
        private void CheckLevelTime(double time)
        {
            
        }

        public static void GetData(string name, short level = 0)
        {
            var filter = new BsonDocument { { "Name", name } };
            var client = new MongoClient("mongodb+srv://User:User@time.ejfbr.mongodb.net/test?authSource=admin&replicaSet=atlas-hqix16-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true");
            var database = client.GetDatabase("Time");
            var collection = database.GetCollection<BsonDocument>(level == 0 ?
                $"Level {level}" : "TestCollection");
            var documents = collection.Find(filter).ToList();
            
            //TODO maybe fix this, it will somehow break and i wont know why
            dynamic jsonFile = Newtonsoft.Json.JsonConvert.DeserializeObject(ToJson(documents[0]));
            Debug.Log(jsonFile["Time"] + "");
        }

        public static void SendToDatabase(string name, double time , short level = 0)
        {
            var client = new MongoClient("mongodb+srv://User:User@time.ejfbr.mongodb.net/test?authSource=admin&replicaSet=atlas-hqix16-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true");
            var database = client.GetDatabase("Time");
            var collection = database.GetCollection<BsonDocument>(level == 0 ?
                $"Level {level}" : "TestCollection");
            
            var document = new BsonDocument
            {
                {"Name" , name},
                {"Time", time}
            };
            collection.InsertOne(document);
            GetData(name);
        }

        private static string ToJson(BsonDocument bson)
        {
            var stream = new MemoryStream();
            using (BsonBinaryWriter writer = new BsonBinaryWriter(stream))
            {
                BsonSerializer.Serialize(writer, typeof(BsonDocument), bson);
            }
            stream.Seek(0, SeekOrigin.Begin);
            var reader = new Newtonsoft.Json.Bson.BsonReader(stream);
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (var jWriter = new JsonTextWriter(sw))
            {
                jWriter.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                jWriter.WriteToken(reader);
            }
            return sb.ToString();
        }
    }
}