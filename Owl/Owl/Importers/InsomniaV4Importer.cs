using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;

namespace Owl.Importers;

internal record struct InsomniaV4Export
{
    [JsonPropertyName("_type")]
    public string Type { get; init; }
    [JsonPropertyName("__export_format")]
    public uint ExportFormat { get; init; }
    [JsonPropertyName("__export_date")]
    public DateTime ExportDate { get; init; }
    [JsonPropertyName("__export_source")]
    public string ExportSource { get; init; }
    public InsomniaV4Resource[] Resources { get; init; }
}

internal record struct InsomniaV4Header
{
    public string Name { get; init; }
    public string Value { get; init; }
}

internal record struct InsomniaV4ResourceBody
{
    public string MimeType { get; init; }
    public string Text { get; init; }
}

internal record struct InsomniaV4Parameter
{
    [JsonPropertyName("_id")]
    public string Id { get; init; }
    public string Name { get; init; }
    public string Value { get; init; }
    public string Description { get; init; }
    public bool Disabled { get; init; }
}

internal record struct InsomniaV4ResourceAuthentication
{
    public string Type { get; init; }
    public string Token { get; init; }
}

internal record struct InsomniaV4Resource
{
    [JsonPropertyName("_id")]
    public string Id { get; init; }
    public string ParentId { get; init; }
    public long Modified { get; init; }
    public long Created { get; init; }
    public string Url { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string Method { get; init; }
    public InsomniaV4ResourceBody Body { get; init; }
    public InsomniaV4Parameter[] Parameters { get; init; }
    public InsomniaV4Header[] Headers { get; init; }
    public InsomniaV4ResourceAuthentication Authentication { get; init; }
    public double MetaSortKey { get; init; }
    public bool IsPrivate { get; init; }
    // TODO: Type this properly
    public object[] PathParameters { get; init; }
    public bool SettingStoreCookies { get; init; }
    public bool SettingSendCookies { get; init; }
    public bool SettingDisableRenderRequestBody { get; init; }
    public bool SettingEncodeUrl { get; init; }
    public bool SettingRebuildPath { get; init; }
    public string SettingFollowRedirects { get; init; }
    [JsonPropertyName("_type")]
    public string Type { get; init; }
}

internal class InsomniaV4Importer
{
    public void Parse(string content)
    {
        Log.Debug("Parsing insomniaV4 content.");
        var export = JsonSerializer.Deserialize<InsomniaV4Export>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Log.Debug("Finished parsing insomniaV4 export.");
    }
}
