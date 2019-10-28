using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Text;

namespace Bb.Web
{
    internal static class SchemaHelper
    {

        /// <summary>
        /// Generates JSON schema for a given C# class using Newtonsoft.Json.Schema library.
        /// </summary>
        /// <param name="myType">class type</param>
        /// <returns>a string containing JSON schema for a given class type</returns>
        public static StringBuilder GenerateSchemaForClass(Type myType, string name)
        {

            Type type = typeof(CustomAccessConfiguration<>).MakeGenericType(myType);

            JSchemaGenerator generator = new JSchemaGenerator()
            {
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName,
                SchemaLocationHandling = SchemaLocationHandling.Definitions,
                DefaultRequired = Newtonsoft.Json.Required.Default,
            };

            generator.GenerationProviders.Add(new StringEnumGenerationProvider());

            JSchema schema = generator.Generate(type, false);
            schema.SchemaVersion = new Uri("http://json-schema.org/draft-04/schema");

            return new StringBuilder(schema
                .ToString()
                .Replace(PropertyName, PropertyName.Replace("GetCustomConfiguration", name))
                .Replace("\"$ref\": \"", "\"$ref\": \"#/definitions/")
                );

        }
      
        public static StringBuilder GenerateSchemaForClass(Type myType)
        {

            JSchemaGenerator generator = new JSchemaGenerator()
            {
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName,
                SchemaLocationHandling = SchemaLocationHandling.Definitions,
                DefaultRequired = Newtonsoft.Json.Required.Default,
            };

            generator.GenerationProviders.Add(new StringEnumGenerationProvider());

            JSchema schema = generator.Generate(myType, false);
            schema.SchemaVersion = new Uri("http://json-schema.org/draft-04/schema");

            return new StringBuilder(schema
                .ToString()
                .Replace("\"$ref\": \"", "\"$ref\": \"#/definitions/")
                );

        }

        private class CustomAccessConfiguration<T>
        {
            public T GetCustomConfiguration { get; set; }
        }

        internal const string PropertyName = @"""GetCustomConfiguration""";

    }
}
