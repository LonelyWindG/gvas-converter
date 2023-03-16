using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using GvasFormat;
using GvasFormat.Serialization;
using GvasFormat.Serialization.UETypes;

namespace GvasConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("\n> GvasConverter.exe \"your.sva\"");
                    return;
                }

                var GVASPath = args[0];
                var jsonOutPath = GVASPath + ".json";

                // 解析
                Gvas save = UESerializer.Read(File.Open(GVASPath, FileMode.Open, FileAccess.Read, FileShare.Read));

                var jsonNode = JsonSerializer.SerializeToNode<Gvas>(save, new JsonSerializerOptions
                {
                    IncludeFields = true,
                    MaxDepth = 64,
                    Converters = { new UEPropJsonConvert() }
                });
                File.WriteAllText(jsonOutPath, jsonNode.ToJsonString(new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                }));
                Console.WriteLine("Convert success, json: " + jsonOutPath);

            }
            catch (Exception e)
            {
                Console.WriteLine("Convert failed, exception: " + e);
            }
            finally
            {
                Console.Write("Press any key to quit. . .");
                Console.Read();
            }
        }
    }

    /// <summary>
    /// 优化json输出
    /// </summary>
    public class UEPropJsonConvert : JsonConverter<UEProperty>
    {
        public override UEProperty? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, UEProperty value, JsonSerializerOptions options)
        {
            writer.WriteRawValue(JsonSerializer.SerializeToUtf8Bytes(value.ToObject(), options));
        }
    }
}
