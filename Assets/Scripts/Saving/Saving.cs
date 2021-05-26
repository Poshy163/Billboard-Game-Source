using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
// ReSharper disable PossibleNullReferenceException
// ReSharper disable ParameterHidesMember
#pragma warning disable 618

namespace Saving
{
    public class Saving : MonoBehaviour
    {
        public static void CheckLevelTime(string name, double time, short level)
        {
            if (!(double.Parse(GetData(name, time, level)) > time)) return;
            DeleteDatabaseEntry(name,time,level);
            SendToDatabase(name,time,level);
        }


        public static bool Login(string name, string password)
        {
            var filter = new BsonDocument { { "Name", name } };
            var client = new MongoClient("mongodb+srv://User:User@time.ejfbr.mongodb.net/test?authSource=admin&replicaSet=atlas-hqix16-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true");
            var database = client.GetDatabase("UserDetails");
            var collection = database.GetCollection<BsonDocument>($"Login Details");
            var documents = collection.Find(filter).ToList();
            dynamic jsonFile = Newtonsoft.Json.JsonConvert.DeserializeObject(ToJson(documents[0]));
            return jsonFile["Password"] == Sha256Hash(password);
        }

        public static bool SignUp(string name, string password)
        {
            try
            { 
                var filter = new BsonDocument { { "Name", name } };
                var client = new MongoClient("mongodb+srv://User:User@time.ejfbr.mongodb.net/test?authSource=admin&replicaSet=atlas-hqix16-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true");
                var database = client.GetDatabase("UserDetails");
                var collection = database.GetCollection<BsonDocument>($"Login Details");
                var documents = collection.Find(filter).ToList();
                dynamic jsonFile = Newtonsoft.Json.JsonConvert.DeserializeObject(ToJson(documents[0]));
                return false;
            }
            catch 
            {
                var client = new MongoClient("mongodb+srv://User:User@time.ejfbr.mongodb.net/test?authSource=admin&replicaSet=atlas-hqix16-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true");
                var database = client.GetDatabase("UserDetails");
                var collection = database.GetCollection<BsonDocument>($"Login Details");

                var document = new BsonDocument
                {
                    {"Name" , name},
                    {"Password", Sha256Hash(password)}
                };
                collection.InsertOne(document);
                return true;
            }
        }
        static string Sha256Hash(string rawData) 
        {
            using (var sha256Hash = SHA256.Create())  
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();  
                foreach (var t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }  
                return builder.ToString();  
            }  
        }  
        


        private static void DeleteDatabaseEntry(string name,double time,short level )
        {
            var filter = new BsonDocument { { "Name",name } };
            var client = new MongoClient("mongodb+srv://User:User@time.ejfbr.mongodb.net/test?authSource=admin&replicaSet=atlas-hqix16-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true");
            var database = client.GetDatabase("Time");
            var collection = database.GetCollection<BsonDocument>($"Level {level}");

             var documents = collection.Find(filter).ToList();
             collection.DeleteMany(documents[0]);
          
        }
        private static string GetData(string name, double time, short level = 0)
        {
            try
            {
                var filter = new BsonDocument { { "Name", name } };
                var client = new MongoClient("mongodb+srv://User:User@time.ejfbr.mongodb.net/test?authSource=admin&replicaSet=atlas-hqix16-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true");
                var database = client.GetDatabase("Time");
                var collection = database.GetCollection<BsonDocument>($"Level {level}");
                var documents = collection.Find(filter).ToList();
                //TODO maybe fix this, it will somehow break and i wont know why
                dynamic jsonFile = Newtonsoft.Json.JsonConvert.DeserializeObject(ToJson(documents[0]));
                return jsonFile["Time"];
            }
            catch 
            {
                SendToDatabase(name,time,level);
                return short.MaxValue.ToString();
            }
            //TODO fix this its so bad and wont work 
        }
        private static void SendToDatabase(string name, double time , short level = 0)
        {
            var client = new MongoClient("mongodb+srv://User:User@time.ejfbr.mongodb.net/test?authSource=admin&replicaSet=atlas-hqix16-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true");
            var database = client.GetDatabase("Time");
            var collection = database.GetCollection<BsonDocument>($"Level {level}");

            var document = new BsonDocument
            {
                {"Name" , name},
                {"Time", time}
            };
            collection.InsertOne(document);
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