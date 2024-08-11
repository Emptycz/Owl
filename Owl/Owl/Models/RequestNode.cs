using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;

namespace Owl.Models;

public class RequestNode : INotifyPropertyChanged
{
    private string _method = "GET";
    private string _name = string.Empty;

    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public string? Url { get; set; }
    public string Body { get; set; } = string.Empty;
    public List<RequestHeader> Headers { get; set; } = [];
    public List<RequestParameter> Parameters { get; set; } = [];
    
    public HttpStatusCode? StatusCode { get; set; }

    public IEnumerable<RequestNode> Children { get; set; } = new List<RequestNode>();

    private string _tagColor = "#FF0000";

    public string TagColor
    {
        get => _tagColor;
        set
        {
            _tagColor = value;
            OnPropertyChanged(nameof(TagColor));
        }
    }

    public string Method
    {
        get => _method;
        set
        {
            _method = value;
            TagColor = GetMethodColor(value);
            OnPropertyChanged(nameof(Method));
        }
    }

    public string Response { get; set; }

    public RequestNode()
    {
    }

    // TOOD: Allow for custom colors
    private string GetMethodColor(string method)
    {
        return method.Trim().ToUpper() switch
        {
            "GET" => "#9933ff",
            "POST" => "#339933",
            "PUT" => "#ff9933",
            "UPDATE" => "#0099ff",
            "DELETE" => "#ff0000",
            _ => "#ffffff"
        };
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
