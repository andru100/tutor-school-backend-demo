using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata.Builders; 

namespace Model;

public static class EntityTypeBuilderExtensions
{
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
    {
        return propertyBuilder.HasColumnType("jsonb")
                              .HasConversion(
                                  v => JsonConvert.SerializeObject(v),
                                  v => JsonConvert.DeserializeObject<T>(v));
    }
}