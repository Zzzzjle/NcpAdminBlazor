﻿using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi;

namespace NcpAdminBlazor.Web.Extensions
{
    public static class SwaggerGenOptionsExtionsions
    {
        public static SwaggerGenOptions AddEntityIdSchemaMap(this SwaggerGenOptions swaggerGenOptions)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()
                         .Where(p => p.FullName != null && p.FullName.Contains("NcpAdminBlazor")))
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && Array.Exists(type.GetInterfaces(), p => p == typeof(IEntityId)))
                    {
                        swaggerGenOptions.MapType(type,
                            () => new OpenApiSchema { Type = JsonSchemaType.String });
                    }
                }
            }

            return swaggerGenOptions;
        }
    }
}