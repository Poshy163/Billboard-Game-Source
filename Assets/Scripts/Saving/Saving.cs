﻿#pragma warning disable 618

#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using UnityEngine;
using static Other.GlobalVar;
using BsonReader = Newtonsoft.Json.Bson.BsonReader;
using JsonConvert = Newtonsoft.Json.JsonConvert;

#endregion

// ReSharper disable PossibleNullReferenceException
// ReSharper disable ParameterHidesMember
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

namespace Saving
{
    public class Saving : MonoBehaviour
    {
        private const string MongoLogin =
            "mongodb+srv://User:User@time.ejfbr.mongodb.net/test?authSource=admin&replicaSet=atlas-hqix16-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true";

        public static void CheckLevelTime(string name, double time, short level)
        {
            if (!(double.Parse(GetData(name, time, level)) > time)) return;

            DeleteDatabaseEntry(name, level);
            SendToDatabase(name, time, level);
        }

        public static void DeleteUser(string name)
        {
            var filter = new BsonDocument {{"Name", name}};
            var client = new MongoClient(MongoLogin);
            var database = client.GetDatabase("UserDetails");
            var collection = database.GetCollection<BsonDocument>("Login Details");
            var documents = collection.Find(filter).ToList();
            collection.DeleteMany(documents[0]);
            var collection2 = database.GetCollection<BsonDocument>("User Statistics");
            var documents2 = collection2.Find(filter).ToList();
            collection2.DeleteMany(documents2[0]);
        }

        public static bool Login(string name, string password)
        {
            try
            {
                var filter = new BsonDocument {{"Name", name}};
                var client = new MongoClient(MongoLogin);
                var database = client.GetDatabase("UserDetails");
                var collection = database.GetCollection<BsonDocument>("Login Details");
                var documents = collection.Find(filter).ToList();
                dynamic jsonFile = JsonConvert.DeserializeObject(ToJson(documents[0]));
                return jsonFile["Password"] == Sha256Hash(password);
            }
            catch
            {
                return false;
            }
        }

        public static bool SignUp(string name, string password)
        {
            try
            {
                var filter = new BsonDocument {{"Name", name}};
                var client = new MongoClient(MongoLogin);
                var database = client.GetDatabase("UserDetails");
                var collection = database.GetCollection<BsonDocument>("Login Details");
                var documents = collection.Find(filter).ToList();
                JsonConvert.DeserializeObject(ToJson(documents[0]));
                return false;
            }
            catch
            {
                var client = new MongoClient(MongoLogin);
                var database = client.GetDatabase("UserDetails");
                var collection = database.GetCollection<BsonDocument>("Login Details");

                var document = new BsonDocument
                {
                    {"Name", name},
                    {"Password", Sha256Hash(password)}
                };
                collection.InsertOne(document);
                return true;
            }
        }

        public static List<KeyValuePair<string, float>> GetTopTimes(short level)
        {
            var client = new MongoClient(MongoLogin);
            var database = client.GetDatabase("Time");
            var collection = database.GetCollection<BsonDocument>($"Level {level}");
            var documents = collection.Find(new BsonDocument()).ToList();
            var topTime = documents.Select(doc => JsonConvert.DeserializeObject(ToJson(doc)))
                .ToDictionary<object, string, float>
                (jsonFile => ((dynamic) jsonFile)["Name"].ToString(),
                    jsonFile => float.Parse(((dynamic) jsonFile)["Time"].ToString()));
            var myList = topTime.ToList();
            myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            return myList;
        }

        public static void UpdateTopStats(string name)
        {
            var filter = new BsonDocument {{"Name", name}};
            var client = new MongoClient(MongoLogin);
            var database = client.GetDatabase("UserDetails");
            var collection = database.GetCollection<BsonDocument>("User Statistics");
            var document = new BsonDocument
            {
                {"Name", name},
                {"MaxCombo", Maxcombo},
                {"Max AirTime", MaxAirTime}
            };
            collection.FindOneAndUpdate(filter, document);
        }

        public static void SendDummyInfo(string name) // This is to send after signing up
        {
            var client = new MongoClient(MongoLogin);
            var database = client.GetDatabase("UserDetails");
            var collection = database.GetCollection<BsonDocument>("User Statistics");
            var document = new BsonDocument
            {
                {"Name", name},
                {"MaxCombo", 0},
                {"Max AirTime", 0}
            };
            collection.InsertOne(document);
        }

        public static Dictionary<string, float> GetUserStats(string name)
        {
            var filter = new BsonDocument {{"Name", name}};
            var client = new MongoClient(MongoLogin);
            var database = client.GetDatabase("UserDetails");
            var collection = database.GetCollection<BsonDocument>("User Statistics");
            var documents = collection.Find(filter).ToList();
            dynamic jsonFile = JsonConvert.DeserializeObject(ToJson(documents[0]));
            var dic = new Dictionary<string, float>
            {
                {"MaxCombo", (float) jsonFile["MaxCombo"]},
                {"Max AirTime", (float) jsonFile["Max AirTime"]}
            };
            return dic;
        }


        public static void DeleteDatabaseEntry(string name, short level)
        {
            var filter = new BsonDocument {{"Name", name}};
            var client = new MongoClient(MongoLogin);
            var database = client.GetDatabase("Time");
            var collection = database.GetCollection<BsonDocument>($"Level {level}");
            var documents = collection.Find(filter).ToList();
            collection.DeleteMany(documents[0]);
        }

        private static string GetData(string name, double time, short level)
        {
            try
            {
                var filter = new BsonDocument {{"Name", name}};
                var client = new MongoClient(MongoLogin);
                var database = client.GetDatabase("Time");
                var collection = database.GetCollection<BsonDocument>($"Level {level}");
                var documents = collection.Find(filter).ToList();
                dynamic jsonFile = JsonConvert.DeserializeObject(ToJson(documents[0]));
                return jsonFile["Time"];
            }
            catch
            {
                SendToDatabase(name, time, level);
                return short.MaxValue.ToString();
            }
        }


        private static void SendToDatabase(string name, double time, short level = 0)
        {
            var client = new MongoClient(MongoLogin);
            var database = client.GetDatabase("Time");
            var collection = database.GetCollection<BsonDocument>($"Level {level}");
            var document = new BsonDocument
            {
                {"Name", name},
                {"Time", time}
            };
            collection.InsertOne(document);
        }

        private static string ToJson(BsonDocument bson)
        {
            var stream = new MemoryStream();
            using (var writer = new BsonBinaryWriter(stream))
            {
                BsonSerializer.Serialize(writer, typeof(BsonDocument), bson);
            }

            stream.Seek(0, SeekOrigin.Begin);
            var reader = new BsonReader(stream);
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (var jWriter = new JsonTextWriter(sw))
            {
                jWriter.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                jWriter.WriteToken(reader);
            }

            return sb.ToString();
        }

        private static string Sha256Hash(string rawData)
        {
            var sha256Hash = SHA256.Create();
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            foreach (var t in bytes) builder.Append(t.ToString("x2"));

            return builder.ToString();
        }
    }
}