using System.IO;
using System.Text;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Models;
using Owl.States;
using Owl.ViewModels.Models;
using Serilog;

namespace Owl.ViewModels.ResponseTabs;

public partial class JsonResponseTabViewModel : ViewModelBase
{
    [ObservableProperty] private string? _response;

    private readonly IRequestNodeState _requestNodeState;

    public JsonResponseTabViewModel()
    {
        _requestNodeState = RequestNodeState.Instance;
        _requestNodeState.CurrentHasChanged += (_, node) =>
        {
            if (node is not HttpRequestVm requestVm) return;
            Response = ParseResponse(requestVm);
        };

        if (_requestNodeState.Current is not HttpRequestVm vm) return;
        Response = ParseResponse(vm);
    }

    /**
     * Serializes the JSON content of the response.
     */
    private static string ParseResponse(HttpRequest? node)
    {
        if (node is null || node.Response is null) return string.Empty;

        Log.Warning("Parsing JSON response content: {0}", node.Response.Headers);

        Log.Debug("Parsing JSON content");
        using var contentStream = node.Response.Content.ReadAsStream();
        using var jsonDocument = JsonDocument.Parse(contentStream);
        using var outputStream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(outputStream, new JsonWriterOptions { Indented = true }))
        {
            jsonDocument.WriteTo(writer);
        }

        return Encoding.UTF8.GetString(outputStream.ToArray());
    }
}
